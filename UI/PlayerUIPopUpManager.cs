using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ZV
{
    public class PlayerUIPopUpManager : MonoBehaviour
    {
        [Header("Message Pop Up")]
        [SerializeField] TextMeshProUGUI popUpMessageText;
        [SerializeField] GameObject popUpMessageGameObject;

        // TO DO: MAKE ONE POP UP OBJECT AND CHANGE THE TEXT VALUES AS NEEDED INSTEAD OF MAKING SEVERAL DIFFERENT GROUPS FOR POP UP FUNCTIONALITY
        [Header("YOU DIED Pop Up")]
        [SerializeField] GameObject youDiedPopUpGameObject;
        [SerializeField] TextMeshProUGUI youDiedPopUpBackgroundText;
        [SerializeField] TextMeshProUGUI youDiedPopUpText;
        [SerializeField] CanvasGroup youDiedPopUpCanvasGroup; // Allows us to set the alpha fade over time

        [Header("BOSS DEFEATED Pop Up")]
        [SerializeField] GameObject bossDefeatedPopUpGameObject;
        [SerializeField] TextMeshProUGUI bossDefeatedPopUpBackgroundText;
        [SerializeField] TextMeshProUGUI bossDefeatedPopUpText;
        [SerializeField] CanvasGroup bossDefeatedPopUpCanvasGroup; // Allows us to set the alpha fade over time

        [Header("SITE OF GRACE Pop Up")]
        [SerializeField] GameObject siteOfGracePopUpGameObject;
        [SerializeField] TextMeshProUGUI siteOfGracePopUpBackgroundText;
        [SerializeField] TextMeshProUGUI siteOfGracePopUpText;
        [SerializeField] CanvasGroup siteOfGracePopUpCanvasGroup; // Allows us to set the alpha fade over time

        public void CloseAllPopUpWindows()
        {
            popUpMessageGameObject.SetActive(false);
            PlayerUIManager.instance.popUpWindowIsOpen = false;
        }

        public void SendPlayerMessagePopUp(string messageText)
        {
            PlayerUIManager.instance.popUpWindowIsOpen = true;
            popUpMessageText.text = messageText;
            popUpMessageGameObject.SetActive(true);
        }

        public void SendYouDiedPopUp()
        {
            // ACTIVATE POST PROCESSING EFFECTS

            youDiedPopUpGameObject.SetActive(true);
            youDiedPopUpBackgroundText.characterSpacing = 0;
            StartCoroutine(StretchPopUpTextOverTime(youDiedPopUpBackgroundText, 8, 19f));
            StartCoroutine(FadeInPopUpTime(youDiedPopUpCanvasGroup, 5));
            StartCoroutine(WaitThenFadeOutPopUpOverTime(youDiedPopUpCanvasGroup, 2, 5));
        }

        public void SendBossDefeatedPopUp(string bossDefeatedMessage)
        {
            // ACTIVATE POST PROCESSING EFFECTS

            bossDefeatedPopUpText.text = bossDefeatedMessage;
            bossDefeatedPopUpBackgroundText.text = bossDefeatedMessage;
            bossDefeatedPopUpGameObject.SetActive(true);
            bossDefeatedPopUpBackgroundText.characterSpacing = 0;
            StartCoroutine(StretchPopUpTextOverTime(bossDefeatedPopUpBackgroundText, 8, 19f));
            StartCoroutine(FadeInPopUpTime(bossDefeatedPopUpCanvasGroup, 5));
            StartCoroutine(WaitThenFadeOutPopUpOverTime(bossDefeatedPopUpCanvasGroup, 2, 5));
        }

        public void SendGraceRestoredPopUp(string siteOfGraceRestoredMessage)
        {
            // ACTIVATE POST PROCESSING EFFECTS

            siteOfGracePopUpText.text = siteOfGraceRestoredMessage;
            siteOfGracePopUpBackgroundText.text = siteOfGraceRestoredMessage;
            siteOfGracePopUpGameObject.SetActive(true);
            siteOfGracePopUpBackgroundText.characterSpacing = 0;
            StartCoroutine(StretchPopUpTextOverTime(siteOfGracePopUpBackgroundText, 8, 19f));
            StartCoroutine(FadeInPopUpTime(siteOfGracePopUpCanvasGroup, 5));
            StartCoroutine(WaitThenFadeOutPopUpOverTime(siteOfGracePopUpCanvasGroup, 2, 5));
        }

        private IEnumerator StretchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount)
        {
            if(duration > 0f)
            {
                text.characterSpacing = 0; // RESETS OUR CHARACTER SPACING
                float timer = 0;

                yield return null;

                while (timer < duration)
                {
                    timer = timer +Time.deltaTime;
                    text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmount, duration * (Time.deltaTime / 20));
                    yield return null;
                }
            }
        }

        private IEnumerator FadeInPopUpTime(CanvasGroup canvas, float duration)
        {
            if(duration > 0)
            {
                canvas.alpha = 0;
                float timer = 0;

                yield return null;

                while(timer < duration)
                {
                    timer = timer + Time.deltaTime;
                    canvas.alpha = Mathf.Lerp(canvas.alpha, 1, duration * Time.deltaTime);
                    yield return null;
                }
            }

            canvas.alpha = 1;

            yield return null;
        }

        private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup canvas, float duration, float delay)
        {
            if (duration > 0)
            {
                while(delay > 0)
                {
                    delay = delay - Time.deltaTime;
                    yield return null;
                }

                canvas.alpha = 1;
                float timer = 0;

                yield return null;

                while (timer < duration)
                {
                    timer = timer + Time.deltaTime;
                    canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);
                    yield return null;
                }
            }

            canvas.alpha = 0;

            yield return null;
        }
    }
}
