using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarSelection : MonoBehaviour
{

    public GameObject[] CarObjects;

    public Button NextButton;
    public Button PrevButton;
    public TMP_Text SelectedCarNameText;
    int CarIndex;
    int CarMaxIndex;

    void Start()
    {
        CarIndex = 0;
        CarMaxIndex = SelectionManager.Instance.CarNames.Length - 1; ;
        SelectedCarNameText.text = SelectionManager.Instance.CarNames[0];
        PlayerPrefs.SetInt("CarIndex", CarIndex);


        for (int i = 0; i < CarObjects.Length; i++)
        {
            CarObjects[i].SetActive(false);
            CarObjects[CarIndex].SetActive(true);
        }
    }


    void Update()
    {
        NextButton.gameObject.SetActive(CarIndex < CarMaxIndex);
        PrevButton.gameObject.SetActive(CarIndex > 0);
    }

    public void Next()
    {
        CarIndex++;

        for (int i = 0; i < CarObjects.Length; i++)
        {
            CarObjects[i].SetActive(false);
            CarObjects[CarIndex].SetActive(true);
            //chosen_car = CarObjects[Index].transform.name;
            SelectedCarNameText.text = SelectionManager.Instance.CarNames[CarIndex];
        }
        PlayerPrefs.SetInt("CarIndex", CarIndex);
        PlayerPrefs.Save();
    }

    public void Prev()
    {
        CarIndex--;

        for (int i = 0; i < CarObjects.Length; i++)
        {
            CarObjects[i].SetActive(false);
            CarObjects[CarIndex].SetActive(true);
            //chosen_car = CarObjects[Index].transform.name;
            SelectedCarNameText.text = SelectionManager.Instance.CarNames[CarIndex];
        }
        PlayerPrefs.SetInt("CarIndex", CarIndex);
        PlayerPrefs.Save();
    }

    /*public void Race()
    {
        race.enabled = false;
        timetrial.enabled = false;
        PlayerPrefs.SetInt("gamemode", 1);
        PlayerPrefs.Save();
        GameManager_CarSelection.Instance.AudioManager_Menu.StopMusic("Menu_theme");
        GameManager_CarSelection.Instance.AudioManager_Menu.PlaySFX("car_start");
        //Debug.Log("Race Scene");
        Invoke("startRace", 3);
        //SceneManager.LoadSceneAsync("Hungaroring_Race");
    }

    public void TimeTrial()
    {
        race.enabled = false;
        timetrial.enabled = false;
        //race.interactable = false;
        //timetrial.interactable = false;
        PlayerPrefs.SetInt("gamemode", 0);
        PlayerPrefs.Save();
        GameManager_CarSelection.Instance.AudioManager_Menu.StopMusic("Menu_theme");
        GameManager_CarSelection.Instance.AudioManager_Menu.PlaySFX("car_start");
        Invoke("startTimeTrial", 3);
    }

    public void startTimeTrial() {
        SceneManager.LoadSceneAsync("Hungaroring_TimeTrial");
    }

    public void startRace()
    {
        SceneManager.LoadSceneAsync("Hungaroring_Race");
    }*/
}
