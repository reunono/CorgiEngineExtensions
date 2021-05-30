using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

namespace CameraUtils {
  /// <summary>
  /// SplitScreenManager toggle full screen to split screen when two
  /// player are reach a certain distance between each other
  /// When a player is leaving by the left, he's gonna be on the left camera
  /// When he leaves by the right, he's gonna be on the right camera
  ///
  /// If you got any issues, you can contact create an issues on the github folder
  /// https://github.com/Levrault/corgi-engine-dynamic-split-screen
  ///
  /// @author Levrault
  /// </summary>
  public class SplitScreenManager : MonoBehaviour {
    // Camera states
    public enum CameraBehaviors { Single, SplitScreen };
    private CameraBehaviors _cameraBehavior = CameraBehaviors.Single;

    [Header("Full Screen Camera")]
    [MMInformation("The Full Screen Camera is the main camera, the one that take all the screen.", MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]
    public Camera mainCamera;

    [Header("Split Screen Camera")]
    [MMInformation("Set the left/right camera. You will need to edit there viewport rec options to left them take half of the screen. Don't forgot to set your camera's Z offset value to -10. If you don't do it, you will have some issues on splitWarning screen mode.", MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]
    public Camera leftCamera;
    public Camera rightCamera;

    [Header("Options")]
    [MMInformation("A trigger that will set show the splitted cameras instead of the full one when the distance between the two player is greater that this value ", MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]
    public float distanceBeforeSplitting = 10f;

    // players positions
    private Transform _target1;
    private Transform _target2;

    // camera controller reference
    private SplitScreen _splitScreenLeft;
    private SplitScreen _splitScreenRight;

    private bool _isCameraNeedToBeTeleported = true;

    void Start() {
      // get left/right camera splitscreen component
      _splitScreenLeft = leftCamera.GetComponent<SplitScreen>();
      _splitScreenRight = rightCamera.GetComponent<SplitScreen>();

      // get our two players
      _target1 = LevelManager.Instance.Players[0].transform;
      _target2 = LevelManager.Instance.Players[1].transform;
    }

    void LateUpdate() {

      // calculte the distance between the players
      float distance = Vector3.Distance(_target1.localPosition, _target2.localPosition);

      // if our distance is greater then our trigger, we split the camera
      if (distance > distanceBeforeSplitting) {

        // player is leaving by the left side, so he's gonna be on the left camera
        if (_target1.localPosition.x < _target2.localPosition.x) {
          _splitScreenLeft.SetTarget(_target1);
          _splitScreenRight.SetTarget(_target2);
        } else {
          // player is leaving by the left side, so he's gonna be on the left camera
          _splitScreenLeft.SetTarget(_target2);
          _splitScreenRight.SetTarget(_target1);
          // _splitScreenRight.TeleportCameraToTarget();
        }

        // if we don't teleport the camera, we will have a laggy camera movement each time
        if (_isCameraNeedToBeTeleported) {
          _splitScreenLeft.TeleportCameraToTarget();
          _splitScreenRight.TeleportCameraToTarget();
          _isCameraNeedToBeTeleported = false;
        }

        // switch camera
        _cameraBehavior = CameraBehaviors.SplitScreen;
        ToggleCamera();
      } else if (distance <= distanceBeforeSplitting) {
        // if our distance is lower then our trigger, we go full screen
        _cameraBehavior = CameraBehaviors.Single;
        // camera will need to be teleporter again when we will switch back to split screen
        _isCameraNeedToBeTeleported = true;
        ToggleCamera();
      }
    }

    /// <summary>
    /// Enable/disable camera depending on the script states
    /// </summary>
    void ToggleCamera() {
      bool isSplitScreen = (_cameraBehavior == CameraBehaviors.SplitScreen);
      mainCamera.gameObject.SetActive(!isSplitScreen);
      leftCamera.gameObject.SetActive(isSplitScreen);
      rightCamera.gameObject.SetActive(isSplitScreen);
    }
  }
}
