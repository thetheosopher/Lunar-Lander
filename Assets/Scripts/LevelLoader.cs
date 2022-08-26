using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

public class LevelLoader : MonoBehaviour
{
    public GameObject landingPadPrefab;
    public GameObject padMultiplierLabelPrefab;

    public void LoadLevel(int levelNumber)
    {
        // Load level from JSON
        TextAsset bindata = Resources.Load($"Levels/Level{levelNumber}") as TextAsset;
        Level level = JsonUtility.FromJson<Level>(bindata.text);

        SetLevelName(level.name);
        SetLevelInstructions(level.instructions);
        ConfigureRocket(level);
        ConfigureGround(level.groundPoints);
        CreateLandingPads(level.landingPads);
        CreatePadMultiplierLabels(level.padMultiplierLabels);
    }
    public void LoadLevelData(string levelData)
    {
        // Load level from JSON
        Level level = JsonUtility.FromJson<Level>(levelData);

        SetLevelName(level.name);
        SetLevelInstructions(level.instructions);
        SetLevelBaseScore(level.baseScore);
        ConfigureRocket(level);
        ConfigureGround(level.groundPoints);
        CreateLandingPads(level.landingPads);
        CreatePadMultiplierLabels(level.padMultiplierLabels);

        GameObject gameController = GameObject.Find("GameController");
        GameController controller = gameController.GetComponent<GameController>();
        controller.ActivateMainCamera();
    }

    void SetLevelName(string name)
    {
        GameObject gameObject = GameObject.Find("LevelIndicator");
        TextMeshProUGUI tmp = gameObject.GetComponent<TextMeshProUGUI>();
        tmp.text = name;
    }

    void SetLevelInstructions(string instructions)
    {
        GameObject gameObject = GameObject.Find("LevelInstructions");
        if(gameObject != null)
        {
            if(!string.IsNullOrEmpty(instructions))
            {
                gameObject.SetActive(true);
                TextMeshProUGUI tmp = gameObject.GetComponent<TextMeshProUGUI>();
                tmp.text = instructions;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    void SetLevelBaseScore(float baseScore)
    {
        if(!Application.isPlaying)
        {
            GameObject gameObject = GameObject.Find("ScoreIndicator");
            TextMeshProUGUI tmp = gameObject.GetComponent<TextMeshProUGUI>();
            tmp.text = ((int)baseScore).ToString();
        }
    }

    void ConfigureRocket(Level level)
    {
        GameObject rocket = GameObject.Find("Rocket");
        RocketController rocketController = rocket.GetComponent<RocketController>();
        rocket.transform.position = new Vector3(level.rocketPositionX, level.rocketPositionY);
        rocketController.startingPosition = rocket.transform.position;
        rocketController.rotationSpeed = level.rocketRotationSpeed;
        rocketController.rocketPower = level.rocketPower;
        rocketController.startingFuel = level.rocketStartingFuel;
        rocketController.fuelRate = level.rocketFuelRate;
        rocketController.startingRotation = level.rocketRotation;
        rocket.transform.rotation = level.rocketRotation;
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
        DeleteChildren(landingPadsParent);
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
            collider.size = new Vector2(pad.colliderSizeX, 1.0f);
        }
    }

    void CreatePadMultiplierLabels(PadMultiplierLabel[] padMultiplierLabels)
    {
        GameObject padMultipliersParent = GameObject.Find("Pad Multipliers");
        DeleteChildren(padMultipliersParent);
        for (int i = 0; i < padMultiplierLabels.Length; i++)
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

    void DeleteChildren(GameObject g)
    {
        if (g == null) return;
        if (Application.isEditor)
        {
            for (var i = g.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(g.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            for (var i = g.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(g.transform.GetChild(i).gameObject);
            }
        }
    }
}
