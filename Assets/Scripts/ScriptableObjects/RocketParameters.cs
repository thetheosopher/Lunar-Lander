using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RocketParameters", menuName = "ScriptableObjects/RocketParameters", order = 1)]
public class RocketParameters : ScriptableObject
{
    public float rotationSpeed;
    public float rocketPower;
    public float startingFuel;
    public float fuelRate;
}
