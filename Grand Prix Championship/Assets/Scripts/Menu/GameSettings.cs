using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public RectTransform BackgroundPanel;
    public GameObject SelectedRaceDistance;
    public GameObject SelectedNumberOfOpponents;
    public GameObject SelectedDifficulty;


    public Button GameMode_NextButton;
    public Button GameMode_PrevButton;
    public TMP_Text SelectedGameModeText;
    int Index_GameModes;
    int MaxIndex_GameModes;

    public Button RaceDistance_NextButton;
    public Button RaceDistance_PrevButton;
    public TMP_Text SelectedRaceDistanceText;
    int Index_RaceDistance;
    int MaxIndex_RaceDistance;

    public Button NumberOfOppontents_NextButton;
    public Button NumberOfOppontents_PrevButton;
    public TMP_Text SelectedNumberOfOppontentsText;
    int Index_NoOfOpponents;
    int MaxIndex_NoOfOpponents;

    public Button Difficulty_NextButton;
    public Button Difficulty_PrevButton;
    public TMP_Text SelectedDifficultyText;
    int Index_Difficulty;
    int MaxIndex_Difficulty;

    void Start()
    {
        Index_GameModes = 0;
        Index_RaceDistance = 2;
        Index_NoOfOpponents = 2;
        Index_Difficulty = 1;

        UpdateGameModeUI();

        PlayerPrefs.SetInt("GameMode", Index_GameModes);
        PlayerPrefs.SetInt("RaceDistance", Index_RaceDistance + 1);
        PlayerPrefs.SetInt("NumberOfPlayers", Index_NoOfOpponents + 2);
        PlayerPrefs.SetString("Difficulty", SelectionManager.Instance.DifficultyNames[Index_Difficulty]);

        MaxIndex_GameModes = SelectionManager.Instance.GameModeNames.Length - 1;
        MaxIndex_RaceDistance = SelectionManager.Instance.RaceDistanceList.Length - 1;
        MaxIndex_NoOfOpponents = SelectionManager.Instance.NumberOfOppontentsList.Length - 1;
        MaxIndex_Difficulty = SelectionManager.Instance.DifficultyNames.Length - 1;

        SelectedGameModeText.text = SelectionManager.Instance.GameModeNames[Index_GameModes];
        SelectedRaceDistanceText.text = SelectionManager.Instance.RaceDistanceList[Index_RaceDistance];
        SelectedNumberOfOppontentsText.text = SelectionManager.Instance.NumberOfOppontentsList[Index_NoOfOpponents].ToString();
        SelectedDifficultyText.text = SelectionManager.Instance.DifficultyNames[Index_Difficulty];
    }


    void Update()
    {
        GameMode_NextButton.gameObject.SetActive(Index_GameModes < MaxIndex_GameModes);
        GameMode_PrevButton.gameObject.SetActive(Index_GameModes > 0);

        RaceDistance_NextButton.gameObject.SetActive(Index_RaceDistance < MaxIndex_RaceDistance);
        RaceDistance_PrevButton.gameObject.SetActive(Index_RaceDistance > 0);

        NumberOfOppontents_NextButton.gameObject.SetActive(Index_NoOfOpponents < MaxIndex_NoOfOpponents);
        NumberOfOppontents_PrevButton.gameObject.SetActive(Index_NoOfOpponents > 0);

        Difficulty_NextButton.gameObject.SetActive(Index_Difficulty < MaxIndex_Difficulty);
        Difficulty_PrevButton.gameObject.SetActive(Index_Difficulty > 0);
    }

    public void NextGameMode()
    {
        Index_GameModes++;

        for (int i = 0; i < SelectionManager.Instance.GameModeNames.Length; i++)
        {
            SelectedGameModeText.text = SelectionManager.Instance.GameModeNames[Index_GameModes];
        }
        PlayerPrefs.SetInt("GameMode", Index_GameModes);
        PlayerPrefs.Save();
        UpdateGameModeUI();
    }

    public void PrevGameMode()
    {
        Index_GameModes--;

        for (int i = 0; i < SelectionManager.Instance.GameModeNames.Length; i++)
        {
            SelectedGameModeText.text = SelectionManager.Instance.GameModeNames[Index_GameModes];
        }
        PlayerPrefs.SetInt("GameMode", Index_GameModes);
        PlayerPrefs.Save();

        UpdateGameModeUI();
    }

    public void NextRaceDistance()
    {
        Index_RaceDistance++;

        for (int i = 0; i < SelectionManager.Instance.RaceDistanceList.Length; i++)
        {
            SelectedRaceDistanceText.text = SelectionManager.Instance.RaceDistanceList[Index_RaceDistance];
        }
        PlayerPrefs.SetInt("RaceDistance", Index_RaceDistance + 1);
        PlayerPrefs.Save();
    }

    public void PrevRaceDistance()
    {
        Index_RaceDistance--;

        for (int i = 0; i < SelectionManager.Instance.RaceDistanceList.Length; i++)
        {
            SelectedRaceDistanceText.text = SelectionManager.Instance.RaceDistanceList[Index_RaceDistance];
        }
        PlayerPrefs.SetInt("RaceDistance", Index_RaceDistance + 1);
        PlayerPrefs.Save();
    }

    public void NextNumberOfOppontents()
    {
        Index_NoOfOpponents++;

        for (int i = 0; i < SelectionManager.Instance.NumberOfOppontentsList.Length; i++)
        {
            SelectedNumberOfOppontentsText.text = SelectionManager.Instance.NumberOfOppontentsList[Index_NoOfOpponents].ToString();
        }
        PlayerPrefs.SetInt("NumberOfPlayers", Index_NoOfOpponents + 2);
        PlayerPrefs.Save();
    }

    public void PrevNumberOfOppontents()
    {
        Index_NoOfOpponents--;

        for (int i = 0; i < SelectionManager.Instance.NumberOfOppontentsList.Length; i++)
        {
            SelectedNumberOfOppontentsText.text = SelectionManager.Instance.NumberOfOppontentsList[Index_NoOfOpponents].ToString();
        }
        PlayerPrefs.SetInt("NumberOfPlayers", Index_NoOfOpponents + 2);
        PlayerPrefs.Save();
    }

    public void NextDifficulty()
    {
        Index_Difficulty++;

        for (int i = 0; i < SelectionManager.Instance.DifficultyNames.Length; i++)
        {
            SelectedDifficultyText.text = SelectionManager.Instance.DifficultyNames[Index_Difficulty];
        }
        PlayerPrefs.SetString("Difficulty", SelectionManager.Instance.DifficultyNames[Index_Difficulty]);
        PlayerPrefs.Save();
    }

    public void PrevDifficulty()
    {
        Index_Difficulty--;

        for (int i = 0; i < SelectionManager.Instance.DifficultyNames.Length; i++)
        {
            SelectedDifficultyText.text = SelectionManager.Instance.DifficultyNames[Index_Difficulty];
        }
        PlayerPrefs.SetString("Difficulty", SelectionManager.Instance.DifficultyNames[Index_Difficulty]);
        PlayerPrefs.Save();
    }




    private IEnumerator SmoothHeightChange(float targetHeight, bool isTimeTrial)
    {
        float duration = 0.25f;
        float elapsedTime = 0f;
        Vector2 initialSize = BackgroundPanel.sizeDelta;
        float initialHeight = initialSize.y;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Interpolate the height value
            float newHeight = Mathf.Lerp(initialHeight, targetHeight, elapsedTime / duration);
            BackgroundPanel.sizeDelta = new Vector2(initialSize.x, newHeight);

            yield return null;
        }

        // Ensure the final size is set
        BackgroundPanel.sizeDelta = new Vector2(initialSize.x, targetHeight);

        // Delayed activation of panels after animation ends
        
    }

    private void UpdateGameModeUI()
    {
        // Check if the current mode is Time Trial
        bool isTimeTrial = SelectionManager.Instance.GameModeNames[Index_GameModes] == "Time trial";

        // Target size based on game mode
        float targetHeight = isTimeTrial ? 50f : 200f;

        SelectedRaceDistance.SetActive(!isTimeTrial);
        SelectedNumberOfOpponents.SetActive(!isTimeTrial);
        SelectedDifficulty.SetActive(!isTimeTrial);

        // Start the smooth height change coroutine and handle panel activation
        StopAllCoroutines(); // Stop any ongoing transitions
        StartCoroutine(SmoothHeightChange(targetHeight, isTimeTrial));
    }
}
