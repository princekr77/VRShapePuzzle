using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HapticManager : MonoBehaviour
{
    private static HapticManager _instance;
    public static HapticManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HapticManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("[HapticManager]");
                    _instance = go.AddComponent<HapticManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TriggerHaptic(float duration = 0.1f, float amplitude = 0.5f, float frequency = 0.5f)
    {
        StopAllCoroutines();
        StartCoroutine(HapticRoutine(duration, amplitude, frequency));
    }

    private IEnumerator HapticRoutine(float duration, float amplitude, float frequency)
    {
        // 1. Try Meta OVRInput for Meta Quest Touch Controllers
        try
        {
            OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.Active);
        }
        catch { }

        // 2. Fallback to standard UnityEngine.XR InputDevices for OpenXR/XR Interaction
        var devices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, devices);
        foreach (var device in devices)
        {
            if (device.isValid)
            {
                device.SendHapticImpulse(0u, amplitude, duration);
            }
        }

        yield return new WaitForSeconds(duration);

        // Stop OVRInput vibration after duration
        try
        {
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.Active);
        }
        catch { }
    }

    public void PlayHoverHaptic() => TriggerHaptic(0.05f, 0.3f, 0.5f);
    public void PlayGrabHaptic() => TriggerHaptic(0.12f, 0.6f, 0.6f);
    public void PlayPlaceHaptic() => TriggerHaptic(0.2f, 0.8f, 0.8f);
    public void PlayErrorHaptic() => TriggerHaptic(0.3f, 1.0f, 0.9f);
}
