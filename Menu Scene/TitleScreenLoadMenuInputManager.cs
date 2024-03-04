using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZV
{
    public class TitleScreenLoadMenuInputManager : MonoBehaviour
    {
        PlayerControls playerControls;

        [Header("Title Screen Input")]
        [SerializeField] bool deleteCharacterSlot = false;

        private void Update()
        {
            if(deleteCharacterSlot)
            {
                deleteCharacterSlot = false;
                TitleScreenManager.instance.AttemtToDeleteCharacterSlot();
            }
        }

        private void OnEnable()
        {
            if(playerControls == null)
            {
                playerControls = new PlayerControls();
                playerControls.UI.X.performed += i => deleteCharacterSlot = true;
            }

            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }
    }
}
