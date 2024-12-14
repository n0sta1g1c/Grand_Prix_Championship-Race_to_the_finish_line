using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject MainMenuPanel;
    [SerializeField] GameObject OptionsPanel;
    [SerializeField] GameObject AudioOptionsPanel;
    [SerializeField] GameObject VideoOptionsPanel;

    private static bool isResolutionInitialized = false;

    private Resolution[] Resolutions;
    public TMP_Dropdown ResolutionDropdown;
    public int ResolutionIndex;

    public string[] GraphicsNamesList = new[] { "Very Low", "Low", "Medium", "High", "Very High", "Ultra" };
    public Button NextGraphicsButton;
    public Button PrevGraphicsButton;
    public TMP_Text SelectedGraphicsText;
    int GraphicsIndex;
    int GraphicskMaxIndex;

    public string[] FullscreenList = new[] { "Off", "On" };
    public Button NextFullscreenButton;
    public Button PrevFullscreenButton;
    public TMP_Text SelectedFullscreenText;
    int FullscreenIndex;
    int FullscreenkMaxIndex;

    [SerializeField] AudioManager AudioManager;

    private void Start()
    {
        GraphicsIndex = GraphicsNamesList.Length - 1;
        GraphicskMaxIndex = GraphicsNamesList.Length - 1;
        SelectedGraphicsText.text = GraphicsNamesList[GraphicsIndex];

        FullscreenIndex = 1;
        FullscreenkMaxIndex = FullscreenList.Length - 1;
        SelectedFullscreenText.text = FullscreenList[1];
        Screen.fullScreen = true;

        ResolutionDropdown.onValueChanged.AddListener(UpdateResolution);

        if (!isResolutionInitialized)
        {
            SetResolution();
            isResolutionInitialized = true;
        }

        LoadMainMenu();
    }

    private void SetResolution()
    {
        Debug.Log("SetResolution called");

        Resolutions = Screen.resolutions
        .GroupBy(r => new { r.width, r.height })
        .Select(g => g.First())
        .ToArray();
        ResolutionIndex = 0;

        List<string> ResolutionOptions = new List<string>();

        for (int i = Resolutions.Length - 1; i >= 0; i--)
        {
            string Option = Resolutions[i].width + "x" + Resolutions[i].height;
            ResolutionOptions.Add(Option);

            if (Resolutions[i].width == Screen.currentResolution.width && Resolutions[i].height == Screen.currentResolution.height)
            {
                
                ResolutionIndex = (Resolutions.Length - 1) - i;
                Debug.Log($"Current Resolution Detected: {Option}, Index: {ResolutionIndex}");
            }
        }

        ResolutionDropdown.ClearOptions(); 
        ResolutionDropdown.AddOptions(ResolutionOptions);
        ResolutionDropdown.value = ResolutionIndex;
        ResolutionDropdown.RefreshShownValue();
    }

    public void UpdateResolution(int resolitionIndex)
    {
        if (resolitionIndex >= 0 && resolitionIndex < Resolutions.Length)
        {
            Resolution resolution = Resolutions[Resolutions.Length - 1 - resolitionIndex]; 
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            Debug.Log($"Updated to Resolution: {resolution.width}x{resolution.height}");
        }
        else
        {
            Debug.LogError("Invalid Resolution Index");
        }
    }

    private void Update()
    {
        NextGraphicsButton.gameObject.SetActive(GraphicsIndex < GraphicskMaxIndex);
        PrevGraphicsButton.gameObject.SetActive(GraphicsIndex > 0);

        NextFullscreenButton.gameObject.SetActive(FullscreenIndex < FullscreenkMaxIndex);
        PrevFullscreenButton.gameObject.SetActive(FullscreenIndex > 0);
    }

    public void LoadOptions()
    {
        MainMenuPanel.SetActive(false);
        OptionsPanel.SetActive(true);
        AudioOptionsPanel.SetActive(false);
        VideoOptionsPanel.SetActive(false);

        ResetButtonScales(OptionsPanel);
    }

    public void LoadAudioOptions()
    {
        OptionsPanel.SetActive(false);
        AudioOptionsPanel.SetActive(true);
        VideoOptionsPanel.SetActive(false);

        ResetButtonScales(AudioOptionsPanel);
    }

    public void LoadVideoOptions()
    {
        OptionsPanel.SetActive(false);
        VideoOptionsPanel.SetActive(true);
        AudioOptionsPanel.SetActive(false);

        ResetButtonScales(VideoOptionsPanel);
    }

    public void LoadMainMenu()
    {
        MainMenuPanel.SetActive(true);
        OptionsPanel.SetActive(false);
        AudioOptionsPanel.SetActive(false);
        VideoOptionsPanel.SetActive(false);

        ResetButtonScales(MainMenuPanel);
    }

    public void LoadSelection()
    {
        AudioManager.StopMusic("MainMenu_Theme");
        SceneManager.LoadSceneAsync("Selection");
    }

    public void NextGraphics()
    {
        GraphicsIndex++;

        for (int i = 0; i < GraphicsNamesList.Length; i++)
        {
            SelectedGraphicsText.text = GraphicsNamesList[GraphicsIndex];
        }
        QualitySettings.SetQualityLevel(GraphicsIndex);
        PlayerPrefs.SetInt("GraphicsIndex", GraphicsIndex);
        PlayerPrefs.Save();
    }

    public void PrevGraphics()
    {
        GraphicsIndex--;

        for (int i = 0; i < GraphicsNamesList.Length; i++)
        {
            SelectedGraphicsText.text = GraphicsNamesList[GraphicsIndex];
        }
        QualitySettings.SetQualityLevel(GraphicsIndex);
        PlayerPrefs.SetInt("GraphicsIndex", GraphicsIndex);
        PlayerPrefs.Save();
    }

    public void NextFullscreen()
    {
        FullscreenIndex++;

        for (int i = 0; i < FullscreenList.Length; i++)
        {
            SelectedFullscreenText.text = FullscreenList[FullscreenIndex];
        }
        bool FullscreenBoolean = FullscreenIndex > 0 ? true : false;
        Screen.fullScreen = FullscreenBoolean;
        Debug.Log(FullscreenBoolean);
        PlayerPrefs.SetInt("FullscreenIndex", FullscreenIndex);
        PlayerPrefs.Save();
    }

    public void PrevFullscreen()
    {
        FullscreenIndex--;

        for (int i = 0; i < FullscreenList.Length; i++)
        {
            SelectedFullscreenText.text = FullscreenList[FullscreenIndex];
        }
        bool FullscreenBoolean = FullscreenIndex > 0 ? true : false;
        Screen.fullScreen = FullscreenBoolean;
        Debug.Log(FullscreenBoolean);
        PlayerPrefs.SetInt("FullscreenIndex", FullscreenIndex);
        PlayerPrefs.Save(); ;
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

    public void ExitGame()
    {
        Application.Quit();
    }


}
