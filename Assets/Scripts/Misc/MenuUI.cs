using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Refugee.Misc{
    class MenuUI : MonoBehaviour{
        [SerializeField]
        private CanvasGroup CampSettings;
        [SerializeField]
        private CanvasGroup Formula1Settings;
        [SerializeField]
        private CanvasGroup Formula2Settings;
        [SerializeField]
        private CanvasGroup MapSettings;

        [SerializeField]
        private Button CampSettingsButton;
        [SerializeField]
        private Button Formula1SettingsButton;
        [SerializeField]
        private Button Formula2SettingsButton;
        [SerializeField]
        private Button MapSettingsButton;

        [SerializeField]
        private Text Label;

        private void Start() {
            OnCampClicked();
        }

        public void OnCampClicked() {
            CampSettingsButton.interactable = false;
            Formula1SettingsButton.interactable = true;
            Formula2SettingsButton.interactable = true;
            MapSettingsButton.interactable = true;

            CampSettings.alpha = 1;
            CampSettings.interactable = true;
            CampSettings.blocksRaycasts = true;

            Formula1Settings.alpha = 0;
            Formula1Settings.interactable = false;
            Formula1Settings.blocksRaycasts = false;

            Formula2Settings.alpha = 0;
            Formula2Settings.interactable = false;
            Formula2Settings.blocksRaycasts = false;

            MapSettings.alpha = 0;
            MapSettings.interactable = false;
            MapSettings.blocksRaycasts = false;

            Label.text = "Camp";
        }

        public void OnFormula1Clicked() {
            CampSettingsButton.interactable = true;
            Formula1SettingsButton.interactable = false;
            Formula2SettingsButton.interactable = true;
            MapSettingsButton.interactable = true;

            CampSettings.alpha = 0;
            CampSettings.interactable = false;
            CampSettings.blocksRaycasts = false;

            Formula1Settings.alpha = 1;
            Formula1Settings.interactable = true;
            Formula1Settings.blocksRaycasts = true;

            Formula2Settings.alpha = 0;
            Formula2Settings.interactable = false;
            Formula2Settings.blocksRaycasts = false;

            MapSettings.alpha = 0;
            MapSettings.interactable = false;
            MapSettings.blocksRaycasts = false;

            Label.text = "Main Formula";
        }

        public void OnFormula2Clicked() {
            CampSettingsButton.interactable = true;
            Formula1SettingsButton.interactable = true;
            Formula2SettingsButton.interactable = false;
            MapSettingsButton.interactable = true;

            CampSettings.alpha = 0;
            CampSettings.interactable = false;
            CampSettings.blocksRaycasts = false;

            Formula1Settings.alpha = 0;
            Formula1Settings.interactable = false;
            Formula1Settings.blocksRaycasts = false;

            Formula2Settings.alpha = 1;
            Formula2Settings.interactable = true;
            Formula2Settings.blocksRaycasts = true;

            MapSettings.alpha = 0;
            MapSettings.interactable = false;
            MapSettings.blocksRaycasts = false;

            Label.text = "Obstacles & Water";
        }

        public void OnMapClicked() {
            CampSettingsButton.interactable = true;
            Formula1SettingsButton.interactable = true;
            Formula2SettingsButton.interactable = true;
            MapSettingsButton.interactable = false;

            CampSettings.alpha = 0;
            CampSettings.interactable = false;
            CampSettings.blocksRaycasts = false;

            Formula1Settings.alpha = 0;
            Formula1Settings.interactable = false;
            Formula1Settings.blocksRaycasts = false;

            Formula2Settings.alpha = 0;
            Formula2Settings.interactable = false;
            Formula2Settings.blocksRaycasts = false;

            MapSettings.alpha = 1;
            MapSettings.interactable = true;
            MapSettings.blocksRaycasts = true;

            Label.text = "Map";
        }

    }
}
