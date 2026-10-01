using Oculus.Interaction;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WristMenuController : MonoBehaviour
{
    [Header("Wrist Offset Settings")]
    public Vector3 localPositionOffset = new Vector3(0.05f, 0.02f, 0.08f);
    
    [Tooltip("Wrist Menu rotation angle relative to left wrist (Euler angles X, Y, Z)")]
    public Vector3 localRotationOffset = new Vector3(16f, -72f, -42f);
    public Vector3 localScale = new Vector3(0.001f, 0.001f, 0.001f);

    [Header("Button References (Optional - Auto-assigned if empty)")]
    public Button resetButton;
    public Button restartButton;
    public Button toggleHandsButton;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoAttachToMenuCanvas()
    {
        GameObject canvasGo = GameObject.Find("MenuCanvas");
        if (canvasGo != null && canvasGo.GetComponent<WristMenuController>() == null)
        {
            canvasGo.AddComponent<WristMenuController>();
        }
    }

    private void Awake()
    {
        EnsurePointableCanvasModule();
        EnsurePointableCanvas();
        EnsureWristParentingAndTransform();
        AutoAssignButtons();
    }

    private void Start()
    {
        EnsurePointableCanvasModule();
        EnsurePointableCanvas();
        EnsureWristParentingAndTransform();
        AutoAssignButtons();
        EnsureButtonRaycastTargets();
        EnsureButtonColliders();
    }

    private void Update()
    {
        if (transform.parent == null || !transform.parent.name.ToLower().Contains("left"))
        {
            EnsureWristParentingAndTransform();
        }
    }

    private void EnsurePointableCanvasModule()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject esGo = new GameObject("EventSystem");
            eventSystem = esGo.AddComponent<EventSystem>();
        }

        PointableCanvasModule module = eventSystem.GetComponent<PointableCanvasModule>();
        if (module == null)
        {
            module = eventSystem.gameObject.AddComponent<PointableCanvasModule>();
        }
    }

    private void EnsurePointableCanvas()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            PointableCanvas pointableCanvas = GetComponent<PointableCanvas>();
            if (pointableCanvas == null)
            {
                pointableCanvas = gameObject.AddComponent<PointableCanvas>();
            }
            pointableCanvas.InjectCanvas(canvas);
        }
    }

    private void EnsureButtonRaycastTargets()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (var btn in buttons)
        {
            Graphic[] graphics = btn.GetComponentsInChildren<Graphic>(true);
            foreach (var g in graphics)
            {
                g.raycastTarget = true;
            }
        }
    }

    private void EnsureButtonColliders()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (var btn in buttons)
        {
            BoxCollider col = btn.GetComponent<BoxCollider>();
            if (col == null)
            {
                col = btn.gameObject.AddComponent<BoxCollider>();
            }
            RectTransform rt = btn.GetComponent<RectTransform>();
            if (rt != null)
            {
                col.size = new Vector3(rt.rect.width, rt.rect.height, 20f);
            }
        }
    }

    public void EnsureWristParentingAndTransform()
    {
        Transform leftWrist = FindLeftWristAnchor();
        if (leftWrist != null && transform.parent != leftWrist)
        {
            transform.SetParent(leftWrist, false);
        }

        transform.localPosition = localPositionOffset;
        transform.localRotation = Quaternion.Euler(localRotationOffset);
        transform.localScale = localScale;
    }

    private Transform FindLeftWristAnchor()
    {
        string[] candidates = new string[] {
            "LeftControllerAnchor", "LeftHandAnchor", "b_l_wrist", 
            "XRHand_Wrist", "LeftWrist", "LeftHand", "LeftController"
        };

        foreach (var candidate in candidates)
        {
            GameObject go = GameObject.Find(candidate);
            if (go != null) return go.transform;
        }

        Transform[] allTransforms = FindObjectsOfType<Transform>(true);
        foreach (var t in allTransforms)
        {
            if (t.name.ToLower().Contains("left") && (t.name.ToLower().Contains("wrist") || t.name.ToLower().Contains("anchor")))
            {
                return t;
            }
        }

        return null;
    }

    private void AutoAssignButtons()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (var btn in buttons)
        {
            string btnName = btn.gameObject.name.ToLower();
            if (btnName.Contains("reset") && resetButton == null)
            {
                resetButton = btn;
            }
            else if (btnName.Contains("restart") && restartButton == null)
            {
                restartButton = btn;
            }
            else if ((btnName.Contains("toggle") || btnName.Contains("hand")) && toggleHandsButton == null)
            {
                toggleHandsButton = btn;
            }
        }

        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(OnResetClicked);
            resetButton.onClick.AddListener(OnResetClicked);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(OnRestartClicked);
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        if (toggleHandsButton != null)
        {
            toggleHandsButton.onClick.RemoveListener(OnToggleHandsClicked);
            toggleHandsButton.onClick.AddListener(OnToggleHandsClicked);
        }
    }

    public void OnResetClicked()
    {
        if (PuzzleManager.Instance != null)
        {
            PuzzleManager.Instance.ResetPuzzle();
        }
    }

    public void OnRestartClicked()
    {
        if (PuzzleManager.Instance != null)
        {
            PuzzleManager.Instance.RestartTask();
        }
    }

    public void OnToggleHandsClicked()
    {
        HandVisualToggle toggle = FindObjectOfType<HandVisualToggle>();
        if (toggle == null)
        {
            GameObject toggleGo = new GameObject("[HandVisualToggle]");
            toggle = toggleGo.AddComponent<HandVisualToggle>();
        }

        toggle.ToggleHandVisuals();
    }
}
