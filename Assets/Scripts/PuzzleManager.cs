using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    private static PuzzleManager _instance;
    public static PuzzleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PuzzleManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("[PuzzleManager]");
                    _instance = go.AddComponent<PuzzleManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    [Header("Puzzle Settings")]
    public int totalObjects = 3;
    public int placedObjectsCount = 0;

    [Header("Environment References")]
    public Animator doorAnimator;

    private float _timer = 0f;
    private bool _isTimerRunning = true;
    private bool _isPuzzleCompleted = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (_isTimerRunning && !_isPuzzleCompleted)
        {
            _timer += Time.deltaTime;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateTimer(_timer);
            }
        }
    }

    public void OnObjectPlaced()
    {
        placedObjectsCount++;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateObjectsRemaining(totalObjects - placedObjectsCount);
        }

        if (placedObjectsCount >= totalObjects && !_isPuzzleCompleted)
        {
            CompletePuzzle();
        }
    }

    private void CompletePuzzle()
    {
        _isPuzzleCompleted = true;
        _isTimerRunning = false;

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCompletion();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowCompletion("Task Completed!");
        }
    }

    public void ResetPuzzle()
    {
        placedObjectsCount = 0;
        _timer = 0f;
        _isPuzzleCompleted = false;
        _isTimerRunning = true;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateObjectsRemaining(totalObjects);
            UIManager.Instance.UpdateTimer(0f);
            UIManager.Instance.ClearError();
        }

        PuzzleObject[] objects = FindObjectsOfType<PuzzleObject>();
        foreach (var obj in objects)
        {
            obj.isPlaced = false;
            obj.isLocked = false;
            obj.ReturnToOriginal();
            var rb = obj.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
            var grabbable = obj.GetComponent<Oculus.Interaction.Grabbable>();
            if (grabbable != null) grabbable.enabled = true;
        }

        PuzzleSocket[] sockets = FindObjectsOfType<PuzzleSocket>();
        foreach (var socket in sockets)
        {
            socket.ResetSocket();
        }
    }

    public void RestartTask()
    {
        ResetPuzzle();
    }
}