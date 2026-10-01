using UnityEngine;

[RequireComponent(typeof(PuzzleSocket))]
public class SocketTrigger : MonoBehaviour
{
    private PuzzleSocket _socket;

    void Awake()
    {
        _socket = GetComponent<PuzzleSocket>();
        var col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
        }
        col.isTrigger = true;

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var puzzleObj = other.GetComponent<PuzzleObject>();
        if (puzzleObj == null || puzzleObj.isLocked) return;

        if (_socket.CanAccept(puzzleObj))
        {
            // Correct
            Debug.Log($"[Puzzle] CORRECT placement: Object '{puzzleObj.gameObject.name}' ({puzzleObj.objectType}) placed into socket '{gameObject.name}'.");
            puzzleObj.LockInSocket(_socket);
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySuccess();
            if (HapticManager.Instance != null) HapticManager.Instance.PlayPlaceHaptic();
            if (_socket.greenVisual != null) _socket.greenVisual.SetActive(true);
            if (PuzzleManager.Instance != null) PuzzleManager.Instance.OnObjectPlaced();
        }
        else
        {
            // Wrong / Misplaced
            Debug.Log($"[Puzzle] MISPLACED OBJECT: '{puzzleObj.gameObject.name}' ({puzzleObj.objectType}) placed into wrong socket '{gameObject.name}' (accepts: {_socket.acceptedType})!");
            puzzleObj.ReturnToOriginal(isMisplaced: true);
        }
    }
}