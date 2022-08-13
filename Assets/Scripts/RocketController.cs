using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RocketController : MonoBehaviour
{
    public ParticleSystem rocketFlame;
    public TextMeshProUGUI verticalVelocityText;
    public TextMeshProUGUI horizontalVelocityText;
    public TextMeshProUGUI attitudeText;

    public float rotationSpeed = 10.0f;
    public float rocketPower = 1000.0f;

    private bool rocketOn = false;
    private AudioSource rocketSound;
    private Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rocketSound = GetComponent<AudioSource>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        gameObject.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime * horizontalInput);

        if(Input.GetKey(KeyCode.Space))
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
        }

        verticalVelocityText.SetText($"Vert Velocity: { rigidBody.velocity.y:0.##} m/Sec");
        horizontalVelocityText.SetText($"Horz Velocity: { rigidBody.velocity.x:0.##} m/Sec");
        attitudeText.SetText($"Attitude: {(360 - gameObject.transform.rotation.eulerAngles.z)%360:0.##}­°");
    }
}
