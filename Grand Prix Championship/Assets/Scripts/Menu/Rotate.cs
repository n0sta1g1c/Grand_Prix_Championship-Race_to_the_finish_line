using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Start is called before the first frame update
    private void FixedUpdate()
    {
        transform.Rotate(0, 0.4f, 0);
    }
}
