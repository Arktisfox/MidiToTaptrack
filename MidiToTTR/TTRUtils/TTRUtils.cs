using System.Text;
using Claunia.PropertyList;
using Sanford.Multimedia.Midi;
using TTR.KBMidi;

namespace TTR;

public static class TTRUtils
{
        private static int? RemapNote_Revenge(int midiNote)
        {
            const int BASE_OFFSET = 60; // Standard TTR themes map to the 60-72 range
            return (midiNote % 12) + BASE_OFFSET;
        }

        private static int? RemapNote_Reloaded(int midiNote)
        {
            const int BASE_OFFSET = 60; // Standard TTR themes map to the 60-72 range
                                        // We need to remap this 
                                        // For reference: [0:Left Tap] [1:Left Shake] [2:Center Tap] [3:Right Shake] [4:Right Tap] [5:Center Shake]
            int localNote = midiNote % 12;
            switch (localNote)
            {
                case 0: // first lane
                    return 0 + BASE_OFFSET;
                case 1: // second lane
                    return 2 + BASE_OFFSET;
                case 2: // third lane
                    return 4 + BASE_OFFSET;
                case 3: // fourth lane, undefined
                    return null;
                case 4: // open note, map to center shake
                    return 5 + BASE_OFFSET;
                default:
                    return null;
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

        private static KBMidiFile? GetFileForNote(Dictionary<string, KBMidiFile> files, int noteIndex)
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
                    file = new KBMidiFile();
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

        private static Dictionary<string, KBMidiFile> ConvertToKB(string path, bool useReloaedMapping)
        {
            // files
            var files = new Dictionary<string, KBMidiFile>();

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
                        int midiNote = cm.Data1;
                        bool isNoteOn = (cm.Data2 != 0);

                        int? localLaneIndex = useReloaedMapping ? RemapNote_Reloaded(midiNote) : RemapNote_Revenge(midiNote);
                        var file = GetFileForNote(files, midiNote);

                        if (localLaneIndex != null && file != null)
                        {
                            var track = file.tracks.First();
                            if (!isNoteOn)
                            {
                                var @event = new KBMidiEvent
                                {
                                    type = KBMidiEvent.TYPE_NOTEOFF,
                                    velocity = 0,
                                    channel = cm.MidiChannel,
                                    note = localLaneIndex.Value,
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
                                    note = localLaneIndex.Value,
                                    time = currentTimeInSeconds,
                                    timeInQuarterNotes = currentTimeInQuarterNotes
                                };
                                track.events.Add(@event);
                            }
                        }
                    }
                    else if(cm.Command == ChannelCommand.NoteOff)
                    {
                        int midiNote = cm.Data1;

                        int? localLaneIndex = (useReloaedMapping) ? RemapNote_Reloaded(midiNote) : RemapNote_Revenge(midiNote);
                        var file = GetFileForNote(files, midiNote);

                        if (localLaneIndex != null && file != null)
                        {
                            var track = file.tracks.First();
                            var @event = new KBMidiEvent
                            {
                                type = KBMidiEvent.TYPE_NOTEOFF,
                                velocity = 0,
                                channel = cm.MidiChannel,
                                note = localLaneIndex.Value,
                                time = currentTimeInSeconds,
                                timeInQuarterNotes = currentTimeInQuarterNotes
                            };
                            track.events.Add(@event);
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

    public static void ConvertToTaptrack(string inputPath, string outputPath, bool useReloaedMapping, bool convertToBplist)
    {
        var files = ConvertToKB(inputPath, useReloaedMapping);
        if(files.Count == 0)
        {
            throw new Exception("No data was converted, check that your MIDI notes line up with what's expected.");
        }

        var archived = NSKeyedUnarchiver.Archiver.Archive(files);
        try
        {
            if (File.Exists(outputPath)) File.Delete(outputPath);
            if (convertToBplist)
            {
                using (var stream = File.OpenWrite(outputPath))
                {
                    BinaryPropertyListWriter.Write(stream, archived);
                }
            }
            else
            {
                File.WriteAllText(outputPath, archived.ToXmlPropertyList());
            }
            // MessageBox.Show("Conversion was successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            throw ex;
        }
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


        public static void ConvertToMidi(string inputPath, string outputPath)
        {
            TTRTrack track = null;
            try
            {
                track = new TTRTrack();
                track.LoadFromPlist(inputPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                var sequence = TapTrackToSequence(track);
                sequence.Save(outputPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

}