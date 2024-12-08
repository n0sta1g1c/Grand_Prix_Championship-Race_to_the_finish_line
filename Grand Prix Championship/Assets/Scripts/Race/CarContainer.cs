using System.Collections.Generic;
using UnityEngine;

public class CarContainer : MonoBehaviour
{
    public List<Transform> CarObjects;

    void Awake()
    {
        foreach (Transform t in gameObject.GetComponentInChildren<Transform>())
        {
            CarObjects.Add(t);
        }
    }
}
