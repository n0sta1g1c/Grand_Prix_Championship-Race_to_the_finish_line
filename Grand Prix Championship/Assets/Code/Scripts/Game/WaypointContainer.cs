using System.Collections.Generic;
using UnityEngine;

public class WaypointContainer : MonoBehaviour
{
    public List<Transform> Waypoints;
    void Awake()
    {
        foreach (Transform t in gameObject.GetComponentInChildren<Transform>())
        {
            Waypoints.Add(t);
        }
    }
}
