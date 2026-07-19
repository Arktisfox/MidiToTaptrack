using Claunia.PropertyList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRToMidi
{
    public enum TTRDifficultyLevel
    {
        Kids = 0,
        Easy = 1,
        Medium = 2,
        Hard = 3,
        Extreme = 4
    }

    public enum TTRMidiEventType
    {
        NoteOn,
        NoteOff,
        Meta,
    }

    public interface ITTRTrackEvent
    {
        public double Time { get; }
    }

    public interface ITTRNoteEvent
    {
        public int Note { get; }
    }
    public struct TTRNoteOnEvent : ITTRTrackEvent, ITTRNoteEvent
    {
        public double Time { get; set; }
        public int Note { get; set; }

    }
    public struct TTRNoteOffEvent : ITTRTrackEvent, ITTRNoteEvent
    {
        public double Time { get; set; }
        public int Note { get; set; }
    }

    public struct TTRMetaEvent : ITTRTrackEvent
    {
        public double Time { get; set; }
        public string Text { get; set; }
    }

    public struct TTRTempoEvent : ITTRTrackEvent
    {
        public double Time { get; set; }
        public double Tempo { get; set; }
    }

    public class TTRTrackDifficulty
    {
        public string Title = null;
        public string Artist = null;
        public double Tempo = 0.0d;
        public double TimeDivision = 480.0;
        public List<ITTRTrackEvent> Events = new List<ITTRTrackEvent>();
    }

    public class TTRTrack
    {
        public readonly Dictionary<TTRDifficultyLevel, TTRTrackDifficulty> Difficulties = new Dictionary<TTRDifficultyLevel, TTRTrackDifficulty>();

        private static T GetValueOrDefault<T>(Dictionary<string, object> dict, string key, T defaultValue = default)
        {
            return dict.TryGetValue(key, out var obj) && obj is T value ? value : defaultValue;
        }

        private static void ValidateClass(Dictionary<string, object> root, string className)
        {
            if (root.TryGetValue("$class", out var classDict))
            {
                var archivedClassName = GetValueOrDefault<string>((Dictionary<string, object>)classDict, "$classname");
                if (archivedClassName != className)
                {
                    throw new InvalidDataException($"TTRTrack load failure: Expected classname {className} but got {archivedClassName}");
                }
            }
            else
            {
                throw new InvalidDataException($"TTRTrack load failure: Expected classname {className} but got none");

            }
        }

        public void LoadFromPlist(Stream plistStream)
        {
            var _rootDictionary = PropertyListParser.Parse(plistStream);
            var root = (Dictionary<string, object>)NSKeyedUnarchiver.Unarchiver.DeepParse(_rootDictionary);

            foreach (var kvp in root)
            {
                var midiFileDict = (Dictionary<string, object>)kvp.Value;
                ValidateClass(midiFileDict, "KBMidiFile");

                var difficulty = new TTRTrackDifficulty();
                int difficultyLevelInt = int.Parse(kvp.Key);

                // get tempo and time division
                if (midiFileDict.ContainsKey("exactTempo"))
                {
                    difficulty.Tempo = GetValueOrDefault<double>(midiFileDict, "exactTempo");
                }
                else if (midiFileDict.ContainsKey("tempo"))
                {
                    difficulty.Tempo = GetValueOrDefault<double>(midiFileDict, "tempo");
                }
                difficulty.TimeDivision = GetValueOrDefault<double>(midiFileDict, "timeDivision");


                // load tempo chnages
                var tempoChangesList = midiFileDict["tempoChanges"] as object[];
                if (tempoChangesList != null)
                {
                    foreach (Dictionary<string, object> tempoChangeData in tempoChangesList)
                    {
                        ValidateClass(tempoChangeData, "KBMidiTempoMap");
                        if (tempoChangeData.ContainsKey("tempo")) // yes.. some songs actually have a tempo map without tempo values
                        {
                            double tempo = GetValueOrDefault<double>(tempoChangeData, "tempo");
                            double time = GetValueOrDefault<double>(tempoChangeData, "startTime");
                            difficulty.Events.Add(new TTRTempoEvent() { Tempo = tempo, Time = time });
                        }
                    }

                    var tempoChanges = difficulty.Events.Where(e => e is TTRTempoEvent).Cast<TTRTempoEvent>().ToList();
                    if (tempoChanges.Count == 0)
                    {
                        difficulty.Tempo = 120;
                        difficulty.Events.Add(new TTRTempoEvent() { Time = 0.0, Tempo = 120 });
                    }
                }
                else
                {
                    if (difficulty.Tempo == 0.0)
                    {
                        difficulty.Tempo = 120.0;
                    }
                    difficulty.Events.Add(new TTRTempoEvent() { Time = 0.0, Tempo = difficulty.Tempo });
                }

                // load tracks
                var tracksList = midiFileDict["tracks"] as object[]; // object[], each a Dictionary<string,object>
                if (tracksList == null)
                {
                    throw new InvalidDataException("Missing tracks dictionary in ttr2_track");
                }

                foreach (Dictionary<string, object> trackData in tracksList)
                {
                    ValidateClass(trackData, "KBMidiTrack");
                    var eventsData = trackData["events"] as object[];
                    if (eventsData == null)
                    {
                        throw new InvalidDataException("events list missing from track in ttr2_track");
                    }

                    foreach (Dictionary<string, object> eventDataDict in eventsData)
                    {
                        ValidateClass(eventDataDict, "KBMidiEvent");
                        TTRMidiEventType type = (TTRMidiEventType)GetValueOrDefault<long>(eventDataDict, "type");
                        int note = (int)GetValueOrDefault<long>(eventDataDict, "note");
                        int velocity = (int)GetValueOrDefault<long>(eventDataDict, "velocity");
                        double time = GetValueOrDefault<double>(eventDataDict, "time");

                        if (type == TTRMidiEventType.Meta)
                        {
                            string text = eventDataDict["text"] as string;
                            if (text != null)
                            {
                                if (text.StartsWith("ARTIST: "))
                                {
                                    difficulty.Artist = text.Substring(8);
                                }
                                else if (text.StartsWith("TITLE: "))
                                {
                                    difficulty.Title = text.Substring(7);
                                }
                            }
                        }
                        else if (type == TTRMidiEventType.NoteOn || type == TTRMidiEventType.NoteOff)
                        {
                            // note_on with velocity 0 is treated as note_off
                            if (type == TTRMidiEventType.NoteOn && velocity == 0)
                            {
                                type = TTRMidiEventType.NoteOff;
                            }

                            if (type == TTRMidiEventType.NoteOn)
                            {
                                difficulty.Events.Add(new TTRNoteOnEvent()
                                {
                                    Note = note,
                                    Time = time,
                                });
                            }
                            else
                            {
                                difficulty.Events.Add(new TTRNoteOffEvent()
                                {
                                    Note = note,
                                    Time = time,
                                });
                            }
                        }
                    }
                }

                // store difficulty data
                Difficulties[(TTRDifficultyLevel)difficultyLevelInt] = difficulty;
            }
        }

        public void LoadFromPlist(string plistPath)
        {
            using (var fs = File.OpenRead(plistPath))
            {
                LoadFromPlist(fs);
            }
        }
    }
}
