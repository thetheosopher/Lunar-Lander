using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Level
{
    public string name;
    public string instructions;
    public float baseScore;

    public float rocketPositionX;
    public float rocketPositionY;
    public float rocketRotationSpeed;
    public float rocketPower;
    public float rocketStartingFuel;
    public float rocketFuelRate;
    public float rocketMass;
    public float rocketGravityScale;
    public SerializableQuaternion rocketRotation;

    public LandingPad[] landingPads;
    public PolygonPoint[] groundPoints;
    public PadMultiplierLabel[] padMultiplierLabels;
    public Obstacle[] obstacles;
}

[Serializable]
public class LandingPad
{
    public string name;
    public float padMultiplier;
    public float positionX;
    public float positionY;
    public float scaleX;
    public float scaleY;
    public float colliderSizeX;
}

[Serializable]
public class PolygonPoint
{
    public float x;
    public float y;
}

[Serializable]
public class PadMultiplierLabel
{
    public string name;
    public string text;
    public float x;
    public float y;
}

[Serializable]
public class Obstacle
{
    public string name;
    public string tag;
    public float positionX;
    public float positionY;
    public SerializableQuaternion rotation;
    public float rotationRate;
    public PolygonPoint[] points;
}
