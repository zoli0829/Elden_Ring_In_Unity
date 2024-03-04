using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace ZV
{
    public class AIBossCharacterManager : AICharacterManager
    {
        public int bossID = 0;

        [Header("Music")]
        [SerializeField] AudioClip bossIntroClip;
        [SerializeField] AudioClip bossBattleLoopClip;

        [Header("Status")]
        public NetworkVariable<bool> bossFightIsActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> hasBeenAwakened = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> hasBeenDefeated = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        [SerializeField] List<FogWallInteractible> fogWalls;
        [SerializeField] string sleepAnimation;
        [SerializeField] string awakenAnimation;

        [Header("Phase Shift")]
        public float minimumHealthPercentageToShift = 50;
        [SerializeField] string phaseShiftAnimation = "Phase_Change_01";
        [SerializeField] CombatStanceState phase02CombatStanceState;

        [Header("States")]
        [SerializeField] BossSleepState sleepState;

        // WHEN THIS AI IS SPAWNED< CHECK OUR SAVE FILE (DICTIONARY)
        // IF THE SAVE FILE DOES NOT CONTAIN A BOSS MONSTER WITH THIS ID ADD IT
        // IF IT IS PRESENT, CHECK IF THE BOSS HAS BEEN DEFEATED
        // IF THE BOSS HAS BEEN DEFEATED, DISABLE THIS GAMEOBJECT
        // IF THE BOSS HAS NOT BEEN DEFEATED, ALLOW THIS OJECT TO CONTINUE TO BE ACTIVE

        protected override void Awake()
        {
            base.Awake();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            bossFightIsActive.OnValueChanged += OnBossFightIsActiveChanged;
            OnBossFightIsActiveChanged(false, bossFightIsActive.Value); // SO IF WE JOIN WHEN THE FIGHT IS ALREADY ACTIVE, WE WILL GET A HP BAR

            if (IsOwner)
            {
                sleepState = Instantiate(sleepState);
                currentState = sleepState;
            }

            if (IsServer)
            {
                // IF OUR SAVE DATA DOES NOT CONTAIN INFORMATION ON THIS BOSS, ADD IT NOW
                if(!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
                {
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, false );
                    WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, false );
                }
                // OTHERWISE, LOAD THE DATA THAT ALREADY EXISTS ON THIS BOSS
                else
                {
                    hasBeenDefeated.Value = WorldSaveGameManager.instance.currentCharacterData.bossesDefeated[bossID];
                    hasBeenAwakened.Value = WorldSaveGameManager.instance.currentCharacterData.bossesAwakened[bossID];
                }

                // LOCATE FOG WALL
                StartCoroutine(GetFogWallsFromWorldObjectManager());

                // IF THE BOSS HAS BEEN AWAKENED, ENABLE THE FOGWALLS
                if(hasBeenAwakened.Value)
                {
                    for(int i = 0; i < fogWalls.Count; i++)
                    {
                        fogWalls[i].isActive.Value = true;
                    }
                }

                // IF THE BOSS HAS BEEN DEFEATED, DISABLE THE FOGWALLS
                if (hasBeenDefeated.Value)
                {
                    for (int i = 0; i < fogWalls.Count; i++)
                    {
                        fogWalls[i].isActive.Value = false;
                    }

                    aiCharacterNetworkManager.isActive.Value = false;
                }
            }

            if(!hasBeenAwakened.Value)
            {
                characterAnimatorManager.PlayTargetActionAnimation(sleepAnimation, true);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            bossFightIsActive.OnValueChanged -= OnBossFightIsActiveChanged;
        }

        private IEnumerator GetFogWallsFromWorldObjectManager()
        {
            while(WorldObjectManager.instance.fogWalls.Count == 0)
                yield return new WaitForEndOfFrame();

            fogWalls = new List<FogWallInteractible>();
            foreach (var fogWall in WorldObjectManager.instance.fogWalls)
            {
                if (fogWall.fogWallID == bossID)
                {
                    fogWalls.Add(fogWall);
                }
            }
        }

        public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            PlayerUIManager.instance.playerUIPopUpManager.SendBossDefeatedPopUp("GREAT FOE FELLED");

            if (IsOwner)
            {
                characterNetworkManager.currentHealth.Value = 0;
                isDead.Value = true;
                bossFightIsActive.Value = false;

                foreach(var fogWall in fogWalls)
                {
                    fogWall.isActive.Value = false;
                }

                // RESET ANY FLAGS HERE THAT NEED TO BE RESET
                // NOTHING YET

                // IF WE ARE NOT GROUNDED, PLAY AN AERIAL DEATH ANIMATION

                if (!manuallySelectDeathAnimation)
                {
                    characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
                }

                hasBeenDefeated.Value = true;
                // IF OUR SAVE DATA DOES NOT CONTAIN INFORMATION ON THIS BOSS, ADD IT NOW
                if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
                {
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                    WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
                }
                // OTHERWISE, LOAD THE DATA THAT ALREADY EXISTS ON THIS BOSS
                else
                {
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Remove(bossID);
                    WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Remove(bossID);
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                    WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
                }

                WorldSaveGameManager.instance.SaveGame();
            }

            // PLAY SOME DEATH SFX

            yield return new WaitForSeconds(5);

            // AWARD PLAYERS WITH RUNES

            // DISABLE CHARACTER
        }

        public void WakeBoss()
        {
            if(IsOwner)
            {
                if (!hasBeenAwakened.Value)
                {
                    characterAnimatorManager.PlayTargetActionAnimation(awakenAnimation, true);
                }

                bossFightIsActive.Value = true;
                hasBeenAwakened.Value = true;
                currentState = idle;

                if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
                {
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                }
                else
                {
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Remove(bossID);
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                }

                for (int i = 0; i < fogWalls.Count; i++)
                {
                    fogWalls[i].isActive.Value = true;
                }
            }
        }

        private void OnBossFightIsActiveChanged(bool oldStatus, bool newStatus)
        {
            if (bossFightIsActive.Value)
            {
                WorldSoundFXManager.instance.PlayBossTrack(bossIntroClip, bossBattleLoopClip);

                GameObject bossHealthBar =
                Instantiate(PlayerUIManager.instance.playerUIHudManager.bossHealthBarObject, PlayerUIManager.instance.playerUIHudManager.bossHealthBarParent);

                UI_Boss_HP_Bar bossHPBar = bossHealthBar.GetComponentInChildren<UI_Boss_HP_Bar>();
                bossHPBar.EnableBossHPBar(this);
            }
            else
            {
                WorldSoundFXManager.instance.StopBossMusic();
            }
        }

        public void PhaseShift()
        {
            characterAnimatorManager.PlayTargetActionAnimation(phaseShiftAnimation, true);
            combatStance = Instantiate(phase02CombatStanceState);
            currentState = combatStance;
        }
    }
}
