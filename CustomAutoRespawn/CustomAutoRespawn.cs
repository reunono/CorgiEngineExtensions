using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    /// Add this to an AI and it will be allowed to respawn after the set time
    /// and activate an animation on both itself and the spawner prefab (if set)
    /// Intended to be used on Corgi Engine 6.1+
    /// v 1.0 (Muppo 2019)
    public class CustomAutoRespawn : AutoRespawn
    {
   		[Information("Add this to an AI and it will be allowed to respawn after the set time and activate an animation on both itself and the spawner prefab (if set)\n"+
           "Use Spawn Animation Duration and Spawner Animation Delay to fine tune animations on each animator.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]

        public bool RepositionToInitOnRespawn = true;
        public float spawnAnimationDuration;
        public GameObject SpawnerPrefab;
        public float SpawnerAnimationDelay;
        protected Character _character;
        protected CharacterHorizontalMovement _characterHMovement;
        protected float originalMovementSpeed;
        protected Animator _animatorCharacter;
        protected SpriteRenderer _spriteRender;


        protected override void Start()
        {
            base.Start();
            _character = GetComponent<Character>();
            _characterHMovement = GetComponent<CharacterHorizontalMovement>();
            originalMovementSpeed = _characterHMovement.WalkSpeed;
            _animatorCharacter = GetComponent<Animator>();
            _spriteRender = GetComponent<SpriteRenderer>();
        }
		protected override void Update()
		{
			if (_reviving)
			{
				if (_timeOfDeath + AutoRespawnDuration < Time.time)
				{
					SpawnStart();
					_reviving = false;
				}
			}
		}

        protected virtual void SpawnStart()
        {
            if (RepositionToInitOnRespawn)
            {
                this.transform.position = _initialPosition;
            }

            if (SpawnerPrefab != null)
            {
                _characterHMovement.WalkSpeed = 0.0f;
                SpawnerPrefab.GetComponent<Animator>().SetBool("Open", true);
                StartCoroutine (WaitSpawnerToEnd());
            }

            _characterHMovement.WalkSpeed = 0.0f;
            _spriteRender.enabled = true;
            _animatorCharacter.SetBool("Spawning", true);
            StartCoroutine (WaitAnimationToEnd());
        }

        protected virtual IEnumerator WaitAnimationToEnd()
        {
            yield return new WaitForSeconds(spawnAnimationDuration);
            SpawnEnd();
        }

        protected virtual void SpawnEnd()
        {
            if (GetComponent<Health>() != null)
            {
                GetComponent<Health>().Revive();
            }
            Revive ();
            _animatorCharacter.SetBool("Spawning", false);
            _characterHMovement.WalkSpeed = originalMovementSpeed;
        }

        protected virtual IEnumerator WaitSpawnerToEnd()
        {
            yield return new WaitForSeconds(SpawnerAnimationDelay);
            if (SpawnerPrefab != null)
            {
                  SpawnerPrefab.GetComponent<Animator>().SetBool("Open", false);
            }
            _characterHMovement.WalkSpeed = originalMovementSpeed;
        }
    }
}