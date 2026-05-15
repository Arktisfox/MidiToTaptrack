using Claunia.PropertyList;
using MidiToTTR.KBMidiFile;
using Sanford.Multimedia.Midi;
using System.Data;
using System.Text;
using WindowsAPICodePack.Dialogs;

namespace MidiToTTR
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void linkLabelHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(@"This tool takes a MIDI input using note mappings from Guitar Hero charting programs (e.g. Moonscraper, Editor On Fire)

[Mapping]
Easy difficulty MIDI notes: 60-71
Medium difficulty MIDI notes: 72-83
Hard difficulty MIDI notes: 84-95
Extreme difficulty MIDI notes: 96-107

All 12 notes per difficulty are available for use.
Typically TTR Themes are mapped to the following notes:
[0:Left Tap] [1:Left Shake] [2:Center Tap] [3:Right Shake] [4:Right Tap] [5:Center Shake]

[Output Format]
Use binary output format for iOS TTR, and Tap Tap Player
Use xml output format for Android TTR4 only
", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonBrowseMidi_Click(object sender, EventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("MIDI Files", "mid;midi"));
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                textBoxMidiPath.Text = dialog.FileName;

                // automagically set ttr2_track path if we don't have one already
                if (string.IsNullOrWhiteSpace(textBoxOutputPath.Text))
                {
                    textBoxOutputPath.Text = Path.Combine(Path.GetDirectoryName(dialog.FileName), "taptrack.ttr2_track");
                }
            }
        }

        private void buttonBrowseOutput_Click(object sender, EventArgs e)
        {
            var dialog = new CommonSaveFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("TTR Track Files", "ttr2_track"));
            dialog.DefaultFileName = "taptrack";
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                textBoxOutputPath.Text = dialog.FileName;
            }
        }

        private static int DifficultyLevelForNote(int noteIndex)
        {
            if (noteIndex >= 60 && noteIndex <= 71) return 1;
            if (noteIndex >= 72 && noteIndex <= 83) return 2;
            if (noteIndex >= 84 && noteIndex <= 95) return 3;
            if (noteIndex >= 96 && noteIndex <= 107) return 4;
            return -1;
        }

        private static KBMidiFile.KBMidiFile? GetFileForNote(Dictionary<string, KBMidiFile.KBMidiFile> files, int noteIndex)
        {
            int level = DifficultyLevelForNote(noteIndex);
            if (level >= 0)
            {
                string levelStr = level.ToString();
                if (files.TryGetValue(levelStr, out var file))
                {
                    return file;
                }
                else
                {
                    file = new KBMidiFile.KBMidiFile();
                    file.tracks.Add(new KBMidiTrack()); // create the track that we'll use

                    files.Add(levelStr, file);
                    return file;
                }
            }
            else
            {
                return null;
            }
        }

        private Dictionary<string, KBMidiFile.KBMidiFile> ConvertToKB(string path)
        {
            // files
            var files = new Dictionary<string, KBMidiFile.KBMidiFile>();

            // load in events
            var sequence = new Sequence(path);
            List<MidiEvent> allEvents = new List<MidiEvent>();

            // we can combine these because EOF does
            // TRACK 0 : bpm changes etc
            // TRACK 1 : events and sections
            // TRACK 2 : all the notes
            for (int i = 0; i < sequence.Count; i++)
            {
                var trk = sequence[i];
                for (int j = 0; j < trk.Count; j++)
                {
                    var evt = trk.GetMidiEvent(j);
                    allEvents.Add(evt);
                }
            }

            // shared things
            string title = "Untitled";
            string copyrightInfo = "";
            double exactTempo = 120.0;
            List<KBMidiTempoMap> sharedTempoMap = new List<KBMidiTempoMap>();
            List<KBMidiEvent> sharedEvents = new List<KBMidiEvent>();

            // state
            double currentTimeInSeconds = 0d;
            double currentTimeInQuarterNotes = 0d;

            long microseconds = 500000;
            float timeSigNumerator = 4f;

            // order and parse events
            allEvents = allEvents.OrderBy(x => x.AbsoluteTicks).ToList();
            MidiEvent? previousEvent = null;

            foreach (var evt in allEvents)
            {
                float kSecondsPerQuarterNote = microseconds / 1000000.0f;
                float kSecondsPerTick = kSecondsPerQuarterNote / sequence.Division;
                float kQuarterNotesPerTick = 1.0f / sequence.Division;

                if (previousEvent == null)
                {
                    currentTimeInSeconds = evt.AbsoluteTicks * kSecondsPerTick;
                    currentTimeInQuarterNotes = evt.AbsoluteTicks * kQuarterNotesPerTick;
                }
                else if (previousEvent.AbsoluteTicks != evt.AbsoluteTicks)
                {
                    long deltaTicks = evt.AbsoluteTicks - previousEvent.AbsoluteTicks;
                    currentTimeInSeconds += deltaTicks * kSecondsPerTick;
                    currentTimeInQuarterNotes += deltaTicks * kQuarterNotesPerTick;
                }

                // meta message?
                if (evt.MidiMessage is MetaMessage mm)
                {
                    if (mm.MetaType == MetaType.Tempo)
                    {
                        byte[] microSecondBytes = { 0x00, 0x00, 0x00, 0x00 };
                        mm.GetBytes().CopyTo(microSecondBytes, 1);
                        Array.Reverse(microSecondBytes);
                        microseconds = BitConverter.ToUInt32(microSecondBytes, 0);

                        //add bpm change
                        double bpm = (60000000d / microseconds) * (timeSigNumerator / 4.0);
                        var bpmChange = new KBMidiTempoMap()
                        {
                            startTime = currentTimeInSeconds,
                            tempo = bpm
                        };
                        sharedTempoMap.Add(bpmChange);

                        if (sharedTempoMap.Count == 1)
                        {
                            // first tempo change, also set exactTempo
                            exactTempo = bpm;
                        }
                    }
                    else if (mm.MetaType == MetaType.TimeSignature)
                    {
                        byte[] tsBytes = { 0x00, 0x00, 0x00, 0x00 };
                        mm.GetBytes().CopyTo(tsBytes, 0);

                        timeSigNumerator = tsBytes[0];
                    }
                    else if (mm.MetaType == MetaType.Text)
                    {
                        string text = Encoding.UTF8.GetString(mm.GetBytes());

                        var @event = new KBMidiEvent
                        {
                            type = KBMidiEvent.TYPE_TEXT,
                            text = text,
                            time = currentTimeInSeconds,
                            timeInQuarterNotes = currentTimeInQuarterNotes
                        };
                        sharedEvents.Add(@event);
                    }
                    else if (mm.MetaType == MetaType.TrackName)
                    {
                        string text = Encoding.UTF8.GetString(mm.GetBytes());
                        title = text;
                    }
                    else if (mm.MetaType == MetaType.Copyright)
                    {
                        string text = Encoding.UTF8.GetString(mm.GetBytes());
                        copyrightInfo = text;
                    }
                }

                // we have a channel message?
                if (evt.MidiMessage is ChannelMessage cm)
                {
                    // check if we have a note
                    if (cm.Command == ChannelCommand.NoteOn)
                    {
                        int laneIndex = cm.Data1;
                        bool isNoteOn = (cm.Data2 != 0);

                        int localLaneIndex = (laneIndex % 12) + 60;
                        var file = GetFileForNote(files, laneIndex);

                        if (file != null)
                        {
                            var track = file.tracks.First();
                            if (!isNoteOn)
                            {
                                var @event = new KBMidiEvent
                                {
                                    type = KBMidiEvent.TYPE_NOTEOFF,
                                    velocity = 0,
                                    channel = cm.MidiChannel,
                                    note = localLaneIndex,
                                    time = currentTimeInSeconds,
                                    timeInQuarterNotes = currentTimeInQuarterNotes
                                };
                                track.events.Add(@event);
                            }
                            else
                            {
                                var @event = new KBMidiEvent
                                {
                                    type = KBMidiEvent.TYPE_NOTEON,
                                    velocity = 127,
                                    channel = cm.MidiChannel,
                                    note = localLaneIndex,
                                    time = currentTimeInSeconds,
                                    timeInQuarterNotes = currentTimeInQuarterNotes
                                };
                                track.events.Add(@event);
                            }
                        }
                    }
                }

                // reset flag
                previousEvent = evt;
            }

            // set all files metadata/tempo map/etc
            foreach (var file in files.Values)
            {
                file.trackID = 0;
                file.title = title;
                file.copyrightInfo = copyrightInfo;
                file.exactTempo = exactTempo;
                file.tempoChanges.AddRange(sharedTempoMap);
                file.tracks.First().events.InsertRange(0, sharedEvents);
            }

            return files;
        }

        private void buttonConvert_Click(object sender, EventArgs e)
        {
            // sanity checking
            if (string.IsNullOrWhiteSpace(textBoxMidiPath.Text))
            {
                MessageBox.Show("MIDI path not set.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(textBoxOutputPath.Text))
            {
                MessageBox.Show("Output path not set.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(textBoxOutputPath.Text.Equals(textBoxMidiPath.Text, StringComparison.CurrentCultureIgnoreCase))
            {
                MessageBox.Show("Output path must not be the same as the MIDI path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (File.Exists(textBoxOutputPath.Text))
            {
                var res = MessageBox.Show("Output path already exists. Do you want to overwrite it?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res != DialogResult.Yes)
                {
                    return;
                }
            }

            // convert to a dictionary of KBMidiFile
            var files = ConvertToKB(textBoxMidiPath.Text);
            if (files.Count == 0)
            {
                MessageBox.Show("No data was converted, check that your MIDI notes line up with what's expected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // archive and save
            var archived = NSKeyedUnarchiver.Archiver.Archive(files);
            try
            {
                if (File.Exists(textBoxOutputPath.Text)) File.Delete(textBoxOutputPath.Text);
                if (radioButtonBinary.Checked)
                {
                    using (var stream = File.OpenWrite(textBoxOutputPath.Text))
                    {
                        BinaryPropertyListWriter.Write(stream, archived);
                    }
                }
                else if(radioButtonXML.Checked)
                {
                    File.WriteAllText(textBoxOutputPath.Text, archived.ToXmlPropertyList());
                }
                MessageBox.Show("Conversion was successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
