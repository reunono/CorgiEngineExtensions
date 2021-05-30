using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager {
    public class MultipleCharactersPointsOfEntryLevelManager : MoreMountains.CorgiEngine.LevelManager {

        /// <summary>
        /// Spawns multiple playable characters into the scene
        /// </summary>
        protected override void SpawnMultipleCharacters () {
            PointsOfEntryStorage point = GameManager.Instance.GetPointsOfEntry (SceneManager.GetActiveScene ().name);
            int checkpointCounter = 0;
            int characterCounter = 1;
            bool spawned = false;

            foreach (Character player in Players) {

                if (AutoAttributePlayerIDs) {
                    player.SetPlayerID ("Player" + characterCounter);
                }

                player.name += " - " + player.PlayerID;

                // entry point spawn
                if (point != null) {
                    Players[characterCounter - 1].RespawnAt (PointsOfEntry[point.PointOfEntryIndex].Position, Character.FacingDirections.Right);
                    spawned = true;
                    characterCounter++;
                }

                if (!spawned && Checkpoints.Count > checkpointCounter + 1) {
                    if (Checkpoints[checkpointCounter] != null) {
                        Checkpoints[checkpointCounter].SpawnPlayer (player);
                        characterCounter++;
                        spawned = true;
                        checkpointCounter++;
                    }
                }

                if (!spawned) {
                    Checkpoints[checkpointCounter].SpawnPlayer (player);
                    characterCounter++;
                }
            }
        }
    }
}
