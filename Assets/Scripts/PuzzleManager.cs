using Oculus.Interaction;
using TMPro;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public int totalObjects = 3;
    public int placedObjects = 0;

    public GameObject door;
    public GameObject completionUI;

    void Awake() => Instance = this;

    private void Start()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateObjectsRemaining(totalObjects - placedObjects);
        }
    }

    public void OnObjectPlaced()
    {
        placedObjects++;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateObjectsRemaining(totalObjects - placedObjects);
        }
        if (placedObjects >= totalObjects)
        {
            CompletePuzzle();
        }
    }

    void CompletePuzzle()
    {
        if (door != null && door.TryGetComponent<Animator>(out var doorAnimator))
        {
            doorAnimator.SetTrigger("Open");
        }
        if (completionUI != null)
        {
            completionUI.SetActive(true);
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCompletion();
        }
        if (UIManager.Instance != null)
        {
            UIManager.Instance.StopTimer();
            UIManager.Instance.ShowCompletion("Puzzle Completed!");
        }
    }

    public void ResetPuzzle()
    {
        placedObjects = 0;
        // Find all PuzzleObjects, call ReturnToOriginal, re-enable grabbable
        foreach (var obj in FindObjectsOfType<PuzzleObject>())
        {
            obj.isPlaced = false;
            obj.isLocked = false;
            if (obj.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = false;
            }
            if (obj.TryGetComponent<Grabbable>(out var grabbable))
            {
                grabbable.enabled = true;
            }
            obj.ReturnToOriginal();
        }
        // Reset sockets
        foreach (var socket in FindObjectsOfType<PuzzleSocket>())
        {
            socket.ResetSocket();
        }
        if (door != null && door.TryGetComponent<Animator>(out var doorAnimator))
        {
            doorAnimator.SetTrigger("Close");
        }
        if (completionUI != null)
        {
            completionUI.SetActive(false);
        }
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ResetTimer();
            UIManager.Instance.UpdateObjectsRemaining(totalObjects - placedObjects);
        }
    }
}