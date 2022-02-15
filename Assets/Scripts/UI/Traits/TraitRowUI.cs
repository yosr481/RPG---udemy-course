using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RPG.Stats;

namespace RPG.UI.Traits
{
	public class TraitRowUI : MonoBehaviour
	{
		[SerializeField] private Trait trait;
		[SerializeField] private TextMeshProUGUI traitValueText;
		[SerializeField] private Button minusButton;
		[SerializeField] private Button plusButton;

		private TraitStore traitStore;

		private void Start()
		{
			traitStore = GameObject.FindGameObjectWithTag("Player").GetComponent<TraitStore>();
			minusButton.onClick.AddListener(() => Allocate(-1));
			plusButton.onClick.AddListener(() => Allocate(+1));
		}

		private void Update()
		{
			minusButton.interactable = traitStore.CanAssignPoints(trait, -1);
			plusButton.interactable = traitStore.CanAssignPoints(trait, +1);
			traitValueText.text = traitStore.GetProposedPoints(trait).ToString();
		}

		public void Allocate(int points)
		{
			traitStore.AssignPoints(trait, points);
		}
	}
}
