using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

namespace CameraUtils
{
    /// <summary>
    /// Calcule when player are two far away from each other and create a split screen.
    /// When they get closer to each other, we go back to a full screen
    /// </summary>
    public class SplitScreenManager : MonoBehaviour
    {
        public enum CameraBehaviors { Single, SplitScreen };
        [Header("Full Screen Camera")]
        [Information("The Full Screen Camera is the camera that take all the screen.", MoreMountains.Tools.InformationAttribute.InformationType.Info, false)]
        public Camera MainCamera;
        [Header("Split Screen Camera")]
        [Information("Set your left and right camera. Will display when player get a distance greater than the distance before splitting option.", MoreMountains.Tools.InformationAttribute.InformationType.Info, false)]
        public Camera LeftCamera;
        public Camera RightCamera;
        [Header("Option")]
        [Information("When camera should be splitted", MoreMountains.Tools.InformationAttribute.InformationType.Info, false)]
        public float DistanceBeforeSplitting = 10f;
        private CameraBehaviors _CameraBehavior = CameraBehaviors.Single;
        private Transform _target1;
        private Transform _target2;
        private SplitScreen _splitScreenLeft;
        private SplitScreen _splitScreenRight;

        // Use this for initialization
        void Start()
        {
            _splitScreenLeft = LeftCamera.GetComponent<SplitScreen>();
            _splitScreenRight = RightCamera.GetComponent<SplitScreen>();
            _target1 = LevelManager.Instance.Players[0].transform;
            _target2 = LevelManager.Instance.Players[1].transform;
        }

        void LateUpdate()
        {
            float distance = Vector3.Distance(_target1.localPosition, _target2.localPosition);

            if (distance > DistanceBeforeSplitting)
            {
                _CameraBehavior = CameraBehaviors.SplitScreen;
                // going left
                if (_target1.localPosition.x < _target2.localPosition.x)
                {
                    _splitScreenLeft.SetTarget(_target1);
                    _splitScreenRight.SetTarget(_target2);
                }
                else
                { // going right
                    _splitScreenLeft.SetTarget(_target2);
                    _splitScreenRight.SetTarget(_target1);
                }

            }
            else
            {
                _CameraBehavior = CameraBehaviors.Single;
            }

            ToggleCamera();
        }

        /// <summary>
        /// Enable/disable camera depending on
        /// the display mode
        /// </summary>
        void ToggleCamera()
        {
            bool isSplitScreen = (_CameraBehavior == CameraBehaviors.SplitScreen);
            MainCamera.gameObject.SetActive(!isSplitScreen);
            LeftCamera.gameObject.SetActive(isSplitScreen);
            RightCamera.gameObject.SetActive(isSplitScreen);
        }
    }
}