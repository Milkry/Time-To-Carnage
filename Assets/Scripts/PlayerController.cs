using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int maxHealth = 100;
    public HealthBar healthbar;
    [HideInInspector] public int currentHealth;

    [SerializeField] private float speed;
    [SerializeField] private Joystick joystick;
    [SerializeField] private GameObject weaponHolder;
    [Space]
    [Header("Tutorial")]
    [Space]
    [SerializeField] private GameObject tutorial;
    [SerializeField] private GameObject firstPart;
    [SerializeField] private GameObject joystickTutorialBox;
    [SerializeField] private GameObject shootButtonTutorialBox;


    private Rigidbody2D rb;
    //private WeaponSwitching weapon;
    private Vector2 moveVelocity;
    private float saveStartDelay = 60f;
    private float saveDelay = 60f;
    private string PPTutorial = "TutorialDone";

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //weapon = weaponHolder.GetComponent<WeaponSwitching>();
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
        InvokeRepeating("AutoSave", saveStartDelay, saveDelay);
        PlayTutorial();
    }

    private void Update()
    {
        //Joystick positioning
        Vector2 joystickInput = new Vector2(joystick.Horizontal, joystick.Vertical);
        moveVelocity = joystickInput.normalized * speed;

        // Keyboard Controls
        /*float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(horizontal, vertical);
        moveVelocity = movement.normalized * speed;

        // Shoot
        if (Input.GetKeyDown(KeyCode.Space))
        {
            weapon.OnButtonDown();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            weapon.OnButtonUp();
        }

        // Track Mouse Position
        var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);*/
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthbar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("Hurt");
        }
    }

    public void AddHealth(int health)
    {
        if (currentHealth + health <= maxHealth)
        {
            currentHealth += health;
            healthbar.SetHealth(currentHealth);
        }
        else
        {
            currentHealth = maxHealth;
            healthbar.SetHealth(maxHealth);
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        EnemySpawner.spawnAllowed = false;
        PowerupSpawner.spawnAllowed = false;
        FindObjectOfType<AudioManager>().Play("PlayerDeath");
        FindObjectOfType<EnemyController>().target = null;
        FindObjectOfType<Gameover>().GetComponent<Gameover>().GameOver();
    }

    private void OnApplicationQuit()
    {
        SaveSystem.SaveData();
    }

    private void AutoSave()
    {
        SaveSystem.SaveData();
    }

    private void PlayTutorial()
    {
        if (PlayerPrefs.GetInt(PPTutorial) == 0)
        {
            tutorial.SetActive(true);
            firstPart.SetActive(true);
            joystickTutorialBox.SetActive(true);
            shootButtonTutorialBox.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void FinishTutorial()
    {
        tutorial.SetActive(false);
        Time.timeScale = 1f;
        PlayerPrefs.SetInt(PPTutorial, 1);
    }

    public void ButtonSound()
    {
        FindObjectOfType<AudioManager>().PlayOnTop("ButtonClick");
    }
}
