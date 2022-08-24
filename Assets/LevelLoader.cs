using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

public class LevelLoader : MonoBehaviour
{
    public GameObject landingPadPrefab;
    public GameObject padMultiplierLabelPrefab;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void LoadLevel(int levelNumber)
    {
        // Load level from JSON
        TextAsset bindata = Resources.Load($"Levels/Level{levelNumber}") as TextAsset;
        Level level = JsonUtility.FromJson<Level>(bindata.text);

        SetLevelName(level.name);
        ConfigureGround(level.groundPoints);
        CreateLandingPads(level.landingPads);
        CreatePadMultiplierLabels(level.padMultiplierLabels);
    }

    void SetLevelName(string name)
    {
        GameObject gameObject = GameObject.Find("LevelIndicator");
        TextMeshProUGUI tmp = gameObject.GetComponent<TextMeshProUGUI>();
        tmp.text = name;
    }

    void ConfigureRocket(Level level)
    {
        GameObject rocket = GameObject.Find("Rocket");
        RocketController rocketController = rocket.GetComponent<RocketController>();
        rocket.transform.position = new Vector3(level.rocketPositionX, level.rocketPositionY);
        rocketController.rotationSpeed = level.rocketRotationSpeed;
        rocketController.rocketPower = level.rocketPower;
        rocketController.startingFuel = level.rocketStartingFuel;
        rocketController.fuelRate = level.rocketFuelRate;
        Rigidbody2D rigidbody2D = rocket.GetComponent<Rigidbody2D>();
        rigidbody2D.mass = level.rocketMass;
        rigidbody2D.gravityScale = level.rocketGravityScale;
    }

    void ConfigureGround(GroundPoint[] groundPoints)
    {
        GameObject ground = GameObject.Find("Ground");
        SpriteShapeController ssc = ground.GetComponent<SpriteShapeController>();

        ssc.spline.Clear();
        for (int i = 0; i < groundPoints.Length; i++)
        {
            ssc.spline.InsertPointAt(i, new Vector3(groundPoints[i].x, groundPoints[i].y, 0));
            ssc.spline.SetTangentMode(i, ShapeTangentMode.Linear);
        }
    }

    void CreateLandingPads(LandingPad[] landingPads)
    {
        GameObject landingPadsParent = GameObject.Find("Landing Pads");
        foreach (Transform child in landingPadsParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < landingPads.Length; i++)
        {
            LandingPad pad = landingPads[i];
            GameObject padObject = Instantiate<GameObject>(landingPadPrefab);
            padObject.transform.parent = landingPadsParent.transform;
            padObject.name = pad.name;
            padObject.transform.position = new Vector3(pad.positionX, pad.positionY);
            padObject.transform.localScale = new Vector3(pad.scaleX, pad.scaleY, 1);

            LandingPadData padData = padObject.GetComponent<LandingPadData>();
            padData.padMultiplier = pad.padMultiplier;

            BoxCollider2D collider = padObject.GetComponent<BoxCollider2D>();
            collider.size = new Vector2(pad.colliderSizeX, 0.1f);
        }
    }

    void CreatePadMultiplierLabels(PadMultiplierLabel[] padMultiplierLabels)
    {
        GameObject padMultipliersParent = GameObject.Find("Pad Multipliers");
        foreach (Transform child in padMultipliersParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for(int i = 0; i < padMultiplierLabels.Length; i++)
        {
            PadMultiplierLabel padMultiplierLabel = padMultiplierLabels[i];
            GameObject padMultiplierObject = Instantiate<GameObject>(padMultiplierLabelPrefab);
            TextMeshProUGUI tmp = padMultiplierObject.GetComponent<TextMeshProUGUI>();
            RectTransform rt = padMultiplierObject.GetComponent<RectTransform>();
            padMultiplierObject.transform.SetParent(padMultipliersParent.transform);
            padMultiplierObject.transform.localScale = Vector3.one;
            padMultiplierObject.name = padMultiplierLabel.name;
            tmp.text = padMultiplierLabel.text;
            rt.position = new Vector3(padMultiplierLabel.x, padMultiplierLabel.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
