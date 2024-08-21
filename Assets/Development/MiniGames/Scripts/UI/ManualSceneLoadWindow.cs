using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManualSceneLoadWindow : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public MiniGameSceneLookup miniGameSceneLookup;
    public Button[] buttons;

    private int gameMode = 0;

    private const string progressionText = "Tippe auf das nächste Minispiel um es zu starten.";
    private const string markerText = "Scanne den Marker und tippe auf das Minispiel um es zu starten.";
    private const string freeText = "Wähle ein Minispiel aus und tippe darauf um es zu starten.";

    private void Awake()
    {
        Debug.Assert(infoText, "ManualSceneLoadWindow is missing a reference to a TextMeshProUGUI");
        Debug.Assert(miniGameSceneLookup, "ManualSceneLoadWindow is missing a reference to a MiniGameSceneLookup");
        Debug.Assert(miniGameSceneLookup.imageMarkerData.Count == buttons.Length, "ManualSceneLoadWindow is not set up correctly");
        miniGameSceneLookup.Initialize();
        RegisterMiniGameLoadButtonCallbacks();
        EventBus.SaveRegisterCallback(this, MiniGame.EventId.GameModeMarker, "OnGameModeMarker");
        EventBus.SaveRegisterCallback(this, MiniGame.EventId.DeletedSaves, "OnDeletedSaves");

        if (PlayerPrefs.HasKey(MiniGame.EventId.GameMode))
        {
            gameMode = PlayerPrefs.GetInt(MiniGame.EventId.GameMode);
        }

        switch (gameMode)
        {
            case 0:
                int progress = 0;

                if (PlayerPrefs.HasKey(MiniGame.EventId.GameModeProgress))
                {
                    progress = PlayerPrefs.GetInt(MiniGame.EventId.GameModeProgress);
                }

                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].interactable = i <= progress;
                }

                infoText.text = progressionText;
                break;
            case 1:
                int markers = 0;

                if (PlayerPrefs.HasKey(MiniGame.EventId.GameModeMarker))
                {
                    markers = PlayerPrefs.GetInt(MiniGame.EventId.GameModeMarker);
                }

                int currentButton = 0;

                while (markers > 0)
                {
                    buttons[currentButton].interactable = markers % 2 > 0;

                    currentButton++;
                    markers >>= 1;
                }

                while (currentButton < buttons.Length)
                {
                    buttons[currentButton].interactable = false;
                    currentButton++;
                }

                infoText.text = markerText;
                break;
            case 2:
                foreach (Button button in buttons)
                {
                    button.interactable = true;
                }

                infoText.text = freeText;
                break;
        }
    }

    private void RegisterMiniGameLoadButtonCallbacks()
    {
        for (int i = 0; i < miniGameSceneLookup.imageMarkerData.Count; i++)
        {
            MiniGameSceneLoadData loadData = miniGameSceneLookup.imageMarkerData[i];
            buttons[i].onClick.AddListener(() =>
            {
                EdTechALLConfig.prefabMapper = loadData.prefabMapper;
                EventBus.Publish(MiniGame.EventId.RequestMiniGameLoadWindow, loadData, true);
            });
        }
    }

    public void OnGameModeMarker(int marker)
    {
        if (gameMode == 1 && !buttons[marker].interactable)
        {
            buttons[marker].interactable = true;

            int markers = 0;
            int currentButton = buttons.Length - 1;

            while (currentButton >= 0)
            {
                markers <<= 1;

                if (buttons[currentButton].interactable)
                {
                    markers++;
                }

                currentButton--;
            }

            PlayerPrefs.SetInt(MiniGame.EventId.GameModeMarker, markers);
        }
    }

    public void OnDeletedSaves()
    {
        switch (gameMode)
        {
            case 0:
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].interactable = i <= 0;
                }

                break;
            case 1:
                foreach (Button button in buttons)
                {
                    button.interactable = false;
                }

                break;
        }
    }

    private void OnDestroy()
    {
        EventBus.SaveDeregisterCallback(this, MiniGame.EventId.GameModeMarker, "OnGameModeMarker");
        EventBus.SaveDeregisterCallback(this, MiniGame.EventId.DeletedSaves, "OnDeletedSaves");
    }
}
