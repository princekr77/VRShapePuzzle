using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("[AudioManager]");
                    _instance = go.AddComponent<AudioManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    [Header("Audio Sources")]
    public AudioSource sfxSource;

    [Header("Audio Clips (Optional - Generated fallback if empty)")]
    public AudioClip hoverClip;
    public AudioClip grabClip;
    public AudioClip placeClip;
    public AudioClip successClip;
    public AudioClip errorClip;
    public AudioClip completionClip;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureAudioSource();
        GenerateFallbackClipsIfNeeded();
    }

    private void EnsureAudioSource()
    {
        if (sfxSource == null)
        {
            sfxSource = GetComponent<AudioSource>();
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
        }
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f; // 2D UI sound
    }

    private void GenerateFallbackClipsIfNeeded()
    {
        if (hoverClip == null) hoverClip = CreateToneClip(880f, 0.04f, 0.2f);       // A5 quick soft ping
        if (grabClip == null) grabClip = CreateToneClip(587.33f, 0.08f, 0.3f);     // D5 grab click
        if (placeClip == null) placeClip = CreateToneClip(659.25f, 0.12f, 0.4f);    // E5 placement tone
        if (successClip == null) successClip = CreateChimeClip(523.25f, 659.25f, 0.25f, 0.4f); // C5-E5 chime
        if (errorClip == null) errorClip = CreateToneClip(180f, 0.25f, 0.5f);        // Low error buzz
        if (completionClip == null) completionClip = CreateFanfareClip();            // Completion triad
    }

    public void PlayHover() => PlayClip(hoverClip);
    public void PlayGrab() => PlayClip(grabClip);
    public void PlayPlace() => PlayClip(placeClip);
    public void PlaySuccess() => PlayClip(successClip);
    public void PlayError() => PlayClip(errorClip);
    public void PlayCompletion() => PlayClip(completionClip);

    private void PlayClip(AudioClip clip)
    {
        EnsureAudioSource();
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    #region Procedural Audio Generator

    private AudioClip CreateToneClip(float frequency, float duration, float volume)
    {
        int sampleRate = 44100;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = 1f - (t / duration);
            samples[i] = Mathf.Sin(2 * Mathf.PI * frequency * t) * volume * envelope;
        }
        AudioClip clip = AudioClip.Create("GeneratedTone_" + frequency, sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateChimeClip(float f1, float f2, float duration, float volume)
    {
        int sampleRate = 44100;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];
        int half = sampleCount / 2;
        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float freq = (i < half) ? f1 : f2;
            float envelope = 1f - (t / duration);
            samples[i] = Mathf.Sin(2 * Mathf.PI * freq * t) * volume * envelope;
        }
        AudioClip clip = AudioClip.Create("GeneratedChime", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateFanfareClip()
    {
        float duration = 0.5f;
        int sampleRate = 44100;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];
        float[] freqs = new float[] { 523.25f, 659.25f, 783.99f }; // C, E, G triad
        int segment = sampleCount / freqs.Length;
        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            int idx = Mathf.Min(i / segment, freqs.Length - 1);
            float envelope = 1f - ((float)(i % segment) / segment);
            samples[i] = Mathf.Sin(2 * Mathf.PI * freqs[idx] * t) * 0.4f * envelope;
        }
        AudioClip clip = AudioClip.Create("GeneratedFanfare", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    #endregion
}
