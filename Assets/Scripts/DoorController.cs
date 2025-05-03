using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public int requiredEnemyKills = 3;
    public float doorCloseDelay = 10f;
    public bool startLocked = true;

    [Header("References")]
    public Sprite openDoorSprite;    // Assign the door_10 sprite in inspector
    public BoxCollider2D doorCollider;
    
    private SpriteRenderer spriteRenderer;
    private Sprite closedDoorSprite; // Store the original door sprite
    private bool isPlayerInRange = false;
    private bool isDoorOpen = false;
    private bool canInteract = false;

    private void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            closedDoorSprite = spriteRenderer.sprite; // Store the initial closed door sprite
        }

        // Subscribe to enemy count changes
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.OnEnemyCountChanged += CheckEnemyCount;
        }

        // Set initial door state
        if (startLocked)
        {
            doorCollider.enabled = true;
            canInteract = false;
        }

        Debug.Log("Door Controller Started");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player entered door range");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player exited door range");
        }
    }

    private void Update()
    {
        // Check for player interaction
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"E pressed. Player in range: {isPlayerInRange}, Can interact: {canInteract}, Door open: {isDoorOpen}");
            
            if (isPlayerInRange && canInteract && !isDoorOpen)
            {
                OpenDoor();
            }
        }
    }

    private void CheckEnemyCount()
    {
        if (EnemyManager.Instance != null)
        {
            Debug.Log($"Checking enemy count: {EnemyManager.Instance.GetGoblinCount()}");
            if (EnemyManager.Instance.GetGoblinCount() == 0)
            {
                canInteract = true;
                Debug.Log("Door can now be opened!");
            }
        }
    }

    private void OpenDoor()
    {
        isDoorOpen = true;
        doorCollider.enabled = false; // Disable the collider to let player pass through

        // Change the sprite to open door
        if (spriteRenderer != null && openDoorSprite != null)
        {
            spriteRenderer.sprite = openDoorSprite;
            Debug.Log("Changed to open door sprite");
        }
        else
        {
            Debug.LogWarning("SpriteRenderer or openDoorSprite is missing!");
        }

        // Reset player health to 100
        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.ResetHealth();
        }

        // Hide tutorial panel
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.HideTutorialPanel();
        }

        // Start the close door timer
        StartCoroutine(CloseDoorAfterDelay());
    }

    private IEnumerator CloseDoorAfterDelay()
    {
        yield return new WaitForSeconds(doorCloseDelay);
        
        isDoorOpen = false;
        doorCollider.enabled = true; // Re-enable the collider
        
        // Change back to closed door sprite
        if (spriteRenderer != null && closedDoorSprite != null)
        {
            spriteRenderer.sprite = closedDoorSprite;
        }

        Debug.Log("Door closed");
    }

    private void OnDestroy()
    {
        // Unsubscribe from enemy count changes
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.OnEnemyCountChanged -= CheckEnemyCount;
        }
    }
}
