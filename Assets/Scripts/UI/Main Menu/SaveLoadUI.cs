using RPG.SceneManagment;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
	public class SaveLoadUI : MonoBehaviour
	{
		[SerializeField] private Transform contentRoot;
		[SerializeField] private GameObject buttonPrefab;
		[SerializeField] private Sprite unselectedButtonSprite;
		[SerializeField] private Sprite selectedButtonSprite;

		private string currentSaveFile;
		
		private void OnEnable()
		{
			UpdateUI();
		}
		private void UpdateUI()
		{

			foreach (Transform child in contentRoot)
			{
				Destroy(child.gameObject);
			}
			var savingWrapper = FindObjectOfType<SavingWrapper>();
			if (!savingWrapper) return;
			foreach (var save in savingWrapper.ListSaves())
			{
				GameObject buttonInstance = Instantiate(buttonPrefab, contentRoot);
				buttonInstance.GetComponentInChildren<TextMeshProUGUI>().text = save;
				var button = buttonInstance.GetComponentInChildren<Button>();

				button.onClick.AddListener(() =>
				{
					foreach (Transform child in contentRoot)
					{
						child.GetComponent<Image>().sprite = unselectedButtonSprite;
					}
					button.GetComponent<Image>().sprite = selectedButtonSprite;
					currentSaveFile = save;
				});
			}
		}

		public void Load()
		{
			if(string.IsNullOrEmpty(currentSaveFile)) return;
			var savingWrapper = FindObjectOfType<SavingWrapper>();
			savingWrapper.LoadGame(currentSaveFile);
		}

		public void Delete()
		{
			if(string.IsNullOrEmpty(currentSaveFile)) return;
			var savingWrapper = FindObjectOfType<SavingWrapper>();
			savingWrapper.DeleteSave(currentSaveFile);
			UpdateUI();
		}
	}
}
