#Scene Structure
##Scene Order
1. **Init** 
	1. [Scene Loader](../api/Global.SceneLoader.html) activates the [Loading Screen](../api/Global.LoadingScreen.html) and loads the Lesson Marker Detection Scene. Scene Loader allows to set the next loaded scene via a scene name field in the editor. 
	2. [File Transfer](../api/CardModule.FileTransfer.html) of all files and directories in StreamingAssets/h5p/... to PersistantDataPath/h5p/... is activated. By doing this, all files can be found at the same place, if a future version of the app decides to download additional content from the internet which is also usually saved in the persistant data path. 
2. **Lesson Marker Detection**
	1. Lets the user select which reference image marker they are using in the [MapSelectionPanel](../api/Global.MapSelectionPanel.html). 
	2. Lets the user select a lesson (1 - 6) by either clicking a button in the list in the lower panel.
	3. The chosen configuration and lesson data is then set as a reference in the [EdTechALLConfig](EdTechALLConfig.md). 
	4. The save data can be deleted in the settings menu of this scene. See [EdTechALLConfig](EdTechALLConfig.md) for a more detailed description of saving in the EdTechALL app. 
3. **Lessons**
	*	**Lesson 1** MapLayer, Gemeinde Suche, Bauernhof Vergleich, Realistic Cow 
	* 	**Lesson 2** Cow Caretaker (Cow Tamagotchi)
	* 	**Lesson 3** Path Programming (depricated but still there for demo purposes)
	* 	**Lesson 4** empty
	* 	**Lesson 5** empty
	* 	**Lesson 6** 	empty

##Scene Loader
The [SceneLoader](../api/Global.SceneLoader.html) is responsible for loading new scenes and activating the loading screen. It has several events that can be subscribed to in order to implement game logic related to scene change eg. image marker selection. 