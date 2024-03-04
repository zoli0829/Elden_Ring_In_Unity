using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

namespace ZV
{
    public class WorldAIManager : MonoBehaviour
    {
        public static WorldAIManager instance;

        [Header("Characters")]
        [SerializeField] List<AICharacterSpawner> aiCharacterSpawners;
        [SerializeField] List<AICharacterManager> spawnedInCharacters;

        [Header("Bosses")]
        [SerializeField] List<AIBossCharacterManager> spawnedInBosses;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SpawnCharacter(AICharacterSpawner aiCharacterSpawner)
        {
            if(NetworkManager.Singleton.IsServer)
            {
                aiCharacterSpawners.Add(aiCharacterSpawner);
                aiCharacterSpawner.AttemptToSpawnCharacters();
            }
        }

        public void AddCharacterToSpawnedCharactersList(AICharacterManager character)
        {
            if (spawnedInCharacters.Contains(character))
                return;

            spawnedInCharacters.Add(character);

            AIBossCharacterManager bossCharacter = character as AIBossCharacterManager;

            if(bossCharacter != null)
            {
                if (spawnedInBosses.Contains(bossCharacter))
                    return;

                spawnedInBosses.Add(bossCharacter);
            }
        }

        public AIBossCharacterManager GetBossCharacterByID(int ID)
        {
            return spawnedInBosses.FirstOrDefault(boss => boss.bossID == ID);
        }

        private void DespawnAllCharacters()
        {
            foreach (var character in spawnedInCharacters)
            {
                character.GetComponent<NetworkObject>().Despawn();
            }
        }

        private void DisableAllCharacters()
        {
            // TO DO DISABLE CHARACHTER GAMEOBJECTS, SYNC DISABLED STATUS ON NETWORK
            // DISABLE GAMEOBJECTS FOR CLIENTS UPON CONNECTING, IF DISABLED STATUS IS TRUE
            // CAN BE USED TO DISABLE CHARACTERS THAT ARE FAR FROM THE PLAYERS TO SAVE MEMORY
            // CHARACTERS CAN BE SPLIT INTO AREAS (AREA_00, AREA_01, AREA_02) ECT
        }
    }
}
