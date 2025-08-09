using UnityEngine;
using TMPro;
using System.Collections;

public class UILivesWidget : MonoBehaviour
{
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text timerText;

    private void OnEnable()
    {
        StartCoroutine(UpdateRoutine());
    }

    private IEnumerator UpdateRoutine()
    {
        while (true)
        {
            if (LifeSystem.I != null)
            {
                livesText.text = $"Lives: {LifeSystem.I.Lives}";
                var t = LifeSystem.I.TimeToNext;
                timerText.text = LifeSystem.I.Lives >= LifeSystem.I.MaxLives
                ? "Full"
                : $"Next: {t.Minutes:D2}:{t.Seconds:D2}";

            }
            yield return new WaitForSeconds(0.25f);
        }
    }
}
