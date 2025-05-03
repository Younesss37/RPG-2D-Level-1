using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }
    public int currentHealth;
    public int maxHealth;
    public TMP_Text healthText;
    public Animator healthTextAnim;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        healthText.text = "HP: " + currentHealth + "/" + maxHealth;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        healthText.text = "HP: " + currentHealth + "/" + maxHealth;
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        healthTextAnim.Play("textUpdate");
        healthText.text = "HP: " + currentHealth + "/" + maxHealth;

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
            // Show game over dialog
            if (GameOverManager.Instance != null)
            {
                GameOverManager.Instance.ShowGameOver();
            }
        }
    }
}
