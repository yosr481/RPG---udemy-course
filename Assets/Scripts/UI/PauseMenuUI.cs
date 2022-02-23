using RPG.Control;
using RPG.SceneManagment;
using UnityEngine;

namespace RPG.UI
{
	public class PauseMenuUI : MonoBehaviour
	{
		
		private PlayerController playerController;
		private SavingWrapper savingWrapper;

		private void Awake()
		{
			playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
			savingWrapper = FindObjectOfType<SavingWrapper>();
		}
		
		private void OnEnable()
		{
			if(!playerController) return;
			Time.timeScale = 0;
			playerController.enabled = false;
		}

		private void OnDisable()
		{
			if(!playerController) return;
			Time.timeScale = 5;
			playerController.enabled = true;
		}

		public void Save()
		{
			savingWrapper.Save();
		}

		public void SaveAndQuit()
		{
			savingWrapper.Save();
			savingWrapper.LoadMainMenu();
		}
	}
}
