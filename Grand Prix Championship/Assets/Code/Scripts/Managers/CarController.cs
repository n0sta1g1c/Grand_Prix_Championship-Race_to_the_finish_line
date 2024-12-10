using UnityEngine;


public class CarController : MonoBehaviour
{

    public Transform CenterOfMass;
    public float MotorTorque = 3000f;
    public float MaxSteer = 20f;
    public float BrakeForce = 100f;
    public float MaxSpeed = 100f;
    public float CentreOfGravityOffset = -1f;

    public Rigidbody Rigidbody;

    public float CurrentSpeed { get; private set; }
    public bool Brake { get; set; }
    public float Steer { get; set; }
    public float Throttle { get; set; }
    private Wheel[] Wheels;

    private void Start()
    {
        Wheels = GetComponentsInChildren<Wheel>();
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.centerOfMass = CenterOfMass.localPosition;
    }

    private void FixedUpdate()
    {

    }

    private void Update()
    {
        if (Rigidbody.velocity.magnitude * 3.6 > MaxSpeed)
        {
            Rigidbody.velocity = Vector3.ClampMagnitude(Rigidbody.velocity, MaxSpeed / 3.6f);
        }
        CurrentSpeed = (float)(Rigidbody.velocity.magnitude * 3.6);
        ApplyThrottle(Throttle);
        ApplySteering(Steer);
        ApplyHandBrake();
    }
    void ApplySteering(float Steer)
    {
        foreach (var wheel in Wheels)
        {
            wheel.SteerAngle = Steer * MaxSteer;
        }
    }
    void ApplyThrottle(float Throttle)
    {
        foreach (var wheel in Wheels)
        {
            wheel.Torque = Throttle * MotorTorque;
        }
    }

    void ApplyHandBrake()
    {
        foreach (var wheel in Wheels)
        {
            if (Brake)
            {
                wheel.BrakeForce = BrakeForce * 50;
                wheel.Torque = 0;
            }
            else
            {
                wheel.BrakeForce = 0;
                wheel.Torque = Throttle * MotorTorque;
            }

        }

    }
}
