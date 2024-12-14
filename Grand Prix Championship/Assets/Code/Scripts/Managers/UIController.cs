using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();


    public GameObject UIRacePanel;
    public GameObject UIResultPanel;
    public GameObject PlacesPanel;
    public GameObject LaptimesPanel;
    public GameObject OptionsPanel;
    public GameObject PausePanel;
    public GameObject WaitingPanel;
    public GameObject MainPauseMenu;
    public GameObject HUD;
    public GameObject CompletedChallengePanel;

    public Text UITextCurrentLap;
    public Text UITextCurrentTime;
    public Text UITextLastTime;
    public Text UITextBestLapTime;
    public Text UITextCurrentPostion;
    public Text UITextAllPostion;
    public Text UITextTimeToBeat;

    public Image WarningCurrImage;
    public Image WarningLastImage;

    public GameObject PlayerCar;

    public GameObject RestartButton;
    public GameObject BackToMainMenuButton;

    private Player Player;
    private PlayerController PlayerController;

    private int CurrentLap = -1;
    private float CurrentLapTime;
    private float LastLapTime;
    private float BestLapTime;

    private bool LaptimesUpdated = false;

    private int RaceLaps;

    private void Awake()
    {
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
        WarningCurrImage.enabled = false;
        WarningLastImage.enabled = false;

        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
    }

    public void SetPlayerReference(GameObject playerCar)
    {
        PlayerCar = playerCar;
        PlayerController = PlayerCar.GetComponent<PlayerController>();
        Player = PlayerController.Player;
    }

    private void Start()
    {
        RaceLaps = GameManager.Instance.GameSetup.RaceDistance;
        SetPlayerReference(GameManager.Instance.ChosenCarObject);
        SetTimeToBeat();
    }
    void Update()
    {
        UpdateRacePanel();
        CheckIfPlayerHasFinished();
    }

    public void ShowAudioSettings()
    {
        PausePanel.SetActive(false);
        OptionsPanel.SetActive(true);

        ResetButtonScales(OptionsPanel);
    }

    public void HideAudioSettings()
    {
        PausePanel.SetActive(true);
        OptionsPanel.SetActive(false);

        ResetButtonScales(PausePanel);
    }

    public void CheckIfPlayerHasFinished()
    {
        if (GameManager.Instance.ResultCamera.isActiveAndEnabled)
        {
            WaitingPanel.SetActive(true);
        }
    }

    void UpdateRacePanel()
    {
        if (PlayerController == null || PlayerController.Player == null)
        {
            Debug.LogWarning("PlayerController or player is null.");
            return;
        }

        UpdatePlayerPosition();
        UpdateWarnings();
        UpdateLapInfo();
        UpdateLapTimes();
    }

    public void ShowPauseMenu(int gameMode) {
        if (gameMode == 0)
        {
            MainPauseMenu.SetActive(true);
            MainPauseMenu.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
        else {
            MainPauseMenu.SetActive(true);
            MainPauseMenu.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        }
    }

    public void HidePauseMenu(int gameMode) {
        if (gameMode == 0)
        {
            MainPauseMenu.SetActive(false);
            MainPauseMenu.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            MainPauseMenu.SetActive(false);
            MainPauseMenu.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
    }

    public void SetUIForGameMode(int gameMode)
    {
        if (gameMode == 0)
        {
            HUD.transform.GetChild(0).gameObject.SetActive(true);
            HUD.transform.GetChild(1).gameObject.SetActive(true);
            HUD.transform.GetChild(2).gameObject.SetActive(false);
        }
        else {

            HUD.transform.GetChild(0).gameObject.SetActive(true);
            HUD.transform.GetChild(1).gameObject.SetActive(false);
            HUD.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    private void UpdatePlayerPosition()
    {
        UITextAllPostion.text = GameManager.Instance.Players.Count.ToString();

        
        if (Player.controlType == Player.ControlType.HumanInput)
        {
            if (GameManager.Instance.RaceCompleted)
            {
                int finishPosition = GameManager.Instance.FinishedPlayers.IndexOf(Player) + 1;
                UITextCurrentPostion.text = finishPosition > 0 ? finishPosition.ToString() : "-";
            }
            else
            {
                UITextCurrentPostion.text = Player.RacePosition.ToString();
            }
        }
    }

    private void UpdateWarnings()
    {
        WarningCurrImage.enabled = !PlayerController.Player.ValidLap;
        WarningLastImage.enabled = !PlayerController.Player.ValidLastLap;
    }

    public void SetTimeToBeat() {
        UITextTimeToBeat.text = FormatTime(GameManager.Instance.GameSetup.TimeToBeat);
    }

    public void ShowCompletedPanel(){
        if (GameManager.Instance.ChallengeCompleted)
        {
            CompletedChallengePanel.gameObject.SetActive(true);
        }
    }

    private void UpdateLapInfo()
    {
        

        if (GameManager.Instance.GameSetup.GameMode == 0)
        {
            if (Player.CurrentLap != CurrentLap)
            {
                CurrentLap = Player.CurrentLap;
                UITextCurrentLap.text = $"{CurrentLap}/{RaceLaps}";
            }
        }
        else {
            CurrentLap = Player.CurrentLap;
            UITextCurrentLap.text = $"{CurrentLap}";
        }
        
    }

    private void UpdateLapTimes()
    {
        

        if (Player.CurrentLapTime != CurrentLapTime)
        {
            CurrentLapTime = Player.CurrentLapTime;
            UITextCurrentTime.text = FormatTime(CurrentLapTime);
            WarningCurrImage.enabled = !Player.ValidLap;
        }

        if (Player.LastLapTime != LastLapTime)
        {
            LastLapTime = Player.LastLapTime;
            UITextLastTime.text = FormatTime(LastLapTime);
            WarningLastImage.enabled = !Player.ValidLastLap;

            UpdateLastLapColor();
        }

        if (Player.BestLapTime != BestLapTime && Player.ValidLap)
        {
            BestLapTime = Player.BestLapTime;
            UITextBestLapTime.text = BestLapTime < 1000000 ? FormatTime(BestLapTime) : "-";
        }
    }

    private void UpdateLastLapColor()
    {
        if (LastLapTime < BestLapTime && CurrentLap > 0)
        {
            UITextLastTime.color = Color.magenta; // New best lap
        }
        else if (LastLapTime > BestLapTime)
        {
            UITextLastTime.color = Color.red; // Worse than best lap
        }
    }

    private string FormatTime(float time)
    {
        return $"{(int)time / 60}:{time % 60:00.000}";
    }

    private void DisplayLapTimes(Player player, bool isPlayerControlled = false)
    {
        TMP_Text panelPlayerNameText = LaptimesPanel.transform.GetChild(0).GetComponent<TMP_Text>();
        TMP_Text panelLapTimesText = LaptimesPanel.transform.GetChild(2).GetComponent<TMP_Text>();
        TMP_Text panelTotalTimeText = LaptimesPanel.transform.GetChild(3).GetComponent<TMP_Text>();

        string lapTimesText = "";
        foreach (RaceLap lap in player.RaceLapsList)
        {
            string formattedLapTime = FormatTime(lap.LapTime);
            lapTimesText += $"{lap.LapNumber}    |   {formattedLapTime}  |   {(lap.Valid ? "yes" : " no")}\n";
        }

        string formattedPenaltyTime = FormatTime(player.PenaltyTime);
        if (player.PenaltyTime > 0)
        {
            formattedPenaltyTime = "+" + formattedPenaltyTime;
        }
        string penaltyInfo = player.PenaltyTime > 0 ? formattedPenaltyTime : "Clear race!";
        string formattedTotalTime = $"{FormatTime(player.TotalTime)} ({penaltyInfo})";

        // Update UI texts
        panelPlayerNameText.text = isPlayerControlled ? "Your lap time(s)" : $"{player.Name}'s lap time(s)";
        panelLapTimesText.text = lapTimesText;
        panelTotalTimeText.text = $"Total time: {formattedTotalTime}";
    }

    public void ShowLapTimes(Button button)
    {
        TMP_Text buttonPlayerNameText = button.transform.GetChild(0).GetComponent<TMP_Text>();

        
        Player targetPlayer = GameManager.Instance.FinishedPlayers
            .FirstOrDefault(p => p.Name == buttonPlayerNameText.text);

        if (targetPlayer != null)
        {
            DisplayLapTimes(targetPlayer, targetPlayer.controlType == Player.ControlType.HumanInput);
        }
    }

    public void ShowPlaces()
    {
        if (LaptimesUpdated) return;

        if (PlacesPanel == null)
        {
            Debug.LogError("PlacesPanel is not assigned!");
            return;
        }

        
        for (int i = 0; i < GameManager.Instance.FinishedPlayers.Count; i++)
        {
            Transform placeButton = PlacesPanel.transform.GetChild(i);
            placeButton.gameObject.SetActive(true);

            TMP_Text playerNameText = placeButton.transform.GetChild(0).GetComponent<TMP_Text>();
            TMP_Text totalTimeOrGapText = placeButton.transform.GetChild(3).GetComponent<TMP_Text>();
            Transform penaltyIndicator = placeButton.transform.GetChild(2);

            Player player = GameManager.Instance.FinishedPlayers[i];
            if (player.PenaltyApplied)
            {
                penaltyIndicator.gameObject.SetActive(true);
            }

            
            playerNameText.text = player.Name;
            if (i == 0)
            {
                string formattedTotalTime = FormatTime(player.TotalTime);
                totalTimeOrGapText.text = $"Total time\n{formattedTotalTime}";
            }
            else
            {
                string formattedGap = FormatTime(Math.Abs(player.Gap));
                totalTimeOrGapText.text = $"Gap\n- {formattedGap}";
            }
        }

        
        Player humanPlayer = GameManager.Instance.FinishedPlayers
            .FirstOrDefault(p => p.controlType == Player.ControlType.HumanInput);

        if (humanPlayer != null)
        {
            DisplayLapTimes(humanPlayer, true);
        }

        LaptimesUpdated = true;
    }

    public void ClearPlaces()
    {
        if (PlacesPanel == null)
        {
            Debug.LogError("PlacesPanel is not assigned!");
            return;
        }

        
        foreach (Transform child in PlacesPanel.transform)
        {
            child.gameObject.SetActive(false);
        }

        ResetButtonScales(UIResultPanel);

        WaitingPanel.SetActive(false);

        Debug.Log("PlacesPanel cleared.");
        RestartButton.SetActive(false);
        BackToMainMenuButton.SetActive(false);
        UIResultPanel.gameObject.SetActive(false);
        LaptimesUpdated = false; 
    }

    private void ResetButtonScales(GameObject panel)
    {
        
        Button[] buttons = panel.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.transform.localScale = Vector3.one;

            button.interactable = false;
            button.interactable = true;

            Animator animator = button.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Rebind();
                animator.Update(0);
            }
        }
    }
}
