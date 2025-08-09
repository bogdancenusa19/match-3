// Assets/_Project/Scripts/UI/UIFloatText.cs
using UnityEngine;
using TMPro;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(TMP_Text))]
public class UIFloatText : MonoBehaviour
{
    [SerializeField] float duration = 0.9f;
    [SerializeField] float rise = 70f;
    [SerializeField] float startScale = 0.8f;
    [SerializeField] float endScale = 1.25f;

    TMP_Text label;
    RectTransform rt;
    float t;
    Vector2 start;
    AnimationCurve ease;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        label = GetComponent<TMP_Text>();
        ease = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        gameObject.SetActive(false);
    }

    public void Show(Vector2 screenPos, string text)
    {
        t = 0f;
        start = screenPos;

        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(220f, 90f);
        rt.anchoredPosition = start;
        rt.localScale = Vector3.one * startScale;

        label.text = text;
        var c = label.color; c.a = 1f; label.color = c;
        label.ForceMeshUpdate();

        gameObject.SetActive(true);
    }

    void Update()
    {
        t += Time.deltaTime;
        float k = Mathf.Clamp01(t / duration);
        float e = ease.Evaluate(k);

        rt.anchoredPosition = start + new Vector2(0f, rise * e);
        rt.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, e);

        var c = label.color; c.a = 1f - k; label.color = c;

        if (k >= 1f) gameObject.SetActive(false);
    }
}
