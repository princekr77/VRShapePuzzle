using UnityEngine;

[RequireComponent(typeof(PuzzleSocket))]
public class SocketTrigger : MonoBehaviour
{
    private PuzzleSocket _socket;

    void Awake() => _socket = GetComponent<PuzzleSocket>();

    void OnTriggerEnter(Collider other)
    {
        var puzzleObj = other.GetComponent<PuzzleObject>();
        if (puzzleObj == null || puzzleObj.isLocked) return;

        if (_socket.CanAccept(puzzleObj))
        {
            // Correct
            puzzleObj.LockInSocket(_socket);
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySuccess();
            if (_socket.greenVisual != null) _socket.greenVisual.SetActive(true);
            if (PuzzleManager.Instance != null) PuzzleManager.Instance.OnObjectPlaced();
        }
        else
        {
            // Wrong
            puzzleObj.ReturnToOriginal();
            if (UIManager.Instance != null) UIManager.Instance.ShowError("Wrong socket! Try the matching shape.");
            if (AudioManager.Instance != null) AudioManager.Instance.PlayError();
        }
    }
}