using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Portal Settings")]
    public string targetSceneName; // Name of the scene to load
    public Vector2 spawnPosition; // Position where player will spawn in the new scene
    public bool requiresKey = false; // Whether the portal requires a key to activate
    public string keyName = "PortalKey"; // Name of the key item required
    
    [Header("Visual Effects")]
    public GameObject portalEffect; // Optional particle effect for the portal
    public float fadeTime = 1f; // Time it takes to fade in/out
    
    private bool isPlayerInRange = false;
    private bool isTransitioning = false;

    private void Start()
    {
        // Ensure the portal effect is visible if assigned
        if (portalEffect != null)
            portalEffect.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isTransitioning)
        {
            TryActivatePortal();
        }
    }

    private void TryActivatePortal()
    {
        if (requiresKey)
        {
            // Check if player has the required key
            // You'll need to implement your own inventory system check here
            // For now, we'll just log a message
            Debug.Log($"Portal requires {keyName} to activate!");
            return;
        }

        ActivatePortal();
    }

    private void ActivatePortal()
    {
        isTransitioning = true;
        
        // Save the spawn position for the new scene
        PlayerPrefs.SetFloat("SpawnX", spawnPosition.x);
        PlayerPrefs.SetFloat("SpawnY", spawnPosition.y);
        
        // Load the new scene
        SceneManager.LoadScene(targetSceneName);
    }

    // Optional: Visual feedback when player is in range
    private void OnGUI()
    {
        if (isPlayerInRange)
        {
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height - 100, 200, 20), 
                     "Press E to enter portal");
        }
    }
} 