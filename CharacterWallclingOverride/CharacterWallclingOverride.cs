using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
    /// Wallclinging ability override to allow player wallcling on every wall or just overriden ones
    /// Classic: Player can wallcling on every wall unless it have an override set on False. Reverse: Player can wallcling only on walls with an override set as True
    /// Also allows player to preserve their slow factor or get the override one.
    /// Intended for Corgi v7.0+
    /// v1.0 / muppo - 2021

    public class CharacterWallclingOverride : CharacterWallClinging
    {
        public override string HelpBoxText() { return "Classic: Player can wallcling on every wall unless it have an override set on False. " +
                "Reverse: Player can wallcling only on walls with an override set as True. Also allows player to preserve their slow factor or get the override one."; }

        public enum WallclingType { Reverse, Classic }
        public WallclingType wallClingType = WallclingType.Reverse;
        public bool preserveSlowFallFactor;

        protected override void EnterWallClinging()
        {
            // we check for an override
            if (_controller.CurrentWallCollider != null)
            {
                _wallClingingOverride = _controller.CurrentWallCollider.gameObject.MMGetComponentNoAlloc<WallClingingOverride>();
            }
            else if (_raycast.collider != null)
            {
                _wallClingingOverride = _raycast.collider.gameObject.MMGetComponentNoAlloc<WallClingingOverride>();
            }


            switch(wallClingType)
            {
                case WallclingType.Reverse:

                    if (_wallClingingOverride != null)
                    {
                        CheckOverride();

                        if ((_movement.CurrentState != CharacterStates.MovementStates.WallClinging) && !_startFeedbackIsPlaying)
                        { PlayAbilityStartFeedbacks(); }

                        _movement.ChangeState(CharacterStates.MovementStates.WallClinging);
                    }

                    break;


                case WallclingType.Classic:
                    if (_wallClingingOverride != null)
                    {
                        CheckOverride();
                    }
                    else
                    {
                        _controller.SlowFall(WallClingingSlowFactor);
                    }

                    if ((_movement.CurrentState != CharacterStates.MovementStates.WallClinging) && !_startFeedbackIsPlaying)
                    { PlayAbilityStartFeedbacks(); }

                    _movement.ChangeState(CharacterStates.MovementStates.WallClinging);

                    break;
            }
        }

        protected virtual void CheckOverride()
        {
            // if we can't wallcling to this wall, we do nothing and exit
            if (!_wallClingingOverride.CanWallClingToThis)
            { return; }

            if (!preserveSlowFallFactor)
            {
                _controller.SlowFall(_wallClingingOverride.WallClingingSlowFactor);
            }
            else
            {
                _controller.SlowFall(WallClingingSlowFactor);
            }
        }
    }
}