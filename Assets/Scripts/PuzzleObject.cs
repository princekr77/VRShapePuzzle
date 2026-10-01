using Oculus.Interaction;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PuzzleObject : MonoBehaviour
{
    public ObjectType objectType;
    public Vector3 originalPosition;
    public Quaternion originalRotation;
    private bool _isGrabbed;
    private float _releaseTime;

    [HideInInspector] public bool isPlaced;
    [HideInInspector] public bool isLocked;

    private Rigidbody _rb;
    private Grabbable _grabbable; // Meta's component

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _grabbable = GetComponent<Grabbable>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
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

    public void OnGrab() 
    { 
        _isGrabbed = true; 
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGrab();
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
        _rb.isKinematic = true;
        if (_grabbable) _grabbable.enabled = false; // prevent re-grab
        socket.Occupy(this);
    }

    public void ReturnToOriginal()
    {
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
}