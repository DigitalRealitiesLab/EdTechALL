#Saving & EdTechALL Config
Configuration and settings are handled in different ways. The first approach before there was a save load system was to simply use the Unity PlayerPrefs to save settings. The approach was later expanded to using a simple save load system, later again with a static class that is easy to access anywhere in code.

##Player Prefs
Early app systems use Unity PlayerPrefs for saving and loading settings. For example the debug view that shows the image marker transparently overlayed, which can be activated in the settings UI, saves its toggle state as a bool in the PlayerPrefs. This way of saving is 

##Saving System
The saving system's heartpeace is the [SaveManager](../api/Global.SaveManager.html). An example of how to use it can be found in the [CardDataLoader](../api/CardModule.CardDataLoader.html) which is responsible for loading the mini game and h5p content configurations and later saving the results of the user. It does so by overriding the [CardData](../api/CardModule.CardData.html) scriptable object that every lesson needs to create. 

##EdTEchALLConfig
The [EdTechALLConfig](../api/Global.EdTechALLConfig.html) contains settings to configure ARFoundation tracking. You can change the TextureToPrefabMapper, the active reference image library, change the lerp rate of the image marker, or check if the user is tracking a large map (3.98m x 3.48m) or the smaller map (0.625m x 0.593m). There are events that allow listening to changing settings values. 
The config is mostly set in the LessonMarkerDetectionScene [(for more about scene structure see here)](SceneStructure.md). To allow using the app in Unity without having to start at the init scene every time, a default config can be specified by placing a GameObject with the script [DefaultEdTechALLConfig](../api/Global.DefaultEdTechALLConfig.html) on it in the scene and filling in the settings there. 