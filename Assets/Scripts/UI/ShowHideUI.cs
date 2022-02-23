using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.UI
{
    public class ShowHideUI : MonoBehaviour
    {
        [SerializeField] private KeyCode toggleKey = KeyCode.Escape;
        [SerializeField] private GameObject uiContainer = null;

        // Start is called before the first frame update
        private void Start()
        {
            uiContainer.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            uiContainer.SetActive(!uiContainer.activeSelf);
        }
    }
}