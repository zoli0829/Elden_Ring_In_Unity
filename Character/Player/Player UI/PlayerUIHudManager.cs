using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

namespace ZV
{
    public class PlayerUIHudManager : MonoBehaviour
    {
        [Header("Stat Bars")]
        [SerializeField] UI_StatBar healthBar;
        [SerializeField] UI_StatBar staminaBar;

        [Header("Quick Slots")]
        [SerializeField] Image rightWeaponQuickSlotIcon;
        [SerializeField] Image leftWeaponQuickSlotIcon;

        [Header("Boss Health Bar")]
        public Transform bossHealthBarParent;
        public GameObject bossHealthBarObject;

        public void RefreshHUD()
        {
            healthBar.gameObject.SetActive(false);
            healthBar.gameObject.SetActive(true);

            staminaBar.gameObject.SetActive(false);
            staminaBar.gameObject.SetActive(true);
        }

        public void SetNewHealthValue(int oldValue, int newValue)
        {
            healthBar.SetStat(newValue);
        }

        public void SetMaxHealthValue(int maxHealth)
        {
            healthBar.SetMaxStat(maxHealth);
        }

        public void SetNewStaminaValue(float oldValue, float newValue)
        {
            staminaBar.SetStat(Mathf.RoundToInt(newValue));
        }

        public void SetMaxStaminaValue(int maxStamina)
        {
            staminaBar.SetMaxStat(maxStamina);
        }

        public void SetRightWeaponQuickSlotIcon(int weaponID)
        {
            // IF THE DATABASE DOES NOT CONTAIN A WEAPON MATCHING THE GIVEN ID, RETURN
            WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);
            if (weapon == null)
            {
                Debug.Log("ITEM IS NULL");
                rightWeaponQuickSlotIcon.enabled = false;
                rightWeaponQuickSlotIcon.sprite = null;
                return;
            }

            if(weapon.itemIcon == null)
            {
                Debug.Log("ITEM HAS NO ICON");
                rightWeaponQuickSlotIcon.enabled = false;
                rightWeaponQuickSlotIcon.sprite = null;
                return;
            }

            // THIS IS WHERE YOU WOULD CHECK TO SEE IF YOU MEET THE ITEM REQUIREMENTS IF YOU WANT TO CREATE THE WARNING FOR NOT BEING ABLE TO WIELD IT IN THE UI

            rightWeaponQuickSlotIcon.sprite = weapon.itemIcon;
            rightWeaponQuickSlotIcon.enabled = true;
        }

        public void SetLeftWeaponQuickSlotIcon(int weaponID)
        {
            // IF THE DATABASE DOES NOT CONTAIN A WEAPON MATCHING THE GIVEN ID, RETURN
            WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);
            if (weapon == null)
            {
                Debug.Log("ITEM IS NULL");
                leftWeaponQuickSlotIcon.enabled = false;
                leftWeaponQuickSlotIcon.sprite = null;
                return;
            }

            if (weapon.itemIcon == null)
            {
                Debug.Log("ITEM HAS NO ICON");
                leftWeaponQuickSlotIcon.enabled = false;
                leftWeaponQuickSlotIcon.sprite = null;
                return;
            }

            // THIS IS WHERE YOU WOULD CHECK TO SEE IF YOU MEET THE ITEM REQUIREMENTS IF YOU WANT TO CREATE THE WARNING FOR NOT BEING ABLE TO WIELD IT IN THE UI

            leftWeaponQuickSlotIcon.sprite = weapon.itemIcon;
            leftWeaponQuickSlotIcon.enabled = true;
        }
    }
}
