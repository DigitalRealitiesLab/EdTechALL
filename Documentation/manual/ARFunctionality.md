#AR Functionality
There are 2 ways that image marker tracking is handled by the EdTechALL app. 

##Texture to Prefab Mapper & ImageTrackingObjectManager
This was the initial approach to communicating with ARFoundation's marker detection. Following - "never change a running system", when this approach proved to be insufficient to the app's needs, the second variant (chapter below) was added. By now this variant could be replaced entirely with the second handling option, but there was no time for implementing that. 

###TextureToPrefabMapper
The texture to prefab mapper is a scriptable object, that allows to define Game Objects as prefabs that get spawned on a detected image marker. Every Lesson needs a different Prefab Mapper. The prefab mapper used by the application is therefore selected in the [LessonMarkerDetectionScene](SceneStructure.md) and set as a static reference in the [EdTechALLConfig](../api/Global.EdTechALLConfig.html). 

##ImageMarkerDetector & MarkerReactionBehaviour
The ImageMarkerDetector class manages the state of detected reference images. ARFoundation has its own system of defining states for the reference images that is somehwat unpretictalbe and needs further processing to be useful.
When the state of an image changes or it is updated, the ImageMarkerDetector broadcasts events to MonoBehaviours on the same or child GameObjects that implement the following methods:

* **OnImageMarkerDetected** - A new image marker was detected.
* **OnImageMarkerUpdate** - An existing image marker was updated, eg. changed its prosition or rotation.
* **OnImageMarkerLost** - An existing image marker was lost, meaning the marker is not tracked anymore at all.

The abstract AMarkerReactionBehaviour class forces can be used to implement behaviours that react to image markers chaning their state. 
Examples - show an UI element, select the lesson, or change 
