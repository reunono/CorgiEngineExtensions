using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

namespace CameraUtils {
  /// <summary>
  /// Extends corgi's camera controller just to set the target
  /// </summary>
  public class SplitScreen : MoreMountains.CorgiEngine.CameraController
  {
    /// <summary>
    /// Set new target for camera
    /// </summary>
    /// <param name="target"></param>
    public override void SetTarget(Transform target) 
    {
      Target = target;

      if (Target.GetComponent<CorgiController>() == null) {
        Debug.LogWarning("CameraController : The Player character doesn't have a CorgiController associated to it, the Camera won't work.");
        return;
      }

      TargetController = target.GetComponent<CorgiController>();
    }
  }
}
