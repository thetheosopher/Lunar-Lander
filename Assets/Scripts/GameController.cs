using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public GaugeScript fuelGauge;
    public TextMeshProUGUI successText;
    public TextMeshProUGUI landingScoreText;
    public TextMeshProUGUI failureText;
    public TextMeshProUGUI failureReasonText;
    public Button playAgainButton;
    public Canvas introCanvas;
    public Canvas instrumentCanvas;
    public Image landingStatsPanel;
    public TextMeshProUGUI landingVelocityText;
    public TextMeshProUGUI landingAttitudeText;
    public TextMeshProUGUI landingFuelRemainingText;
    public TextMeshProUGUI landingPadPositionText;
    public TextMeshProUGUI landingPadMultiplierText;

    public LevelLoader levelLoader;

    private void Start()
    {
        levelLoader.LoadLevel(1);
    }

    public void UpdateFuelGauge(float value)
    {
        fuelGauge.SetValue(value);
    }

    public void StartGame()
    {
        introCanvas.gameObject.SetActive(false);
        instrumentCanvas.gameObject.SetActive(true);
        levelLoader.LoadLevel(1);
        Time.timeScale = 1;
    }

    public void HideLandingInfoUI()
    {
        failureText.gameObject.SetActive(false);
        failureReasonText.gameObject.SetActive(false);
        successText.gameObject.SetActive(false);
        playAgainButton.gameObject.SetActive(false);
        landingScoreText.gameObject.SetActive(false);
        landingStatsPanel.gameObject.SetActive(false);
    }

    public void UpdateFlightStats(float verticalVelocity, float horizontalVelocity, float attitude, float currentFuel)
    {
        verticalVelocityText.SetText($"{verticalVelocity:0.##} m/Sec");
        horizontalVelocityText.SetText($"{horizontalVelocity:0.##} m/Sec");
        attitudeText.SetText($"{attitude:0.##}­°");
        fuelGauge.SetValue(currentFuel);
    }

    public void UpdateLandingStats(LandingStats stats)
    {
        landingVelocityText.SetText($"{stats.velocity:0.##} m/Sec");
        landingAttitudeText.SetText($"{stats.attitude:0.##}­°");
        landingFuelRemainingText.SetText($"{stats.fuelRemaining * 100.0f:0.#}%");
        landingPadPositionText.SetText($"{stats.padPosition:0.##}");
        landingPadMultiplierText.SetText($"{stats.padMultiplier}x");
        landingScoreText.SetText($"Landing Score: {ComputeScore(stats):0}");
    }

    private float ComputeScore(LandingStats stats)
    {
        float baseScore = 1000f;
        float velocityBonus = 1000 - Mathf.Abs(stats.velocity) * 1000.0f;
        float attitudeBonus = 1000 - Mathf.Abs(stats.attitude) * 1000.0f;
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
        StartCoroutine(PlayAgainCoroutine());
    }

    public void OnFailure(string reason)
    {
        failureText.gameObject.SetActive(true);
        failureReasonText.gameObject.SetActive(true);
        failureReasonText.text = reason;
        StartCoroutine(PlayAgainCoroutine());
    }

    IEnumerator PlayAgainCoroutine()
    {
        yield return new WaitForSeconds(3);
        playAgainButton.gameObject.SetActive(true);
    }
}
