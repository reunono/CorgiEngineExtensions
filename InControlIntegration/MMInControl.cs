using InControl;
 
namespace MoreMountains.Tools
{
    using UnityEngine;
 
    public class MMInControl : MoreMountains.Tools.MMSingleton<MMInControl>
    {
        public MMInputActions playerActions;
        string saveData;
        public string MMPlayerPrefsKey = "MMInControlKeyBinds";
 
        void OnEnable()
        {
            // See PlayerActions.cs for this setup.
            playerActions = MMInputActions.CreateWithDefaultBindings();
            //playerActions.Move.OnLastInputTypeChanged += ( lastInputType ) => Debug.Log( lastInputType );
 
            LoadBindings();
        }
 
 
        void OnDisable()
        {
            // This properly disposes of the action set and unsubscribes it from
            // update events so that it doesn't do additional processing unnecessarily.
            playerActions.Destroy();
        }
 
        public void SaveBindings()
        {
            saveData = playerActions.Save();
            PlayerPrefs.SetString( MMPlayerPrefsKey, saveData );
        }
 
 
        void LoadBindings()
        {
            if (PlayerPrefs.HasKey( MMPlayerPrefsKey ))
            {
                saveData = PlayerPrefs.GetString( MMPlayerPrefsKey );
                playerActions.Load( saveData );
            }
        }
 
 
        void OnApplicationQuit()
        {
            PlayerPrefs.Save();
        }
 
 
    }
 
    public class MMInputActions : PlayerActionSet
    {
        public PlayerAction Jump           ;
        public PlayerAction Swim           ;
        public PlayerAction Glide          ;
        public PlayerAction Interact       ;
        public PlayerAction Jetpack        ;
        public PlayerAction Run            ;
        public PlayerAction Dash           ;
        public PlayerAction Roll           ;
        public PlayerAction Fly            ;
        public PlayerAction Shoot          ;
        public PlayerAction SecondaryShoot ;
        public PlayerAction Reload         ;
        public PlayerAction SwitchWeapon   ;
        public PlayerAction Pause          ;
        public PlayerAction Push           ;
        public PlayerAction SwitchCharacter;
        public PlayerAction TimeControl    ;
        public PlayerAction Grab           ;
        public PlayerAction Throw          ;
        public PlayerAction Left;
        public PlayerAction Right;
        public PlayerAction Up;
        public PlayerAction Down;
        public PlayerAction DLeft;
        public PlayerAction DRight;
        public PlayerAction DUp;
        public PlayerAction DDown;
        public PlayerOneAxisAction MoveHorizontal;
        public PlayerOneAxisAction MoveVertical;
        public PlayerOneAxisAction MoveHorizontalSecondary;
        public PlayerOneAxisAction MoveVerticalSecondary;
 
 
 
