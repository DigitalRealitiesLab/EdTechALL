using UnityEngine;

public class SetTitle : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;

    private void Update()
    {
        text.text = gameObject.name;
    }
}
