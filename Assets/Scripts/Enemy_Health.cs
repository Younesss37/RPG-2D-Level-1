using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{

    public int currentHealth;
    public int maxHealth;
    // Start is called before the first frame update
    private void Start()
    {
        currentHealth = maxHealth;
        // Register this enemy with the EnemyManager
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.RegisterEnemy(gameObject);
        }
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            // Notify EnemyManager before destroying
            if (EnemyManager.Instance != null)
            {
                EnemyManager.Instance.EnemyDied(gameObject);
            }
            Destroy(gameObject);
        }
    }
}
