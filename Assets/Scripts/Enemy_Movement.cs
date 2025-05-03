using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.XR;

[System.Serializable]
public class PatrolPoint
{
    public float x;
    public float y;
}

public class Enemy_Movement : MonoBehaviour
{
    public float speed;
    public float attackRange = 2;
    public float attackCooldown = 2;
    public float playerDetectRange = 3;
    public Transform detectionPoint;
    public LayerMask playerLayer;
    
    [Header("Patrol Settings")]
    public PatrolPoint[] patrolPoints; // Array of patrol points coordinates
    public float waitTimeAtPoint = 1f;
    public float pointReachedDistance = 0.1f;

    private float attackCooldownTimer;
    private int facingDirection = 1;
    private EnemyState enemyState;
    private int currentPatrolIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
        
        // Validate patrol points
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogWarning("No patrol points assigned to enemy! Please assign patrol points in the inspector.");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyState != EnemyState.Knockback)
        {
            CheckForPlayer();
            if(attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
            }

            if (enemyState == EnemyState.Chasing)
            {
                Chase();
            }
            else if (enemyState == EnemyState.Attacking)
            {
                rb.velocity = Vector2.zero;
            }
            else if (enemyState == EnemyState.Idle)
            {
                Patrol();
            }
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        PatrolPoint targetPoint = patrolPoints[currentPatrolIndex];
        Vector2 targetPosition = new Vector2(targetPoint.x, targetPoint.y);
        
        // Check if we've reached the current patrol point
        if (Vector2.Distance(transform.position, targetPosition) <= pointReachedDistance)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = waitTimeAtPoint;
                rb.velocity = Vector2.zero;
                // Set to Idle when waiting at point
                anim.SetBool("isChasing", false);
                anim.SetBool("isIdle", true);
            }
            
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                // Move to next patrol point
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            }
        }
        else
        {
            // Set to Move animation while patrolling
            anim.SetBool("isIdle", false);
            anim.SetBool("isChasing", true);
            
            // Move towards patrol point
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            rb.velocity = direction * speed;
            
            // Update facing direction
            if (direction.x != 0)
            {
                facingDirection = (int)Mathf.Sign(direction.x);
                transform.localScale = new Vector3(facingDirection, 1, 1);
            }
        }
    }

    void Chase()
    {

        if (player.position.x > transform.position.x && facingDirection == -1 ||
            player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * speed;
    }

    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);
        if(hits.Length > 0)
        {
            player = hits[0].transform;

            if (Vector2.Distance(transform.position, player.position) < attackRange
                && attackCooldownTimer <= 0)
            {
                attackCooldownTimer = attackCooldown;
                ChangeState(EnemyState.Attacking);
            }
            else if(Vector2.Distance(transform.position, player.position) > attackRange 
                && enemyState != EnemyState.Attacking)
            {
                ChangeState(EnemyState.Chasing);
            } 
        }
        else
        {
            rb.velocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
        }
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void ChangeState(EnemyState newState)
    {
        //Exit the current animation
        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", false);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", false);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", false);

        //Update our current state
        enemyState = newState;
        
        //Update the new animation
        if (enemyState == EnemyState.Idle)
        {
            anim.SetBool("isIdle", true);
            anim.SetBool("isChasing", false);
        }
        else if (enemyState == EnemyState.Chasing)
        {
            anim.SetBool("isChasing", true);
            anim.SetBool("isIdle", false);
        }
        else if (enemyState == EnemyState.Attacking)
        {
            anim.SetBool("isAttacking", true);
            anim.SetBool("isChasing", false);
            anim.SetBool("isIdle", false);
        }
    }
}


public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    Knockback
}