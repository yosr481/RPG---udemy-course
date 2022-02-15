using GameDevTV.Utils;
using RPG.SceneManagment;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
	public class MainMenuUI : MonoBehaviour
	{
		[SerializeField] private TMP_InputField newGameNameText;
		private LazyValue<SavingWrapper> savingWrapper;

		private void Awake()
		{
			savingWrapper = new LazyValue<SavingWrapper>(GetSavingWrapper);
		}
		private SavingWrapper GetSavingWrapper() => FindObjectOfType<SavingWrapper>();

		public void ContinueGame()
		{
			savingWrapper.Value.ContinueGame();
		}

		public void NewGame()
		{
			savingWrapper.Value.NewGame(newGameNameText.text);
		}

		public void QuitGame()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}
