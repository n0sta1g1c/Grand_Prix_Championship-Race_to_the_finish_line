using UnityEngine;
using UnityEngine.SceneManagement;

public class Car_Sound : MonoBehaviour
{
    // Start is called before the first frame update
    public float MinSpeed;
    public float MaxSpeed;
    private float CurrentSpeed;

    private Rigidbody CarRigidBody;
    private AudioSource CarAudio;

    public float MinPitch;
    public float MaxPitch;
    private float PitchFromCar;

    public Transform PlayerCar;
    public float MaxHearableDistance = 50f;

    private Scene Scene;

    private bool IsInGameScene = false;

    private void Start()
    {
        Scene = SceneManager.GetActiveScene();

        if (Scene.buildIndex > 1)
        {
            IsInGameScene = true;
        }
        CarAudio = GetComponent<AudioSource>();
        CarRigidBody = GetComponent<Rigidbody>();

        if (CarAudio != null)
        {
            CarAudio.spatialBlend = 1.0f; // Full 3D sound
            CarAudio.loop = true;
            //carAudio.volume = 0.5f;
        }
        if (!SceneManager.GetActiveScene().name.StartsWith("Main") && !SceneManager.GetActiveScene().name.StartsWith("Selection"))
        {
            PlayerCar = GameManager.Instance.ChosenCarObject.transform;
        }


    }

    private void Update()
    {
        if (IsInGameScene)
        {
            if (GameManager.Instance.GameIsPaused || GameManager.Instance.CountdownState)
            {
                //Debug.Log("Game is paused");
                CarAudio.Pause();
            }
            else
            {
                CarAudio.UnPause();
                //Debug.Log("Game is on");
                EngineSound();
                HandleProximityVolume();
            }
        }


    }

    void EngineSound()
    {
        CurrentSpeed = CarRigidBody.velocity.magnitude;
        PitchFromCar = CarRigidBody.velocity.magnitude / 50f;

        if (CurrentSpeed < MinSpeed)
        {
            CarAudio.pitch = MinPitch;
        }

        if (CurrentSpeed > MinSpeed && CurrentSpeed < MaxSpeed)
        {
            CarAudio.pitch = MinPitch + PitchFromCar;
        }

        if (CurrentSpeed > MaxSpeed)
        {
            CarAudio.pitch = MaxPitch;
        }

    }

    void HandleProximityVolume()
    {
        if (PlayerCar == null || CarAudio == null) return;

        // Calculate distance from the player's car
        float distance = Vector3.Distance(transform.position, PlayerCar.position);

        if (distance > MaxHearableDistance)
        {
            CarAudio.volume = 0; // Mute the sound if too far
        }
        else
        {
            // Linearly adjust volume based on distance
            CarAudio.volume = Mathf.Lerp(1f, 0.2f, distance / MaxHearableDistance);
        }

        //Debug.Log($"Car Distance: {distance}, Volume: {carAudio.volume}, Speed: {currentSpeed}, Pitch: {carAudio.pitch}");
    }

}

