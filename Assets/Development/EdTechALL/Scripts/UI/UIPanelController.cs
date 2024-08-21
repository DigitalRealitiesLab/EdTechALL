using System.Collections.Generic;
using UnityEngine;

public class UIPanelController : MonoBehaviour
{
    public List<UIPanel> panels;
    public Transform activePanelParent;
    public Transform inactivePanelParent;
    private SortedList<int, UIPanel> activePanels = new SortedList<int, UIPanel>();

    [SerializeField]
    private UIPanel defaultPanel = null;

    private void Awake()
    {
        Debug.Assert(activePanelParent, "UIPanelController is missing a reference to the active panel parent Transform");
        if (!inactivePanelParent)
        {
            inactivePanelParent = transform;
        }
    }

    private void Start()
    {
        EventBus.SaveRegisterCallback(this, EventId.UIEvents.ActivatePanelSingle, "ActivatePanelSingleByIndex");
        EventBus.SaveRegisterCallback(this, EventId.UIEvents.ActivatePanel, "ActivatePanelByIndex");
        EventBus.SaveRegisterCallback(this, EventId.UIEvents.DeactivatePanel, "DeactivatePanelByIndex");
        EventBus.SaveRegisterCallback(this, EventId.UIEvents.ChangePanelPriority, "ChangePanelPriority");

        foreach (var panel in panels)
        {
            if (panel)
            {
                panel.gameObject.SetActive(false);
            }
        }
        if (!defaultPanel)
        {
            return;
        }
        ActivatePanel(defaultPanel);
    }

    public void ChangePanelPriority(int panelIndex, int reorderPriority)
    {
        UIPanel panel = GetPanelByIndex(panelIndex);
        if (activePanels.ContainsKey(panel.priorityIndex))
        {
            activePanels.Remove(panel.priorityIndex);
            panel.priorityIndex = reorderPriority;
            activePanels.Add(panel.priorityIndex, panel);
            ReorderChildrenByActivePanelIndex();
        }
        else
        {
            panel.priorityIndex = reorderPriority;
        }
    }

    public UIPanel GetPanelByIndex(int index)
    {
        UIPanel panel = index >= 0 && index < panels.Count ? panels[index] : null;
        Debug.Assert(panel, "Requested UIPanel at index " + index + " does not exist");
        return panel;
    }

    public bool GetPanelActiveByIndex(int index)
    {
        foreach (var activePanel in activePanels)
        {
            if (activePanel.Key == index)
            {
                return true;
            }
        }
        return false;
    }

    private void ReorderChildrenByActivePanelIndex()
    {
        int index = 0;
        foreach (var activePanel in activePanels)
        {
            activePanel.Value.transform.SetSiblingIndex(index);
            ++index;
        }
    }

    public void ActivatePanelByIndex(int index) { ActivatePanel(GetPanelByIndex(index)); }
    private void ActivatePanel(UIPanel panel)
    {
        if (activePanels.ContainsValue(panel))
        {
            return;
        }
        activePanels.Add(panel.priorityIndex, panel);
        panel.transform.SetParent(activePanelParent);
        panel.gameObject.SetActive(true);
        ReorderChildrenByActivePanelIndex();
    }

    public void ActivatePanelSingleByIndex(int index)
    {
        DeactivateAllPanels();
        ActivatePanelByIndex(index);
    }

    private void DeactivateAllPanels()
    {
        List<UIPanel> activePanelCopy = new List<UIPanel>(activePanels.Values);
        foreach (var panel in activePanelCopy)
        {
            DeactivatePanel(panel);
        }
    }

    public void DeactivatePanelByIndex(int index) { DeactivatePanel(GetPanelByIndex(index)); }
    private void DeactivatePanel(UIPanel panel)
    {
        if (!activePanels.ContainsKey(panel.priorityIndex))
        {
            return;
        }
        activePanels.Remove(panel.priorityIndex);
        panel.transform.SetParent(inactivePanelParent);
        panel.gameObject.SetActive(false);
    }

    public void DeactivateHighestPriorityPanel()
    {
        UIPanel highestPriorityPanel = activePanels.Values[activePanels.Count - 1];
        DeactivatePanel(highestPriorityPanel);
        if (activePanels.Count == 0)
        {
            ActivatePanel(defaultPanel);
        }
    }

    private void OnDestroy()
    {
        EventBus.DeregisterAllCallbacks(this); 
    }
}

public partial class EventId
{
    public static class UIEvents
    {
        public const string ActivatePanelSingle = "ActivatePanelSingle";
        public const string ActivatePanel = "ActivatePanel";
        public const string DeactivatePanel = "DeactivatePanel";
        public const string ChangePanelPriority = "ChangePanelPriority";
        public const string RefreshVisualGuidance = "RefreshVisualGuidance";
    }
}