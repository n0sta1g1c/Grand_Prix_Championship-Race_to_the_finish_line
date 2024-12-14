using UnityEngine;

public class BrakingZone : MonoBehaviour
{
    [SerializeField] private int CarTypeID; //1: Terepj�r�, 2: Retro sportaut�, 3: Modern sportaut�

    private void OnTriggerEnter(Collider other)
    {
        PlayerController PlayerController = other.transform.parent.GetComponent<PlayerController>();

        if (PlayerController != null && PlayerController.Player.CarType == CarTypeID) 
        {
            PlayerController.IsInsideBraking = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController PlayerController = other.transform.parent.GetComponent<PlayerController>();

        if (PlayerController != null && PlayerController.Player.CarType == CarTypeID) 
        {
            PlayerController.IsInsideBraking = false;
        }
    }
}

