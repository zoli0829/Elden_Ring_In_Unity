using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZV
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Stamina Damage")]
    public class TakeStaminaDamageEffect : InstantCharacterEffect
    {
        public float staminaDamage;

        public override void ProcessEffect(CharacterManager character)
        {
            CalculateStaminaDamage(character);
        }

        private void CalculateStaminaDamage(CharacterManager character)
        {
            //  COMPARED THE BASE STAMINA DAMAGE AGAINST OTHER PLAYER EFFECTS/MODIFIERS
            // CHANGE THE VALUE BEFORE SUBTRACTING/ADDING IT
            //PLAY SOUND FX OR VFX DURING EFFECT

            if(character.IsOwner)
            {
                Debug.Log("Character is taking " + staminaDamage + " Stamina Damage!");
                character.characterNetworkManager.currentStamina.Value -= staminaDamage;
            }
        }
    }
}
