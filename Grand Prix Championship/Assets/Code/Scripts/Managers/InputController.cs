using UnityEngine;

public class InputController : MonoBehaviour
{
    public string InputSteerAxis = "Horizontal";
    public string InputThrottleAxis = "Vertical";

    public float ThrottleInput { get; private set; }
    public float SteerInput { get; private set; }
    public bool BrakeInput { get; private set; }

    void Update()
    {
        SteerInput = Input.GetAxis(InputSteerAxis);
        ThrottleInput = Input.GetAxis(InputThrottleAxis);
        BrakeInput = Input.GetKey(KeyCode.Space);
    }
}
