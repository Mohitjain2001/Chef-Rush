using UnityEngine;
using UnityEngine.UI;
using YesChef.Managers;

namespace YesChef.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Start Window UI")]
        public GameObject startWindowPanel;
        public Button startButton;

        [Header("HUD UI")]
        public GameObject hudPanel;
        public Text timerText;
        public Text scoreText;
        public Text highScoreText;
        public GameObject promptContainerPanel;
        public Text promptText;
        public Button pauseButton;

        [Header("Pause UI")]
        public GameObject pausePanel;
        public Button resumeButton;
        public Button pauseRestartButton;
        public Button pauseQuitButton;

        [Header("Game Over UI")]
        public GameObject gameOverPanel;
        public Text finalScoreText;
        public Text gameOverHighScoreText;
        public GameObject newHighScoreBadge;
        public Button playAgainButton;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitPanels();
        }

        private void InitPanels()
        {
            if (startWindowPanel != null) startWindowPanel.SetActive(true);
            if (hudPanel != null) hudPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            HideInteractionPrompt();
        }

        private void Start()
        {
            // Bind buttons
            if (startButton != null) startButton.onClick.AddListener(OnStartClicked);
            if (pauseButton != null) pauseButton.onClick.AddListener(OnPauseClicked);
            if (resumeButton != null) resumeButton.onClick.AddListener(OnResumeClicked);
            if (pauseRestartButton != null) pauseRestartButton.onClick.AddListener(OnRestartClicked);
            if (pauseQuitButton != null) pauseQuitButton.onClick.AddListener(OnQuitClicked);
            if (playAgainButton != null) playAgainButton.onClick.AddListener(OnRestartClicked);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
                GameManager.Instance.OnScoreChanged += HandleScoreChanged;
                GameManager.Instance.OnTimerUpdated += HandleTimerUpdated;

                HandleGameStateChanged(GameManager.Instance.currentState);
                HandleScoreChanged(GameManager.Instance.currentScore);
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
                GameManager.Instance.OnScoreChanged -= HandleScoreChanged;
                GameManager.Instance.OnTimerUpdated -= HandleTimerUpdated;
            }
        }

        private void HandleGameStateChanged(GameState state)
        {
            if (startWindowPanel != null) startWindowPanel.SetActive(state == GameState.StartMenu);
            if (hudPanel != null) hudPanel.SetActive(state == GameState.Playing || state == GameState.Paused);
            if (pausePanel != null) pausePanel.SetActive(state == GameState.Paused);
            if (gameOverPanel != null) gameOverPanel.SetActive(state == GameState.GameOver);

            if (state == GameState.GameOver && GameManager.Instance != null)
            {
                if (finalScoreText != null) finalScoreText.text = $"Final Score: {GameManager.Instance.currentScore}";
                if (gameOverHighScoreText != null) gameOverHighScoreText.text = $"High Score: {GameManager.Instance.highScore}";
                if (newHighScoreBadge != null) newHighScoreBadge.SetActive(GameManager.Instance.isNewHighScore);
            }
        }

        private void HandleScoreChanged(int score)
        {
            if (scoreText != null) scoreText.text = $"SCORE: {score}";
            if (highScoreText != null && GameManager.Instance != null)
            {
                highScoreText.text = $"HIGH SCORE: {GameManager.Instance.highScore}";
            }
        }

        private void HandleTimerUpdated(float timeRemaining)
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60f);
                int seconds = Mathf.FloorToInt(timeRemaining % 60f);
                timerText.text = $"TIME: {minutes:00}:{seconds:00}";
                timerText.color = timeRemaining <= 30f ? new Color(1f, 0.3f, 0.3f) : new Color(0.3f, 0.95f, 1f);
            }
        }

        public void ShowInteractionPrompt(string prompt)
        {
            if (promptText != null)
            {
                promptText.text = prompt;
            }
            if (promptContainerPanel != null)
            {
                promptContainerPanel.SetActive(true);
            }
            else if (promptText != null)
            {
                promptText.gameObject.SetActive(true);
            }
        }

        public void HideInteractionPrompt()
        {
            if (promptContainerPanel != null)
            {
                promptContainerPanel.SetActive(false);
            }
            else if (promptText != null)
            {
                promptText.gameObject.SetActive(false);
            }
        }

        private void OnStartClicked() => GameManager.Instance?.StartGame();
        private void OnPauseClicked() => GameManager.Instance?.TogglePause();
        private void OnResumeClicked() => GameManager.Instance?.TogglePause();
        private void OnRestartClicked() => GameManager.Instance?.RestartGame();
        private void OnQuitClicked() => GameManager.Instance?.QuitGame();
    }
}
