using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrackSelection : MonoBehaviour
{
    public GameObject[] TrackObjects;

    public Button NextButton;
    public Button PrevButton;
    public TMP_Text SelectedTrackNameText;
    int TrackIndex;
    int TrackMaxIndex;
    //string chosen_car;

    float levelLoadDelay;

    void Start()
    {
        TrackIndex = 0;
        TrackMaxIndex = SelectionManager.Instance.TrackNames.Length - 1;
        PlayerPrefs.SetInt("TrackIndex", TrackIndex);
        SelectedTrackNameText.text = SelectionManager.Instance.TrackNames[0];


        for (int i = 0; i < TrackObjects.Length; i++)
        {
            TrackObjects[i].SetActive(false);
            TrackObjects[TrackIndex].SetActive(true);
        }
    }


    void Update()
    {




        // Enable or disable the "Next" button
        NextButton.gameObject.SetActive(TrackIndex < TrackMaxIndex);

        // Enable or disable the "Prev" button
        PrevButton.gameObject.SetActive(TrackIndex > 0);
    }

    public void Next()
    {
        TrackIndex++;

        for (int i = 0; i < SelectionManager.Instance.TrackNames.Length; i++)
        {
            TrackObjects[i].SetActive(false);
            TrackObjects[TrackIndex].SetActive(true);
            //chosen_car = CarObjects[Index].transform.name;
            SelectedTrackNameText.text = SelectionManager.Instance.TrackNames[TrackIndex];
        }
        PlayerPrefs.SetInt("TrackIndex", TrackIndex);
        //PlayerPrefs.SetInt("carIndex", Index);
        PlayerPrefs.Save();
    }

    public void Prev()
    {
        TrackIndex--;

        for (int i = 0; i < SelectionManager.Instance.TrackNames.Length; i++)
        {
            TrackObjects[i].SetActive(false);
            TrackObjects[TrackIndex].SetActive(true);
            //chosen_car = CarObjects[Index].transform.name;
            SelectedTrackNameText.text = SelectionManager.Instance.TrackNames[TrackIndex];
        }
        PlayerPrefs.SetInt("TrackIndex", TrackIndex);
        //PlayerPrefs.SetInt("carIndex", Index);
        PlayerPrefs.Save();
    }
}
