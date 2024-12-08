using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using static Player;
//using static UnityEditor.Experimental.GraphView.GraphView;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    NumberFormatInfo NFI = new NumberFormatInfo();

    public Player Player;
    Rigidbody CarRigidbody;

    public List<Transform> Waypoints;
    public int CurrentWaypoint;
    public float WaypointRange;

    private float CurrentAngle;
    public bool IsInsideBraking;
    private float GasInput;
    private bool BrakeInput;
    private Vector3 LastPosition;
    private float StuckTime = 0f;
    private float ResetThreshold = 2f;
    private bool IsResetting = false;
    public float SpeedThreshold = 0.5f;

    private List<int> BoostWaypoints_Hungaroring = new List<int> { 5, 22, 28 };
    private List<int> BoostWaypoints_Interlagos = new List<int> { 9, 19 };

    private List<int> LiftWaypoints_Hungaroring = new List<int> { 5, 7, 17, 19, 23 };
    private List<int> LiftWaypoints_Interlagos = new List<int> { 2, 10, 19, 20, 21, 25 };

    private Transform[] Checkpoints;
    private int CheckpointCount;
    private int CheckpointLayer;
    public CarController CarController;

    private bool IsBoosting = false;
    private bool IsLifting = false;

    void Awake()
    {
        CarController = GetComponent<CarController>();
        CarRigidbody = CarController.GetComponent<Rigidbody>();
    }

    public void Initialize(Player assignedPlayer)
    {
        Player = assignedPlayer;

        SetCarMaxSpeed(Player.CarType);
    }

    private void Start()
    {
        NFI.NumberDecimalSeparator = ".";
        CheckpointCount = GameManager.Instance.Checkpoints.Count;
        Checkpoints = GameManager.Instance.Checkpoints.ToArray();
        CheckpointLayer = GameManager.Instance.CheckpointLayer;

        Waypoints = GameManager.Instance.Waypoints;
        CurrentWaypoint = 0;
        WaypointRange = 5;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPlayer();
        }

        //Calculating current laptime
        UpdateCurrentLapTime();

        if (!CheckIfFinished())
        {
            if (Player.controlType == ControlType.HumanInput)
            {
                HumanControl();
            }
            else if (Player.controlType == ControlType.AI)
            {
                //AI Logic

                FollowWaypoints();

                AIControl();

                CheckIfStuck();

                CheckIfInBoostZone();

                CheckIfLiftIsNecessary();

            }
        }
    }

    /*-------------------------------------------------------------------------------------------------------*/

    bool CheckIfFinished()
    {
        if (Player.Finished)
        {
            CarController.Throttle = 0;
            ApplyBrakeWithDelay(3);
            Player.CurrentLapTime = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ApplyBrakeWithDelay(float delay)
    {
        StartCoroutine(DelayedBrake(delay));
    }

    private IEnumerator DelayedBrake(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Apply the brake
        CarController.Brake = true;

        //Debug.Log("Brake applied after delay.");
    }

    public void SetCarMaxSpeed(int carType)
    {
        if (carType == 1)
        {
            CarController.MaxSpeed = 70f;
        }
        else if (carType == 2)
        {
            CarController.MaxSpeed = 90f;
        }
        else
        {
            CarController.MaxSpeed = 100f;
        }
    }

    void StartLap()
    {
        //Debug.Log("Lap started");
        if (Player.CurrentLap < 0) // Initialize on first lap
        {
            Player.CurrentLap = 0;
            Player.BestLapTime = float.MaxValue; // Set best time to an unachievable value initially
        }
        else
        {
            Player.CurrentLap++; // Increment lap counter
        }

        Player.LastCheckpointPassed = 1;
        Player.LapTimerTimeStamp = Time.time;
        Player.ValidLap = true; // Assume lap starts as valid
    }

    void EndLap()
    {
        // Ensure lap ends correctly
        Player.LastLapTime = Time.time - Player.LapTimerTimeStamp;

        // Update best lap time if current lap is valid
        if (Player.ValidLastLap)
        {
            Player.BestLapTime = Mathf.Min(Player.LastLapTime, Player.BestLapTime);
        }

        // Log the completed lap
        //Debug.Log("Lap ended - missed checkpoint: " + player.LastCheckpointPassed);
        RaceLap lap = new RaceLap((int)Player.CurrentLap, Player.ValidLastLap, Player.LastLapTime, CalculatePenalty(Player.LastCheckpointPassed, CheckpointCount));
        if (lap.Penalty > 0)
        {
            Player.PenaltyApplied = true;
            Player.PenaltyTime += lap.Penalty;
        }
        //Debug.Log(player.Name + lap.Penalty);
        Player.TotalTime += Player.LastLapTime;
        Player.TotalTime += lap.Penalty;
        Player.RaceLapsList.Add(lap);



        //Debug.Log(player.Name + ", lap:" + lap.LapNumber + ", Laptime: " + lap.LapTime + ", Penalty: " + lap.Penalty);

    }

    public float CalculatePenalty(int lastCheckpointCount, int checkpointCount)
    {
        if (lastCheckpointCount == checkpointCount)
        {
            return 0f;
        }
        else
        {
            return (float)(checkpointCount - (lastCheckpointCount + 1)) * 3;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        //Debug.Log(player.Name + ": " + ", Last Checkpoint passed:"+player.LastCheckpointPassed);

        // Checkpoint collision logic
        if (collider.gameObject.layer != CheckpointLayer)
            return;

        int checkpointNumber;
        if (int.TryParse(collider.gameObject.name, out checkpointNumber))
        {
            //Debug.Log(player.Name + ", Last Checkpoint passed: " + player.LastCheckpointPassed);
            if (checkpointNumber == Player.LastCheckpointPassed + 1) // Correct order
            {
                Player.LastCheckpointPassed = checkpointNumber;
                Player.MissedCheckpoint = false;
                // Reset missed checkpoint flag
            }
            else if (checkpointNumber > Player.LastCheckpointPassed + 1) // Skipped checkpoints
            {
                if (!Player.MissedCheckpoint) // Play error audio only once per missed checkpoint
                {
                    if (Player.controlType == Player.ControlType.HumanInput)
                    {
                        GameManager.Instance.AudioManager.PlaySFX("Error");
                    }
                    Player.MissedCheckpoint = true; // Set flag indicating a checkpoint was missed
                }
                Player.ValidLap = false; // Mark lap as invalid
            }
            else if (checkpointNumber <= Player.LastCheckpointPassed) // Backtracking or duplicate trigger
            {

                // Optional: You can log a warning or ignore this case
            }
        }


        // Handle the starting line checkpoint
        if (collider.gameObject.name == "1")
        {
            if (Player.LastCheckpointPassed == CheckpointCount) // Completed a valid lap
            {
                Player.ValidLastLap = true;
                EndLap();
                StartLap();
            }
            else if (Player.LastCheckpointPassed > 1 && Player.LastCheckpointPassed <= CheckpointCount) // Skipped checkpoints
            {
                Player.ValidLastLap = false;
                EndLap();
                StartLap();
            }
            else if (Player.CurrentLap == 0) // Initial lap condition
            {
                StartLap();
            }
            if (Player.CurrentLap > GameManager.Instance.GameSetup.RaceDistance && GameManager.Instance.GameSetup.GameMode == 0)
            {
                if (!Player.Finished) // Check if the player has already been marked as finished
                {
                    GameManager.Instance.FinishedPlayers.Add(Player);
                    if (Player == GameManager.Instance.FinishedPlayers[0])
                    {
                        Player.Gap = 0;
                    }
                    else
                    {
                        Player.Gap = GameManager.Instance.FinishedPlayers[0].TotalTime - Player.TotalTime;
                    }
                    Player.FinishedPosition = this.GetRacePosition();
                    Player.Finished = true;
                }
                return;
            }
            return;
        }


    }

    private void ResetPlayer()
    {
        if (this.gameObject == GameManager.Instance.ChosenCarObject && Player.LastCheckpointPassed > 0)
        {

            Transform checkpoint = Checkpoints[Player.LastCheckpointPassed - 1];

            CarController.transform.position = checkpoint.position;
            CarController.transform.rotation = checkpoint.rotation;


            if (CarRigidbody != null)
            {
                CarRigidbody.velocity = Vector3.zero;
                CarRigidbody.angularVelocity = Vector3.zero;
            }
        }
    }

    private void ResetToLastCheckpoint()
    {
        if (Player.LastCheckpointPassed > 0)
        {
            Transform checkpoint = Checkpoints[Player.LastCheckpointPassed - 1];

            CarController.transform.position = checkpoint.position;
            CarController.transform.rotation = checkpoint.rotation;

            if (CarRigidbody != null)
            {
                CarRigidbody.velocity = Vector3.zero;
                CarRigidbody.angularVelocity = Vector3.zero;
            }
        }
    }

    private void UpdateAIWaypoint(int lastCheckpointPassed)
    {
        // Define a mapping of checkpoints to waypoints for each track
        Dictionary<int, int> checkpointToWaypointMap_Hungaroring = new Dictionary<int, int>
        {
        { 1, 1 }, { 2, 3 }, { 3, 7 }, { 4, 8 },
        { 5, 11 }, { 6, 13 }, { 7, 15 }, { 8, 16 },
        { 9, 17 }, { 10, 19 }, { 11, 21 }, { 12, 22 },
        { 13, 25 }, { 14, 28 }, { 15, 31 }
        };

        Dictionary<int, int> checkpointToWaypointMap_BrandsHatch = new Dictionary<int, int>
        {
        { 2, 2 }, { 3, 7 }, { 4, 10 },
        { 5, 13 }, { 6, 18 }, { 7, 21 }, { 8, 24 },
        { 9, 26 }, { 10, 28 }, { 11, 30 }, { 12, 33 },
        { 13, 36 }, { 14, 39 }
        };

        Dictionary<int, int> checkpointToWaypointMap_Interlagos = new Dictionary<int, int>
        {
        { 2, 3 }, { 3, 4 }, { 4, 7 },
        { 5, 11 }, { 6, 13 }, { 7, 17 }, { 8, 20 },
        { 9, 22 }, { 10, 26 }, { 11, 29 }, { 12, 32 },
        { 13, 37 }, { 14, 41 }
        };

        if (GameManager.Instance.GameSetup.TrackIndex == 0)
        {
            if (checkpointToWaypointMap_Hungaroring.TryGetValue(lastCheckpointPassed, out int waypointIndex))
            {
                CurrentWaypoint = waypointIndex;
            }
        }
        else if (GameManager.Instance.GameSetup.TrackIndex == 1)
        {
            if (checkpointToWaypointMap_BrandsHatch.TryGetValue(lastCheckpointPassed, out int waypointIndex))
            {
                CurrentWaypoint = waypointIndex;
            }
        }
        else
        {
            if (checkpointToWaypointMap_Interlagos.TryGetValue(lastCheckpointPassed, out int waypointIndex))
            {
                CurrentWaypoint = waypointIndex;
            }
        }
    }

    public int GetRacePosition()
    {
        return Player.RacePosition;
    }

    private void UpdateCurrentLapTime()
    {
        Player.CurrentLapTime = Player.LapTimerTimeStamp > 0 ? Time.time - Player.LapTimerTimeStamp : 0;
    }

    private void HumanControl()
    {
        if (!GameManager.Instance.CountdownState)
        {
            CarController.Throttle = GameManager.Instance.InputController.ThrottleInput;
            CarController.Steer = GameManager.Instance.InputController.SteerInput;
            CarController.Brake = GameManager.Instance.InputController.BrakeInput;
        }
    }

    private void AIControl()
    {
        SettingInputAI();

        //Setting AI car inputs
        CarController.Steer = CalculateSteerAngle();
        CarController.Throttle = GasInput;
        CarController.Brake = BrakeInput;

        //Visualization
        Debug.DrawRay(transform.position, Waypoints[CurrentWaypoint].position - transform.position, Color.yellow);
    }

    private void SettingInputAI()
    {
        if (!GameManager.Instance.CountdownState)
        {
            if (IsInsideBraking)
            {
                GasInput = 0;
                BrakeInput = true;
            }
            else
            {
                float targetGasInput = Mathf.Clamp01((1f - Mathf.Abs(CarController.CurrentSpeed * 0.01f * CurrentAngle) / 20f));
                float gasIncreaseSpeed = 0.8f;
                // Gradually move gasInput toward the target value
                GasInput = Mathf.MoveTowards(GasInput, targetGasInput, gasIncreaseSpeed * Time.deltaTime);

                //gasInput = Mathf.Clamp01((1f - Mathf.Abs(carController.currentSpeed * 0.01f * currentAngle) / 20f));
                BrakeInput = false;
            }
        }
    }

    private void FollowWaypoints()
    {
        if (UnityEngine.Vector3.Distance(Waypoints[CurrentWaypoint].position, transform.position) < WaypointRange)
        {
            CurrentWaypoint++;
            if (CurrentWaypoint == Waypoints.Count)
            {
                CurrentWaypoint = 0;
            }
        }
    }

    private void CheckIfStuck()
    {
        // If the car hasn't moved significantly and is below the speed threshold
        if ((transform.position - LastPosition).magnitude < 0.1f && CarRigidbody.velocity.magnitude < SpeedThreshold)
        {
            StuckTime += Time.deltaTime;
            if (StuckTime >= ResetThreshold && !IsResetting)
            {
                //Debug.Log("Car is stuck. Resetting...");
                ResetToLastCheckpoint();
                UpdateAIWaypoint(Player.LastCheckpointPassed);
            }
        }
        else
        {
            StuckTime = 0f; // Reset the stuck timer if the car moves
        }

        LastPosition = transform.position;
    }

    private void CheckIfInBoostZone()
    {
        if (GameManager.Instance.GameSetup.TrackIndex == 0)
        {
            if (BoostWaypoints_Hungaroring.Contains(CurrentWaypoint) && !IsBoosting)
            {
                StartCoroutine(ApplyBoost(1.4f));
            }
        }
        else if (GameManager.Instance.GameSetup.TrackIndex == 1)
        {

        }
        else
        {
            if (BoostWaypoints_Interlagos.Contains(CurrentWaypoint) && !IsBoosting)
            {
                StartCoroutine(ApplyBoost(1.4f));
            }
        }
    }
    private void CheckIfLiftIsNecessary()
    {
        if (GameManager.Instance.GameSetup.TrackIndex == 0)
        {
            if (Player.CarType == 2 && GameManager.Instance.GameSetup.Difficulty == "Medium" && CurrentWaypoint == 23) {
                StartCoroutine(ApplyGasList(0.8f));
            }

            if (LiftWaypoints_Hungaroring.Contains(CurrentWaypoint) && !IsLifting)
            {
                if (Player.CarType == 2)
                {
                    StartCoroutine(ApplyGasList(0.9f));
                }
                else if (Player.CarType == 3) {
                    StartCoroutine(ApplyGasList(0.8f));
                }
                
            }
        }
        else if (GameManager.Instance.GameSetup.TrackIndex == 1)
        {

        }
        else
        {
            if (LiftWaypoints_Interlagos.Contains(CurrentWaypoint) && !IsLifting)
            {
                if (Player.CarType == 3)
                {
                    StartCoroutine(ApplyGasList(0.8f));
                }
            }
        }
    }


    private float CalculateSteerAngle()
    {
        // Get the position of the car and the next waypoint
        Vector3 carPosition = transform.position;
        Vector3 waypointPosition = Waypoints[CurrentWaypoint].position;

        // Calculate the direction to the next waypoint
        Vector3 directionToWaypoint = (waypointPosition - carPosition).normalized;

        // Calculate the angle between the car's forward direction and the direction to the waypoint
        float angleToWaypoint = Vector3.SignedAngle(transform.forward, directionToWaypoint, Vector3.up);

        // Return the normalized steering angle (-1 to 1)
        return Mathf.Clamp(angleToWaypoint / 20f, -1f, 1f);
    }

    private IEnumerator ApplyBoost(float boost)
    {
        if (Player.CarType == 1 && (GameManager.Instance.GameSetup.Difficulty == "Easy" || GameManager.Instance.GameSetup.Difficulty == "Medium"))
        {
            IsBoosting = true;

            float originalMaxSpeed = CarController.MaxSpeed;
            CarController.MaxSpeed = CarController.MaxSpeed * boost; // Temporarily increase max speed

            yield return new WaitForSeconds(2f); // Wait for the boost duration

            CarController.MaxSpeed = originalMaxSpeed; // Reset to original speed
            IsBoosting = false;
        }

    }

    private IEnumerator ApplyGasList(float lift)
    {
        if (Player.CarType != 0)
        {
            if (IsLifting) yield break;

            IsLifting = true;

            float originalMaxSpeed = CarController.MaxSpeed;
            CarController.MaxSpeed = CarController.MaxSpeed * lift; // Temporarily increase max speed

            yield return new WaitForSeconds(2f); // Wait for the boost duration

            CarController.MaxSpeed = originalMaxSpeed; // Reset to original speed
            IsLifting = false;
        }
    }
}
