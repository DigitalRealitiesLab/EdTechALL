using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SettingsToggle)),CanEditMultipleObjects]
public class SettingsToggleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var targetAsSettingsToggle = target as SettingsToggle;
        targetAsSettingsToggle.chooseWithDropdown = EditorGUILayout.Toggle("Choose with dropdown", targetAsSettingsToggle.chooseWithDropdown);

        if (targetAsSettingsToggle.chooseWithDropdown)
        {
            targetAsSettingsToggle.index = EditorGUILayout.Popup("Event Type", targetAsSettingsToggle.index, MiniGame.EventId.SettingsToggle);
            targetAsSettingsToggle.playerPrefsKey = MiniGame.EventId.SettingsToggle[targetAsSettingsToggle.index];

            if (GUI.changed)
            {
                EditorUtility.SetDirty(targetAsSettingsToggle);
                PrefabUtility.RecordPrefabInstancePropertyModifications(targetAsSettingsToggle);
            }
        }
    }
}
