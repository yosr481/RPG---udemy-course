using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private RectTransform forground;
        [SerializeField] private Canvas rootCanvas = null;

        private void Update()
        {
            if(Mathf.Approximately(health.GetFraction(), 0) || Mathf.Approximately(health.GetFraction(), 1))
            {
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;

            forground.localScale = new Vector3(health.GetFraction(), 1, 1);
        }
    }

}