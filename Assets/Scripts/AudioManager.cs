using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip hoverClip;
    public AudioClip grabClip;
    public AudioClip placeClip;
    public AudioClip successClip;
    public AudioClip errorClip;
    public AudioClip completionClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (sfxSource == null)
        {
            sfxSource = GetComponent<AudioSource>();
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    public void PlayHover() => PlayClip(hoverClip);
    public void PlayGrab() => PlayClip(grabClip);
    public void PlayPlace() => PlayClip(placeClip);
    public void PlaySuccess() => PlayClip(successClip);
    public void PlayError() => PlayClip(errorClip);
    public void PlayCompletion() => PlayClip(completionClip);

    private void PlayClip(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
