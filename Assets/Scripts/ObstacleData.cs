using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleData : MonoBehaviour
{
    public float rotationRate = 0;

    public void Update()
    {
        gameObject.transform.Rotate(Vector3.forward, rotationRate * Time.deltaTime);
    }
}
