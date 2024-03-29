using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ZV
{
    public class WorldItemDatabase : MonoBehaviour
    {
        public static WorldItemDatabase instance;

        public WeaponItem unarmedWeapon;

        [Header("Weapons")]
        [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();

        // A LIST OF EVERY ITEM WE HAVE IN THE GAME
        [Header("Items")]
        private List<Item> items = new List<Item>();

        public void Awake()
        {
            if (instance == null )
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // ADD ALL OF OUR WEAPONS TO THE LIST OF ITEMS
            foreach ( var weapon in weapons )
            {
                items.Add(weapon);
            }

            // ASSIGN ALL OF OUR ITEMS A UNIQUE ID
            for (int i = 0; i < items.Count; i++)
            {
                items[i].itemID = i;
            }
        }

        public WeaponItem GetWeaponByID(int ID)
        {
            return weapons.FirstOrDefault(weapon => weapon.itemID == ID);
        }
    }
}
