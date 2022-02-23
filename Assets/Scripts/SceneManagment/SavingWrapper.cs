using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagment
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] private float fadeInTime = 0.3f;
        [SerializeField] private float fadeOutTime = 0.2f;
        [SerializeField] private int FirstLevelBuildIndex = 1;
        [SerializeField] private int menuLevelBuildIndex = 0;

        private const string CurrentSaveKey = "currentSaveName";
        
        public void ContinueGame()
        {
            if (!PlayerPrefs.HasKey(CurrentSaveKey)) return;
            if(!GetComponent<SavingSystem>().SaveFileExist(GetCurrentSave())) return;
            StartCoroutine(LoadLastScene());
        }

        public void NewGame(string saveFile)
        {
            if(string.IsNullOrEmpty(saveFile)) return;
            SetCurrentSave(saveFile);
            StartCoroutine(LoadFirstScene());
        }

        public void LoadGame(string saveFile)
        {
            SetCurrentSave(saveFile);
            ContinueGame();
        }
        
        public void LoadMainMenu()
        {
            StartCoroutine(LoadMenuScene());
        }

        public void DeleteSave(string saveFile)
        {
            GetComponent<SavingSystem>().Delete(saveFile);
        }

        private void SetCurrentSave(string saveFile)
        {
            PlayerPrefs.SetString(CurrentSaveKey, saveFile);
        }

        private string GetCurrentSave() => PlayerPrefs.GetString(CurrentSaveKey);

        private IEnumerator LoadFirstScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            yield return SceneManager.LoadSceneAsync(FirstLevelBuildIndex);
            yield return fader.FadeIn(fadeInTime);
        }
        
        private IEnumerator LoadMenuScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            yield return SceneManager.LoadSceneAsync(menuLevelBuildIndex);
            yield return fader.FadeIn(fadeInTime);
        }

        private IEnumerator LoadLastScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            yield return GetComponent<SavingSystem>().LoadLastScene(GetCurrentSave());
            yield return fader.FadeIn(fadeInTime);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(GetCurrentSave());
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(GetCurrentSave());
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(GetCurrentSave());
        }

        public IEnumerable<string> ListSaves()
        {
            return GetComponent<SavingSystem>().ListSaves();
        }
    }
}
