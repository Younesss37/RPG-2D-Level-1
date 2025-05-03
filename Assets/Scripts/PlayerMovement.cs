using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }
    
    // Add event for player movement
    public event System.Action OnPlayerMoved;
    
    public int facingDirection = 1;
    public float speed = 5;
    public Rigidbody2D rb;
    public Animator anim;

    private bool isKnockedBack;

    public Player_Combat player_Combat;

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
        if (Input.GetButtonDown("Slash"))
        {
            player_Combat.Attack();
        }

        // Get input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Calculate movement
        Vector2 movement = new Vector2(moveX, moveY).normalized * speed;
        rb.velocity = movement;

        // Trigger movement event if player is actually moving
        if (movement != Vector2.zero)
        {
            OnPlayerMoved?.Invoke();
        }

        if (moveX > 0 && transform.localScale.x < 0 ||
            moveX < 0 && transform.localScale.x > 0)
        {
            Flip();
        }

        anim.SetFloat("horizontal", Mathf.Abs(moveX));
        anim.SetFloat("vertical", Mathf.Abs(moveY));
    }

    private void FixedUpdate()
    {
        if(isKnockedBack == false) { 
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (horizontal > 0 && transform.localScale.x < 0 ||
                horizontal < 0 && transform.localScale.x > 0)
            {
                Flip();
            }

            anim.SetFloat("horizontal", Mathf.Abs(horizontal));
            anim.SetFloat("vertical", Mathf.Abs(vertical));

            rb.velocity = new Vector2(horizontal, vertical) * speed;
        }
    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockedBack = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.velocity = direction * force;
        StartCoroutine(KnockbackCounter(stunTime));
    }

    private IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(1);
        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }
}
