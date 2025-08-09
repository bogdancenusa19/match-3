// Assets/_Project/Scripts/UI/UIFloatingTextSpawner.cs
using UnityEngine;

public class UIFloatingTextSpawner : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRoot;
    [SerializeField] private UIFloatText floatTextPrefab;
    [SerializeField] private int pool = 8;

    private UIFloatText[] items;
    private Camera cam;
    private Canvas canvas;

    private void Awake()
    {
        if (!canvasRoot)
        {
            var cv = GetComponentInParent<Canvas>();
            if (cv) canvasRoot = cv.transform as RectTransform;
        }
        canvas = canvasRoot ? canvasRoot.GetComponent<Canvas>() : null;
        cam = Camera.main;

        items = new UIFloatText[Mathf.Max(1, pool)];
        for (int i = 0; i < items.Length; i++)
        {
            var inst = Instantiate(floatTextPrefab, canvasRoot);
            inst.gameObject.SetActive(false);
            items[i] = inst;
        }
    }

    public void SpawnWorld(Vector3 worldPos, string text)
    {
        if (!canvasRoot) return;

        if (!cam) cam = Camera.main;
        var cv = canvasRoot.GetComponent<Canvas>();
        Camera worldCam = (cv && cv.renderMode == RenderMode.ScreenSpaceCamera && cv.worldCamera)
                        ? cv.worldCamera
                        : cam;

        Vector2 screen = RectTransformUtility.WorldToScreenPoint(worldCam, worldPos);

        Camera uiCam = (cv && cv.renderMode == RenderMode.ScreenSpaceCamera) ? worldCam : null;

        Vector2 uiPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRoot, screen, uiCam, out uiPos);

        for (int i = 0; i < items.Length; i++)
        {
            if (!items[i].gameObject.activeSelf)
            {
                items[i].Show(uiPos, text);
                return;
            }
        }
        items[0].Show(uiPos, text);
    }


    public void SpawnScreenCenter(string text)
    {
        if (!canvasRoot) return;
        var size = canvasRoot.rect.size;
        var center = new Vector2(0f, 0f);
        for (int i = 0; i < items.Length; i++)
        {
            if (!items[i].gameObject.activeSelf)
            {
                items[i].Show(center, text);
                return;
            }
        }
        items[0].Show(center, text);
    }
}
