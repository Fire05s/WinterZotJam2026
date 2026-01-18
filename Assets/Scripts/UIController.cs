using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pauseMenu;

    private bool isPaused;

    private void Start()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ToggleUI()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Debug.Log("pressed resume");
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }
}
