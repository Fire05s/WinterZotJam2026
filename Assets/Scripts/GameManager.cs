using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string _nextScene;
    [SerializeField] private string _playerLoseScene;

    [Header("Exit")]
    [SerializeField] private DoorController _exit;

    public static GameManager Instance;
    private int _enemiesAlive;

    private void Awake()
    {
        // Ensures there's only one Game Manager Instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _enemiesAlive = GameObject.FindGameObjectsWithTag("NPC").Length;
    }

    public void OnEnemyDeath()
    {
        _enemiesAlive--;

        if (_enemiesAlive <= 0)
        {
            _exit.OpenDoor();
        }
    }

    public void OpenExit()
    {
        _exit.OpenDoor();
    }

    public void PlayerWin()
    {
        Debug.Log("Player Wins");
        SceneManager.LoadScene(_nextScene);
    }

    public void PlayerLose()
    {
        Debug.Log("Player Loses");
        SceneManager.LoadScene(_playerLoseScene);
    }
}
