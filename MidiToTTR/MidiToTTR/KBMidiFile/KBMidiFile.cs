namespace MidiToTTR.KBMidiFile
{
    internal class KBMidiFile
    {
        public string title = string.Empty;
        public string copyrightInfo = string.Empty;
        public int trackID = 0;
        public double timeDivision = 960.0;
        public double exactTempo = 0.0;
        public byte[] rawData = null;
        public readonly List<KBMidiTempoMap> tempoChanges = new List<KBMidiTempoMap>();
        public readonly List<KBMidiTrack> tracks = new List<KBMidiTrack>();
    }
}
