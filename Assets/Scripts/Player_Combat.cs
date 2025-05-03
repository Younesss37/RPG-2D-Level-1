using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public static Player_Combat Instance { get; private set; }
    
    // Add event for player attack
    public event System.Action OnPlayerAttacked;
    
    public Transform attackPoint;
    public float weaponRange = 1;
    public float knockbackForce = 50;
    public float knockbackTime = .15f;
    public float stunTime = .3f;
    public LayerMask enemyLayer;
    public int damage = 1;

    public Animator anim;
    public float coooldown = 2;
    private float timer;

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

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public void Attack()
    {
        // Trigger attack event
        OnPlayerAttacked?.Invoke();
        
        if (timer <= 0)
        {
            anim.SetBool("isAttacking", true);
            
            timer = coooldown;
        }
    }

    public void DealDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayer);

        if (enemies.Length > 0)
        {
            enemies[0].GetComponent<Enemy_Health>().ChangeHealth(-damage);
            enemies[0].GetComponent<Enemy_Knockback>().Knockback(transform, knockbackForce, knockbackTime, stunTime);
        }
    }

    public void FinishAttacking()
    {
        anim.SetBool("isAttacking", false);
    }
}
