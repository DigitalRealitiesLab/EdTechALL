using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class SettingsToggleGroup : MonoBehaviour
{
    [HideInInspector]
    public bool chooseWithDropdown = true;
    [SerializeField]
    public string playerPrefsKey = "default";
    [HideInInspector]
    public int index = 0;
    public ToggleGroup toggleGroup;
    public int defaultToggle = 0;

    private Toggle[] toggles;

    private void Awake()
    {
        toggleGroup = GetComponent<ToggleGroup>();
        toggles = GetComponentsInChildren<Toggle>(true);
        int i = 0;
        int isOn = LoadFromPlayerPrefs(playerPrefsKey);
        foreach (Toggle toggle in toggles)
        {
            if (i == isOn)
            {
                toggle.isOn = true;
            }
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
            i++;
        }
    }

    public void OnToggleValueChanged(bool value)
    {
        int i = 0;
        foreach (Toggle toggle in toggles)
        {
            if (toggle.isOn)
            {
                EventBus.Publish(playerPrefsKey, i);
                SaveToPlayerPrefs(playerPrefsKey, i);
            }
            i++;
        }
    }

    public void SaveToPlayerPrefs(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public int LoadFromPlayerPrefs(string key)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            return defaultToggle;
        }
        return PlayerPrefs.GetInt(key);
    }
}
