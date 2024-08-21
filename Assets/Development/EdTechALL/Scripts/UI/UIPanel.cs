using UnityEngine;

public class UIPanel : MonoBehaviour
{
    [SerializeField]
    private int _priorityIndex;
    public int priorityIndex
    {
        get { return _priorityIndex; }
        set { _priorityIndex = value; }
    }

    public static int None = -1;
    public static int DefaultPanel = 0;
    public static int SettingsPanel = 1;
    public static int MiniGamePanel = 2;

    public static void ActivateUIPanelSingle(int panelIndex)
    {
        EventBus.Publish(EventId.UIEvents.ActivatePanelSingle, panelIndex);
    }

    public static void ActivateUIPanel(int panelIndex)
    {
        EventBus.Publish(EventId.UIEvents.ActivatePanel, panelIndex);
    }

    public static void DeactivateUIPanel(int panelIndex)
    {
        EventBus.Publish(EventId.UIEvents.DeactivatePanel, panelIndex);
    }

    public static void ChangePanelPriority(int panelIndex, int reorderPriority)
    {
        EventBus.Publish(EventId.UIEvents.ChangePanelPriority, panelIndex, reorderPriority);
    }

    public static void ReturnToDefaultPanel()
    {
        ActivateUIPanelSingle(DefaultPanel);
    }
}
