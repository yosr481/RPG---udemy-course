using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes.UI
{
	public class ManaDisplay : MonoBehaviour
	{
		private Mana mana;

		private void Awake()
		{
			mana = GameObject.FindGameObjectWithTag("Player").GetComponent<Mana>();
		}

		private void Update()
		{
			GetComponent<Text>().text = $"Mana: {mana.GetMana():0}/{mana.GetMaxMana():0}";
		}
	}
}
