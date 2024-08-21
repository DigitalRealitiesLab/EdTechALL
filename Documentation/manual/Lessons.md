#Lessons
The EdTechALL App is strucutred into 6 lessons. The content of these lessons was defined in several didactical workshops together with project partners. The content is subject to continuous updates and changes. It is possible that you will have to create and integrate a new lesson and surely you are going to have to modify existing lessons. 

##Adding a new Lesson 
While some of the existing lessons are empty, you can use them as templates for adding a new lesson. You can find them at Assets > Lessons > Lesson_X. Otherwise you have to create them by yourself. 

###Files
* Scene 
	* AR Session - prefab, ARSession Wrapper and checks for ARKit support
	* Directional Light - prefab, light estimation
	* AR Session Origin - prefab, contains objects placed in world space
		* Image Tracking Object Manager is here and updates instances of prefabs configured in the Marker Mapper (TextureToPrefabMapper.cs)
		* Guidance System - prefab (probably depricated)
		* ARCamera - prefab default ARFoundation + Collider and Arrow Guidance System
		* HandController - prefab controls the interaction system state machine
	* Lesson_X_UI
		* UIPanelController 
			* ActivePanels - contains visible panels
			* DefaultPanel - default visible panel at start of the application 
			* SettingsPanel - contains settings, accessible via DefaultPanel
			* MiniGameUI - contains UIPanels shared by all MiniGames, as well as Game Specific UI
			* SnapshotUIPanel - used to make screenshots, is currently dactivated but was accessable over the settings
			* CardPanel - shows Info, Quiz, and Task Cards via h5p or a preconfigured UI. Opens when interacting with a ProxyCard on the map.
			* CardStackPanel - shows cards collected in this lesson. (Also the ones loaded from a game save)
			* MapSelectionPanel - Allows to change between image markers for the map. Accessable via button in the default panel. It also pops up in the LessonMarkerDetection scene on start of the application. 
			* TaskController (Not a Panel) - Contains the full task system and load/save functionality. 
		* ErrorWindow - shows errors, warnings, and infos that should be displayed to the user (must be explicitly published in code)
	* EventSystem
	* DefaultEdTechALLConfig
* Card Data - contains paths to h5p content, defines a red line through a lesson with tasks and unlockables 
* Sprite Lookup - lookup for texture overlays in proxy cards - yellow (MiniGame) and orange(red) (Task) proxy cards that hide the alt text below.
* Marker Mapper - Maps Image Marker Texures from a XRReferenceImageLibrary (Unity) to prefabs that are instantiated and updated on detected image markers
* Prefabs
	* BodenKarte - refrenced in Marker Mapper for large map (4x3.5m)
	* HandKarte - referenced in Marker Mapper for small map (hand size)
	* SharedMapObjects - contains objects that are in Hand and Boden Karte prefabs
	* UI
		* Lesson_X_UI - prefab of the UI instantiated in the scene. The scenes differ from each other in SpriteLookup and CardData all of which are set in the Task Controller (found as child of the UIPanelController in the Lesson's UI)
* Textures - Usually contains the texture that is used in the lesson selection menu to open the scene via image marker detection. 

###Making a new lesson accessible in the Lesson Selection Menu and integrating it in the existing system
When you created all your files as described in the previous section, continue as follows:
1. Open the LessonMarkerDetectionScene
2. Open the MarkerSearchPanel prefab and add a button for your marker here. (Duplicate an existing button and change the texture)
3. Add the Button to the ManualSceneLoadWindow on the MarkerButtons object
4. Open the LessonSceneLookup_0 scriptable object
5. Add a new entry with the texture name as the lookup key and the name of the scene you want to load. Add the MarkerMapper of your lesson in the field PrefabMapper. The other fields contain a description and heading that is displayed to the user.
6. Add your Marker Texture to the XRReferenceImageLibrary LessonLoaderImageMarkerLibrary in Assets > Lessons. For ARKit you have to specify a physical size. The name of the texture entered here must match the name of the texture entered in the MarkerMapper.