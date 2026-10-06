using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Attach to an empty GameObject named "GameManager".
// Keeps track of coins and lives, shows messages, and ends the game.
public class GameManager : MonoBehaviour
{
    // Lets any other script reach this one with GameManager.Instance.
    public static GameManager Instance;

    public int lives = 3;
    public float quitDelay = 3f;      // seconds the final message stays up before exiting.

    [HideInInspector] public bool gameEnded = false;

    int totalCoins = 0;               // counted automatically as coins register themselves.
    int coinsCollected = 0;
    string endMessage = "";
    PlayerController player;

    void Awake()
    {
        // Awake runs before every Start, so Instance is ready when coins register.
        Instance = this;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        // Escape always quits (the mouse cursor is locked during play).
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            QuitGame();
        }
    }

    // Called by every Coin in its Start().
    public void RegisterCoin()
    {
        totalCoins++;
    }

    // Called by a Coin when the player touches it.
    public void CollectCoin()
    {
        if (gameEnded) return;

        coinsCollected++;
        Debug.Log("Coins collected: " + coinsCollected + " / " + totalCoins);

        if (coinsCollected >= totalCoins)
        {
            EndGame("You Win!");
        }
    }

    // Called by an Obstacle on touch, or by the player when falling off the level.
    public void LoseLife()
    {
        if (gameEnded) return;

        lives--;
        Debug.Log("Lives left: " + lives);

        if (lives <= 0)
        {
            EndGame("Game Over!");
        }
        else
        {
            player.Respawn();
        }
    }

    void EndGame(string message)
    {
        gameEnded = true;
        endMessage = message;
        Debug.Log(message);
        Invoke("RestartGame", quitDelay);   // wait quitDelay seconds, then call RestartGame
    }

    void RestartGame()
    {
        // Reloads the current scene, which recreates every object in its starting state.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // stops Play mode in the Editor.
#else
        Application.Quit();                                // closes the standalone build
#endif
    }

    // Simple on-screen text so the messages are visible in the standalone build.
    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 28;
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(20, 20, 400, 40), "Coins: " + coinsCollected + " / " + totalCoins, style);
        GUI.Label(new Rect(20, 60, 400, 40), "Lives: " + lives, style);

        if (gameEnded)
        {
            style.fontSize = 64;
            style.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), endMessage, style);
        }
    }
}
