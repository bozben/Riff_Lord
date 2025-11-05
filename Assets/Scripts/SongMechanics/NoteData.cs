
public enum NoteType { Tap, Sustain }

[System.Serializable]
public class NoteData
{
    public float timestamp;
    public int laneIndex;
    public NoteType noteType;
    public float sustainDuration;
}