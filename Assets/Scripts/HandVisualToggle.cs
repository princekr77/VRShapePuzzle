using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HandVisualToggle : MonoBehaviour
{
    [Header("Hand Visual Roots (Auto-discovered if unassigned)")]
    [Tooltip("Left hand visual GameObject or container")]
    public GameObject leftHandVisual;

    [Tooltip("Right hand visual GameObject or container")]
    public GameObject rightHandVisual;

    [Header("Optional: Wrist Menu Status Text")]
    public TextMeshProUGUI statusText;

    [Header("Start State")]
    public bool startVisible = true;

    private bool _isVisible;
    private List<Renderer> _handRenderers = new List<Renderer>();

    private void Awake()
    {
        AutoDiscoverHandVisuals();
    }

    private void Start()
    {
        AutoDiscoverHandVisuals();
        _isVisible = startVisible;
        ApplyVisibility(_isVisible);
        UpdateStatusText();
    }

    public void AutoDiscoverHandVisuals()
    {
        _handRenderers.Clear();

        // 1. If assigned directly in Inspector, use them
        if (leftHandVisual != null) AddRenderersFromObject(leftHandVisual);
        if (rightHandVisual != null) AddRenderersFromObject(rightHandVisual);

        // 2. Auto-search scene for hand anchors, ISDK hand visuals, or OVRHand meshes
        string[] handNames = new string[] {
            "LeftHandVisual", "RightHandVisual", "SyntheticHandLeft", "SyntheticHandRight",
            "OVRHandPrefab", "HandVisual", "LeftHand", "RightHand", "b_l_wrist", "b_r_wrist",
            "LeftHandAnchor", "RightHandAnchor"
        };

        foreach (var name in handNames)
        {
            GameObject[] found = GameObject.FindObjectsOfType<GameObject>(true);
            foreach (var go in found)
            {
                if (go.name.ToLower().Contains(name.ToLower()))
                {
                    if (leftHandVisual == null && go.name.ToLower().Contains("left")) leftHandVisual = go;
                    if (rightHandVisual == null && go.name.ToLower().Contains("right")) rightHandVisual = go;
                    AddRenderersFromObject(go);
                }
            }
        }
    }

    private void AddRenderersFromObject(GameObject go)
    {
        if (go == null) return;
        Renderer[] rends = go.GetComponentsInChildren<Renderer>(true);
        foreach (var r in rends)
        {
            if (!_handRenderers.Contains(r))
            {
                _handRenderers.Add(r);
            }
        }
    }

    public void ToggleHandVisuals()
    {
        _isVisible = !_isVisible;
        ApplyVisibility(_isVisible);
        UpdateStatusText();

        // Feedback
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGrab();
        if (HapticManager.Instance != null) HapticManager.Instance.PlayHoverHaptic();
    }

    private void ApplyVisibility(bool visible)
    {
        if (leftHandVisual != null) leftHandVisual.SetActive(visible);
        if (rightHandVisual != null) rightHandVisual.SetActive(visible);

        foreach (var rend in _handRenderers)
        {
            if (rend != null)
            {
                rend.enabled = visible;
            }
        }
    }

    private void UpdateStatusText()
    {
        if (statusText != null)
        {
            statusText.text = _isVisible ? "Hands: Visible" : "Hands: Hidden";
        }
    }
}