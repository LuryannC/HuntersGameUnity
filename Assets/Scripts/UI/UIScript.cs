using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScript : MonoBehaviour
{
   public GameObject mainMenuLayer;
   public GameObject gameLayer;
   
   [SerializeField] public float fadeDuration = 1f;

   public void OnInteraction(GameObject openingWidget)
   {
      UIWidgetsTracker widgetsTracker = openingWidget.GetComponentInParent<UIWidgetsTracker>();
      if (widgetsTracker)
      {
         FadeOut(widgetsTracker.previousOpenedWidget);
         FadeIn(openingWidget);
         widgetsTracker.previousOpenedWidget = openingWidget;
      }
   }
   
   public void FadeIn(GameObject Widget)
   {
      CanvasGroup canvasGroup = Widget.GetComponent<CanvasGroup>();
      if (canvasGroup != null)
      {
         StartCoroutine(Fade(canvasGroup, 0f, 1f));
         Widget.SetActive(true);
      }
   }

   // Coroutine for fading out the UI
   public void FadeOut(GameObject Widget)
   {
      CanvasGroup canvasGroup = Widget.GetComponent<CanvasGroup>();
      if (canvasGroup != null)
      {
         StartCoroutine(Fade(canvasGroup, 1f, 0f));
         Widget.SetActive(false);
      }
   }

   private IEnumerator Fade(CanvasGroup canvasGroup, float startAlpha, float endAlpha)
   {
      float timeElapsed = 0f;

      canvasGroup.alpha = startAlpha;

      while (timeElapsed < fadeDuration)
      {
         canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timeElapsed / fadeDuration);

         timeElapsed += Time.deltaTime;
         
         yield return null;
      }
      
      canvasGroup.alpha = endAlpha;
   }
}
