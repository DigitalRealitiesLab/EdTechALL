#Interaction System
The interaction system has undergone several overhauls because the required functionality changed a lot during its development. The outcome is a very adaptable system thats working well for our current purposes. 
There are some features like Selectables and Placeholders, that are hardly used right now, but should still work as intended for future use. 

##Hand
The [Hand](../api/InteractionSystem.Hand.Hand.html) is visualized by a sphrere with a ring (looking like a Saturn planet). The and can change color and change size given an Interactable.
The hand holds a state machine for the different sizes, colours, and its visibility. 

The states also communicate with the target object and take care of hovering, interaction, and selection. 

###Hand State Invisible
[HandStateInvisible](../api/InteractionSystem.Hand.HandStateInvisible.html) is the default state. In this state the hand waits for a touch of a interactable with a finger. It changes to hand state hover when an interactable is hit.

###Hand State Hover
[HandStateHover](../api/InteractionSystem.Hand.HandStateHover.html) interacts with an Interactable on touch or returns to invisible when nothing is hit. If a selectable is hit it changes to hand state 

###Hand State Selected
[HandStateSelected](../api/InteractionSystem.Hand.HandStateSelected.html) interacts with a Selectable 

##Sensables
[Sensables](../api/InteractionSystem.Sensable.html) provide the base class of interactables and Placeholders. They are objects that are sensed by the hand. The class and its root functionality are not used that much anymore because the interaction system was originally meant to be used differently and has gone through several iterations. 

##Interactables
[Interactalbes](../api/InteractionSystem.Interactable.Interactable.html) are objects that are once interacted with. They call the method "OnInteract" on all subclasses of [AInteractionBehaviour](../api/InteractionSystem.Interactable.AInteractionBehaviour.html) attatched to this GameObject or its children. 
They do the same for [AHoverBehaviours](../api/InteractionSystem.Hoverable.AHoverBehaviour.html) where the call the methods *"HoverEnter"* and *"HoverExit"*.

There are a lot of different interaction behaviours implemented already. Most of them you can find at Assets > InteractionSystem > Scripts > InteractionBehaviours

It is very easy to add a new InteractionBehaviour by simply deriving from AInteractionBehaviour and then implementing the abstract methods. It works the same vor hover behaviours. 

##Selectables
[Selectables](../api/InteractionSystem.Selectable.Selectable.html) are a subclass of Interactables that toggle their selection status when *"OnInteract"* is called. They call the methods *"OnSelect"* and *"OnDeselect"* on all subclasses of [ASelectionBehaviour](../api/InteractionSystem.Selectable.ASelectionBehaviour.html).

Again, it is very easy to add a SelectableBehaviour by derriving from the ASelectionBehaviour and implementing the *"OnSelect"* and *"OnDeselect"* methods. 

##Placeholder
[Placeholders](../api/InteractionSystem.Placeholder.Placeholder.html) are a type of Sensable that interacts with the hand. For example, a [HandFollowBehaviour](../api/InteractionSystem.Selectable.HandFollowBehaviour.html) (subclass of ASelectionBehaviour) will ask the Hand for Placeholders in range and snap the transform of its Selectable to the position of the placeholder. 