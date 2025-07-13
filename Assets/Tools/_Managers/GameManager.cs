using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsNewGame { get; private set; }

    public UI_Manager uiManager;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartNewGame()
    {
        IsNewGame = true;
    }

    public void LoadGame()
    {
        IsNewGame = false;
    }

    
}
