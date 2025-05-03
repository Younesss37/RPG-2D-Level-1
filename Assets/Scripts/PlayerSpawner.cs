using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private void Start()
    {
        // Check if we have saved spawn coordinates
        if (PlayerPrefs.HasKey("SpawnX") && PlayerPrefs.HasKey("SpawnY"))
        {
            float spawnX = PlayerPrefs.GetFloat("SpawnX");
            float spawnY = PlayerPrefs.GetFloat("SpawnY");
            
            // Move the player to the spawn position
            transform.position = new Vector3(spawnX, spawnY, transform.position.z);
            
            // Clear the saved coordinates
            PlayerPrefs.DeleteKey("SpawnX");
            PlayerPrefs.DeleteKey("SpawnY");
        }
    }
} 