        public MMInputActions()
        {
            Jump                    = CreatePlayerAction( "Player1_Jump" );
            Swim                    = CreatePlayerAction( "Player1_Swim" );
            Glide                   = CreatePlayerAction( "Player1_Glide" );
            Interact                = CreatePlayerAction( "Player1_Interact" );
            Jetpack                 = CreatePlayerAction( "Player1_Jetpack" );
            Run                     = CreatePlayerAction( "Player1_Run" );
            Dash                    = CreatePlayerAction( "Player1_Dash" );
            Roll                    = CreatePlayerAction( "Player1_Roll" );
            Fly                     = CreatePlayerAction( "Player1_Fly" );
            Shoot                   = CreatePlayerAction( "Player1_Shoot" );
            SecondaryShoot          = CreatePlayerAction( "Player1_SecondaryShoot" );
            Reload                  = CreatePlayerAction( "Player1_Reload" );
            SwitchWeapon            = CreatePlayerAction( "Player1_SwitchWeapon" );
            Pause                   = CreatePlayerAction( "Player1_Pause" );
            Push                    = CreatePlayerAction( "Player1_Push" );
            SwitchCharacter         = CreatePlayerAction( "Player1_SwitchCharacter" );
            TimeControl             = CreatePlayerAction( "Player1_TimeControl" );
            Grab                    = CreatePlayerAction( "Player1_Grab" );
            Throw                   = CreatePlayerAction( "Player1_Throw" );
 
            Left                    = CreatePlayerAction( "Move Left" );
            Right                   = CreatePlayerAction( "Move Right" );
            Up                      = CreatePlayerAction( "Move Up" );
            Down                    = CreatePlayerAction( "Move Down" );
            DLeft                   = CreatePlayerAction( "Alt Move Left");
            DRight                  = CreatePlayerAction( "Alt Move Right");
            DUp                     = CreatePlayerAction( "Alt Move Up");
            DDown                   = CreatePlayerAction( "Alt Move Down");
            MoveHorizontal          = CreateOneAxisPlayerAction(Left, Right);
            MoveVertical            = CreateOneAxisPlayerAction(Down, Up);
            MoveHorizontalSecondary = CreateOneAxisPlayerAction(DLeft, DRight);
            MoveVerticalSecondary   = CreateOneAxisPlayerAction(DDown, DUp);
        }
 
 
        public static MMInputActions CreateWithDefaultBindings()
        {
            var playerActions = new MMInputActions();
 
            // How to set up mutually exclusive keyboard bindings with a modifier key.
            // playerActions.Back.AddDefaultBinding( Key.Shift, Key.Tab );
            // playerActions.Next.AddDefaultBinding( KeyCombo.With( Key.Tab ).AndNot( Key.Shift ) );
 
            playerActions.Shoot.AddDefaultBinding          ( InputControlType.Action3 );
            playerActions.Shoot.AddDefaultBinding          ( Mouse.LeftButton );
 
            playerActions.SecondaryShoot.AddDefaultBinding (InputControlType.RightBumper);
 
            playerActions.SwitchCharacter.AddDefaultBinding(InputControlType.Action4);
 
            playerActions.Jump.AddDefaultBinding           ( Key.Space );
            playerActions.Jump.AddDefaultBinding           ( InputControlType.Action1 );
 
            playerActions.Up.AddDefaultBinding             ( Key.W );
            playerActions.Down.AddDefaultBinding           ( Key.S );
            playerActions.Left.AddDefaultBinding           ( Key.A );
            playerActions.Right.AddDefaultBinding          ( Key.D );
 
            playerActions.Run.AddDefaultBinding            (Key.Shift);
            playerActions.Run.AddDefaultBinding            (InputControlType.Action3);
 
            playerActions.Pause.AddDefaultBinding          (Key.Escape);
            playerActions.Pause.AddDefaultBinding          (InputControlType.Start);
            playerActions.Grab.AddDefaultBinding           (Key.Tab);
            playerActions.Throw.AddDefaultBinding          (InputControlType.Back);
 
            playerActions.Left.AddDefaultBinding           ( InputControlType.LeftStickLeft );
            playerActions.Right.AddDefaultBinding          ( InputControlType.LeftStickRight );
            playerActions.Up.AddDefaultBinding             ( InputControlType.LeftStickUp );
            playerActions.Down.AddDefaultBinding           ( InputControlType.LeftStickDown );
 
            playerActions.DLeft.AddDefaultBinding          ( InputControlType.DPadLeft );
            playerActions.DRight.AddDefaultBinding         ( InputControlType.DPadRight );
            playerActions.DUp.AddDefaultBinding            ( InputControlType.DPadUp );
            playerActions.DDown.AddDefaultBinding          ( InputControlType.DPadDown );
 
 
            playerActions.ListenOptions.IncludeUnknownControllers = true;
            playerActions.ListenOptions.MaxAllowedBindings = 2;
            playerActions.ListenOptions.MaxAllowedBindingsPerType = 2;
            playerActions.ListenOptions.AllowDuplicateBindingsPerSet = true;
            // playerActions.ListenOptions.UnsetDuplicateBindingsOnSet = true;
            playerActions.ListenOptions.IncludeMouseButtons = true;
            playerActions.ListenOptions.IncludeMouseButtons = true;
            playerActions.ListenOptions.IncludeMouseScrollWheel = true;
            // playerActions.ListenOptions.IncludeModifiersAsFirstClassKeys = true;
 
            playerActions.ListenOptions.OnBindingFound = ( action, binding ) => {
                if (binding == new KeyBindingSource( Key.Escape ))
                {
                    action.StopListeningForBinding();
                    return false;
                }
                return true;
            };
 
            playerActions.ListenOptions.OnBindingAdded += ( action, binding ) => {
                Debug.Log( "Binding added... " + binding.DeviceName + ": " + binding.Name );
            };
 
            playerActions.ListenOptions.OnBindingRejected += ( action, binding, reason ) => {
                Debug.Log( "Binding rejected... " + reason );
            };
 
            return playerActions;
        }
    }
 
}
 
 