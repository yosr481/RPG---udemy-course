using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health health;
        [SerializeField] RectTransform forground;
        [SerializeField] Canvas rootCanvas = null;

        void Update()
        {
            if(Mathf.Approximately(health.GetFracction(), 0) || Mathf.Approximately(health.GetFracction(), 1))
            {
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;

            forground.localScale = new Vector3(health.GetFracction(), 1, 1);
        }
    }

}