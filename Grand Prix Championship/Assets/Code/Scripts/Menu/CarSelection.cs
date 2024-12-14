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
            SelectedCarNameText.text = SelectionManager.Instance.CarNames[CarIndex];
        }
        PlayerPrefs.SetInt("CarIndex", CarIndex);
        PlayerPrefs.Save();
    }
}
