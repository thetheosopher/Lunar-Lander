using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using TMPro;

public class LevelInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Export landing pad data
        // ExportLandingPads();
    }

    public void SaveData()
    {
        string levelData = ExportData();
        File.WriteAllText(@"F:\Temp\Game Data\Lunar Lander\Level.json", levelData);
    }

    private string ExportData()
    {
        Level level = new Level();
        level.name = "Level XXX";
        level.baseScore = 1000.0f;

        // Export rocket data
        GameObject rocket = GameObject.Find("Rocket");
        RocketController rocketController = rocket.GetComponent<RocketController>();
        level.rocketPositionX = rocket.transform.position.x;
        level.rocketPositionY = rocket.transform.position.y;
        level.rocketRotationSpeed = rocketController.rotationSpeed;
        level.rocketPower = rocketController.rocketPower;
        level.rocketStartingFuel = rocketController.startingFuel;
        level.rocketFuelRate= rocketController.fuelRate;
        Rigidbody2D rigidbody2D = rocket.GetComponent<Rigidbody2D>();
        level.rocketMass = rigidbody2D.mass;
        level.rocketGravityScale = rigidbody2D.gravityScale;

        // Export landing pads
        GameObject landingPads = GameObject.Find("Landing Pads");
        level.landingPads = new LandingPad[landingPads.transform.childCount];
        int landingPadIndex = 0;
        foreach(Transform t in landingPads.transform)
        {
            GameObject child = t.gameObject;
            LandingPad landingPad = new LandingPad();
            landingPad.name = child.name;
            landingPad.positionX = t.position.x;
            landingPad.positionY = t.position.y;
            landingPad.scaleX = t.localScale.x;
            landingPad.scaleY = t.localScale.y;

            LandingPadData padData = child.GetComponent<LandingPadData>();
            landingPad.padMultiplier = padData.padMultiplier;

            BoxCollider2D collider = child.GetComponent<BoxCollider2D>();
            landingPad.colliderSizeX = collider.size.x;
            level.landingPads[landingPadIndex++] = landingPad;
        }

        // Export landing pad multiplier labels
        GameObject padMultipliers = GameObject.Find("Pad Multipliers");
        level.padMultiplierLabels = new PadMultiplierLabel[padMultipliers.transform.childCount];
        int padMultiplierLabelIndex = 0;
        foreach(Transform t in padMultipliers.transform)
        {
            TextMeshProUGUI child = t.gameObject.GetComponent<TextMeshProUGUI>();
            RectTransform rt = t.gameObject.GetComponent<RectTransform>();
            PadMultiplierLabel padMultiplierLabel = new PadMultiplierLabel();
            padMultiplierLabel.name = t.gameObject.name;
            padMultiplierLabel.x = rt.position.x;
            padMultiplierLabel.y = rt.position.y;
            padMultiplierLabel.text = child.text;
            level.padMultiplierLabels[padMultiplierLabelIndex++] = padMultiplierLabel;
        }

        // Export ground points
        GameObject ground = GameObject.Find("Ground");
        SpriteShapeController ssc = ground.GetComponent<SpriteShapeController>();
        level.groundPoints = new GroundPoint[ssc.spline.GetPointCount()];
        for(int groundPointIndex = 0; groundPointIndex < level.groundPoints.Length; groundPointIndex++)
        {
            Vector3 pointPosition = ssc.spline.GetPosition(groundPointIndex);
            GroundPoint point = new GroundPoint();
            point.x = pointPosition.x;
            point.y = pointPosition.y;
            level.groundPoints[groundPointIndex] = point;
        }

        string serialized = JsonUtility.ToJson(level);
        return serialized;
    }
}
