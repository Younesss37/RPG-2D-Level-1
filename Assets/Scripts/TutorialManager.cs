using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI enemyCountText;
    public GameObject tutorialPanel;
    
    [Header("Tutorial States")]
    private bool hasMoved = false;
    private bool hasAttacked = false;
    private bool hasShownDoorMessage = false;
    
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
        // Show initial movement tutorial
        ShowTutorial("Use → ↑ ↓ ← to move");
        
        // Subscribe to player movement
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.OnPlayerMoved += HandlePlayerMoved;
        }
        
        // Subscribe to player attack
        if (Player_Combat.Instance != null)
        {
            Player_Combat.Instance.OnPlayerAttacked += HandlePlayerAttacked;
        }
        
        // Subscribe to enemy count changes
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.OnEnemyCountChanged += UpdateEnemyCount;
        }
    }
    
    private void HandlePlayerMoved()
    {
        if (!hasMoved)
        {
            hasMoved = true;
            ShowTutorial("Press K to attack");
        }
    }
    
    private void HandlePlayerAttacked()
    {
        if (!hasAttacked)
        {
            hasAttacked = true;
            ShowTutorial("Defeat all goblins to proceed");
            ShowEnemyCount();
        }
    }
    
    public void ShowTutorial(string message)
    {
        if (tutorialText != null)
        {
            tutorialText.text = message;
            tutorialPanel.SetActive(true);
        }
    }
    
    public void HideTutorialPanel()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }
    
    private void ShowEnemyCount()
    {
        if (enemyCountText != null)
        {
            enemyCountText.gameObject.SetActive(true);
            UpdateEnemyCount();
        }
    }
    
    private void UpdateEnemyCount()
    {
        if (enemyCountText != null && EnemyManager.Instance != null)
        {
            int goblinCount = EnemyManager.Instance.GetGoblinCount();
            
            if (goblinCount > 0)
            {
                enemyCountText.text = $"Goblins Remaining: {goblinCount}";
                enemyCountText.gameObject.SetActive(true);
            }
            else
            {
                enemyCountText.gameObject.SetActive(false);
                if (!hasShownDoorMessage)
                {
                    hasShownDoorMessage = true;
                    ShowTutorial("Go to the Door and open it using the 'E' button");
                }
            }
        }
    }
    
    private void OnDestroy()
    {
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.OnPlayerMoved -= HandlePlayerMoved;
        }
        if (Player_Combat.Instance != null)
        {
            Player_Combat.Instance.OnPlayerAttacked -= HandlePlayerAttacked;
        }
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.OnEnemyCountChanged -= UpdateEnemyCount;
        }
    }
}