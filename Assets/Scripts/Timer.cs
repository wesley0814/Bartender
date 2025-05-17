using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public RectTransform transform_sec;
    public Text text_date;

    private float elapsedTime = 0f;
    private float duration = 60f;

    private void Start()
    {
        transform_sec.pivot = new Vector2(0.5f, 0.1f);
    }

    private void Update()
    {
        if (duration - elapsedTime > 0f)
            UpdateTimer();
    }

    private void UpdateTimer()
    {
        elapsedTime += Time.deltaTime;

        float angle = Mathf.Clamp01(elapsedTime / duration) * 360f;
        transform_sec.localRotation = Quaternion.Euler(0f, 0f, -angle);

        text_date.text = $"TIMER\n{(duration - elapsedTime):F1}s";
    }
}
