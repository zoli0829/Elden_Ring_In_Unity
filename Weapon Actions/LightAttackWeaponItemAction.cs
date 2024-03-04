using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZV
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
    public class LightAttackWeaponItemAction : WeaponItemAction
    {
        [Header("Light Attacks")]
        [SerializeField] string light_Attack_01 = "Main_Light_Attack_01"; // Main = main hand
        [SerializeField] string light_Attack_02 = "Main_Light_Attack_02"; // Main = main hand

        [Header("Running Attack")]
        [SerializeField] string run_Attack_01 = "Main_Run_Attack_01";

        [Header("Rolling Attack")]
        [SerializeField] string roll_Attack_01 = "Main_Roll_Attack_01";

        [Header("Backstep Attack")]
        [SerializeField] string backstep_Attack_01 = "Main_Backstep_Attack_01";

        public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

            if (!playerPerformingAction.IsOwner)
                return;

            if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0)
                return;

            if (!playerPerformingAction.characterLocomotionManager.isGrounded)
                return;

            // IF WE ARE SPRINTING, PERFORM A RUNNING ATTACK
            if(playerPerformingAction.characterNetworkManager.isSprinting.Value)
            {
                PerformRunningAttack(playerPerformingAction, weaponPerformingAction);
                return;
            }

            // IF WE ARE ROLLING, PERFORM A ROLLING ATTACK
            if (playerPerformingAction.characterCombatManager.canPerformRollingAttack)
            {
                PerformRollingAttack(playerPerformingAction, weaponPerformingAction);
                return;
            }

            // IF WE ARE BACKSTEPPING, PERFORM A BACKSTEP ATTACK
            if (playerPerformingAction.characterCombatManager.canPerformBackstepAttack)
            {
                PerformBackstepAttack(playerPerformingAction, weaponPerformingAction);
                return;
            }

            PerformLightAttack(playerPerformingAction, weaponPerformingAction);
        }

        private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            // IF WE ARE ATTACKING CURRENTLY, AND WE CAN PERFORM A COMBO ATTACK
            if(playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

                // PERFORM AN ATTACK BASED ON THE PREVIOUS ATTACK WE JUST PLAYED
                if(playerPerformingAction.playerCombatManager.lastAttackAnimationPerformed == light_Attack_01)
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack02, light_Attack_02, true);
                }
                else
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true);
                }
            }
            // OTHERWISE, IF WE ARE NOT ALREADY ATTACKING JUST PERFORM A REGULAR ATTACK
            else if(!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true);
            }
        }

        private void PerformRunningAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            // IF WE ARE TWO HANDING OUR WEAPON PERFORM A TWO HAND RUN ATTACK (TO DO)
            // ELSE PERFORM A ONE HAND RUN ATTACK

            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.RunningAttack01, run_Attack_01, true);
        }

        private void PerformRollingAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            // IF WE ARE TWO HANDING OUR WEAPON PERFORM A TWO HAND RUN ATTACK (TO DO)
            // ELSE PERFORM A ONE HAND RUN ATTACK

            playerPerformingAction.playerCombatManager.canPerformRollingAttack = false;
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.RollingAttack01, roll_Attack_01, true);
        }

        private void PerformBackstepAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            // IF WE ARE TWO HANDING OUR WEAPON PERFORM A TWO HAND RUN ATTACK (TO DO)
            // ELSE PERFORM A ONE HAND RUN ATTACK

            playerPerformingAction.playerCombatManager.canPerformBackstepAttack = false;
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.BackstepAttack01, backstep_Attack_01, true);
        }
    }
}
