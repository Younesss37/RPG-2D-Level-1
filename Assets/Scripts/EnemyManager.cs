using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    
    [Header("Enemy Settings")]
    public List<GameObject> goblinEnemies = new List<GameObject>();
    public List<GameObject> romanEnemies = new List<GameObject>();
    
    [Header("Portal Settings")]
    public GameObject portal;
    
    // Add event for enemy count changes
    public event System.Action OnEnemyCountChanged;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Clear the enemies lists when a new scene is loaded
        goblinEnemies.Clear();
        romanEnemies.Clear();
        // Trigger the event to update the UI
        OnEnemyCountChanged?.Invoke();
    }

    private void Start()
    {
        // Disable portal at start
        if (portal != null)
        {
            portal.SetActive(false);
        }
    }

    public void RegisterEnemy(GameObject enemy)
    {
        if (!goblinEnemies.Contains(enemy) && !romanEnemies.Contains(enemy))
        {
            if (enemy.CompareTag("Goblin"))
            {
                goblinEnemies.Add(enemy);
            }
            else if (enemy.CompareTag("Roman"))
            {
                romanEnemies.Add(enemy);
            }
            OnEnemyCountChanged?.Invoke();
        }
    }

    public void EnemyDied(GameObject enemy)
    {
        if (goblinEnemies.Contains(enemy))
        {
            goblinEnemies.Remove(enemy);
            OnEnemyCountChanged?.Invoke();
        }
        else if (romanEnemies.Contains(enemy))
        {
            romanEnemies.Remove(enemy);
            OnEnemyCountChanged?.Invoke();
            
            // Check if all Roman enemies are defeated
            if (romanEnemies.Count == 0 && portal != null)
            {
                portal.SetActive(true);
                Debug.Log("All Roman enemies defeated! Portal is now accessible.");
                
                // Show tutorial panel with new message
                if (TutorialManager.Instance != null)
                {
                    TutorialManager.Instance.ShowTutorial("Go to the Temple and enter using the 'E' button");
                }
            }
        }
    }

    public int GetGoblinCount()
    {
        return goblinEnemies.Count;
    }

    public int GetRomanCount()
    {
        return romanEnemies.Count;
    }
} 