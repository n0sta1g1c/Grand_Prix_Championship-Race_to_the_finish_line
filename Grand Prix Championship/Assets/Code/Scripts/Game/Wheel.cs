using UnityEngine;

public class Wheel : MonoBehaviour
{
    public bool Steer;
    public bool InvertSteer;
    public bool Power;

    public float BrakeForce { get; set; }
    public float SteerAngle { get; set; }
    public float Torque { get; set; }

    private WheelCollider WheelCollider;
    private Transform WheelTransform;
    void Start()
    {
        WheelCollider = GetComponentInChildren<WheelCollider>();
        WheelTransform = GetComponentInChildren<MeshRenderer>().GetComponent<Transform>();
    }
    void Update()
    {
        WheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        WheelTransform.position = pos;
        WheelTransform.rotation = rot;
    }

    private void FixedUpdate()
    {
        if (Steer)
        {
            WheelCollider.steerAngle = SteerAngle * (InvertSteer ? -1 : 1);
        }
        if (Power)
        {
            WheelCollider.motorTorque = Torque;

        }
        WheelCollider.brakeTorque = BrakeForce;

    }
}
