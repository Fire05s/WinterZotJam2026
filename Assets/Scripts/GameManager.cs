using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private SceneAsset _playerWinScene;
    [SerializeField] private SceneAsset _playerLoseScene;

    [Header("Exit")]
    [SerializeField] private DoorController _exit;

    public static GameManager Instance;

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

    public void OpenExit()
    {
        _exit.OpenDoor();
    }

    public void PlayerWin()
    {
        Debug.Log("Player Wins");
        //SceneManager.LoadScene(_playerWinScene.name);
    }

    public void PlayerLose()
    {
        Debug.Log("Player Loses");
        //SceneManager.LoadScene(_playerLoseScene.name);
    }
}
