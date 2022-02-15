using System.Security.Cryptography;
using GameDevTV.Utils;
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
		
		private void OnEnable()
		{
			foreach (Transform child in contentRoot)
			{
				Destroy(child.gameObject);
			}
			var savingWrapper = FindObjectOfType<SavingWrapper>();
			if(!savingWrapper) return;
			foreach (var save in savingWrapper.ListSaves())
			{
				GameObject buttonInstance = Instantiate(buttonPrefab, contentRoot);
				buttonInstance.GetComponentInChildren<TextMeshProUGUI>().text = save;
				var button = buttonInstance.GetComponentInChildren<Button>();
				button.onClick.AddListener(() =>
				{
					savingWrapper.LoadGame(save);
				});
			}
		}
	}
}
