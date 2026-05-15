namespace MidiToTTR.KBMidiFile
{
    internal class KBMidiEvent
    {
        public const int TYPE_NOTEON = 0;
        public const int TYPE_NOTEOFF = 1;
        public const int TYPE_TEXT = 2;

        public int type;
        public double time;
        public double timeInQuarterNotes;
        public int note;
        public int channel;
        public int velocity;
        public string? text = null;
    }
}
