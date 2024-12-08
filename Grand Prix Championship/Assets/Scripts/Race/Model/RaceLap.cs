public class RaceLap
{
    public bool Valid { get; set; }
    public float LapTime { get; set; }
    public int LapNumber { get; set; }
    public float Penalty { get; set; }

    public RaceLap(int lapNumber, bool valid = true, float lapTime = 0f, float penalty = 0)
    {
        LapNumber = lapNumber;
        Valid = valid;
        LapTime = lapTime;
        Penalty = penalty;
    }
}
