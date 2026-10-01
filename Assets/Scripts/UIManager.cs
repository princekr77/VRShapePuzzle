using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Text References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI objectsRemainingText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI errorText;

    private float _elapsedTime = 0f;
    private bool _isTimerRunning = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Ensure timer and objects remaining text game objects are active and initialized
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            UpdateTimer(0f);
        }

        if (objectsRemainingText != null)
        {
            objectsRemainingText.gameObject.SetActive(true);
            int initialCount = (PuzzleManager.Instance != null) ? (PuzzleManager.Instance.totalObjects - PuzzleManager.Instance.placedObjects) : 3;
            UpdateObjectsRemaining(initialCount);
        }

        // Keep status and error texts hidden until triggered
        if (statusText != null && string.IsNullOrEmpty(statusText.text))
        {
            statusText.gameObject.SetActive(false);
        }
        if (errorText != null && string.IsNullOrEmpty(errorText.text))
        {
            errorText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (_isTimerRunning)
        {
            _elapsedTime += Time.deltaTime;
            UpdateTimer(_elapsedTime);
        }
    }

    public void StartTimer()
    {
        _isTimerRunning = true;
    }

    public void StopTimer()
    {
        _isTimerRunning = false;
    }

    public void ResetTimer()
    {
        _elapsedTime = 0f;
        _isTimerRunning = true;
        UpdateTimer(_elapsedTime);
    }

    public void UpdateTimer(float timeInSeconds)
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60F);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }

    public void UpdateObjectsRemaining(int count)
    {
        if (objectsRemainingText != null)
        {
            objectsRemainingText.text = $"Objects Remaining: {count}";
        }
    }

    public void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.gameObject.SetActive(true);
            CancelInvoke(nameof(ClearError));
            Invoke(nameof(ClearError), 3f);
        }
    }

    public void ClearError()
    {
        if (errorText != null)
        {
            errorText.text = string.Empty;
            errorText.gameObject.SetActive(false);
        }
    }

    public void ShowCompletion(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.gameObject.SetActive(true);
        }
    }
}
