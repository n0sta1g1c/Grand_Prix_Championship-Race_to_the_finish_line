using UnityEngine;

public class ButtonSFX : MonoBehaviour
{
    public AudioManager audiomanger;

    [SerializeField] public Transform ActiveLine;

    void Start()
    {

    }
    public void onEnter()
    {
        if (ActiveLine != null)
        {
            ActiveLine.gameObject.SetActive(true);
        }
        audiomanger.PlaySFX("Menu_Tap");
    }

    public void onExit()
    {
        if (ActiveLine != null)
        {
            ActiveLine.gameObject.SetActive(false);
        }
    }
}
