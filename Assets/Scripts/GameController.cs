using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public struct LandingStats
{
    public float velocityX;
    public float velocityY;
    public float velocity;
    public float attitude;
    public float fuelRemaining;
    public float padMultiplier;
    public float padPosition;
}

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI verticalVelocityText;
    public TextMeshProUGUI horizontalVelocityText;
    public TextMeshProUGUI attitudeText;
    public TextMeshProUGUI totalScoreText;
    public TextMeshProUGUI altitudeText;
    public GaugeScript fuelGauge;
    public TextMeshProUGUI successText;
    public TextMeshProUGUI landingScoreText;
    public TextMeshProUGUI failureText;
    public TextMeshProUGUI failureReasonText;
    public Button playAgainButton;
    public Button continueButton;
    public Canvas introCanvas;
    public Canvas instrumentCanvas;
    public Image landingStatsPanel;
    public TextMeshProUGUI landingVelocityText;
    public TextMeshProUGUI landingAttitudeText;
    public TextMeshProUGUI landingFuelRemainingText;
    public TextMeshProUGUI landingPadPositionText;
    public TextMeshProUGUI landingPadMultiplierText;
    public GameObject rocket;
    public Camera mainCamera;
    public Camera closeUpCamera;
    public float distanceScalingFactor = 2.0f;

    public LevelLoader levelLoader;
    public int startingLevel = 0;
    public int currentLevel = 0;
    public int maxLevel = 3;
    public int lastScore = 0;
    public int totalScore = 0;
    public bool designMode = false;

    private float zoomStartSize;
    private float zoomEndSize;
    private float zoomStartX;
    private float zoomStartY;
    private float zoomEndX;
    private float zoomEndY;
    private float lastZoomTime;
    private bool zooming;

    public float zoomSteps = 60;
    public float zoomTime = 0.5f;

    private bool closeUpCameraActive;

    private void Start()
    {
        if(startingLevel != 0)
        {
            levelLoader.LoadLevel(startingLevel);
            currentLevel = startingLevel;
        }
    }

    private void FixedUpdate()
    {
        if (!zooming)
        {
            closeUpCamera.transform.position = new Vector3(
                Mathf.Min(Mathf.Max(rocket.transform.position.x, -5.53f), 5.53f),
                Mathf.Max(-3, rocket.transform.position.y - 1), -10);
        }
    }

    public void UpdateFuelGauge(float value)
    {
        fuelGauge.SetValue(value);
    }

    public void StartGame()
    {
        ActivateMainCamera();
        introCanvas.gameObject.SetActive(false);
        instrumentCanvas.gameObject.SetActive(true);
        levelLoader.LoadLevel(startingLevel);
        totalScore = 0;
        totalScoreText.text = "0";
        RocketController rc = rocket.GetComponent<RocketController>();
        rc.Reset();
        Time.timeScale = 1;
    }

    public void StartTest()
    {
        ActivateMainCamera();
        introCanvas.gameObject.SetActive(false);
        instrumentCanvas.gameObject.SetActive(true);
        RocketController rc = rocket.GetComponent<RocketController>();
        rc.Reset();
        Time.timeScale = 1;
    }

    public void LoadNextLevel()
    {
        currentLevel++;
        levelLoader.LoadLevel(currentLevel);
        RocketController rc = rocket.GetComponent<RocketController>();
        rc.Reset();
    }

    public void ActivateMainCamera()
    {
        zoomStartSize = 2;
        zoomEndSize = 5;
        zoomStartX = Mathf.Min(Mathf.Max(rocket.transform.position.x, -5.53f), 5.53f);
        zoomStartY = Mathf.Max(-3, rocket.transform.position.y - 1);
        zoomEndX = 0;
        zoomEndY = 0;
        StartCoroutine(ZoomToMainCoroutine());
    }

    IEnumerator ZoomToMainCoroutine()
    {
        zooming = true;
        closeUpCamera.enabled = true;
        mainCamera.enabled = false;
        float zoomSizeStep = (zoomEndSize - zoomStartSize) / zoomSteps;
        float zoomXStep = (zoomEndX - zoomStartX) / zoomSteps;
        float zoomYStep = (zoomEndY - zoomStartY) / zoomSteps;
        for(int i = 0; i < zoomSteps; i++)
        {
            closeUpCamera.orthographicSize = zoomStartSize + zoomSizeStep * i;
            closeUpCamera.transform.position = new Vector3(
                zoomStartX + zoomXStep * i,
                zoomStartY + zoomYStep * i, -10);
            yield return new WaitForSeconds(zoomTime / zoomSteps);
        }
        closeUpCamera.orthographicSize = zoomEndSize;
        closeUpCamera.transform.position = new Vector3(zoomEndX, zoomEndY, -10);
        mainCamera.enabled = true;
        closeUpCamera.enabled = false;
        closeUpCameraActive = false;
        lastZoomTime = Time.fixedTime;
        zooming = false;
    }

    public void ActivateCloseUpCamera()
    {
        zoomStartSize = 5;
        zoomEndSize = 2;
        zoomEndX = Mathf.Min(Mathf.Max(rocket.transform.position.x, -5.53f), 5.53f);
        zoomEndY = Mathf.Max(-3, rocket.transform.position.y - 1);
        zoomStartX = 0;
        zoomStartY = 0;
        StartCoroutine(ZoomToCloseUpCoroutine());
    }

    IEnumerator ZoomToCloseUpCoroutine()
    {
        zooming = true;
        closeUpCameraActive = true;
        closeUpCamera.enabled = true;
        mainCamera.enabled = false;
        float zoomSizeStep = (zoomEndSize - zoomStartSize) / zoomSteps;
        float zoomXStep = (zoomEndX - zoomStartX) / zoomSteps;
        float zoomYStep = (zoomEndY - zoomStartY) / zoomSteps;
        for (int i = 0; i < zoomSteps; i++)
        {
            closeUpCamera.orthographicSize = zoomStartSize + zoomSizeStep * i;
            closeUpCamera.transform.position = new Vector3(
                zoomStartX + zoomXStep * i,
                zoomStartY + zoomYStep * i, -10);
            yield return new WaitForSeconds(zoomTime / zoomSteps);
        }
        closeUpCamera.orthographicSize = zoomEndSize;
        closeUpCamera.transform.position = new Vector3(zoomEndX, zoomEndY, -10);
        lastZoomTime = Time.fixedTime;
        zooming = false;
    }

    public void HideLandingInfoUI()
    {
        failureText.gameObject.SetActive(false);
        failureReasonText.gameObject.SetActive(false);
        successText.gameObject.SetActive(false);
        playAgainButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        landingScoreText.gameObject.SetActive(false);
        landingStatsPanel.gameObject.SetActive(false);
    }

    public void UpdateFlightStats(float verticalVelocity, float horizontalVelocity, 
        float attitude, float currentFuel, float altitude)
    {
        verticalVelocityText.SetText($"{verticalVelocity:0} m/Sec");
        horizontalVelocityText.SetText($"{horizontalVelocity:0} m/Sec");
        attitudeText.SetText($"{attitude:0.#}­°");
        fuelGauge.SetValue(currentFuel);
        altitudeText.SetText($"{altitude:0}­ meters");

        if(altitude < 200 && !closeUpCameraActive && !zooming && Time.fixedTime - lastZoomTime > 1)
        {
            ActivateCloseUpCamera();
        }
        if(altitude > 300 && closeUpCameraActive && !zooming && Time.fixedTime - lastZoomTime > 1)
        {
            ActivateMainCamera();
        }
    }

    public void UpdateLandingStats(LandingStats stats)
    {
        landingVelocityText.SetText($"{stats.velocity:0} m/Sec");
        landingAttitudeText.SetText($"{stats.attitude:0.#}­°");
        landingFuelRemainingText.SetText($"{stats.fuelRemaining * 100.0f:0.#}%");
        landingPadPositionText.SetText($"{stats.padPosition:0.#}");
        landingPadMultiplierText.SetText($"{stats.padMultiplier}x");
        lastScore = (int)ComputeScore(stats);
        landingScoreText.SetText($"Landing Score: {lastScore:#,###}");
    }

    private float ComputeScore(LandingStats stats)
    {
        float baseScore = 1000f;
        float velocityBonus = 1000 - Mathf.Abs(stats.velocity/distanceScalingFactor) * 1000.0f;
        float attitudeBonus = 1000 - Mathf.Abs(stats.attitude/distanceScalingFactor) * 1000.0f;
        float fuelBonus = 1000 - stats.fuelRemaining * 1000.0f;
        float padPositionBonus = 1000 - Mathf.Abs(stats.padPosition) * 1000f;
        float score = baseScore + velocityBonus + attitudeBonus + fuelBonus + padPositionBonus;
        score *= stats.padMultiplier;

        return score;
    }

    public void OnSuccess()
    {
        successText.gameObject.SetActive(true);
        landingScoreText.gameObject.SetActive(true);
        landingStatsPanel.gameObject.SetActive(true);
        if (!designMode)
        {
            totalScore += lastScore;
            totalScoreText.text = $"{totalScore:#,###}";
        }
        StartCoroutine(PlayAgainCoroutine(true));
    }

    public void OnFailure(string reason)
    {
        failureText.gameObject.SetActive(true);
        failureReasonText.gameObject.SetActive(true);
        failureReasonText.text = reason;
        StartCoroutine(PlayAgainCoroutine(false));
    }

    IEnumerator PlayAgainCoroutine(bool success)
    {
        yield return new WaitForSeconds(3);
        ActivateMainCamera();
        if (designMode)
        {
            playAgainButton.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(playAgainButton.gameObject);
        }
        else
        {
            if (success)
            {
                if (currentLevel < maxLevel)
                {
                    continueButton.gameObject.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
                }
            }
            else
            {
                playAgainButton.gameObject.SetActive(true);
                EventSystem.current.SetSelectedGameObject(playAgainButton.gameObject);
            }
        }
    }
}
