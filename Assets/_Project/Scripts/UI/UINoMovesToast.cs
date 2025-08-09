// Assets/_Project/Scripts/UI/UINoMovesToast.cs
using UnityEngine;
using TMPro;
using System.Collections;

public class UINoMovesToast : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text text;
    [SerializeField] private float time = 1.2f;

    private Coroutine co;

    public void Show(string msg)
    {
        if (co != null) StopCoroutine(co);
        text.text = msg;
        root.SetActive(true);
        co = StartCoroutine(HideLater());
    }

    private IEnumerator HideLater()
    {
        yield return new WaitForSeconds(time);
        root.SetActive(false);
        co = null;
    }
}
