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
    private bool _hasDoorOpened = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureDoorAnimator();
    }

    private void Start()
    {
        EnsureDoorAnimator();
    }

    private void EnsureDoorAnimator()
    {
        if (doorAnimator == null)
        {
            Animator[] animators = FindObjectsOfType<Animator>(true);
            foreach (var anim in animators)
            {
                string name = anim.gameObject.name.ToLower();
                if (name.Contains("door") || name.Contains("gate") || name.Contains("exit"))
                {
                    doorAnimator = anim;
                    break;
                }
            }
        }

        if (doorAnimator != null)
        {
            DoorExitTrigger trigger = doorAnimator.GetComponent<DoorExitTrigger>();
            if (trigger == null)
            {
                doorAnimator.gameObject.AddComponent<DoorExitTrigger>();
            }
        }
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
        if (_isPuzzleCompleted) return;
        _isPuzzleCompleted = true;
        _isTimerRunning = false;

        EnsureDoorAnimator();

        // 1. Open door animation
        OpenDoorOnce();

        // 2. Play completion audio immediately upon 3rd shape placement (before going to door)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCompletion();
        }

        // 3. Trigger completion haptic pulse immediately
        if (HapticManager.Instance != null)
        {
            HapticManager.Instance.PlayPlaceHaptic();
        }

        // 4. Print "Task Completed!" text immediately on Canvas panel before going to door
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowCompletion("Task Completed!");
        }
    }

    private void OpenDoorOnce()
    {
        if (_hasDoorOpened) return;
        _hasDoorOpened = true;

        Debug.Log("[PuzzleManager] All puzzle objects correctly placed! Opening door.");

        if (doorAnimator != null)
        {
            SetDoorOpenParameter(true);

            DoorScript.Door doorScript = doorAnimator.GetComponent<DoorScript.Door>();
            if (doorScript == null) doorScript = doorAnimator.GetComponentInChildren<DoorScript.Door>();
            if (doorScript == null) doorScript = doorAnimator.GetComponentInParent<DoorScript.Door>();
            if (doorScript != null && !doorScript.open)
            {
                doorScript.OpenDoor();
            }
        }
        else
        {
            DoorScript.Door[] doors = FindObjectsOfType<DoorScript.Door>();
            foreach (var door in doors)
            {
                if (!door.open)
                {
                    door.OpenDoor();
                }
            }
        }
    }

    public void OnPlayerExited()
    {
        _isPuzzleCompleted = true;
        _isTimerRunning = false;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCompletion();
        }

        if (HapticManager.Instance != null)
        {
            HapticManager.Instance.PlayPlaceHaptic();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowCompletion("Task Completed!");
        }

        if (VRScreenFader.Instance != null)
        {
            VRScreenFader.Instance.FadeToBlack(1.5f);
        }
    }

    private void SetDoorOpenParameter(bool isOpen)
    {
        if (doorAnimator == null) return;

        foreach (AnimatorControllerParameter param in doorAnimator.parameters)
        {
            if (param.name.Equals("Open", System.StringComparison.OrdinalIgnoreCase))
            {
                if (param.type == AnimatorControllerParameterType.Trigger)
                {
                    if (isOpen) doorAnimator.SetTrigger(param.name);
                    else doorAnimator.ResetTrigger(param.name);
                }
                else if (param.type == AnimatorControllerParameterType.Bool)
                {
                    doorAnimator.SetBool(param.name, isOpen);
                }
            }
        }
    }

    public void ResetPuzzle()
    {
        placedObjectsCount = 0;
        _timer = 0f;
        _isPuzzleCompleted = false;
        _isTimerRunning = true;
        _hasDoorOpened = false;

        if (doorAnimator != null)
        {
            SetDoorOpenParameter(false);
            DoorExitTrigger trigger = doorAnimator.GetComponent<DoorExitTrigger>();
            if (trigger != null)
            {
                trigger.ResetTriggerState();
            }

            DoorScript.Door doorScript = doorAnimator.GetComponent<DoorScript.Door>();
            if (doorScript == null) doorScript = doorAnimator.GetComponentInChildren<DoorScript.Door>();
            if (doorScript == null) doorScript = doorAnimator.GetComponentInParent<DoorScript.Door>();
            if (doorScript != null && doorScript.open)
            {
                doorScript.OpenDoor();
            }
        }
        else
        {
            DoorScript.Door[] doors = FindObjectsOfType<DoorScript.Door>();
            foreach (var door in doors)
            {
                if (door.open)
                {
                    door.OpenDoor();
                }
            }
        }

        if (VRScreenFader.Instance != null)
        {
            VRScreenFader.Instance.FadeFromBlack(1.0f);
        }

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
