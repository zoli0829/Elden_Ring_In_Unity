using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZV
{
    public class PlayerInteractionManager : MonoBehaviour
    {
        PlayerManager player;

        [SerializeField] private List<Interactable> currentInteractibleActions; 

        private void Awake()
        {
            player = GetComponent<PlayerManager>();
        }

        private void Start()
        {
            currentInteractibleActions = new List<Interactable>();
        }

        private void FixedUpdate()
        {
            if (!player.IsOwner)
                return;

            // IF OUR UI MENU IS NOT OPEN, AND WE DONT HAVE A POP UP, (CURRENT INTERACTION MESSAGE) CHECK FOR INTERACTABLE
            if (!PlayerUIManager.instance.menuWindowIsOpen && !PlayerUIManager.instance.popUpWindowIsOpen)
                CheckForInteractable();
        }

        private void CheckForInteractable()
        {
            if (currentInteractibleActions.Count == 0)
                return;

            if (currentInteractibleActions[0] == null)
            {
                currentInteractibleActions.RemoveAt(0); // IF THE CURRENT INTERACTABLE ITEM AT POSTION 0 BECOMES NULL (REMOVED FROM GAME), WE REMOVE POSITION 0 FROM THE LIST
                return;
            }

            // IF WE HAVE AN INTERACTABLE ACTION AND HAVE NOT NOTIFIED OUR PLAYER, WE DO SO HERE
            if (currentInteractibleActions[0] != null)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendPlayerMessagePopUp(currentInteractibleActions[0].interactibleText);
            }
        }

        private void RefreshInteractionList()
        {
            for (int i = currentInteractibleActions.Count - 1; i > -1; i--)
            {
                if (currentInteractibleActions[i] == null)
                    currentInteractibleActions.RemoveAt(i);
            }
        }

        public void AddInteractionToList(Interactable interactableObject)
        {
            RefreshInteractionList();
            if (!currentInteractibleActions.Contains(interactableObject))
                currentInteractibleActions.Add(interactableObject);
        }

        public void RemoveInteractionFromList(Interactable interactableObject)
        {
            if (currentInteractibleActions.Contains(interactableObject))
                currentInteractibleActions.Remove(interactableObject);

            RefreshInteractionList();
        }

        public void Interact()
        {
            if (currentInteractibleActions.Count == 0)
                return;

            if (currentInteractibleActions[0] != null)
            {
                currentInteractibleActions[0].Interact(player);
                RefreshInteractionList();
            }
        }
    }
}
