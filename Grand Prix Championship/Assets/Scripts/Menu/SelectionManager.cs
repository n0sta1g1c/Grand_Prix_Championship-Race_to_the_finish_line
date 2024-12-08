using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }

    public Camera CarCamera;
    public Camera TrackCamera;

    public GameObject NavigationPanel;

    public GameObject CarSelection_Rotating;
    public GameObject CarSelection_Background;

    public GameObject StartButton;

    public string[] CarNames = new[] { "Nissan Navara", "Porsche 911", "Audi R8" };
    public string[] TrackNames = new[] { "Hungaroring", "Brands Hatch", "Interlagos" };
    public string[] GameModeNames = new[] { "Race", "Time trial" };
    public string[] RaceDistanceList = new[] { "1 lap", "2 laps", "3 laps", "4 laps", "5 laps", "6 laps", "7 laps", "8 laps", "9 laps", "10 laps" };
    public int[] NumberOfOppontentsList = new[] { 1, 2, 3, 4, 5, 6, 7 };
    public string[] DifficultyNames = new[] { "Easy", "Medium", "Hard" };

    public int Phase;

    public Button RaceButton;
    public Button SettignsButton;

    public TMP_Text CurrentPhaseText;

    public TMP_Text PlayerNameText;

    public AudioManager AudioManager;

    public GameObject CarSelectionPhase;
    public GameObject TrackSelectionPhase;
    public GameObject GameSettingsPhase;
    public GameObject PlayerConfigPhase;

    public GameObject ImageLine_Menu;
    public RectTransform ImageLine_Menu_RectTransform;


    private void Awake()
    {
        Phase = 0;
        Instance = this;
        AudioManager = GetComponentInChildren<AudioManager>();
    }

    void Start()
    {
        ImageLine_Menu_RectTransform = ImageLine_Menu.transform.GetComponent<RectTransform>();
        CarCamera.gameObject.SetActive(true);
        TrackCamera.gameObject.SetActive(false);
    }

    public void StartGameMode()
    {
        PlayerConfigPhase.GetComponent<CanvasGroup>().interactable = false;
        AudioManager.StopMusic("Selection_Theme");
        AudioManager.PlaySFX("Engine_Start");
        //Debug.Log(PlayerNameText.text);
        ButtonSFX ButtonSFX = StartButton.GetComponent<ButtonSFX>();
        if (ButtonSFX != null)
        {
            Destroy(ButtonSFX);
        }

        PlayerPrefs.SetString("PlayerName", PlayerNameText.text);
        PlayerPrefs.Save();
        Invoke("LoadChosenScene", 3);
    }

    public void LoadChosenScene()
    {
        if (PlayerPrefs.GetInt("TrackIndex") == 0)
        {
            Debug.Log("Hungaroring is starting...");
            SceneManager.LoadScene("Hungaroring");
        }
        else if (PlayerPrefs.GetInt("TrackIndex") == 1)
        {
            Debug.Log("Brand Hatch is starting...");
            SceneManager.LoadScene("Brands_Hatch");
        }
        else
        {
            Debug.Log("Interlagos is starting...");
            SceneManager.LoadScene("Interlagos");
        }

    }

    public void ShowNextPhase(Button button)
    {
        Phase++;
        if (Phase == 1) //Car selection phase
        {
            CarCamera.gameObject.SetActive(true);
            TrackCamera.gameObject.SetActive(false);

            CarSelection_Background.gameObject.SetActive(false);
            CarSelection_Rotating.gameObject.SetActive(true);

            CarSelectionPhase.SetActive(true);
            TrackSelectionPhase.SetActive(false);
            GameSettingsPhase.SetActive(false);

            CurrentPhaseText.text = "select car";

            if (ImageLine_Menu_RectTransform != null)
            {
                // Start the coroutine for smooth transition
                StartCoroutine(SmoothWidthChange(600f));
            }
            else
            {
                Debug.LogWarning("Target Image RectTransform is not assigned.");
            }
        }
        else if (Phase == 2) //Track selection phase
        {
            CarCamera.gameObject.SetActive(false);
            TrackCamera.gameObject.SetActive(true);
            CarSelectionPhase.SetActive(false);
            TrackSelectionPhase.SetActive(true);
            GameSettingsPhase.SetActive(false);

            CurrentPhaseText.text = "select track";

        }
        else if (Phase == 3) //Game settigns phase
        {
            CarCamera.gameObject.SetActive(true);
            TrackCamera.gameObject.SetActive(true);

            CarCamera.rect = new Rect(0, 0, 0.5f, 1f); // Left half (x, y, width, height)

            // Set trackCamera to render the right half of the screen
            TrackCamera.rect = new Rect(0.5f, 0, 0.5f, 1f); // Right half

            CarSelectionPhase.SetActive(false);
            TrackSelectionPhase.SetActive(false);
            GameSettingsPhase.SetActive(true);

            CurrentPhaseText.text = "save and continue";
        }
        else
        {
            NavigationPanel.gameObject.SetActive(false);
            GameSettingsPhase.GetComponent<CanvasGroup>().interactable = false;
            PlayerConfigPhase.SetActive(true);
        }
    }

    public void ShowPrevPhase(Button button)
    {
        Phase--;
        if (Phase == 0) // Welcome phase
        {
            CarCamera.gameObject.SetActive(true);
            TrackCamera.gameObject.SetActive(false);

            CarSelection_Background.gameObject.SetActive(true);
            CarSelection_Rotating.gameObject.SetActive(false);

            CarSelectionPhase.SetActive(false);
            TrackSelectionPhase.SetActive(false);
            GameSettingsPhase.SetActive(false);

            CurrentPhaseText.text = "begin selection";
        }
        else if (Phase == 1) // Car selection phase
        {
            CarCamera.gameObject.SetActive(true);
            TrackCamera.gameObject.SetActive(false);

            CarSelection_Background.gameObject.SetActive(false);
            CarSelection_Rotating.gameObject.SetActive(true);

            CarSelectionPhase.SetActive(true);
            TrackSelectionPhase.SetActive(false);
            GameSettingsPhase.SetActive(false);

            CurrentPhaseText.text = "select car";

            if (ImageLine_Menu_RectTransform != null)
            {
                // Start the coroutine for smooth transition
                StartCoroutine(SmoothWidthChange(600f));
            }
            else
            {
                Debug.LogWarning("Target Image RectTransform is not assigned.");
            }
        }
        else if (Phase == 2) // Track selection phase
        {
            CarCamera.gameObject.SetActive(false);
            TrackCamera.gameObject.SetActive(true);

            CarCamera.rect = new Rect(0, 0, 1f, 1f); // Left half (x, y, width, height)

            // Set trackCamera to render the right half of the screen
            TrackCamera.rect = new Rect(0, 0, 1f, 1f); // Right half

            CarSelectionPhase.SetActive(false);
            TrackSelectionPhase.SetActive(true);
            GameSettingsPhase.SetActive(false);

            CurrentPhaseText.text = "select track";
        }
        else if (Phase == 3) // Game settings phase
        {
            CarCamera.gameObject.SetActive(true);
            TrackCamera.gameObject.SetActive(true);

            CarCamera.rect = new Rect(0, 0, 0.5f, 1f); // Left half (x, y, width, height)

            // Set trackCamera to render the right half of the screen
            TrackCamera.rect = new Rect(0.5f, 0, 0.5f, 1f); // Right half

            CarSelectionPhase.SetActive(false);
            TrackSelectionPhase.SetActive(false);
            GameSettingsPhase.SetActive(true);

            CurrentPhaseText.text = "save and continue";
        }

        // Ensure Phase doesn't go below 1
        if (Phase < 0)
        {
            Phase = 0;
            ResetPlayerPrefs();
            SceneManager.LoadScene("MainMenu");
        }
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

    private IEnumerator SmoothWidthChange(float targetWidth)
    {
        float duration = 0.5f;
        float elapsedTime = 0f;
        Vector2 initialSize = ImageLine_Menu_RectTransform.sizeDelta;
        float initialWidth = initialSize.x;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Lerp the width value
            float newWidth = Mathf.Lerp(initialWidth, targetWidth, elapsedTime / duration);
            ImageLine_Menu_RectTransform.sizeDelta = new Vector2(newWidth, initialSize.y);

            yield return null;
        }

        // Ensure the final size is set
        ImageLine_Menu_RectTransform.sizeDelta = new Vector2(targetWidth, initialSize.y);
    }

}
