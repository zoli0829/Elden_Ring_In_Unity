using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace ZV
{
    public class SiteOfGraceInteractable : Interactable
    {
        [Header("Site of Grace Info")]
        [SerializeField] int siteOfGraceID;
        public NetworkVariable<bool> isActivated = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("VFX")]
        [SerializeField] GameObject activatedParticles;
        
        [Header("Interaction Text")]
        [SerializeField] string unactivatedInteractionText = "Restore Site of Grace";
        [SerializeField] string activatedInteractionText = "Rest";

        protected override void Start()
        {
            base.Start();

            if (IsOwner)
            {
                if (WorldSaveGameManager.instance.currentCharacterData.sitesOfGrace.ContainsKey(siteOfGraceID))
                {
                    isActivated.Value = WorldSaveGameManager.instance.currentCharacterData.sitesOfGrace[siteOfGraceID];
                }
                else
                {
                    isActivated.Value = false;
                }
            }

            if (isActivated.Value)
            {
                interactableText = activatedInteractionText;
            }
            else
            {
                interactableText = unactivatedInteractionText;
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // IF WE JOIN WHEN THE STATUS ALREADY CHANGED, WE FORCE THE ONCHANGE FUNCTION TO RUN HERE UPON JOINING
            if (!IsOwner)
            {
                OnIsActivatedChanged(false, isActivated.Value);
            }

            isActivated.OnValueChanged += OnIsActivatedChanged;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            isActivated.OnValueChanged -= OnIsActivatedChanged;
        }

        private void RestoreSiteOfGrace(PlayerManager player)
        {
            // ADDS SITE OF GRACE TO ACTIVATED SITES IN SAVE FILES
            isActivated.Value = true;
            // IF OUR SAVE FILE CONTAINS INFO ON THIS SITE OF GRACE, REMOVE IT
            if (WorldSaveGameManager.instance.currentCharacterData.sitesOfGrace.ContainsKey(siteOfGraceID)) 
            {
                WorldSaveGameManager.instance.currentCharacterData.sitesOfGrace.Remove(siteOfGraceID); 
            }

            // THEN RE-ADD IT WITH THE VALUE OF "TRUE" (IS ACTIVATED)
            WorldSaveGameManager.instance.currentCharacterData.sitesOfGrace.Add(siteOfGraceID, true);

            // PLAYS AN ANIMATION
            player.playerAnimatorManager.PlayTargetActionAnimation("Activate_Site_Of_Grace_01", true);

            // HIDE WEAPON MODELS WHILST PLAYING ANIMATION (TO DO)

            // SENDS A POP UP
            PlayerUIManager.instance.playerUIPopUpManager.SendGraceRestoredPopUp("SITE OF GRACE RESTORED");

            // ENABLES/ACTIVATES THIS SITE OF GRACE
            StartCoroutine(WaitForAnimationPopUpThenRestoreCollider());
        }

        private void RestAtSiteOfGrace(PlayerManager player)
        {
            Debug.Log("RESTING...");
            // TEMPORARY CODE SECTION
            interactibleCollider.enabled = true; // TEMPORALIY RE-ENABLING THE COLLIDER HERE UNTIL WE ADD THE MENU SO WE CAN RESPAWN MONSTERS INDEFINITELY
            player.playerNetworkManager.currentHealth.Value = player.playerNetworkManager.maxHealth.Value;
            player.playerNetworkManager.currentStamina.Value = player.playerNetworkManager.maxStamina.Value;

            // REFILL FLASKS (TO DO)
            // UPDATE/FORCE MOVE QUEST CHARACTERS (TO DO)
            // RESET MONSTERS/CHARACTER LOCATIONS
            WorldAIManager.instance.ResetAllCharacters();
        }

        private IEnumerator WaitForAnimationPopUpThenRestoreCollider()
        {
            yield return new WaitForSeconds(2); // THIS SHOULD GIVE ENOUGH TIME FOR THE ANIMATION TO PLAY AND THE POP UP BEGIN TO FADE OUT
            interactibleCollider.enabled = true;
        }

        public override void Interact(PlayerManager player)
        {
            base.Interact(player);

            if (!isActivated.Value)
            {
                RestoreSiteOfGrace(player);
            }
            else
            {
                RestAtSiteOfGrace(player);
            }
        }

        private void OnIsActivatedChanged(bool oldStatus, bool newStatus)
        {
            if (isActivated.Value)
                {
                // PLAY SOME FX HERE OR ENABLE A LIGHT TO INDICATE THIS CHECKPOINT IS ON
                if (activatedParticles != null)
                {
                    activatedParticles.SetActive(true);
                }

                interactableText = activatedInteractionText;
            }
            else
            {
                interactableText = unactivatedInteractionText;
            }
        }
    }
}
