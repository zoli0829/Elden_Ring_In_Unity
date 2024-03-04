using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ZV
{
    [CreateAssetMenu(menuName = "A.I./States/Combat Stance")]
    public class CombatStanceState : AIState
    {
        // 1. Select an attack for the attack state, depending on distance and angle of target in relation to character
        // 2. process any combat logic here whilst waiting to attack (blocking, strafing, dodging etc)
        // 3. If target moves out of combat range, switch to pursue target
        // 4. If target is no longer present, switch to idle state

        [Header("Attacks")]
        public List<AICharacterAttackAction> aiCharacterAttacks; // A list of all possible attacks this character can do
        protected List<AICharacterAttackAction> potentialAttacks; // All attacks in this situation (based on angle, distance etc)
        [SerializeField] AICharacterAttackAction chosenAttack;
        [SerializeField] AICharacterAttackAction previousAttack;
        protected bool hasAttack = false;

        [Header("Combo")]
        [SerializeField] protected bool canPerformCombo = false; // If the character can perform a combo attack, after the initial attack
        [SerializeField] protected int chanceToPerformCombo = 25; // The chance (%) of the character to perform a combo on the next attack
        protected bool hasRolledForComboChance = false; // If we have already rolled ro the chance during this state

        [Header("Engagement Distance")]
        [SerializeField] public float maximumEngagementDistance = 5; // The distance whe have to be away from the target before we enter the pursue target state

        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.isPerformingAction)
                return this;

            if (!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;

            // IF WE WANT THE AI CHARACTER TO FACE AND TURN TOWARDS ITS TARGET WHEN ITS OUTSIDE ITS FOV INCLUDE THIS

            if(aiCharacter.aiCharacterCombatManager.enablePivot)
            {
                if (!aiCharacter.aiCharacterNetworkManager.isMoving.Value)
                {
                    if (aiCharacter.aiCharacterCombatManager.viewableAngle < -30 || aiCharacter.aiCharacterCombatManager.viewableAngle > 30)
                        aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
                }
            }
            
            // ROTATE AND FACE OUR TARGET
            aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

            // IF OUR TARGET IS NO LONGER PRESENT, SWITCH BACK TO IDLE 
            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);

            // IF WE DO NOT HAVE AN ATTACK, GET ONE
            if (!hasAttack)
            {
                GetNewAttack(aiCharacter);
            }
            else
            {
                aiCharacter.attack.currentAttack = chosenAttack;
                // ROLL FOR COMBO CHANCE
                // SWITCH STATE
                return SwitchState(aiCharacter, aiCharacter.attack);
            }

            // IF WE ARE OUTSIDE OF THE COMBAT ENGAGEMENT DISTANCE, SWITCH TO PURSUE TARGET STATE
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget > maximumEngagementDistance)
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);

            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);

            return this;
        }
        

        protected virtual void GetNewAttack(AICharacterManager aiCharacter)
        {
            potentialAttacks = new List<AICharacterAttackAction>();

            foreach(var potentialAttack in aiCharacterAttacks)
            {
                // IF WE ARE TOO CLOSE FOR THIS ATTACK, CHECK THE NEXT
                if (potentialAttack.minimumAttackDistance > aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                    continue;

                // IF WE ARE TOO FAR FOR THIS ATTACK, CHECK THE NEXT
                if (potentialAttack.maximumAttackDistance < aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                    continue;

                // IF THE TARGET IS OUTSIDE MINIMUM FIELD OF VIEW FOR THIS ATTACK, CHECK THE NEXT
                if (potentialAttack.minimumAttackAngle > aiCharacter.aiCharacterCombatManager.viewableAngle)
                    continue;

                // IF THE TARGET IS OUTSIDE MAXIMUM FIELD OF VIEW FOR THIS ATTACK, CHECK THE NEXT
                if (potentialAttack.maximumAttackAngle < aiCharacter.aiCharacterCombatManager.viewableAngle)
                    continue;

                potentialAttacks.Add(potentialAttack);
            }

            if (potentialAttacks.Count <= 0)
                return;

            var totalWeight = 0;

            foreach(var attack in potentialAttacks)
            {
                totalWeight += attack.attackWeight;
            }

            var randomWeightValue = Random.Range(1, totalWeight + 1);
            var processedWeight = 0;

            foreach(var attack in potentialAttacks)
            {
                processedWeight += attack.attackWeight;

                if(randomWeightValue < processedWeight)
                {
                    // THIS IS OUR ATTACK
                    chosenAttack = attack;
                    previousAttack = chosenAttack;
                    hasAttack = true;
                    return;
                }
            }
            // 1. Sort through all possible attacks
            // 2. Remove attacks that cant be used in this situation (Based on angle and distance)
            // 3. Place remaining attacks into a list
            // 4. Pick one of the remaining attacks randomly, based on weight
            // 5. Select this attack and pass it through to the attack state
        }

        protected virtual bool RollForOutcomeChance(int outcomeChance)
        {
            bool outcomeWillBePerformed = false;

            int randomPercentage = Random.Range(0, 100);

            if(randomPercentage < outcomeChance)
            {
                outcomeWillBePerformed = true;
            }

            return outcomeWillBePerformed;
        }

        protected override void ResetStateFlags(AICharacterManager aICharacter)
        {
            base.ResetStateFlags(aICharacter);

            hasAttack = false;
            hasRolledForComboChance = false;
        }
    }
}
