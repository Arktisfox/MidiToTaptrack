using Sanford.Multimedia.Midi;
using WindowsAPICodePack.Dialogs;

namespace TTRToMidi
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private static Sequence TapTrackToSequence(TTRTrack ttrTrack)
        {
            float TapAndHoldThresholdDuration = 0.45f;
            TapAndHoldThresholdDuration = 0.3f;

            var timeDivision = ttrTrack.Difficulties.Values.First().TimeDivision;

            //write midi
            var midi = new Sequence((int)Math.Round(timeDivision));
            var track = new Track();

            var trackNameEvent = new MetaTextBuilder(MetaType.TrackName, "PART GUITAR");
            trackNameEvent.Build();
            track.Insert(0, trackNameEvent.Result);

            // process the notes in the weird way TTR does
            // we do this because some customs are garbage without doing this
            List<ITTRTrackEvent> processNotes = new List<ITTRTrackEvent>();
            List<ITTRTrackEvent> finalNotes = new List<ITTRTrackEvent>();

            foreach (var diff in ttrTrack.Difficulties)
            {
                int offset = 12 * ((int)diff.Key - 1);
                processNotes.Clear();

                var diffData = diff.Value;
                var diffNotes = diff.Value.Events;

                for (int i = 0; i < diffNotes.Count; i++)
                {
                    var ttrEvent = diffNotes[i];
                    if (ttrEvent is TTRNoteOffEvent noteOff)
                    {
                        for (int j = 0; j < processNotes.Count; j++)
                        {
                            var pairNote = (TTRNoteOnEvent)processNotes[j];
                            if (pairNote.Note == noteOff.Note)
                            {
                                int finalNote = noteOff.Note;

                                double duration = noteOff.Time - pairNote.Time;
                                double endTime = noteOff.Time;
                                if (duration <= TapAndHoldThresholdDuration)
                                {
                                    endTime = pairNote.Time + 0.001;
                                }

                                var onNote = new TTRNoteOnEvent()
                                {
                                    Note = finalNote + offset,
                                    Time = pairNote.Time
                                };

                                var offNote = new TTRNoteOffEvent()
                                {
                                    Note = finalNote + offset,
                                    Time = endTime
                                };

                                finalNotes.Add(offNote);
                                finalNotes.Add(onNote);

                                processNotes.RemoveAt(j--);
                            }
                        }
                    }
                    else if (ttrEvent is TTRNoteOnEvent)
                    {
                        processNotes.Add(ttrEvent);
                    }
                }
            }

            // compile a final list of events
            var finalEvents = new List<ITTRTrackEvent>();
            finalEvents.AddRange(finalNotes);
            finalEvents.AddRange(ttrTrack.Difficulties.Values.First().Events.Where(x => x is TTRTempoEvent));
            finalEvents = finalEvents.OrderBy(x => x.Time).ThenBy(x => !(x is TTRTempoEvent)).ToList();

            // convert ttr2track
            double lastEventTime = 0.0;
            int lastTick = 0;
            double microsecondsPerQuarterNote = 60000000.0 / 120.0; // Default to 120 BPM

            foreach (var @event in finalEvents)
            {
                double kSecondsPerQuarterNote = microsecondsPerQuarterNote / 1000000.0;
                double kSecondsPerTick = kSecondsPerQuarterNote / midi.Division;

                int deltaTicks = (int)Math.Round((@event.Time - lastEventTime) / kSecondsPerTick);
                lastTick += deltaTicks;

                if (@event is TTRTempoEvent tempoEvent)
                {
                    microsecondsPerQuarterNote = 60000000.0 / tempoEvent.Tempo;
                    var tempoChangeBuilder = new TempoChangeBuilder() { Tempo = (int)microsecondsPerQuarterNote };
                    tempoChangeBuilder.Build();
                    track.Insert(lastTick, tempoChangeBuilder.Result);
                }
                else if (@event is ITTRNoteEvent noteEvent)
                {
                    var noteBuilder = new ChannelMessageBuilder();
                    noteBuilder.Command = ChannelCommand.NoteOn;
                    noteBuilder.Data1 = noteEvent.Note;
                    noteBuilder.Data2 = (@event is TTRNoteOffEvent) ? 0 : 100;
                    noteBuilder.Build();

                    track.Insert(lastTick, noteBuilder.Result);
                }

                lastEventTime = @event.Time;
            }

            midi.Add(track);
            return midi;
        }

        private void buttonBrowseInput_Click(object sender, EventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("Taptrack Files", "ttr2_track;ttr2_track.xml"));
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                textBoxInputPath.Text = dialog.FileName;

                // automagically set midi path if we don't have one already
                if (string.IsNullOrWhiteSpace(textBoxOutputPath.Text))
                {
                    textBoxOutputPath.Text = Path.Combine(Path.GetDirectoryName(dialog.FileName), "notes.mid");
                }
            }
        }

        private void buttonBrowseOutput_Click(object sender, EventArgs e)
        {
            var dialog = new CommonSaveFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("MIDI Files", "mid"));
            dialog.DefaultFileName = "notes";
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                textBoxOutputPath.Text = dialog.FileName;
            }
        }

        private void buttonConvert_Click(object sender, EventArgs e)
        {
            // sanity checking
            if (string.IsNullOrWhiteSpace(textBoxInputPath.Text))
            {
                MessageBox.Show("Taptrack path not set.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(textBoxOutputPath.Text))
            {
                MessageBox.Show("Output path not set.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBoxOutputPath.Text.Equals(textBoxInputPath.Text, StringComparison.CurrentCultureIgnoreCase))
            {
                MessageBox.Show("Output path must not be the same as the Taptrack path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            // load in the ttr2_track
            TTRTrack track = null;
            try
            {
                track = new TTRTrack();
                track.LoadFromPlist(textBoxInputPath.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Taptrack Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // convert it to a midi sequence then save it
            try
            {
                var sequence = TapTrackToSequence(track);
                sequence.Save(textBoxOutputPath.Text);
                MessageBox.Show("Conversion was successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
