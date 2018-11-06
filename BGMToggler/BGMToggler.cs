using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	[RequireComponent(typeof(BoxCollider2D))]

	/// Add this component to an empty object and it will add a collider which will change/mute background music when a player stay on collider or exit it.
	/// v1.0 / Muppo (2018)

	public class BGMToggler : BackgroundMusic
	{
		public enum SoundModes { ChangeMusic, ChangeVolume, Mute, MuteAndRestart }

		// Set music for this area defined by the collider
		public AudioClip thisAreaBGM;
		// Set volume on this area
		[Range(0,1)]
		public float newVolume;
		// And the type of modifier
		public SoundModes MuteType = SoundModes.ChangeVolume;
		
		protected float defaultVolume;
		protected AudioClip defaultBGM;

		///
		protected override void Start()
		{
			_source = gameObject.AddComponent<AudioSource>() as AudioSource;	
			_source.playOnAwake = true; // need it this way in order to play the swapped music
			_source.spatialBlend = 0;
			_source.rolloffMode = AudioRolloffMode.Logarithmic;
			_source.loop = true;

			_source.clip = SoundClip;
			SoundManager.Instance.PlayBackgroundMusic(_source);

			defaultVolume = _source.volume;
			defaultBGM = SoundClip;
		}

		/// Actions when player enters the zone
		public virtual void OnTriggerEnter2D(Collider2D other)
		{
			if(other.CompareTag("Player"))
			{
				if (MuteType == SoundModes.ChangeMusic)
				{
					if(thisAreaBGM != null)
					{
						_source.clip = thisAreaBGM;
						SoundManager.Instance.PlayBackgroundMusic(_source);
					}
				}

				if (MuteType == SoundModes.ChangeVolume)
				{ _source.volume = newVolume; }

				if (MuteType == SoundModes.Mute)
				{ _source.mute = true; }

				if (MuteType == SoundModes.MuteAndRestart)
				{ _source.enabled = false; }
			}
		}

		/// Actions when player leaves the zone
		public virtual void OnTriggerExit2D(Collider2D other)
		{
			if(other.CompareTag("Player"))
			{
				if (MuteType == SoundModes.ChangeMusic)
				{
					if(defaultBGM != null)
					{
						_source.clip = defaultBGM;
						SoundManager.Instance.PlayBackgroundMusic(_source);
					}
				}

				if (MuteType == SoundModes.ChangeVolume)
				{ _source.volume = defaultVolume; }

				if (MuteType == SoundModes.Mute)
				{ _source.mute = false; }

				if (MuteType == SoundModes.MuteAndRestart)
				{ _source.enabled = true; }
			}
		}
	}
}