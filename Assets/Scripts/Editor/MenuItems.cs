using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.U2D;
using TMPro;

public class MenuItems
{
    [MenuItem("Tools/Load Level...")]
    private static void LoadLevel()
    {
        string path = EditorUtility.OpenFilePanel("Load Level File", "", "json");
        if (path.Length != 0)
        {
            string levelData = File.ReadAllText(path);
            GameObject levelLoader = GameObject.Find("LevelLoader");
            LevelLoader loader = levelLoader.GetComponent<LevelLoader>();
            loader.LoadLevelData(levelData);
        }
    }

    [MenuItem("Tools/Save Level...")]
    private static void SaveLevel()
    {
        string path = EditorUtility.SaveFilePanel("Save Level File", "", "level.json", "json");
        if (path.Length != 0)
        {
            string levelData = ExportLevel();
            File.WriteAllText(path, levelData);
        }
    }

    private static string ExportLevel()
    {
        Level level = new Level();

        ExportLevelData(level);
        ExportRocketData(level);
        ExportLandingPads(level);
        ExportPadMultipliers(level);
        ExportGroundPoints(level);
        ExportObstacles(level);

        string serialized = JsonUtility.ToJson(level, true);
        return serialized;
    }

    private static void ExportLevelData(Level level)
    {
        // Get level data
        GameObject levelIndicator = GameObject.Find("LevelIndicator");
        TextMeshProUGUI levelIndicatorText = levelIndicator.gameObject.GetComponent<TextMeshProUGUI>();
        level.name = levelIndicatorText.text;

        GameObject levelInstructions = GameObject.Find("LevelInstructions");
        TextMeshProUGUI levelInstructionsText = levelInstructions.gameObject.GetComponent<TextMeshProUGUI>();
        level.instructions = levelInstructionsText.text;

        GameObject scoreIndicator = GameObject.Find("ScoreIndicator");
        TextMeshProUGUI scoreIndicatorText = scoreIndicator.gameObject.GetComponent<TextMeshProUGUI>();
        level.baseScore = float.Parse(scoreIndicatorText.text);
    }

    private static void ExportRocketData(Level level)
    {
        // Export rocket data
        GameObject rocket = GameObject.Find("Rocket");
        RocketController rocketController = rocket.GetComponent<RocketController>();
        level.rocketPositionX = rocket.transform.position.x;
        level.rocketPositionY = rocket.transform.position.y;
        level.rocketRotationSpeed = rocketController.rotationSpeed;
        level.rocketPower = rocketController.rocketPower;
        level.rocketStartingFuel = rocketController.startingFuel;
        level.rocketFuelRate = rocketController.fuelRate;
        level.rocketRotation = rocket.transform.rotation;
        Rigidbody2D rigidbody2D = rocket.GetComponent<Rigidbody2D>();
        level.rocketMass = rigidbody2D.mass;
        level.rocketGravityScale = rigidbody2D.gravityScale;
    }

    private static void ExportLandingPads(Level level)
    {
        // Export landing pads
        GameObject landingPads = GameObject.Find("Landing Pads");
        level.landingPads = new LandingPad[landingPads.transform.childCount];
        int landingPadIndex = 0;
        foreach (Transform t in landingPads.transform)
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
    }

    private static void ExportPadMultipliers(Level level)
    {
        // Export landing pad multiplier labels
        GameObject padMultipliers = GameObject.Find("Pad Multipliers");
        level.padMultiplierLabels = new PadMultiplierLabel[padMultipliers.transform.childCount];
        int padMultiplierLabelIndex = 0;
        foreach (Transform t in padMultipliers.transform)
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
    }

    private static void ExportGroundPoints(Level level)
    {
        // Export ground points
        GameObject ground = GameObject.Find("Ground");
        SpriteShapeController ssc = ground.GetComponent<SpriteShapeController>();
        level.groundPoints = new PolygonPoint[ssc.spline.GetPointCount()];
        for (int groundPointIndex = 0; groundPointIndex < level.groundPoints.Length; groundPointIndex++)
        {
            Vector3 pointPosition = ssc.spline.GetPosition(groundPointIndex);
            PolygonPoint point = new PolygonPoint();
            point.x = pointPosition.x;
            point.y = pointPosition.y;
            level.groundPoints[groundPointIndex] = point;
        }
    }

    private static void ExportObstacles(Level level)
    {
        // Export obstacles
        GameObject obstacles = GameObject.Find("Obstacles");
        level.obstacles = new Obstacle[obstacles.transform.childCount];
        int obstacleIndex = 0;
        foreach (Transform t in obstacles.transform)
        {
            Obstacle obstacle = new Obstacle();
            GameObject obstacleObject = t.gameObject;
            obstacle.name = obstacleObject.name;
            obstacle.tag = obstacleObject.tag;
            obstacle.positionX = obstacleObject.transform.position.x;
            obstacle.positionY = obstacleObject.transform.position.y;
            obstacle.rotation = obstacleObject.transform.rotation;
            SpriteShapeController ossc = obstacleObject.GetComponent<SpriteShapeController>();
            obstacle.points = new PolygonPoint[ossc.spline.GetPointCount()];
            for (int polygonPointIndex = 0; polygonPointIndex < obstacle.points.Length; polygonPointIndex++)
            {
                Vector3 pointPosition = ossc.spline.GetPosition(polygonPointIndex);
                PolygonPoint point = new PolygonPoint();
                point.x = pointPosition.x;
                point.y = pointPosition.y;
                obstacle.points[polygonPointIndex] = point;
            }
            ObstacleData obstacleData = obstacleObject.GetComponent<ObstacleData>();
            obstacle.rotationRate = obstacleData.rotationRate;
            level.obstacles[obstacleIndex++] = obstacle;
        }
    }
}