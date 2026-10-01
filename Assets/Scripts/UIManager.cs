using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("[UIManager]");
                    _instance = go.AddComponent<UIManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    [Header("UI Text References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI objectsRemainingText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI errorText;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        AutoDiscoverTextReferences();
    }

    private void Start()
    {
        AutoDiscoverTextReferences();
    }

    public void AutoDiscoverTextReferences()
    {
        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>(true);
        foreach (var tmp in allTexts)
        {
            string name = tmp.gameObject.name.ToLower();
            string content = tmp.text.ToLower();

            if (timerText == null && (name.Contains("time") || name.Contains("timer") || content.Contains("time")))
            {
                timerText = tmp;
            }
            else if (objectsRemainingText == null && (name.Contains("remain") || name.Contains("object") || content.Contains("remain")))
            {
                objectsRemainingText = tmp;
            }
            else if (statusText == null && (name.Contains("status") || name.Contains("complete") || content.Contains("complete")))
            {
                statusText = tmp;
            }
            else if (errorText == null && (name.Contains("error") || name.Contains("wrong") || content.Contains("wrong")))
            {
                errorText = tmp;
            }
        }

        // If statusText or errorText are unassigned, attach them to the primary Canvas panel
        Canvas mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas != null)
        {
            if (statusText == null)
            {
                statusText = CreateTMPText(mainCanvas.transform, "StatusText", "", new Vector2(0, 50), 32);
                statusText.alignment = TextAlignmentOptions.Center;
                statusText.color = Color.green;
                statusText.gameObject.SetActive(false);
            }
            if (errorText == null)
            {
                errorText = CreateTMPText(mainCanvas.transform, "ErrorText", "", new Vector2(0, -50), 28);
                errorText.alignment = TextAlignmentOptions.Center;
                errorText.color = Color.red;
                errorText.gameObject.SetActive(false);
            }
        }
    }

    private TextMeshProUGUI CreateTMPText(Transform parent, string name, string defaultText, Vector2 anchoredPos, float fontSize)
    {
        GameObject textGo = new GameObject(name);
        textGo.transform.SetParent(parent, false);
        var tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = defaultText;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        var rt = textGo.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(400, 50);
        return tmp;
    }

    public void UpdateTimer(float timeInSeconds)
    {
        AutoDiscoverTextReferences();
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60F);
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }

    public void UpdateObjectsRemaining(int count)
    {
        AutoDiscoverTextReferences();
        if (objectsRemainingText != null)
        {
            objectsRemainingText.text = $"Objects Remaining: {count}";
        }
    }

    public void ShowError(string message)
    {
        AutoDiscoverTextReferences();
        if (errorText != null)
        {
            errorText.text = message;
            errorText.gameObject.SetActive(true);
            CancelInvoke(nameof(ClearError));
            Invoke(nameof(ClearError), 3.5f);
        }
    }

    public void ClearError()
    {
        if (errorText != null)
        {
            errorText.text = string.Empty;
            errorText.gameObject.SetActive(false);
        }
    }

    public void ShowCompletion(string message)
    {
        AutoDiscoverTextReferences();
        if (statusText != null)
        {
            statusText.text = message;
            statusText.gameObject.SetActive(true);
        }
    }
}
