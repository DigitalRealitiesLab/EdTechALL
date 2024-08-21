#CardModule
The card module works hand in hand with the [Task System](TaskSystem.md). The card module could be regarded as frontend for the task system, since it provides a way to display the tasks to the user with ProxyCards.

##CardData & CardDataLoader
All the data of a card module is collected in a [**CardData**](../api/CardModule.CardData.html) scriptable object. It contains an array of [H5PCardData](../api/CardModule.H5PCardData.html) for quiz and info proxy cards, an array of [MiniGameConfiguration](../api/CardModule.MiniGameConfig.html) for mini game cards, and a [TaskData](../api/TaskSystem.Tasks.TaskData.html) array for task cards. 
While the first three reference actual content, the task data only references positions of the other three with an index and a [CardType](../api/api/CardModule.CardData.CardType.html) enum.

The [**CardDataLoader**](../api/CardModule.CardDataLoader.html) provides a single accesspoint for other scripts to access the CardData. This is necessary because the CardData scriptable object is serialized and saved. The CardDataLoader loads the existing data from the save file and overrides the scriptable object. 

##How are Proxy Cards Displayed
Proxy Cards are displayed in 3 ways: 

1. World Space (somewhere on the Salzburg Begreifen map)
2. Screen Space (Card Panel) 
3. Screen Space (Card Stack Panel)

###World Space

There are Proxy Cards in 4 colours:

* **Blue**: Info Cards - They can be reopened any time after picking them up from the [CardStackPanelUI](../api/CardModule.UI.CardStackPanelUI.html). The results are saved in the [H5PCardData](../api/CardModule.H5PCardData.html) but the users results are just appended.
* **Green**: Quiz Cards - Contain information and quizzes. They can only be reopened in the [CardStackPanelUI](../api/CardModule.UI.CardStackPanelUI.html) if there are no results saved to questions in the [H5PCardData](../api/CardModule.H5PCardData.html) 
* **Yellow**: Mini Game Cards display data from the [MiniGameConfiguration](../api/CardModule.MiniGameConfig.html) 
* **Orange (red)**: Task Cards - display [TaskData](../api/TaskSystem.Tasks.TaskData.html) information (usually with a sprite image provided by Marie) with alt text in case there is no image yet. 


###Screen Space
####Card Panel
The Card Panel displays the actual content of the proxy card. It is used to start tasks, start mini games, and display h5p content. It has 4 subpanels - one for each type of proxy card. It contains 2 3D Web Views from the [Vuplex Plugin](https://store.vuplex.com/webview/overview) that display h5p content for displaying h5p data from quiz and info cards. It also contains 2 other panels for tasks and mini games with a similar layout. 

####Card Stack Panel
The Card Stack Panel displays proxy cards that have already been picked up by the user before. It also loads this information from the saved card data - which means it is still availabe after restarting the application. 
By now it does not have the ability to display mini game cards - which is probably good so you can't randomly start mini games again. It could show the score of the mini games though - it might be worth implementing in the future. 