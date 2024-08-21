#Task System
The task system works hand in hand with the [Card Module](CardModule.md). The task system could be regarded as backend for the card module which displays the task's to the user. 

##TaskController 
* Controls the activation and deactivation of tasks
* Processes Unlockables 
* Requests construction of tasks from the task factory

##Task Factory
The [TaskFactory](../api/TaskSystem.TaskFactory.html) takes an index to look up the entry task entry in the card data in the CardDataLoader. It then looks up a the task prefab corresponding to the type of the requested tasks in the TaskFactoryLookup, instantiates it, then fills in the task's data. 

###Task Factory Lookup
The [TaskFactoryLookup](../api/TaskSystem.TaskFactoryLookup.html) contains prefabs for Tasks ordered by a [TaskType](../api/TaskSystem.Tasks.TaskType.html) enum. The TaskFactory requests the prefab for a task it wants to construct and inserts the TaskData into it. This can be used to Create tasks with default values or tasks with special functionality that is not 

##Unlockable Loader
The [UnlockableLoader](../api/TaskSystem.UnlockableLoader.html) is similar to the CardDataLoader in its purpose. It loads Unlockables from the save data and provides access for other scripts to that data. 

##Task Variants

###Card Task
A [CardTask](../api/TaskSystem.Tasks.CardTask.html) is a type of task that spawns a ProxyCard and ensures that the task of that ProxyCard is completed before continuing to the next task. 
You can use them to implement logic where you want the user to do something to complete a goal. 
There are subclasses of the CardTask 

* [InfoCardTask](../api/TaskSystem.Tasks.InfoCardTask.html)
* [QuizCardTask](../api/TaskSystem.Tasks.QuizCardTask.html)
* [MiniGameTask](../api/TaskSystem.Tasks.MiniGameTask.html)

###Automated Tasks
Automated tasks are a variant that is immeidately fulfilled by the task system and not the user. They therefore don't have a success condition. They complete immediately after fulfilling their purpose. They can not be used to implement tasks where the user has to do something to fulfill them. You can use them to implement application logic related to the user's progress. 

* **Auto Spawn Task**: Spawns a GameObject to a [SpawnArea](../api/BuildingBox.SpawnArea.html) given a spawn area id and a prefab. The only spawn area currently in use has the id *"MapSpawnArea"* and spawns objects in spatial relation to the tracked image Salzburg map. 

* **Proxy Card Auto Spawn Task**: Subclass of AutoSpawnTask. Spawns a Proxy Card with given [ProxyCardData](../api/CardModule.ProxyCardData.html).

##Adding a new kind of Task
1. Create a new class as a subclass of [ATask](../api/TaskSystem.Tasks.ATask.html) 
2. Create a [TaskCallback](../api/TaskSystem.TaskCallbacks.html) for your task and implement the corresponding logic in the CheckForIsComplete method in your class. 
3. Add a new entry for your task in the [TaskType](../api/TaskSystem.Tasks.TaskType.html) enum
4. Create a prefab for your task and add it to the TaskFactoryLookup scriptable object found in Assets > TaskSystem > *TaskFactoryLookup_0*
5. Implement your task logic (this could require extensive knowledge of the project and code strucutre sorry)