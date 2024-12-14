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

    public GameObject RestartButton;
    public GameObject BackToMenuButton;

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

    public List<Transform> AssignedPrefabs = new List<Transform>();
    public List<Transform> Waypoints => WaypointContainer.Waypoints;
    public List<Transform> GridPositions => GridContainer.GridPositions;
    public List<Transform> CarObjects => new List<Transform>();

    private Dictionary<int, string> PrefabIndexesForAICars = new Dictionary<int, string>();

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
        Debug.Log($"ID: {GameSetup.Id}, Date: {GameSetup.DateTime}, Track: {GameSetup.TrackName}, CarIndex: {GameSetup.CarIndex}, Race distance: {GameSetup.RaceDistance}, No of players: {GameSetup.NumberOfPlayers}, Difficulty: {GameSetup.Difficulty}, Player name: {GameSetup.PlayerName}");
    }

    private IEnumerator CountdownRoutine()
    {
        CountdownPanel.SetActive(true);
        CountdownState = true;

        AudioManager.PlaySFX("Countdown");

        float realTimeDelay = CountdownDelay; 
        int countdownNumber = 3;

        // Display countdown numbers
        while (countdownNumber > 0)
        {
            CountdownText.text = countdownNumber.ToString(); 
            yield return new WaitForSecondsRealtime(realTimeDelay); 
            countdownNumber--;
        }

        
        CountdownText.text = "GO!";

        yield return new WaitForSecondsRealtime(realTimeDelay); 

        CountdownPanel.SetActive(false);

        CountdownState = false; 
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
        if (RaceCompleted) return; 

        
        foreach (Transform t in PlayerCars)
        {
            if (t.GetComponent<PlayerController>().Player.Finished)
            {
                StartCoroutine(AudioManager.FadeOutAudio(t.GetComponent<AudioSource>(), 1f));
            }
        }

        if (Players.All(player => player.Finished))
        {
            AudioManager.StopMusic("Game_Theme");
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
        
        if (UIController != null)
        {
            UIController.ShowPlaces();
            UIController.UIRacePanel.SetActive(false);
            UIController.UIResultPanel.gameObject.SetActive(true);
            ShowButtonsWithDelay(2);
            for (int i = 0; i < FinishedPlayers.Count; i++)
            {
                Debug.Log("Place: " + i + " | Player name" + FinishedPlayers[i].Name + " | Total time: " + FinishedPlayers[i].TotalTime + " | " + "Gap: " + FinishedPlayers[i].Gap + " | " + "Penalty: " + FinishedPlayers[i].PenaltyTime);
            }

        }
    }

    public void ShowButtonsWithDelay(float delay)
    {
        StartCoroutine(DelayedButtons(delay));
    }

    private IEnumerator DelayedButtons(float delay)
    {
        
        yield return new WaitForSeconds(delay);

        
        RestartButton.SetActive(true);
        BackToMenuButton.SetActive(true);

        
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

        //Debug.Log(playerStartPosition);

        Transform playerGridSlot = GridPositions[playerStartPosition];

        SpawnPlayerCar(playerGridSlot, playerStartPosition + 1);


        
        for (int i = 0; i < GameSetup.NumberOfPlayers; i++)
        {
            if (i == playerStartPosition) continue; 

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
        if (CarPrefabs.Count == 0) return; 

        
        if (GameSetup.CarIndex < 0 || GameSetup.CarIndex >= CarPrefabs.Count)
        {
            Debug.LogError("Invalid CarIndex in GameSetup.");
            return;
        }

        Transform chosenCarPrefab = CarPrefabs[GameSetup.CarIndex];

        AssignedPrefabs.Add(chosenCarPrefab);

        
        ChosenCarObject = Instantiate(chosenCarPrefab.gameObject, gridSlot.position, gridSlot.rotation);

        Player player = new Player(GameSetup.PlayerName, ChosenCarObject.transform, Player.ControlType.HumanInput);
        player.GridPosition = gridpos;
        Players.Add(player);

        var playerController = ChosenCarObject.GetComponent<PlayerController>() ?? ChosenCarObject.AddComponent<PlayerController>();
        playerController.Initialize(player);

        Rigidbody rb = ChosenCarObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

       
        PlayerCars.Add(ChosenCarObject.transform);
        SpawnedCars.Add(ChosenCarObject); 
        CarObjects.Add(ChosenCarObject.transform); 


    }
    

    private void SpawnRandomAICar(Transform gridSlot, int gridpos)
    {
        if (CarPrefabs.Count == 0) return; 

        
        int randomIndex = 1;

        float maxSpeedMultiplier = 1f; 

        switch (GameSetup.Difficulty)
        {
            case "Easy":
                randomIndex = 0; 
                maxSpeedMultiplier = 0.8f; 
                break;

            case "Medium":
                
                randomIndex = Random.Range(0, 10) < 7 ? 1 : 0;
                maxSpeedMultiplier = 0.9f; 
                break;

            case "Hard":
                randomIndex = 2; 
                maxSpeedMultiplier = 1f; 
                break;
        }

        Transform randomCarPrefab = CarPrefabs[randomIndex];

        AssignedPrefabs.Add(randomCarPrefab);

        
        GameObject aiCar = Instantiate(randomCarPrefab.gameObject, gridSlot.position, gridSlot.rotation);
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

        if (playerController.CarController != null)
        {
            playerController.CarController.MaxSpeed *= maxSpeedMultiplier;
        }

        CarObjects.Add(aiCar.transform);
        SpawnedCars.Add(aiCar); 
    }

    void RandomizeCarOrder()
    {
        System.Random random = new System.Random(); 
        for (int i = CarObjects.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(0, i + 1);
            
            Transform temp = CarObjects[i];
            CarObjects[i] = CarObjects[randomIndex];
            CarObjects[randomIndex] = temp;
        }

        //Debug.Log("Cars shuffled for grid placement.");
    }

    public void RestartRace()
    {
        
        foreach (var car in SpawnedCars)
        {
            Destroy(car); 
        }

        
        SpawnedCars.Clear();
        PlayerCars.Clear();
        Players.Clear();
        CarObjects.Clear();
        FinishedPlayers.Clear();
        AINumber = 0; 

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

        
        for (int i = 0; i < GameSetup.NumberOfPlayers; i++)
        {
            if (i == playerStartPosition) continue;

            Transform gridSlot = GridPositions[i];
            Transform aiCarType = AssignedPrefabs[i]; 
            SpawnSpecificAICar(aiCarType, gridSlot, i + 1);
        }
    }

    private void SpawnSpecificAICar(Transform carPrefab, Transform gridSlot, int gridpos)
    {
        
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
        
        Players.Sort((a, b) =>
        {
            
            if (a.CurrentLap != b.CurrentLap)
                return b.CurrentLap.CompareTo(a.CurrentLap);

            
            if (a.LastCheckpointPassed != b.LastCheckpointPassed)
                return b.LastCheckpointPassed.CompareTo(a.LastCheckpointPassed);

            
            Transform nextCheckpointA = GetNextCheckpoint(a);
            Transform nextCheckpointB = GetNextCheckpoint(b);

            float distanceA = Vector3.Distance(a.CarTransform.position, nextCheckpointA.position);
            float distanceB = Vector3.Distance(b.CarTransform.position, nextCheckpointB.position);

            return distanceA.CompareTo(distanceB);
        });

       
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

        
        float leaderTime = FinishedPlayers[0].TotalTime;

        
        for (int i = 0; i < FinishedPlayers.Count; i++)
        {
            Player player = FinishedPlayers[i];

            
            player.Gap = player.TotalTime - leaderTime;

            
            Debug.Log($"Player {player.Name} Gap: {player.Gap:F2} seconds");
        }
    }

    public void ReturnToMainMenu()
    {
        
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
