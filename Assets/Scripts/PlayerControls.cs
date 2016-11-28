﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour {
	public int minSpeed = 10;
	public int maxSpeed = 75;
	private int speedIncrease = 5;
	public int currentSpeed = 10; // Initially is the slowest speed
	public float rotateSpeed = 45.0f;
	public bool slowingDown = false;
	public MenuManager menuManager;
    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate = 0.01f;
    public float deviationIncreaseAmount = 0.1f;
    public float deviationDecreaseAmount = 0.1f;
    public Slider speedSlider;

    private float shotDeviationScale = 1;
    private Time mouseHeldDown;
    private Time mouseUp;
    private bool shooting;
    private float nextFire = 0.0f;
    private float bulletStrayX = 0;
    private float bulletStrayY = 0;
    private float bulletStrayZ = 0;
	private Circle deadzone;
    private GameObject sancho;
    private AsteroidSpawner asteroidSpawner;

    // Use this for initialization
    void Start () {
		Vector2 c = new Vector2 (Screen.width / 2, Screen.height / 2);
		deadzone = new Circle(c.x, c.y, 20f);
        sancho = GameObject.Find("Sancho");
        asteroidSpawner = sancho.GetComponent<AsteroidSpawner>();
        speedSlider.minValue = minSpeed;
        speedSlider.maxValue = maxSpeed;
        speedSlider.value = currentSpeed;
    }
	
    void Update() {
        if (Time.timeScale == 1) {
            if ((Input.GetButton("Fire1")) && (Time.time > nextFire)) {
                nextFire = Time.time + fireRate;
                GameObject newshot = (GameObject)Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
                if (shooting) {
                    // generate random numbers for stray
                    bulletStrayX = Random.Range((-1 * shotDeviationScale), (1 * shotDeviationScale));
                    bulletStrayY = Random.Range((-1 * shotDeviationScale), (1 * shotDeviationScale));
                    bulletStrayZ = Random.Range((-1 * shotDeviationScale), (1 * shotDeviationScale));
                    newshot.transform.Rotate(bulletStrayX, bulletStrayY, bulletStrayZ);
                }
                shooting = true;
            }
            if (Input.GetButtonUp("Fire1")) {
                shooting = false;
            }
            if (shooting) {
                shotDeviationScale += deviationIncreaseAmount;
                if (shotDeviationScale > 5f) {
                    shotDeviationScale = 5;
                }
            }
            else {
                shotDeviationScale -= deviationDecreaseAmount;
                if (shotDeviationScale < 1)
                {
                    shotDeviationScale = 1;
                }
            }
        }
    }

	// Update is called once per frame
	void LateUpdate () {
        if (Time.timeScale == 1.0f) {
            if (Input.GetKey(KeyCode.A)) {
                transform.Rotate(0, 0, Time.deltaTime * rotateSpeed);
            }
            else if (Input.GetKey(KeyCode.D)) {
                transform.Rotate(0, 0, -Time.deltaTime * rotateSpeed);
            }
            if (slowingDown) {
                ModifySpeed(-speedIncrease * 5);
                if (currentSpeed == minSpeed) {
                    currentSpeed = 0;
                    menuManager.increaseLevel();
                }
            }
            else {
                // Spacecraft Acceleration/Deceleration
                if (Input.GetKey(KeyCode.W)) {
                    ModifySpeed(speedIncrease);
                }
                else if (Input.GetKey(KeyCode.S)) {
                    ModifySpeed(-speedIncrease);
                }
            }
            Vector3 mousePos = (Input.mousePosition - (new Vector3(Screen.width, Screen.height, 0) / 2.0f));
            if (deadzone.Contains(Input.mousePosition)) {
                mousePos = Vector3.zero;
            }
            transform.Rotate(new Vector3(-mousePos.y, mousePos.x, -mousePos.x) * 0.005f);
            transform.Translate(Vector3.forward * Time.deltaTime * currentSpeed);
        }
	}

	void OnCollisionEnter(Collision col) {
		if (col.gameObject.tag == "Shot") {
			Destroy (col.gameObject);
		}
	}

	void ModifySpeed(int speed) {
		currentSpeed += speed;
		if (currentSpeed < minSpeed) {
			currentSpeed = minSpeed;
		}
		if (currentSpeed > maxSpeed) {
			currentSpeed = maxSpeed;
		}
        speedSlider.value = currentSpeed;
	}
}
