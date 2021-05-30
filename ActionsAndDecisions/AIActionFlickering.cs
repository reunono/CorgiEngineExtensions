using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{

	public class AIActionFlickering : AIAction
	{
		// Add this component to an AI Brain and agent speed will flicker the same way when damage is taken but enless.
		// Muppo (2018)

		public Color _flickerColor;

		protected float _flickerSpeed = 0.05f; // Using higher values will work as a delay more than a real flicker
		protected Renderer _renderer;
		protected Character _character;
		protected Color _initialColor = new Color32(255, 255, 255, 255);
		protected float flickerDuration = 999999999f;

		///
	 	protected override void Initialization()
        {
			_renderer = GetComponent<SpriteRenderer>();


			_character = GetComponent<Character>();
			if (gameObject.MMGetComponentNoAlloc<SpriteRenderer>() != null)
			{
				_renderer = GetComponent<SpriteRenderer>();	
				_initialColor = GetComponent<SpriteRenderer>().color;
			}
			if (_character != null)
			{
				if (_character.CharacterModel != null)
				{
					if (_character.CharacterModel.GetComponentInChildren<Renderer>() != null)
					{
						_renderer = _character.CharacterModel.GetComponentInChildren<Renderer>();	
					}
				}	
			}
		}

		///
		public override void PerformAction()
		{
			if (_renderer != null)
			{
				StartCoroutine(MMImage.Flicker(_renderer,_initialColor,_flickerColor,_flickerSpeed,flickerDuration));
			}
		}
	}
}
