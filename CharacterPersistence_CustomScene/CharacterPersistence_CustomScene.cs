using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.CorgiEngine.Custom
{
    [MMHiddenProperties("AbilityStartFeedbacks", "AbilityStopFeedbacks")]
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Persistence_CustomScene")]
    public class CharacterPersistence_CustomScene : CharacterPersistence
    {
        public string persistentSceneName;

        protected override void Initialization()
        {
            base.Initialization();

            if (AbilityAuthorized)
            {
                SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName(persistentSceneName));
            }
        }
    }
}
