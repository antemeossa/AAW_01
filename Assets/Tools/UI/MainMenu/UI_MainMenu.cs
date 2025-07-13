using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    public void OnNewGameClicked()
    {
        GameManager.Instance.StartNewGame();
        SceneManager.LoadScene("GameScene");
    }

    public void OnLoadGameClicked()
    {
        GameManager.Instance.LoadGame();
        SceneManager.LoadScene("GameScene");
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }
}
