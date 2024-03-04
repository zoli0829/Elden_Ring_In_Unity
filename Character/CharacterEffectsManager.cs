using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZV
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        // PROCESSING INSTANT EFFECTS (TAKE DAMAGE, HEAL)

        // PROCESS TIMED EFFECTS (POISON, BLEED)

        // PROCESS STATIC EFFECTS (ADDING/REMOVING BUFFS FROM TALISMANS ECT)

        CharacterManager character;

        [Header("VFX")]
        [SerializeField] GameObject bloodSplatterVFX;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
        {
            effect.ProcessEffect(character);
        }

        public void PlayBloodSplatterVFX(Vector3 contactPoint)
        {
            // IF WE MANUALLY HAVE PLACED A BLOOD SPLATTER VFX ON THIS MODEL, PLAY ITS VERSION
            if(bloodSplatterVFX != null)
            {
                GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
            // ELSE, WE USE THE DEFAULT WE HAVE ELSEWHERE
            else
            {
                GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
        }
    }
}
