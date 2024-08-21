#Mini Game

##Existing Mini Games
###Lesson 1 Map Layer
Shows Textures on the map that are adjusted to match with the borders of Salzburg. 
When adding new layers all textures need to have the same positioning of Salzburg. Doing otherwise will result in erraneous layout. 
The game can easily be adjusted with the MapLayerLookup scriptable object that is already linked to the MapLayerMiniGame. You can also create another version of the game by creating a new MapLayerLookup scriptable object and switching them in the mini game prefab.

###Lesson 1 Gemeindesuche & Bauernhof Analyse
This combines two seperate mini game functionalities

1. Gemeindesuche
	* Gives an interface to locating the world position of towns and other municipalities on the map via image marker. It instantiates an interactable at this position which spanws another object on interaction. The other object is defined as a prefab in the GemeindeSucheLookup scriptable object in Assets > MiniGames > Games > GemeindeSuche_Bauernhof and can easily be edited or reused for other games with similar functionality. The UI contains a button that displays the detected image marker and can reset the searching process on click. 
2. Bauernhof Analyse
	* The Gemeindesuche spwans either a Bergbauernhof or Flachlandbauernhof. The prefabs have differend sources of income which have to be interacted with to complete the game. 

You might have to implement a mini game where instead of a farm a dairy factory shows up. You can reuse the first part of the mini game and implement that by:

1. Cereating your image markers (Photoshop)
2. Adding those images to a new XRReferenceImageLibrary
3. Adding your XRReferenceImageLibrary to the [ReferenceImageLibraryManager]() in the *Prefab AR Session Origin* found at Assets > EdTechALL > Prefabs > Scene
4. Creating a new GemeindeSucheLookup scriptable object and filling in the data with your images and prefabs
5. Creating a new MiniGame Prefab following the steps in section *Adding a new Mini Game* at the end of this site


###Lesson 1 Realistic Cow 
This mini game shows 3d models of a more realistic cow in contrast to the otherwise cartoonish style of the app. The models can be switched by clicking a button in world space. 

This game is not yet finished! Your first task will probably be to add a new mechanic to this game, where the user has to find and press certain body parts of the model to collect funy related cow facts. The document with the cow facts can be found in Teams under 

###Lesson 2 Cow Tamagotchi
In this game you need to take care of a cow by giving it food, water, cleaning it's stable and more. It will also get a calf that you need to care for as well.

This game is not finished! It is missing h5p content which will be provided by our project partners. Wenn it is there add a new day in the CowCaretakerTimeline scriptable object in Assets > MiniGames > Games > CowCaretaker and reference the h5p content in this day via ProxyCardData.

##Future Mini Games
You can find short descriptions of the mini games planned for further lessons in Teams under Documents > General > 07 App > Konzepte
A more detailed description of the lessons can be found in Teams under Documents > General > Didaktik_Workshop > Inhaltsübersicht & Didaktische Planung

###Lesson 3 
Farm - Economic Simulation - you might be able to use the scripts in the BuildingBox namespace for this game! I used the BuildingBox to make a game where you have to build a Farm before. If you can't use it just delete it, but watch out for the [SpawningArea](../api/BuildingBox.SpawnArea.html) which is still a part of this namespace.  

###Lesson 4
Diary Production - virtual tour through a place with diary and/or cheese production. 

###Lesson 5
Food falling from the sky and the user chaches it with a shopping cart

###Lesson 6
Milk Production Chain with a decision tree and tracking the economic footprint of the decisions

##Depricated Mini Games
There have been several games implemented already that were removed because they did not match the didactical concept

* **Farming**: Growing wheat 
* **Homogenization & Pasteurization**: Pipe connect game
* **Throwing Range**: Throw balls at groceries
* **Path Programming**: Give a milk truck a sequence of commands to navigate on a 10x10 tile map. The project partners actually liked this game a lot and would like to keep it, though it is not planned yet. Currently you can find it in lesson 3

##Adding a new Mini Game
1. If you create a completely new game, add a new entry to the MiniGameType enum in AMiniGame.cs. There are already some entries for existing and planned games.
2. Create a new subfolder at Assets > MiniGames > Games for your game to match the existing folder structure 
3. Create prefabs and configure them
	1. MiniGame Prefab: Create the prefab you want to be shown in world space on the Salzburg Begreifen map. Then add your prefab to the MiniGamePrefabLookup scriptable object in Assets > MiniGame > *MiniGamePrefabLookup_0.asset* You can either create your own or use the template in Assets > MiniGame > Prefabs > *BaseMiniGame.asset*. The advantage of using the existing prefab as base include: 
		* Lightprobe Group is already set up
		* Has a size reference of 1x1m which is the bound size the prefabs must have in order to be scaled correctly to fit the Salzburg Begreifen map. 


	2. UI Prefab: After creating your prefab, add it to the *MiniGameUI* Prefab found in Assets > MiniGames > Prefabs > UI. Assign a refernce to your UI Panel to the [MiniGameUIController](../api/MiniGame.UI.MiniGameUIController.html) on the MiniGameUI prefab. 