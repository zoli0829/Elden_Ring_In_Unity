using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZV
{
    public class AIState : ScriptableObject
    {
        public virtual AIState Tick(AICharacterManager aiCharacter)
        {
            return this;
        }

        protected virtual AIState SwitchState(AICharacterManager aICharacter, AIState newState)
        {
            ResetStateFlags(aICharacter);
            return newState;
        }

        protected virtual void ResetStateFlags(AICharacterManager aICharacter)
        {
            // RESET ANY STATE FLAGS HERE SO WHEN YOU RETURN TO THE STATE, THEY ARE BLANK ONCE AGAIN
        }
    }
}
