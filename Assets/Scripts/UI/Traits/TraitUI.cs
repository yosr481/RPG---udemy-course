using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Traits
{
	public class TraitUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI unassignedPointsText;
		[SerializeField] private Button commitButton;
		
		private TraitStore traitStore;

		private void Start()
		{
			traitStore = GameObject.FindGameObjectWithTag("Player").GetComponent<TraitStore>();
			commitButton.onClick.AddListener(traitStore.Commit);
		}

		private void Update()
		{
			unassignedPointsText.text = traitStore.GetUnassignedPoints().ToString();
		}
	}
}
