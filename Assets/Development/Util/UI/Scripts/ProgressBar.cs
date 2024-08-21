using UnityEngine;

[ExecuteInEditMode]
public class ProgressBar : MonoBehaviour
{
    public RectTransform rectTransform;

    [SerializeField, Range(0.0f, 1.0f)]
    protected float _progress = 0;
    public float progress
    {
        get => _progress;
        set
        {
            value = Mathf.Clamp(value, 0.0f, 1.0f);
            _progress = value;
            rectTransform.anchorMax = new Vector2(_progress, rectTransform.anchorMax.y);
        }
    }

    private void Awake()
    {
        Debug.Assert(rectTransform, "ProgressBar is missing a reference to RectTransform");
        progress = _progress;
    }
}
