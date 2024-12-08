using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset;
    public Vector3 EulerRotation;
    public float Damper;

    void Start()
    {
        SetPlayerReference(GameManager.Instance.ChosenCarObject);
        transform.eulerAngles = EulerRotation;
    }

    void Update()
    {
        if (Target == null)
        {
            return;
        }
        transform.position = Vector3.Lerp(transform.position, Target.position + Offset, Damper * Time.deltaTime);
    }

    public void SetPlayerReference(GameObject playerCar)
    {
        Target = playerCar.transform;

    }
}
