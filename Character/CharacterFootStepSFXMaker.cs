using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZV
{
    public class CharacterFootStepSFXMaker : MonoBehaviour
    {
        CharacterManager character;

        AudioSource audioSource;
        GameObject steppedOnObject;

        private bool hasTouchedGround = false;
        private bool hasPlayedFootStepSFX = false;
        [SerializeField] float distanceToGround = 0.05f;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            character = GetComponentInParent<CharacterManager>();
        }

        private void FixedUpdate()
        {
            CheckForFootSteps();
        }

        private void CheckForFootSteps()
        {
            if(character == null) 
                return;

            if (!character.characterNetworkManager.isMoving.Value)
                return;

            RaycastHit hit;

            if (Physics.Raycast(transform.position, character.transform.TransformDirection(Vector3.down), out hit, distanceToGround, WorldUtilityManager.instance.GetEnviroLayers()))
            { 
                hasTouchedGround = true;

                if(!hasPlayedFootStepSFX)
                    steppedOnObject = hit.transform.gameObject;
                
            }
            else
            {
                hasTouchedGround = false;
                hasPlayedFootStepSFX = false;
                steppedOnObject = null;
            }

            if(hasTouchedGround && !hasPlayedFootStepSFX)
            {
                hasPlayedFootStepSFX = true;
                PlayFootStepSFX();
            }
        }

        private void PlayFootStepSFX()
        {
            // HERE WE COULD PLAY A DIFFERENT SFX DEPENDING ON THE LAYER OF THE GROUND OR A TAG OR SUCH (SNOW, WOOD, STONE, ECT)

            // METHOD 02 (SIMPLE)
            character.characterSoundFXManager.PlayFootStepSoundFX();
        }
    }
}
