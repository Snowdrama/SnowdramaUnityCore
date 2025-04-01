The SceneManager is automatically boostrapped for you so you don't need to do anything to set that up

To Set up transitions first create a "RequiredScene" similar to the one provided.

Then make sure that it's added to the build list by going to File->Build Settings annd making sue the required scene as well as any scenes you want to transition to are in the list.

The RequiredScene should have a TransitionDriver on a GameObject and that GameObject should have a child canvas that contains Transitions.

Check out the "FadeImageTransition.cs" script to see an example of how to make your own transitions.

Next Create or copy the Sample RequiredSceneListObject into the Resources folder. In this case the only Required scene is the one containing the TransitionDriver and Canvas. 

Finally you'll need to create and define SceneTransitionObjects by right clicking and going to Snowdrama->Transitions->Scene Transition

The Transition object will let you make a list of scenes to load, choose if they're loaded additively, wether to reload them if they're already loaded, as well as define Allowed transitions(for example if you wanted a battle transition to play when entering a random encounter vs a house enter transition)

Now all you need to do is for your script or object or button to reference the SceneTransitionObject scriptable object and call `TransitionToThis()` and you will start a transition. 

Check out the sample scenes that have UI buttons with onClick components to see an example of calling the function. 