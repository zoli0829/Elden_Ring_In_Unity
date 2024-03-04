using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace ZV
{
    public class WorldObjectManager : MonoBehaviour
    {
        public static WorldObjectManager instance;

        [Header("Network Objects")]
        [SerializeField] List<NetworkObjectSpawner> networkObjectSpawners;
        [SerializeField] List<GameObject> spawnedInObjects;

        [Header("Fog Walls")]
        public List<FogWallInteractible> fogWalls;

        // 2. Spawn in those fogwalls as network objects during start of game (Must have a spawner object)
        // 3. Create general object spawner script and prefab
        // 4. When the fog walls are spawned, add them to the world fog wall list
        // 5. Grab the correct fogwall from the list on the boss manager when the boss is initilized

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            } 
            else
            {
                Destroy(gameObject);
            }   
        }

        public void SpawnObject(NetworkObjectSpawner networkObjectSpawner)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                networkObjectSpawners.Add(networkObjectSpawner);
                networkObjectSpawner.AttemptToSpawnCharacters();
            }
        }

        public void AddFogWallToList(FogWallInteractible fogWall)
        {
            if(!fogWalls.Contains(fogWall))
            {
                fogWalls.Add(fogWall);
            }
        }

        public void RemoveFogWallFromList(FogWallInteractible fogWall)
        {
            if (fogWalls.Contains(fogWall))
            {
                fogWalls.Remove(fogWall);
            }
        }
    }
}
