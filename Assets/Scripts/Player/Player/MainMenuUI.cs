using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject menuPanel;
    public NetworkManagerFusion networkManager;

    public void PlayGame()
    {
        menuPanel.SetActive(false);
        networkManager.StartGame();
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Salir del juego");
    }
}