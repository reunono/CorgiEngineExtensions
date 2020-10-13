## Corgi Engine Extensions
This repository contains community-created extensions for the Corgi Engine, More Mountains' bestselling platforming solution, [available on the Unity Asset Store](https://assetstore.unity.com/packages/templates/systems/corgi-engine-2d-2-5d-platformer-26617?aid=1011lKhG). These extensions can be anything, from an alternate take on an existing Character Ability, to brand new ones, or new ways to use the engine.

## Contents
* **ActionsAndDecisions**, _by Muppo_ : a collection of actions and decisions to use with the new (v5.0) Advanced AI system.
* **AICrawler**, _by Muppo_ : lets you create an AI that will crawl on surfaces and rotate accordingly
* **BGMToggler**, _by Muppo_ : a component used to change/mute background music when a player stays or exits a collider.
* **CharacterLadderExtended**, _by Keith_ : This script will allow you to reset your jump counter after attaching to a ladder. It also allows you jump from a ladder with your Run Speed if the Run Button is pressed while jumping off.
* **CharacterPushCorgiControllerExtended**, _by Morriekken_ : It's normal push ability but you can move Raycast position vertically to detect pushable object.
* **ControlFreak2Integration**, _by christougher_ : Control Freak 2 integration with the Corgi Engine.
* **CustomAutoRespawn**, _by Muppo_ : Add this to an AI and it will be allowed to respawn after the set time and activate an animation on both itself and the spawner prefab (if set).
* **DialogueZoneExtended**, _by Keith_ : This will allow your AI to patrol and then stop to speak to the player, and then continue on a regular patrol.
* **DialogueZoneItems**, _by Muppo_ : A class that will enable the possibility for dialogues to request and give items.
* **HealthGUI**, _by Nathan_ : a component you can use to display your current Health as hearts instead of as a progress bar
* **ItemDrop**, _by Quinn221_ : a class to have a random item drop when an enemy dies. Add this to your enemy death effect prefab, then drag in the items you want to drop, then fill in the drop rate.
* **JumpToJetpack**, _by DavidMBradbury_ : simulates a sort of "struggle jump"
* **KeyOperatedZonesExtended**, _by Muppo_ : allows you to set an ammount of needed keys to open whatever you add this component, the only limitation is keys must not be stackable and have to be the same ID, that's it: one key per slot on inventory.
* **LevelManagerCinemachineBased**, _by Prog-Maker_ : a level manager that makes your Cinemachine powered camera follow your character.
* **MoveAwayPlatform**, _by Muppo_ : Moving platform extension with Moving Away feature.
* **MovingPlatformExtended**, _by Muppo_ : moving platforms that teleport to their initial point once they've reached the end of their path.
* **MultiInventoryDetails**, _by men8_ : an addon to handle all active inventories on scene. See [this repo](https://github.com/men8/MultiInventoryDetails) for more info.
* **MultipleCharactersPointsOfEntryLevelManager**, _by Levrault_ : an extension of the LevelManager class to handle multiple points of entry in multiplayer games.
* **NoClip**, _by Muppo_ : add this to your player Character to be able to press F4 and toggle a no clip mode (Invulnerable, no collisions, fly mode).
* **ObjectActivatedZones**, _by Keith_ : This will allow you to create zones that are activated when colliding with the specified object, similar to the way the native KeyOperatedZones are activated by a key.
* **SimpleConditionalAction**, _by Muppo_ : Add this component to an emtpy gameobject and set a reference tag, when all the tagged objects are disabled, feedbacks and actions will be played before this script disable itself.
* **SplitScreen**, _by Levrault_ : A couple of classes to add split screen to your game, while having the option to fuse both cameras into one if your targets are close enough. You'll need three camera in your scene, a main, a left and right. For the left and right, set the viewport rect to 0.5 to make it take only half of the screen.
* **StackablePushable**, _by Morriekken_ : Modified pushable script along with CorgiController which allows stacking stacking boxes on top of each other.
* **TeleporterInstant**, _by Muppo_ : A script based on the Teleporter script to allow characters to teleport instantly.
* **TeleporterWithZoom**, _by Muppo_ : Teleporter with Pixel Perfect Camera pixel per unit custom value, allows you to simulate zooms on selected rooms (i.e. entering a little room).
* **WeaponDisable**, _by NAiLz_ : This component handles the disabling of weapons during both wall hanging and ladder climbing.  This is to create a more visually pleasing situation and should be supplimented in the animator with animations that include the weapon in the position the designer would like it to be in while climbing or wall hanging.
* **AI Brain Extensions**, _by TheBitCave_ : These are hosted separately, at [https://github.com/thebitcave/ai-brain-extensions-for-corgi-engine](https://github.com/thebitcave/ai-brain-extensions-for-corgi-engine), and provide a way to interact with AI Brains using visual nodes.

## Why aren't these in the engine directly?
Because they weren't created by Renaud, the creator of the Corgi Engine, because I want to keep the Engine simple to use and just pouring features into it (as cool as they may be) wouldn't be such a great idea, and because the Engine is meant to be extended upon, and these extensions are great examples of that.

## Do I need to pay something? Give credit?
No you don't, it's all free to use. Feel free to give credit to their creators though (they'll mention that in the class' comments if they want to).

## Are more extensions coming? Can I contribute?
Absolutely, contributions are always welcome. If you want to contribute, drop me a line using [the form on this page](http://corgi-engine.moremountains.com/corgi-engine-contact), or create a pull request on this very repository.

## Hey this extension is not working!
You'd need to contact the creator of the extension if they've put their contact info in the class somewhere. If not, not much I can do, I won't provide support for these.
