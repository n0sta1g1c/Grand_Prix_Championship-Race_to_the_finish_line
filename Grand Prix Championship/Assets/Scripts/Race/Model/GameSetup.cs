using System;
using UnityEngine;


public class GameSetup
{
    System.Random Random = new System.Random();
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public int CarIndex { get; set; }
    public int TrackIndex { get; set; }
    public int GameMode { get; set; }
    public int RaceDistance { get; set; }
    public int NumberOfPlayers { get; set; }
    public string Difficulty { get; set; }
    public string TrackName { get; set; }
    public string PlayerName { get; set; }
    public float TimeToBeat { get; set; }


    public GameSetup(DateTime dateTime, int carIndex, int trackIndex, int gameMode, int raceDistance, int numberOfPlayers, string difficulty, string playerName)
    {
        Id = GetRandomNumber(1000, 9999);
        DateTime = dateTime;
        CarIndex = carIndex;
        TrackIndex = trackIndex;
        SetTrackName(TrackIndex);
        GameMode = gameMode;
        RaceDistance = raceDistance;
        NumberOfPlayers = numberOfPlayers;
        Difficulty = difficulty;
        PlayerName = playerName;
        SetTimeToBeat(GameMode,TrackIndex,CarIndex);
    }

    public int GetRandomNumber(int min, int max)
    {
        return Random.Next(min, max);
    }

    public void SetTrackName(int index)
    {
        if (index == 0)
        {
            this.TrackName = "Hungaroring";
        }
        else if (index == 1)
        {
            this.TrackName = "Brand Hatch";
        }
        else if (index == 2)
        {
            this.TrackName = "Interlagos";
        }
    }

    public void SetTimeToBeat(int gameMode, int trackindex, int carIndex) {
        if ( gameMode == 1 )
        {
            if (trackindex == 0)
            {
                if (carIndex == 0)
                {
                    this.TimeToBeat = 42f;
                }
                else if (carIndex == 1) {
                    this.TimeToBeat = 38f;
                }
                else if (carIndex == 2)
                {
                    this.TimeToBeat = 35f;
                }
                
            }
            else if (trackindex == 1)
            {
                if (carIndex == 0)
                {
                    this.TimeToBeat = 34f;
                }
                else if (carIndex == 1)
                {
                    this.TimeToBeat = 31f;
                }
                else if (carIndex == 2)
                {
                    this.TimeToBeat = 28f;
                }
            }
            else if (trackindex == 2)
            {
                if (carIndex == 0)
                {
                    this.TimeToBeat = 50f;
                }
                else if (carIndex == 1)
                {
                    this.TimeToBeat = 42f;
                }
                else if (carIndex == 2)
                {
                    this.TimeToBeat = 39f;
                }
            }
        }  
    }

}
