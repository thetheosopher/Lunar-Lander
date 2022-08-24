using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RocketController : MonoBehaviour
{
    public ParticleSystem rocketFlame;
    public ParticleSystem explosion;
    public AudioSource explosionSound;

    public GameController gameController;

    public float rotationSpeed = 10.0f;
    public float rocketPower = 200000.0f;
    public float startingFuel = 1000.0f;
    public float fuelRate = 100.0f;

    private float currentFuel;
    private bool rocketOn = false;
    private bool exploded = false;
    private bool collided = false;
    private AudioSource rocketSound;
    private Rigidbody2D rigidBody;
    private Vector3 startingPosition;
    private Quaternion startingRotation;

    private float landingVelocity;
    private float landingVelocityX;
    private float landingVelocityY;
    private float landingAttitude;
    private float landingFuelRemaining;
    private float landingPadPosition;
    private float landingPadMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        rocketSound = GetComponent<AudioSource>();
        rigidBody = GetComponent<Rigidbody2D>();
        gameController.fuelGauge.SetMaxValue(startingFuel);
        gameController.UpdateFuelGauge(startingFuel);
        startingPosition = gameObject.transform.position;
        startingRotation = gameObject.transform.rotation;
        currentFuel = startingFuel;
        exploded = false;
        collided = false;
    }

    public void Reset()
    {
        rigidBody.constraints = RigidbodyConstraints2D.None;
        exploded = false;
        collided = false;
        currentFuel = startingFuel;
        gameController.UpdateFuelGauge(currentFuel);
        gameObject.transform.position = startingPosition;
        gameObject.transform.rotation = startingRotation;
        rigidBody.velocity = Vector2.zero;
        rocketOn = false;
        rocketFlame.Stop();
        rocketSound.Stop();
        gameController.HideLandingInfoUI();
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(!collided)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            gameObject.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime * horizontalInput);
        }

        if (Input.GetKey(KeyCode.Space) && currentFuel > 0 && !collided)
        {
            if(!rocketOn)
            {
                rocketFlame.Play();
                rocketOn = true;
                rocketSound.Play();
            }
        }
        else
        {
            if(rocketOn)
            {
                rocketFlame.Stop();
                rocketOn=false;
                rocketSound.Stop();
            }
        }
    }

    public void FixedUpdate()
    {
        // Add force if rocket is on
        if (rocketOn)
        {
            rigidBody.AddRelativeForce(Vector2.up * Time.deltaTime * rocketPower, ForceMode2D.Force);
            currentFuel -= fuelRate * Time.deltaTime;
            if (currentFuel < 0 || collided)
            {
                currentFuel = 0;
                rocketFlame.Stop();
                rocketSound.Stop();
                rocketOn = false;
            }
        }
        if (!collided)
        {
            landingAttitude = getAttitude();
            landingVelocityY = rigidBody.velocity.y;
            landingVelocityX = rigidBody.velocity.x;
            landingFuelRemaining = currentFuel / startingFuel;
            gameController.UpdateFlightStats(landingVelocityY, landingVelocityX, landingAttitude, currentFuel);
        }
    }

    public float getAttitude()
    {
        float angle = 360 - (gameObject.transform.rotation.eulerAngles.z % 360);
        if(angle > 180)
        {
            angle -= 360;
        }
        return angle;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collided)
        {
            return;
        }
        collided = true;
        if(collision.gameObject.CompareTag("Ground"))
        {
            Explode();
            OnFailure("You crashed");
        }
        else if(collision.gameObject.CompareTag("Boundary"))
        {
            gameObject.SetActive(false);
            OnFailure("You went off course");
        }
        else if(collision.gameObject.CompareTag("Landing Pad"))
        {
            landingVelocity = collision.GetContact(0).relativeVelocity.magnitude;
            landingFuelRemaining = currentFuel / startingFuel;

            LandingPadData lpd = collision.gameObject.GetComponent<LandingPadData>();
            landingPadMultiplier = lpd.padMultiplier;

            if (landingVelocity > 0.66f)
            {
                Explode();
                OnFailure("You landed too hard");
            }
            else if(Mathf.Abs(landingAttitude) > 3f)
            {
                Explode();
                OnFailure("You landed at a tilt");
            }
            else
            {
                var point = collision.GetContact(0);
                var go = collision.gameObject.transform.position;

                var delta = point.point.x - go.x;
                landingPadPosition = delta;

                rigidBody.velocity = Vector2.zero;
                gameObject.transform.rotation = Quaternion.identity;
                OnSuccess();
            }
        }
    }

    private void Explode()
    {
        if (!exploded)
        {
            explosionSound.Play();
            exploded = true;
            explosion.transform.position = transform.position;
            explosion.Play();
            gameObject.SetActive(false);
        }
    }

    private void OnSuccess()
    {
        rocketFlame.Stop();
        rocketFlame.Clear();
        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        gameController.UpdateLandingStats(new LandingStats
        {
            velocity = landingVelocity,
            attitude = landingAttitude,
            fuelRemaining = landingFuelRemaining,
            padPosition = landingPadPosition,
            padMultiplier = landingPadMultiplier
        });
        gameController.OnSuccess();
    }

    private void OnFailure(string reason)
    {
        gameController.OnFailure(reason);
    }
}
