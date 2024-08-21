using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CreditsButton : MonoBehaviour
{
    public Button button;
    public VideoClip videoClip;

    private void Awake()
    {
        if (!button && !TryGetComponent(out button))
        {
            Debug.LogError("CreditsButton is missing a reference to its Button");
        }

        Debug.Assert(videoClip, "CreditsButton is missing a reference to a VideoClip");

        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        FullScreen.FullScreenPromptPanel.RequestFullScreenPromptPanel("", 0.0f, null, videoClip, null, true);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClick);
    }
}
