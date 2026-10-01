using UnityEngine;

public class DropZoneDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var puzzleObj = other.GetComponent<PuzzleObject>();
        if (puzzleObj != null)
        {
            puzzleObj.CancelScheduledReturn();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var puzzleObj = other.GetComponent<PuzzleObject>();
        if (puzzleObj == null || puzzleObj.isLocked) return;

        // Object left the designated drop zone - schedule return to table
        if (!puzzleObj.isPlaced)
        {
            puzzleObj.ScheduleReturnToOriginal(2f);
        }
    }
}
