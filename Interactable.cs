using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace ZV
{
    public class Interactable : NetworkBehaviour
    {
        public string interactibleText; // TEXT PROMPT WHEN ENTERING THE INTERACTION COLLIDER (PICK UP ITEM, PULL LEVER ECT)
        [SerializeField] protected Collider interactibleCollider; // COLLIDER THAT CHECKS FOR PLAYER INTERACTION
        [SerializeField] protected bool hostOnlyInteractible = true; // WHEN TRUE, OBJECT CANNOT BE INTERACTED WITH BY CO-OP PLAYERS

        protected virtual void Awake()
        {
            // CHECKS IF ITS NULL, IN SOME CASES WE MAY WANT TO MANUALLY ASSIGN A COLLIDER AS A CHILD OBJECT (DEPENDING ON INTERACTABLE)
            if (interactibleCollider == null)
                interactibleCollider = GetComponent<Collider>();
        }

        protected virtual void Start()
        {

        }

        public virtual void Interact(PlayerManager player)
        {
            Debug.Log("YOU HAVE INTERACTED");

            if (!player.IsOwner)
                return;

            // REMOVE THE INTERACTION FROM THE PLAYER
            interactibleCollider.enabled = false;
            player.playerInteractionManager.RemoveInteractionFromList(this);
            PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
        }

        public virtual void OnTriggerEnter(Collider other)
        {
            PlayerManager player = other.GetComponent<PlayerManager>();

            if (player != null)
            {
                if (!player.playerNetworkManager.IsHost && hostOnlyInteractible)
                    return;

                if (!player.IsOwner)
                    return;

                // PASS THE INTERACTION TO THE PLAYER
                player.playerInteractionManager.AddInteractionToList(this);
            }
        }

        public virtual void OnTriggerExit(Collider other)
        {
            PlayerManager player = other.GetComponent<PlayerManager>();

            if (player != null)
            {
                if (!player.playerNetworkManager.IsHost && hostOnlyInteractible)
                    return;

                if (!player.IsOwner)
                    return;

                // REMOVE THE INTERACTION FROM THE PLAYER
                player.playerInteractionManager.RemoveInteractionFromList(this);
                PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
            }
        }
    }
}
