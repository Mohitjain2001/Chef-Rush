using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace YesChef.Managers
{
    public enum GameState
    {
        StartMenu,
        Playing,
        Paused,
        GameOver
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public const float GAME_DURATION = 180f; // 3 minutes

        [Header("Game State")]
        public GameState currentState = GameState.StartMenu;
        public float timeRemaining = GAME_DURATION;
        public int currentScore = 0;
        public int highScore = 0;
        public bool isNewHighScore = false;

        public event Action<GameState> OnGameStateChanged;
        public event Action<int> OnScoreChanged;
        public event Action<float> OnTimerUpdated;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            LoadHighScore();
        }

        private void Start()
        {
            SetGameState(GameState.StartMenu);
        }

        private void Update()
        {
            if (currentState == GameState.Playing)
            {
                timeRemaining -= Time.deltaTime;
                OnTimerUpdated?.Invoke(timeRemaining);

                if (timeRemaining <= 0f)
                {
                    timeRemaining = 0f;
                    EndGame();
                }

                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
                {
                    TogglePause();
                }
            }
            else if (currentState == GameState.Paused)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
                {
                    TogglePause();
                }
            }
        }

        public void StartGame()
        {
            currentScore = 0;
            timeRemaining = GAME_DURATION;
            isNewHighScore = false;
            OnScoreChanged?.Invoke(currentScore);
            SetGameState(GameState.Playing);

            if (OrderManager.Instance != null)
            {
                OrderManager.Instance.InitializeOrders();
            }
        }

        public void SetGameState(GameState newState)
        {
            currentState = newState;
            Time.timeScale = (currentState == GameState.Playing) ? 1f : 0f;
            OnGameStateChanged?.Invoke(currentState);
        }

        public void TogglePause()
        {
            if (currentState == GameState.Playing)
            {
                SetGameState(GameState.Paused);
            }
            else if (currentState == GameState.Paused)
            {
                SetGameState(GameState.Playing);
            }
        }

        public void AddScore(int amount)
        {
            currentScore += amount;
            OnScoreChanged?.Invoke(currentScore);
        }

        private void EndGame()
        {
            if (currentScore > highScore)
            {
                highScore = currentScore;
                isNewHighScore = true;
                SaveHighScore();
            }
            else
            {
                isNewHighScore = false;
            }

            SetGameState(GameState.GameOver);
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void QuitGame()
        {
            Time.timeScale = 1f;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void LoadHighScore()
        {
            highScore = PlayerPrefs.GetInt("YesChef_HighScore", 0);
        }

        private void SaveHighScore()
        {
            PlayerPrefs.SetInt("YesChef_HighScore", highScore);
            PlayerPrefs.Save();
        }

        public bool IsPlaying() => currentState == GameState.Playing;
    }
}
