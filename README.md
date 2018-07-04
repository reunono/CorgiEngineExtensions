## Corgi Engine Extensions
This repository contains community-created extensions for the Corgi Engine, More Mountains' bestselling platforming solution, [available on the Unity Asset Store](https://www.assetstore.unity3d.com/#!/content/26617?aid=1011lKhG). These extensions can be anything, from an alternate take on an existing Character Ability, to brand new ones, or new ways to use the engine.

## Contents
* **ActionsAndDecisions**, _by Muppo_ : a collection of actions and decisions to use with the new (v5.0) Advanced AI system. 
* **CharacterLadderExtended**, _by Keith_ : This script will allow you to reset your jump counter after attaching to a ladder. It also allows you jump from a ladder with your Run Speed if the Run Button is pressed while jumping off.
* **DialogueZoneExtended**, _by Keith_ : This will allow your AI to patrol and then stop to speak to the player, and then continue on a regular patrol.
* **DialogueZoneItems**, _by Muppo_ : A class that will enable the possibility for dialogues to request and give items.
* **KeyOperatedZonesExtended**, _by Muppo_ : allows you to set an ammount of needed keys to open whatever you add this component, the only limitation is keys must not be stackable and have to be the same ID, that's it: one key per slot on inventory.
* **LevelManagerCinemachineBased**, _by Prog-Maker_ : a level manager that makes your Cinemachine powered camera follow your character.
* **MultipleCharactersPointsOfEntryLevelManager**, _by Levrault_ : an extension of the LevelManager class to handle multiple points of entry in multiplayer games.
* **ObjectActivatedZones**, _by Keith_ : This will allow you to create zones that are activated when colliding with the specified object, similar to the way the native KeyOperatedZones are activated by a key.
* **SplitScreen**, _by Levrault_ : A couple of classes to add split screen to your game, while having the option to fuse both cameras into one if your targets are close enough. You'll need three camera in your scene, a main, a left and right. For the left and right, set the viewport rect to 0.5 to make it take only half of the screen.
* **TeleporterInstant**, _by Muppo_ : A script based on the Teleporter script to allow characters to teleport instantly.
* **WeaponDisable**, _by NAiLz_ : This component handles the disabling of weapons during both wall hanging and ladder climbing.  This is to create a more visually pleasing situation and should be supplimented in the animator with animations that include the weapon in the position the designer would like it to be in while clibming or wall hanging.

## Why aren't these in the engine directly?
Because they weren't created by Renaud, the creator of the Corgi Engine, because I want to keep the Engine simple to use and just pouring features into it (as cool as they may be) wouldn't be such a great idea, and because the Engine is meant to be extended upon, and these extensions are great examples of that.

## Do I need to pay something? Give credit?
No you don't, it's all free to use. Feel free to give credit to their creators though (they'll mention that in the class' comments if they want to.

## Are more extensions coming? Can I contribute?
Absolutely, contributions are always welcome. If you want to contribute, drop me a line using [the form on this page](http://corgi-engine.moremountains.com/corgi-engine-contact), or create a pull request on this very repository.

## Hey this extension is not working!
You'd need to contact the creator of the extension if they've put their contact info in the class somewhere. If not, not much I can do, I won't provide support for these.
