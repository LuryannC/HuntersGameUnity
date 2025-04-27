using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWidgetsTracker : MonoBehaviour
{
   public GameObject defaultWidget;
   public GameObject previousOpenedWidget;

   private void Start()
   {
      if (previousOpenedWidget == null)
      {
         previousOpenedWidget = defaultWidget;
      }
   }
}
