using UnityEngine;
using UnityEngine.U2D;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.MMInterface;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
	/// Teleporter with Pixel Perfect Camera pixel per unit custom value, allows you to simulate zooms on selected rooms (i.e. entering a little room)
    /// v1.0 / Muppo (2020)
    public class TeleporterWithZoom : Teleporter
    {
        [Header("Extra Settings")]
        public Camera _mainCamera;
        public int _pixelPerUnit;
        protected PixelPerfectCamera _pixelPerfect;
        
        ///
        protected override void Awake()
        {
            _pixelPerfect = _mainCamera.GetComponent<PixelPerfectCamera>();
            base.Awake();
        }

        ///
        protected override void TeleportCollider(Collider2D collider)
        {
            _pixelPerfect.assetsPPU = _pixelPerUnit;
            base.TeleportCollider(collider);
        }
    }
}