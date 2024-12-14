using UnityEngine;

public class Rotate : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.Rotate(0, 0.4f, 0);
    }
}
