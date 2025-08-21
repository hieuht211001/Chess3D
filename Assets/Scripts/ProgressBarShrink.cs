using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class ProgressBarShrink : MonoBehaviour
{
    public RectTransform progressBar;
    float fullWidth; 
    private static string COLOR_GREEN = "#12FF08";
    private static string COLOR_ORANGE = "#FF9B00";
    private static string COLOR_RED = "#E43636";

    void Start()
    {
        progressBar.pivot = new Vector2(0f, 0.5f);
    }

    private void Awake()
    {
        fullWidth = progressBar.sizeDelta.x;
    }

    public void SetProgress(float current, float max)
    {
        float ratio = Mathf.Clamp01(current / max); 
        float newWidth = ratio * fullWidth;
        progressBar.sizeDelta = new Vector2(newWidth, progressBar.sizeDelta.y);
        SetColor(ratio);
    }

    private void SetColor(float ratio)
    {
        Image img = progressBar.GetComponent<Image>();
        if (img == null) return;
        Color newColor = Color.gray;
        if (ratio >= 0.4f) ColorUtility.TryParseHtmlString(COLOR_GREEN, out newColor);
        else if (ratio >= 0.2f) ColorUtility.TryParseHtmlString(COLOR_ORANGE, out newColor);
        else ColorUtility.TryParseHtmlString(COLOR_RED, out newColor);
        img.color = newColor;
    }

    public void SetPositionY(float yOffset)
    {
        progressBar.localPosition = new Vector3(progressBar.localPosition.x, progressBar.localPosition.y + yOffset, progressBar.localPosition.z);
    }
}
