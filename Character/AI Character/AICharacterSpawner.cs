using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace ZV
{
    public class AICharacterSpawner : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] GameObject characterGameObject;
        [SerializeField] GameObject instantiatedGameObject;

        private void Awake()
        {
            WorldAIManager.instance.SpawnCharacter(this);
        }

        private void Start()
        {
            
            gameObject.SetActive(false);
        }

        public void AttemptToSpawnCharacters()
        {
            if(characterGameObject != null)
            {
                instantiatedGameObject = Instantiate(characterGameObject);
                instantiatedGameObject.transform.position = transform.position;
                instantiatedGameObject.transform.rotation = transform.rotation;
                instantiatedGameObject.GetComponent<NetworkObject>().Spawn();
                WorldAIManager.instance.AddCharacterToSpawnedCharactersList(instantiatedGameObject.GetComponent<AICharacterManager>());
            }
        }
    }
}
