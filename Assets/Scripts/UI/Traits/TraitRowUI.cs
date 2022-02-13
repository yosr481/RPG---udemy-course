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

		private int value = 0;

		private void Start()
		{
			minusButton.onClick.AddListener(() => Allocate(-1));
			plusButton.onClick.AddListener(() => Allocate(+1));
		}

		private void Update()
		{
			minusButton.interactable = value > 0;
			traitValueText.text = value.ToString();
		}

		public void Allocate(int points)
		{
			value += points;
			if (value < 0)
			{
				value = 0;
			}
		}
	}
}
