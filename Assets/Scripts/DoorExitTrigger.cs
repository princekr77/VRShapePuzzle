using UnityEngine;

public class DoorExitTrigger : MonoBehaviour
{
    [Header("References")]
    public PuzzleManager puzzleManager;
    public GameObject completionUI;

    [Header("Optional")]
    public AudioSource exitAudio;
    public bool showCompletionOnExit = true;

    private bool _hasTriggered;

    private void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
        }
        col.isTrigger = true;

        if (puzzleManager == null)
        {
            puzzleManager = PuzzleManager.Instance;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered) return;

        string otherName = other.gameObject.name.ToLower();
        if (other.CompareTag("Player") || otherName.Contains("camera") || otherName.Contains("player") || otherName.Contains("head") || other.GetComponent<Camera>() != null || other.GetComponent<OVRPlayerController>() != null)
        {
            TriggerExitSequence();
        }
    }

    private void OnMouseDown()
    {
        if (!_hasTriggered)
        {
            TriggerExitSequence();
        }
    }

    public void TriggerExitSequence()
    {
        _hasTriggered = true;

        if (showCompletionOnExit && completionUI != null)
        {
            completionUI.SetActive(true);
        }

        if (exitAudio != null)
        {
            exitAudio.Play();
        }

        if (puzzleManager == null)
        {
            puzzleManager = PuzzleManager.Instance;
        }

        if (puzzleManager != null)
        {
            puzzleManager.OnPlayerExited();
        }
    }

    public void ResetTriggerState()
    {
        _hasTriggered = false;
        if (VRScreenFader.Instance != null)
        {
            VRScreenFader.Instance.FadeFromBlack(1.0f);
        }
    }
}
