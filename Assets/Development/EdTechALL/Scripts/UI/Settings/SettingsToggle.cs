using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SettingsToggle : MonoBehaviour
{
    [HideInInspector]
    public bool chooseWithDropdown = true;
    [SerializeField]
    public string playerPrefsKey = "default";
    [HideInInspector]
    public int index = 0;
    public Toggle toggle;

    private bool initialized = false;

    private void Awake()
    {
        EventBus.SaveRegisterCallback(this, playerPrefsKey, "ChangeToggleValue");
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        bool isOn = LoadFromPlayerPrefs(playerPrefsKey);
        if (toggle.isOn == isOn)
        {
            OnToggleValueChanged(isOn);
        }
        else
        {
            toggle.isOn = isOn;
        }
    }

    public void OnToggleValueChanged(bool value)
    {
        if (initialized)
        {
            EventBus.Publish(playerPrefsKey, value);
            SaveToPlayerPrefs(playerPrefsKey, value);
        }
        else
        {
            initialized = true;
        }
    }

    public void ChangeToggleValue(bool value)
    {
        if (toggle.isOn != value)
        {
            toggle.isOn = value;
        }
    }

    public void SaveToPlayerPrefs(string key, bool value)
    {
        int prefsValue = value ? 1 : 0;
        PlayerPrefs.SetInt(key, prefsValue);
        PlayerPrefs.Save();
    }

    public bool LoadFromPlayerPrefs(string key)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            return GetComponent<Toggle>().isOn;
        }
        return PlayerPrefs.GetInt(key) != 0;
    }

    public void OnShowSliders(int toggleIndex)
    {
        if (toggleIndex == 0)
        {
            toggle.interactable = false;
        }
        else
        {
            toggle.interactable = true;
        }
    }

    private void OnDestroy()
    {
        EventBus.SaveDeregisterCallback(this, playerPrefsKey, "ChangeToggleValue");
    }
}
