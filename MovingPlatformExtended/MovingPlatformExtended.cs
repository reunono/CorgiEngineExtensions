using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    public class MovingPlatformExtended : MovingPlatform
    {
        [Header("Repeat Loop")]
        [MMInformation("Select '<b>Only Once</b>' then check this and the platform will respawn at the first point when last one is reached.", MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]
        public bool Repeat = false;


        protected override void Update()
        {
            if (PathElements == null
                || PathElements.Count < 1
                || _endReached
                || !CanMove)
            {
                if (_endReached && Repeat)
                {
                    Initialization();
                }
                else
                {
                    return;
                }
            }
            Move();
        }
    }
}