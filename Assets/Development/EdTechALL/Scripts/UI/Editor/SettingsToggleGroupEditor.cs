using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SettingsToggleGroup)),CanEditMultipleObjects]
public class SettingsToggleGroupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var targetAsSettingsToggleGroup = target as SettingsToggleGroup;
        targetAsSettingsToggleGroup.chooseWithDropdown = EditorGUILayout.Toggle("Choose with dropdown", targetAsSettingsToggleGroup.chooseWithDropdown);

        if (targetAsSettingsToggleGroup.chooseWithDropdown)
        {
            targetAsSettingsToggleGroup.index = EditorGUILayout.Popup("Event Type", targetAsSettingsToggleGroup.index, MiniGame.EventId.SettingsToggleGroup);
            targetAsSettingsToggleGroup.playerPrefsKey = MiniGame.EventId.SettingsToggleGroup[targetAsSettingsToggleGroup.index];

            if (GUI.changed)
            {
                EditorUtility.SetDirty(targetAsSettingsToggleGroup);
                PrefabUtility.RecordPrefabInstancePropertyModifications(targetAsSettingsToggleGroup);
            }
        }
    }
}
