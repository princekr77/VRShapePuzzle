using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VRScreenFader : MonoBehaviour
{
    private static VRScreenFader _instance;
    public static VRScreenFader Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<VRScreenFader>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("[VRScreenFader]");
                    _instance = go.AddComponent<VRScreenFader>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private Canvas _fadeCanvas;
    private Image _fadeImage;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureFadeCanvas();
    }

    private void EnsureFadeCanvas()
    {
        if (_fadeCanvas != null && _fadeImage != null) return;

        GameObject canvasGo = new GameObject("VRFadeCanvas");
        canvasGo.transform.SetParent(transform, false);
        _fadeCanvas = canvasGo.AddComponent<Canvas>();
        _fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _fadeCanvas.sortingOrder = 9999; // Always on top

        GameObject imgGo = new GameObject("FadeImage");
        imgGo.transform.SetParent(canvasGo.transform, false);
        _fadeImage = imgGo.AddComponent<Image>();
        _fadeImage.color = new Color(0, 0, 0, 0); // Start transparent

        RectTransform rt = imgGo.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    public void FadeToBlack(float duration = 1.5f, System.Action onComplete = null)
    {
        EnsureFadeCanvas();
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(0f, 1f, duration, onComplete));
    }

    public void FadeFromBlack(float duration = 1.5f, System.Action onComplete = null)
    {
        EnsureFadeCanvas();
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(1f, 0f, duration, onComplete));
    }

    private IEnumerator FadeRoutine(float startAlpha, float targetAlpha, float duration, System.Action onComplete)
    {
        float timer = 0f;
        Color c = Color.black;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            c.a = alpha;
            if (_fadeImage != null) _fadeImage.color = c;
            yield return null;
        }
        c.a = targetAlpha;
        if (_fadeImage != null) _fadeImage.color = c;
        onComplete?.Invoke();
    }
}
