using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace ZV
{
    public class FogWallInteractible : Interactable
    {
        [Header("Fog")]
        [SerializeField] GameObject[] fogGameObjects;

        [Header("Collision")]
        [SerializeField] Collider fogWallCollider;

        [Header("ID")]
        public int fogWallID;

        [Header("Sound")]
        private AudioSource fogWallAudioSource;
        [SerializeField] AudioClip fogWallSFX;

        [Header("Active")]
        public NetworkVariable<bool> isActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        protected override void Awake()
        {
            base.Awake();

            fogWallAudioSource = gameObject.GetComponent<AudioSource>();
        }

        public override void Interact(PlayerManager player)
        {
            base.Interact(player);

            // Face the fog wall
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward);
            player.transform.rotation = targetRotation;

            // Disable Collisions With Fog Wall (on all clients, so client on one players screen doesnt get stuck and then teleport on resync)
            AllowPlayerTrhoughFogWallCollidersServerRpc(player.NetworkObjectId);

            // Walk through fog wall
            player.playerAnimatorManager.PlayTargetActionAnimation("Pass_Through_Fog_01", true); // TODO: ENABLE INVULNERABILITY TRU ANIM OR CODE

            // Reenable collisions with the fog wall
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            OnIsActiveChanged(false, isActive.Value);
            isActive.OnValueChanged += OnIsActiveChanged;
            WorldObjectManager.instance.AddFogWallToList(this);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            isActive.OnValueChanged -= OnIsActiveChanged;
            WorldObjectManager.instance.RemoveFogWallFromList(this);
        }

        private void OnIsActiveChanged(bool oldStatus, bool newStatus)
        {
            if(isActive.Value)
            {
                foreach(var fogObject in fogGameObjects)
                {
                    fogObject.SetActive(true);
                }
            }
            else
            {
                foreach (var fogObject in fogGameObjects)
                {
                    fogObject.SetActive(false);
                }
            }
        }

        // WHEN A SERVER RPC DOES NOT REQUIRE OWNERSHIP, A NON OWNER CAN ACTIVATE THE FUNCTION CALL
        [ServerRpc(RequireOwnership = false)]
        private void AllowPlayerTrhoughFogWallCollidersServerRpc(ulong playerObjectID)
        {
            if(IsServer)
            {
                AllowPlayerTrhoughFogWallCollidersClientRpc(playerObjectID);
            }
        }

        [ClientRpc]
        private void AllowPlayerTrhoughFogWallCollidersClientRpc(ulong playerObjectID)
        {
            PlayerManager player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].GetComponent<PlayerManager>();

            if(fogWallAudioSource != null)
            {
                fogWallAudioSource.PlayOneShot(fogWallSFX);
            }

            if(player != null)
            {
                StartCoroutine(DisableCollisionForTime(player));
            }
        }

        private IEnumerator DisableCollisionForTime(PlayerManager player)
        {
            // MAKE THIS FUNCTION THE SAME TIME AS THE WALKING THROUGH FOG WALL ANIMATION LENGTH
            Physics.IgnoreCollision(player.characterController, fogWallCollider, true);
            yield return new WaitForSeconds(3);
            Physics.IgnoreCollision(player.characterController, fogWallCollider, false);
        }
    }
}
