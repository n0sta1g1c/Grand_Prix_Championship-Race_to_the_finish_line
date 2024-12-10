using UnityEngine;

public class BrakingZone : MonoBehaviour
{
    [SerializeField] private int CarTypeID; // Assign a unique ID for each braking zone (1, 2, or 3)

    private void OnTriggerEnter(Collider other)
    {
        PlayerController PlayerController = other.transform.parent.GetComponent<PlayerController>();

        if (PlayerController != null && PlayerController.Player.CarType == CarTypeID) // Match the car type
        {
            PlayerController.IsInsideBraking = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController PlayerController = other.transform.parent.GetComponent<PlayerController>();

        if (PlayerController != null && PlayerController.Player.CarType == CarTypeID) // Match the car type
        {
            PlayerController.IsInsideBraking = false;
        }
    }
}

