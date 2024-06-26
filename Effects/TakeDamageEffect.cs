using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace ZV
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
    public class TakeDamageEffect : InstantCharacterEffect
    {
        [Header("Character Causing Damage")]
        public CharacterManager characterCausingDamage; // If the damage is caused by another characters attack it will be stored here

        [Header("Damage")]
        public float physicalDamage = 0; // (In the future will be split into "Standard", "Strike", "Slash" and "Pierce")
        public float magicDamage = 0;
        public float fireDamage = 0;
        public float lightningDamage = 0;
        public float holyDamage = 0;

        [Header("Final Damage")]
        private int finalDamageDealt = 0; // The damage the character takes after ALL calculations have been made

        [Header("Poise")]
        public float poiseDamage = 0;
        public bool poiseIsBroken = false; // If a character's poise is broken, they will be "Stunned" and play a damage animation

        // (TO DO) BUILD UPS
        // build up effect amounts

        [Header("Animation")]
        public bool playDamageAnimation = true;
        public bool manuallySelectDamageAnimation = false;
        public string damageAnimation;

        [Header("Sound FX")]
        public bool willPlayDamageSFX = true;
        public AudioClip elementalDamageSoundFX; // Used on top of regular SFX if there is elemental damage present (Magic/Fire/Lightning/Holy)

        [Header("Direction Damage Taken From")]
        public float angleHitFrom;              // USED TO DETERMINE WHAT DAMAGE ANIMATION TO PLAY (Move backwards, to the left, to the right etc)
        public Vector3 contactPoint;            // USED TO DETERMINE WHERE THE BLOOD FX INSTANTIATE

        public override void ProcessEffect(CharacterManager character)
        {
            // CHECK FOR "INVULNERABILITY"
            if (character.characterNetworkManager.isInvulnerable.Value)
                return;

            base.ProcessEffect(character);

            // IF THE CHARACTER IS DEAD, NO ADDITIONAL DAMAGE EFFECTS SHOULD BE PROCESSED
            if (character.isDead.Value)
                return;

            CalculateDamage(character);
            PlayDirectionalBasedDamageAnimation(character);
            // CHECK FOR BUILD UPS (POISON, BLEED ECT)
            PlayDamageSFX(character);
            PlayDamageVFX(character);

            // IF CHARACTER IS A.I. CHECK FOR NEW TARGET IF CHARACTER CAUSING DAMAGE IS PRESENT
        }

        private void CalculateDamage(CharacterManager character)
        {
            if (!character.IsOwner)
                return;

            if(characterCausingDamage != null)
            {
                // CHECK FOR DAMAGE MODIFIERS AND MODIFY BASE DAMAGE (Physical damage buff, elemental damage buff etc)
            }

            // CHECK CHARACTER FOR FLAT DEFENSES AND SUBTRACT THEM FROM THE DAMAGE

            // CHECK CHARACTER FOR ARMOR ABSORPTIONS, AND SUBTRACT THE PERCENTAGE FROM THE DAMAGE

            // ADD ALL DAMAGE TYPES TOGETHER, AND APPLY FINAL DAMAGE
            finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

            if(finalDamageDealt <= 0)
            {
                finalDamageDealt = 1;
            }

            character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;

            // CALCULATE POISE DAMAGE TO DETERMINE IF THE CHARACTER WILL BE STUNNED
        }

        private void PlayDamageVFX(CharacterManager character)
        {
            // IF WE HAVE FIRE DAMAGE, PLAY FIRE PARTICLES
            // LIGHTNING DAMAGE, LIGHTNING PARTICLES ECT

            character.characterEffectsManager.PlayBloodSplatterVFX(contactPoint);
        }

        private void PlayDamageSFX(CharacterManager character)
        {
            AudioClip physicalDamageSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.physicalDamageSFX);

            character.characterSoundFXManager.PlaySoundFX(physicalDamageSFX);
            character.characterSoundFXManager.PlayDamageGruntSoundFX();
            // IF FIRE DAMAGE IS GREATER THAN 0, PLAY BURN SFX
            // IF LIGHTNING DAMAGE IS GREATER THAN 0, PLAY ZAP SFX
        }

        private void PlayDirectionalBasedDamageAnimation(CharacterManager character)
        {
            if(!character.IsOwner) 
                return;

            if (character.isDead.Value)
                return;

            // TODO CALCULATE IF OUR POISE IS BROKEN
            poiseIsBroken = true;

            if(angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Medium_Damage);
            }
            else if(angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Medium_Damage);
            }
            else if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.backward_Medium_Damage);
            }
            else if (angleHitFrom >= -144 && angleHitFrom <= -45)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.left_Medium_Damage);
            }
            else if (angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.right_Medium_Damage);
            }

            // IF POISE IS BROKEN, PLAY A STAGGERING DAMAGE ANIMATION
            if(poiseIsBroken)
            {
                character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
                character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
            }
        }
    }
}
