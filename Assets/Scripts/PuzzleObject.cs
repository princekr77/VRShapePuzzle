using Oculus.Interaction;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PuzzleObject : MonoBehaviour
{
    public ObjectType objectType;
    public Vector3 originalPosition;
    public Quaternion originalRotation;

    [Header("Hover & Visual Highlight")]
    public Color highlightColor = new Color(0.2f, 0.8f, 1.0f, 1.0f); // Bright cyan highlight
    public GameObject highlightOutlineObject; // Optional outline child object

    [HideInInspector] public bool isPlaced;
    [HideInInspector] public bool isLocked;

    private bool _isGrabbed;
    private bool _isHovered;
    private float _releaseTime;

    private Rigidbody _rb;
    private Grabbable _grabbable; // Meta's component
    private Renderer _renderer;
    private Color _originalColor;
    private MaterialPropertyBlock _propBlock;
    private PointableElement _pointableElement;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _grabbable = GetComponent<Grabbable>();
        _renderer = GetComponent<Renderer>();

        if (_renderer != null && _renderer.material != null)
        {
            _originalColor = _renderer.material.color;
        }

        _propBlock = new MaterialPropertyBlock();
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Clean up conflicting Meta ISDK InteractableColorVisual to prevent coroutine errors on inactive objects
        InteractableColorVisual[] colorVisuals = GetComponentsInChildren<InteractableColorVisual>(true);
        foreach (var visual in colorVisuals)
        {
            Destroy(visual);
        }
    }

    void Start()
    {
        _pointableElement = GetComponent<PointableElement>();
        if (_pointableElement != null)
        {
            _pointableElement.WhenPointerEventRaised += HandlePointerEvent;
        }
    }

    private void OnDestroy()
    {
        if (_pointableElement != null)
        {
            _pointableElement.WhenPointerEventRaised -= HandlePointerEvent;
        }
    }

    void Update()
    {
        if (!_isGrabbed && !isPlaced && !isLocked)
        {
            // Only count release time if object has moved away from its original position
            if (Vector3.Distance(transform.position, originalPosition) > 0.1f)
            {
                _releaseTime += Time.deltaTime;
                if (_releaseTime > 2f) // 2 seconds grace period before auto-returning
                {
                    ReturnToOriginal();
                }
            }
            else
            {
                _releaseTime = 0f;
            }
        }
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        switch (evt.Type)
        {
            case PointerEventType.Hover:
                OnHoverEnter();
                break;
            case PointerEventType.Unhover:
                OnHoverExit();
                break;
            case PointerEventType.Select:
                OnGrab();
                break;
            case PointerEventType.Unselect:
                OnRelease();
                break;
        }
    }

    #region Hover & Highlight Events

    public void OnHoverEnter()
    {
        if (isLocked || _isHovered) return;
        _isHovered = true;

        // Visual highlight
        SetHighlight(true);

        // Audio feedback
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHover();
        }

        // Haptic feedback
        if (HapticManager.Instance != null)
        {
            HapticManager.Instance.PlayHoverHaptic();
        }
    }

    public void OnHoverExit()
    {
        if (!_isHovered) return;
        _isHovered = false;

        // Restore original visual state
        SetHighlight(false);
    }

    private void SetHighlight(bool enable)
    {
        if (highlightOutlineObject != null)
        {
            highlightOutlineObject.SetActive(enable);
        }

        if (_renderer != null)
        {
            Color targetColor = enable ? highlightColor : _originalColor;
            
            // Set MaterialPropertyBlock for URP (_BaseColor) and Built-in (_Color)
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor("_BaseColor", targetColor);
            _propBlock.SetColor("_Color", targetColor);
            _renderer.SetPropertyBlock(_propBlock);

            // Direct material property fallback if material instance is modified
            if (_renderer.material != null)
            {
                if (_renderer.material.HasProperty("_BaseColor"))
                    _renderer.material.SetColor("_BaseColor", targetColor);
                else if (_renderer.material.HasProperty("_Color"))
                    _renderer.material.color = targetColor;
            }
        }
    }

    private void OnMouseEnter() => OnHoverEnter();
    private void OnMouseExit() => OnHoverExit();

    #endregion

    #region Grab & Interaction Events

    public void OnGrab() 
    { 
        _isGrabbed = true; 
        CancelScheduledReturn();
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGrab();
        if (HapticManager.Instance != null) HapticManager.Instance.PlayGrabHaptic();
    }
    
    public void OnRelease() 
    { 
        _isGrabbed = false; 
        _releaseTime = 0f; 
    }

    public void LockInSocket(PuzzleSocket socket)
    {
        isPlaced = true;
        isLocked = true;
        _isGrabbed = false;
        _isHovered = false;
        CancelScheduledReturn();
        SetHighlight(false);

        _rb.isKinematic = true;
        if (_grabbable) _grabbable.enabled = false; // prevent re-grab
        socket.Occupy(this);
    }

    public void ScheduleReturnToOriginal(float delay = 2f)
    {
        CancelScheduledReturn();
        Invoke(nameof(ReturnToOriginal), delay);
    }

    public void CancelScheduledReturn()
    {
        CancelInvoke(nameof(ReturnToOriginal));
    }

    public void ReturnToOriginal()
    {
        _isGrabbed = false;
        _isHovered = false;
        CancelScheduledReturn();
        SetHighlight(false);

        transform.position = originalPosition;
        transform.rotation = originalRotation;
#if UNITY_6000_0_OR_NEWER
        _rb.linearVelocity = Vector3.zero;
#else
        _rb.velocity = Vector3.zero;
#endif
        _rb.angularVelocity = Vector3.zero;
        _releaseTime = 0f;
    }

    #endregion
}