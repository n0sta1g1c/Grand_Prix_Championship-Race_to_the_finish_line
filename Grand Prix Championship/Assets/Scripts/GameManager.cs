using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public InputController InputController { get; private set; }
    public AudioManager AudioManager { get; private set; }
    public UIController UIController { get; private set; }
    public Camera PlayerCamera { get; set; }
    public Camera ResultCamera { get; set; }

    public GameObject ChosenCarObject;

    public GameSetup GameSetup { get; set; }

    public bool GameIsPaused = false;
    public GameObject PauseMenuPanel;

    public float CountdownDelay = 1f;
    public GameObject CountdownPanel;
    public TMP_Text CountdownText;
    public bool CountdownState = true;
    public List<Transform> Checkpoints { get; private set; }
    public int CheckpointLayer { get; private set; }

    [SerializeField] private Transform CheckpointParent;

    [SerializeField] private WaypointContainer WaypointContainer;
    [SerializeField] private GridContainer GridContainer;
    [SerializeField] private CarContainer CarContainer;

    public bool ResultCameraIsShown = false;

    public int AINumber = 1;

    public bool RaceCompleted = false;
    public bool ChallengeCompleted = false;

    [SerializeField] public List<Player> Players = new List<Player>();

    public List<Transform> PlayerCars = new List<Transform>();

    public List<Transform> CarPrefabs;

    private List<GameObject> SpawnedCars = new List<GameObject>();

    public List<Player> FinishedPlayers = new List<Player>();

    private List<Transform> AssignedAICarTypes = new List<Transform>();
    public List<Transform> Waypoints => WaypointContainer.Waypoints;
    public List<Transform> GridPositions => GridContainer.GridPositions;
    //public List<Transform> CarObjects => CarContainer.CarObjects;
    public List<Transform> CarObjects => new List<Transform>();

    void Awake()
    {
        Instance = this;
        PlayerCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        ResultCamera = GameObject.Find("ResultCamera").GetComponent<Camera>();
        ResultCamera.gameObject.SetActive(false);
        InputController = GetComponentInChildren<InputController>();
        AudioManager = GetComponentInChildren<AudioManager>();
        UIController = GetComponentInChildren<UIController>();
        SetupPlayerPrefs();
        SetupCheckpoints();
    }

    private void Start()
    {
        SetGameMode();
        StartCoroutine(CountdownRoutine());
    }

    private void Update()
    {
        CheckIfGameIsPaused();

        if (GameSetup.GameMode == 0)
        {
            UpdatePlayerPositions();
            CheckIfRaceIsOver();
            CheckIfPlayerHasFinished();
        }
        else {
            CheckIfPlayerBeatTimeChallenge();
        }
        
    }

    private void SetupPlayerPrefs()
    {
        int RaceDistance = PlayerPrefs.GetInt("RaceDistance");
        string Difficulty = PlayerPrefs.GetString("Difficulty");
        int GameMode = PlayerPrefs.GetInt("GameMode");
        int NumberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers"); ;
        int CarIndex = PlayerPrefs.GetInt("CarIndex");
        int TrackIndex = PlayerPrefs.GetInt("TrackIndex");
        string PlayerName = PlayerPrefs.GetString("PlayerName");

        this.GameSetup = new GameSetup(DateTime.Now, CarIndex, TrackIndex, GameMode, RaceDistance, NumberOfPlayers, Difficulty, PlayerName);
        Debug.Log(GameSetup.TimeToBeat);
        Debug.Log($"ID: {GameSetup.Id}, Date: {GameSetup.DateTime}, Track: {GameSetup.TrackName}, CarIndex: {GameSetup.CarIndex}, Race distance: {GameSetup.RaceDistance}, No of players: {GameSetup.NumberOfPlayers}, Difficulty: {GameSetup.Difficulty}, Player name: {GameSetup.PlayerName}");
    }

    private IEnumerator CountdownRoutine()
    {
        CountdownPanel.SetActive(true);
        CountdownState = true;

        AudioManager.PlaySFX("Countdown");

        float realTimeDelay = CountdownDelay; // Time between each countdown step
        int countdownNumber = 3;

        // Display countdown numbers
        while (countdownNumber > 0)
        {
            CountdownText.text = countdownNumber.ToString(); // Display the number
            yield return new WaitForSecondsRealtime(realTimeDelay); // Wait in real time
            countdownNumber--;
        }

        // Display "GO!" and release brakes
        CountdownText.text = "GO!";

        yield return new WaitForSecondsRealtime(realTimeDelay); // Allow "GO!" to display briefly

        // Hide the countdown UI
        CountdownPanel.SetActive(false);

        CountdownState = false; // Signal that the countdown is finished
    }

    private void SetGameMode()
    {

        if (GameSetup.GameMode == 0)
        {
            InitializeRace();
        }
        else
        {
            InializeTimeTrial();
        }

    }

    public void CheckIfGameIsPaused()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        AudioManager.transform.GetChild(0).GetComponent<AudioSource>().volume = 0.5f;
        UIController.HidePauseMenu(GameSetup.GameMode);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void PauseGame()
    {
        AudioManager.transform.GetChild(0).GetComponent<AudioSource>().volume = 0.25f;
        UIController.ShowPauseMenu(GameSetup.GameMode);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    private void CheckIfRaceIsOver()
    {
        if (RaceCompleted) return; // Avoid repeating the end logic

        // Check if all players have finished
        foreach (Transform t in PlayerCars)
        {
            if (t.GetComponent<PlayerController>().Player.Finished)
            {
                StartCoroutine(AudioManager.FadeOutAudio(t.GetComponent<AudioSource>(), 2f));
            }
        }

        if (Players.All(player => player.Finished))
        {
            AudioManager.StopMusic("Game_theme");
            AudioManager.PlaySFX("Game_Over");
            RaceCompleted = true;
            SortEndResult();
            ShowRaceResults();
        }
    }

    public void CheckIfPlayerBeatTimeChallenge() {

        if (ChallengeCompleted) return;

        if (ChosenCarObject.GetComponent<PlayerController>().Player.BestLapTime < GameSetup.TimeToBeat) {
            ChallengeCompleted = true;
            UIController.ShowCompletedPanel();
        }
    }

    private void CheckIfPlayerHasFinished()
    {
        if (!ResultCameraIsShown)
        {
            foreach (Player p in Players)
            {
                if (p.controlType == Player.ControlType.HumanInput && p.Finished)
                {
                    PlayerCamera.gameObject.SetActive(false);
                    ResultCamera.gameObject.SetActive(true);
                    ResultCameraIsShown = true;
                }
            }
        }

    }

    private void ShowRaceResults()
    {
        // Enable the results canvas
        if (UIController != null)
        {
            UIController.ShowPlaces();
            UIController.UIRacePanel.SetActive(false);
            UIController.UIResultPanel.gameObject.SetActive(true);
            for (int i = 0; i < FinishedPlayers.Count; i++)
            {
                Debug.Log("Place: " + i + " | Player name" + FinishedPlayers[i].Name + " | Total time: " + FinishedPlayers[i].TotalTime + " | " + "Gap: " + FinishedPlayers[i].Gap + " | " + "Penalty: " + FinishedPlayers[i].PenaltyTime);
            }

        }
    }

    private void SetupCheckpoints()
    {
        if (CheckpointParent == null)
        {
            CheckpointParent = GameObject.Find("Checkpoints").transform;
        }

        int checkpointCount = CheckpointParent.childCount;
        Checkpoints = new List<Transform>(checkpointCount);

        for (int i = 0; i < checkpointCount; i++)
        {
            Checkpoints.Add(CheckpointParent.GetChild(i));
        }

        CheckpointLayer = LayerMask.NameToLayer("Checkpoint");

        // Debug.Log("Checkpoints initialized.");
    }

    private void InitializeRace()
    {

        UIController.SetUIForGameMode(GameSetup.GameMode);

        int playerStartPosition = 0;

        // Determine the player's starting position based on difficulty
        switch (GameSetup.Difficulty)
        {
            case "Easy":
                playerStartPosition = 0; // Start in the first grid slot
                break;
            case "Medium":
                playerStartPosition = 3; // Start in the 5th grid slot
                break;
            case "Hard":
                playerStartPosition = 7; // Start in the last grid slot
                break;
        }

        //Debug.Log(playerStartPosition);

        Transform playerGridSlot = GridPositions[playerStartPosition];

        SpawnPlayerCar(playerGridSlot, playerStartPosition + 1);


        // Spawn AI cars in the remaining positions
        for (int i = 0; i < GameSetup.NumberOfPlayers; i++)
        {
            if (i == playerStartPosition) continue; // Skip the player's position

            Transform gridSlot = GridPositions[i];
            SpawnRandomAICar(gridSlot, i + 1);
            //RandomizeCarOrder();
        }
    }

    private void InializeTimeTrial()
    {
        UIController.SetUIForGameMode(GameSetup.GameMode);

        SpawnPlayerCar(GridPositions[7], 8);
    }

    private void SpawnPlayerCar(Transform gridSlot, int gridpos)
    {
        if (CarPrefabs.Count == 0) return; // Ensure prefabs exist

        // Get the car prefab based on GameSetup.CarIndex
        if (GameSetup.CarIndex < 0 || GameSetup.CarIndex >= CarPrefabs.Count)
        {
            Debug.LogError("Invalid CarIndex in GameSetup.");
            return;
        }

        Transform chosenCarPrefab = CarPrefabs[GameSetup.CarIndex];

        // Instantiate the car at the grid slot
        ChosenCarObject = Instantiate(chosenCarPrefab.gameObject, gridSlot.position, gridSlot.rotation);

        //playerCar = ChosenCarObject;

        // Create the Player object
        Player player = new Player(GameSetup.PlayerName, ChosenCarObject.transform, Player.ControlType.HumanInput);
        player.GridPosition = gridpos;
        Players.Add(player);

        // Ensure the car is active
        //playerCar.SetActive(true);

        // Initialize PlayerController
        var playerController = ChosenCarObject.GetComponent<PlayerController>() ?? ChosenCarObject.AddComponent<PlayerController>();
        playerController.Initialize(player);

        // Reset physics properties
        Rigidbody rb = ChosenCarObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Add the car to tracking lists
        PlayerCars.Add(ChosenCarObject.transform);
        SpawnedCars.Add(ChosenCarObject); // Track the spawned player car
        CarObjects.Add(ChosenCarObject.transform); // Add to the general car list

        //Debug.Log($"{player.Name}, {player.CarTransform.name}, Car type: {player.CarType}, Grid Position: {gridpos}");
    }

    private void SpawnRandomAICar(Transform gridSlot, int gridpos)
    {
        if (CarPrefabs.Count == 0) return; // Ensure prefabs exist

        // Select a random prefab
        //int randomIndex = Random.Range(0, CarPrefabs.Count);
        int randomIndex = 1;

        float maxSpeedMultiplier = 1f; // Default to no speed reduction

        switch (GameSetup.Difficulty)
        {
            case "Easy":
                randomIndex = 0; // Always choose cars from index 0
                maxSpeedMultiplier = 0.8f; // Reduce max speed to 90%
                break;

            case "Medium":
                // 70% chance to choose index 1, 30% chance for index 0
                randomIndex = Random.Range(0, 10) < 7 ? 1 : 0;
                maxSpeedMultiplier = 0.9f; // Reduce max speed to 95%
                break;

            case "Hard":
                randomIndex = 2; // Always choose cars from index 2
                maxSpeedMultiplier = 1f; // No speed reduction
                break;
        }

        Transform randomCarPrefab = CarPrefabs[randomIndex];

        AssignedAICarTypes.Add(randomCarPrefab);

        // Instantiate the car
        GameObject aiCar = Instantiate(randomCarPrefab.gameObject, gridSlot.position, gridSlot.rotation);
        //aiCar.name = $"AI_{AINumber}";
        Player aiPlayer = new Player($"AI_{AINumber}", aiCar.transform);
        //Debug.Log(aiPlayer.Name + ", " + aiPlayer.CarTransform.name + ", Car tpye: " + aiPlayer.CarType);
        aiPlayer.GridPosition = gridpos;

        AudioListener aiAudioListener = aiCar.GetComponent<AudioListener>();
        if (aiAudioListener != null)
        {
            Destroy(aiAudioListener);
        }

        PlayerCars.Add(aiCar.transform);
        Players.Add(aiPlayer);
        AINumber++;

        // Initialize PlayerController
        var playerController = aiCar.GetComponent<PlayerController>() ?? aiCar.AddComponent<PlayerController>();
        playerController.Initialize(aiPlayer);

        if (playerController.CarController != null)
        {
            playerController.CarController.MaxSpeed *= maxSpeedMultiplier;
        }

        CarObjects.Add(aiCar.transform);
        SpawnedCars.Add(aiCar); // Track the spawned car
    }

    void RandomizeCarOrder()
    {
        System.Random random = new System.Random(); // Random number generator
        for (int i = CarObjects.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(0, i + 1);
            // Swap the cars
            Transform temp = CarObjects[i];
            CarObjects[i] = CarObjects[randomIndex];
            CarObjects[randomIndex] = temp;
        }

        //Debug.Log("Cars shuffled for grid placement.");
    }

    public void RestartRace()
    {
        // Clear existing cars
        foreach (var car in SpawnedCars)
        {
            Destroy(car); // Destroy the car GameObjects
        }

        // Clear all runtime data, except the assigned car types
        SpawnedCars.Clear();
        PlayerCars.Clear();
        Players.Clear();
        CarObjects.Clear();
        FinishedPlayers.Clear();
        AINumber = 0; // Reset AI numbering

        RaceCompleted = false;

        UIController.ClearPlaces();
        UIController.UIRacePanel.SetActive(true);

        StartCoroutine(CountdownRoutine());

        // Reinitialize the race, reusing assigned car types
        ReinitializeRace();
        UIController.SetUIForGameMode(GameSetup.GameMode);
        PlayerCamera.gameObject.SetActive(true);
        ResultCamera.gameObject.SetActive(false);
        ResultCameraIsShown = false;
        PlayerCamera.GetComponent<CameraFollow>().SetPlayerReference(ChosenCarObject);
        UIController.SetPlayerReference(ChosenCarObject);
    }

    private void ReinitializeRace()
    {
        int playerStartPosition = 0;

        // Determine the player's starting position based on difficulty
        switch (GameSetup.Difficulty)
        {
            case "Easy":
                playerStartPosition = 0;
                break;
            case "Medium":
                playerStartPosition = 3;
                break;
            case "Hard":
                playerStartPosition = 7;
                break;
        }

        Transform playerGridSlot = GridPositions[playerStartPosition];
        if (ChosenCarObject != null)
        {
            SpawnPlayerCar(playerGridSlot, playerStartPosition + 1);
        }

        // Respawn AI cars with the saved types
        for (int i = 0; i < GameSetup.NumberOfPlayers; i++)
        {
            if (i == playerStartPosition) continue;

            Transform gridSlot = GridPositions[i];
            Transform aiCarType = AssignedAICarTypes[i]; // Use the previously assigned type
            SpawnSpecificAICar(aiCarType, gridSlot, i + 1);
        }
    }

    private void SpawnSpecificAICar(Transform carPrefab, Transform gridSlot, int gridpos)
    {
        // Instantiate the specific car prefab
        GameObject aiCar = Instantiate(carPrefab.gameObject, gridSlot.position, gridSlot.rotation);

        Player aiPlayer = new Player($"AI_{AINumber}", aiCar.transform);
        aiPlayer.GridPosition = gridpos;

        AudioListener aiAudioListener = aiCar.GetComponent<AudioListener>();
        if (aiAudioListener != null)
        {
            Destroy(aiAudioListener);
        }

        PlayerCars.Add(aiCar.transform);
        Players.Add(aiPlayer);
        AINumber++;

        var playerController = aiCar.GetComponent<PlayerController>() ?? aiCar.AddComponent<PlayerController>();
        playerController.Initialize(aiPlayer);

        CarObjects.Add(aiCar.transform);
        SpawnedCars.Add(aiCar);
    }


    void UpdatePlayerPositions()
    {
        // Sort players by lap, checkpoint progress, and distance to next checkpoint
        Players.Sort((a, b) =>
        {
            // First, compare lap numbers (higher lap = better position)
            if (a.CurrentLap != b.CurrentLap)
                return b.CurrentLap.CompareTo(a.CurrentLap);

            // Then, compare checkpoints passed (higher = better position)
            if (a.LastCheckpointPassed != b.LastCheckpointPassed)
                return b.LastCheckpointPassed.CompareTo(a.LastCheckpointPassed);

            // Finally, compare distance to the next checkpoint (shorter distance = better position)
            Transform nextCheckpointA = GetNextCheckpoint(a);
            Transform nextCheckpointB = GetNextCheckpoint(b);

            float distanceA = Vector3.Distance(a.CarTransform.position, nextCheckpointA.position);
            float distanceB = Vector3.Distance(b.CarTransform.position, nextCheckpointB.position);

            return distanceA.CompareTo(distanceB);
        });

        // Update each player's position
        for (int i = 0; i < Players.Count; i++)
        {
            Players[i].RacePosition = i + 1;
        }
    }

    Transform GetNextCheckpoint(Player player)
    {
        int nextCheckpointIndex = player.LastCheckpointPassed % Checkpoints.Count;
        return Checkpoints[nextCheckpointIndex];
    }

    void SortEndResult()
    {
        FinishedPlayers.Sort((player1, player2) => player1.TotalTime.CompareTo(player2.TotalTime));
        CalculateGap();
    }
    void CalculateGap()
    {
        if (FinishedPlayers == null || FinishedPlayers.Count == 0)
        {
            Debug.LogWarning("Finished player list is empty or null. Cannot calculate gaps.");
            return;
        }

        // Get the total time of the leader (first player in the list)
        float leaderTime = FinishedPlayers[0].TotalTime;

        // Loop through all players and calculate the gap
        for (int i = 0; i < FinishedPlayers.Count; i++)
        {
            Player player = FinishedPlayers[i];

            // Calculate the gap relative to the leader
            player.Gap = player.TotalTime - leaderTime;

            // Debug log for verification
            Debug.Log($"Player {player.Name} Gap: {player.Gap:F2} seconds");
        }
    }

    public void ReturnToMainMenu()
    {
        // Optionally, reset any persistent data here.
        Time.timeScale = 1.0f;
        ResetPlayerPrefs();
        SceneManager.LoadScene("MainMenu");
    }

    private void ResetPlayerPrefs()
    {
        PlayerPrefs.SetInt("TrackIndex", 0);
        PlayerPrefs.SetInt("CarIndex", 0);
        PlayerPrefs.SetInt("GameMode", 0);
        PlayerPrefs.SetInt("RaceDistance", 3);
        PlayerPrefs.SetInt("NumberOfPlayers", 4);
        PlayerPrefs.SetString("Difficulty", "Medium");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
