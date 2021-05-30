I was able to add InControl support to the Corgi Engine by way of the following:

These steps should be OK for stock Input setups if you haven't deviated from Corgi's default inputs.

- First, I had to add InControl's Assembly Reference to the MoreMountains.CorgiEngine Assembly Reference so that the InputManager could access InControl's functions.
- Then I added MMInControl.cs -- this is a component I made based off of MMSingleton.
- InControl users can then drag one instance of MMInControl onto a GameObject within their scene. I recommend sticking it on the InControl Manager for organizational purposes.
- The MM Player Prefs Key value is the name of the PlayerPrefs registry key that it will attempt to save/restore from.
I had to make a few edits to InputManager.cs. My edits are all encapuslated within #if USE_INCONTROL preprocs

Check the initial post (with images) at https://forum.unity.com/threads/released-corgi-engine-complete-2d-2-5d-platformer-new-v6-6-ability-split-recoil-hitscan.286289/page-152#post-7187170