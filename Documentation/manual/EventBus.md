#Event Bus
The event bus is a wrapper around Unity's messaging system that allows to call any public method on any other object without having a reference via observer pattern. Publishers and Subscribers have a n : n relationship - many publishers and many subscribers. 

##Publish
Publishes an event given an event id and a variable number of parameters.

##Register Callback
* RegisterCallback: Add a callback.
* SaveRegisterCallback: Check if there is a calback with matching signature and add a new callback if there is none.
* RegisterCallbackQueued: Put the callback inforation in a queue and register the callback later. Events to that id that are published in the same frame will not come through. 

##Deregister Callback
* DeregisterCallback: Remove a callback.
* SaveDeregisterCallback: Check if there is a callback with matching signature registered and remove it. 
* DeregisterCallbackQueued: Put the callback information in a queue and deregister the callback later. Events to that id that are published in the same frame will still be received. 

##Updater
The event bus is not a MonoBehaviour. To enable the queued (de)registration of callbacks, the current scene needs to have a [EventBusUpdater](../api/Global.EventBusUpdater.html) component somewhere. Queued (de)registration is handled in the late update function. 