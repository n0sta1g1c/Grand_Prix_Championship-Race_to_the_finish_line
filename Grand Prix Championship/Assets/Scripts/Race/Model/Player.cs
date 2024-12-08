using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player
{
    public enum ControlType { HumanInput, AI }
    public ControlType controlType { get; set; }
    public string Name { get; set; }
    public bool Finished { get; set; }
    public int RacePosition { get; set; }
    public Transform CarTransform { get; set; }
    public int CarType { get; set; }
    public int GridPosition { get; set; }
    public int FinishedPosition { get; set; }
    public bool ValidLastLap { get; set; }
    public bool ValidLap { get; set; }
    public bool PenaltyApplied { get; set; }
    public float PenaltyTime { get; set; }
    public float BestLapTime { get; set; }
    public float LastLapTime { get; set; }
    public float CurrentLapTime { get; set; }
    public int CurrentLap { get; set; }
    public float LapTimerTimeStamp { get; set; }
    public float Gap { get; set; }
    public float TotalTime { get; set; }
    public int LastCheckpointPassed { get; set; }
    public bool MissedCheckpoint { get; set; }
    public bool TimeTrialChallengeCompleted { get; set; }

    public List<RaceLap> RaceLapsList { get; set; }

    public Player(string name, Transform carTransform, ControlType controlType = ControlType.AI)
    {
        this.Name = name;
        this.CarTransform = carTransform;
        setCarType(this.CarTransform);
        this.GridPosition = 1;
        this.FinishedPosition = -1;
        this.controlType = controlType;
        this.Finished = false;
        this.RacePosition = 0;
        this.ValidLastLap = true;
        this.ValidLap = true;
        this.PenaltyApplied = false;
        this.PenaltyTime = 0;
        this.BestLapTime = Mathf.Infinity;
        this.LastLapTime = 0;
        this.CurrentLapTime = 0;
        this.TotalTime = 0;
        this.CurrentLap = 0;
        this.LapTimerTimeStamp = 0;
        this.Gap = 0;
        this.LastCheckpointPassed = 0;
        this.MissedCheckpoint = false;
        this.RaceLapsList = new List<RaceLap>();
        this.TimeTrialChallengeCompleted = false;
    }

    public void setCarType(Transform carTS)
    {
        if (this.CarTransform.name.StartsWith("Nissan"))
        {
            this.CarType = 1;
        }
        else if (this.CarTransform.name.StartsWith("Porsche"))
        {
            this.CarType = 2;
        }
        else if (this.CarTransform.name.StartsWith("Audi"))
        {
            this.CarType = 3;
        }
        else
        {
            this.CarType = 0;
        }
    }

    public void ResetPlayer()
    {
        this.GridPosition = 1;
        this.FinishedPosition = -1;
        this.controlType = controlType;
        this.Finished = false;
        this.RacePosition = 0;
        this.ValidLastLap = true;
        this.ValidLap = true;
        this.PenaltyApplied = false;
        this.PenaltyTime = 0;
        this.BestLapTime = Mathf.Infinity;
        this.LastLapTime = 0;
        this.CurrentLapTime = 0;
        this.TotalTime = 0;
        this.CurrentLap = 0;
        this.LapTimerTimeStamp = 0;
        this.Gap = 0;
        this.LastCheckpointPassed = 0;
        this.MissedCheckpoint = false;
        this.RaceLapsList.Clear();
    }
}
