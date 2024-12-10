using System.Collections.Generic;
using UnityEngine;

public class GridContainer : MonoBehaviour
{
    public List<Transform> GridPositions;

    void Awake()
    {
        foreach (Transform t in gameObject.GetComponentInChildren<Transform>())
        {
            GridPositions.Add(t);
        }
    }

}
