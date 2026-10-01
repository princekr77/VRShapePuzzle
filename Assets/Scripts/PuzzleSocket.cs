using UnityEngine;

public class PuzzleSocket : MonoBehaviour
{
    public ObjectType acceptedType;
    public Transform snapPoint;
    public GameObject greenVisual; // optional highlight

    private bool _isOccupied;

    public bool CanAccept(PuzzleObject obj)
    {
        return !_isOccupied && obj.objectType == acceptedType;
    }

    public void Occupy(PuzzleObject obj)
    {
        _isOccupied = true;
        if (snapPoint != null)
        {
            obj.transform.position = snapPoint.position;
            obj.transform.rotation = snapPoint.rotation;
        }
        else
        {
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
        }
    }

    public void ResetSocket()
    {
        _isOccupied = false;
        if (greenVisual) greenVisual.SetActive(false);
    }
}