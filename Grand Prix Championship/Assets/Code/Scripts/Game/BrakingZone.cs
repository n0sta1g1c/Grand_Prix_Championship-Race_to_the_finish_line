using UnityEngine;

public class BrakingZone : MonoBehaviour
{
    [SerializeField] private int CarTypeID; //1: Terepjáró, 2: Retro sportautó, 3: Modern sportautó

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

