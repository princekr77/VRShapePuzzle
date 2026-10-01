using UnityEngine;

public class PuzzleSocket : MonoBehaviour
{
    public ObjectType acceptedType;
    public Transform snapPoint;
    public GameObject greenVisual; // optional highlight

    private bool _isOccupied;
    private Renderer _renderer;
    private Color _originalColor;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null && _renderer.material != null)
        {
            if (_renderer.material.HasProperty("_BaseColor"))
                _originalColor = _renderer.material.GetColor("_BaseColor");
            else if (_renderer.material.HasProperty("_Color"))
                _originalColor = _renderer.material.color;
            else
                _originalColor = _renderer.material.color;
        }

        // Ensure socket does not fall down due to gravity
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

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

        if (greenVisual != null)
        {
            greenVisual.SetActive(true);
        }
        else if (_renderer != null)
        {
            if (_renderer.material.HasProperty("_BaseColor"))
                _renderer.material.SetColor("_BaseColor", Color.green);
            else
                _renderer.material.color = Color.green;
        }
    }

    public void ResetSocket()
    {
        _isOccupied = false;
        if (greenVisual != null)
        {
            greenVisual.SetActive(false);
        }
        else if (_renderer != null)
        {
            if (_renderer.material.HasProperty("_BaseColor"))
                _renderer.material.SetColor("_BaseColor", _originalColor);
            else
                _renderer.material.color = _originalColor;
        }
    }
}
