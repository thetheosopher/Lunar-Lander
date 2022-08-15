using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RocketController : MonoBehaviour
{
    public ParticleSystem rocketFlame;
    public ParticleSystem explosion;
    public TextMeshProUGUI verticalVelocityText;
    public TextMeshProUGUI horizontalVelocityText;
    public TextMeshProUGUI attitudeText;
    public GaugeScript fuelGauge;
    public TextMeshProUGUI successText;
    public TextMeshProUGUI failureText;
    public TextMeshProUGUI failureReasonText;
    public Button playAgainButton;
    public AudioSource explosionSound;

    public float rotationSpeed = 10.0f;
    public float rocketPower = 1000.0f;
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

    // Start is called before the first frame update
    void Start()
    {
        rocketSound = GetComponent<AudioSource>();
        rigidBody = GetComponent<Rigidbody2D>();
        fuelGauge.SetMaxValue(startingFuel);
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
        gameObject.transform.position = startingPosition;
        gameObject.transform.rotation = startingRotation;
        rigidBody.velocity = Vector2.zero;
        rocketOn = false;
        rocketFlame.Stop();
        rocketSound.Stop();
        failureText.gameObject.SetActive(false);
        failureReasonText.gameObject.SetActive(false);
        successText.gameObject.SetActive(false);
        playAgainButton.gameObject.SetActive(false);
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

        // Add force if rocket is on
        if(rocketOn)
        {
            rigidBody.AddRelativeForce(Vector2.up * Time.deltaTime * rocketPower, ForceMode2D.Force);
            currentFuel -= fuelRate * Time.deltaTime;
            if(currentFuel < 0 || collided)
            {
                currentFuel = 0;
                rocketFlame.Stop();
                rocketSound.Stop();
                rocketOn = false;
            }
            fuelGauge.SetValue(currentFuel);
        }

        verticalVelocityText.SetText($"{ rigidBody.velocity.y:0.##} m/Sec");
        horizontalVelocityText.SetText($"{ rigidBody.velocity.x:0.##} m/Sec");
        attitudeText.SetText($"{getAttitude():0.##}­°");
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
            OnFailure("You missed the landing pad");
        }
        else if(collision.gameObject.CompareTag("Boundary"))
        {
            gameObject.SetActive(false);
            OnFailure("You went too far off course.");
        }
        else if(collision.gameObject.CompareTag("Landing Pad"))
        {
            float attitude = getAttitude();
            Debug.Log("Collision Velocity:" + collision.GetContact(0).relativeVelocity.magnitude);
            if (collision.GetContact(0).relativeVelocity.magnitude > 0.66f)
            {
                Explode();
                OnFailure("You landed too hard.");
            }
            else if(Mathf.Abs(rigidBody.velocity.x) > 0.5f)
            {
                Explode();
                OnFailure("You landed with too much horizontal velocity.");
            }
            else if(Mathf.Abs(attitude) > 3f)
            {
                Explode();
                OnFailure("You landed with too much tilt");
            }
            else
            {
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
        successText.gameObject.SetActive(true);
        playAgainButton.gameObject.SetActive(true);
    }

    private void OnFailure(string reason)
    {
        failureText.gameObject.SetActive(true);
        failureReasonText.gameObject.SetActive(true);
        failureReasonText.text = reason;
        playAgainButton.gameObject.SetActive(true);
    }
}
