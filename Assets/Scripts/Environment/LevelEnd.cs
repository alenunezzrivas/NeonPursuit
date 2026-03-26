using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    public GameObject completeUI;
    public float delay = 2f;
    public string nextSceneName;

    private bool levelCompleted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (levelCompleted) return;

        if (other.CompareTag("Player"))
        {
            levelCompleted = true;
            CompleteLevel();
        }
    }

    void CompleteLevel()
    {
        if (completeUI != null)
        {
            completeUI.SetActive(true);
        }

        Invoke("LoadNextLevel", delay);
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}