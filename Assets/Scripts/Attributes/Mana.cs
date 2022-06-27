using System;
using RPG.Utils;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
	public class Mana : MonoBehaviour, ISaveable
	{
		private LazyValue<float> mana;

		private void Awake()
		{
			mana = new LazyValue<float>(GetMaxMana);
		}

		private void Update()
		{
			if (!(mana.Value < GetMaxMana())) return;
			mana.Value += GetRegenerationRate() * Time.deltaTime;
			if (mana.Value > GetMaxMana())
				mana.Value = GetMaxMana();
		}

		public float GetMana() => mana.Value;
		public float GetMaxMana() => GetComponent<BaseStats>().GetStat(Stat.Mana);
		private float GetRegenerationRate() => GetComponent<BaseStats>().GetStat(Stat.ManaRegeneration);

		public bool UseMana(float manaToUse)
		{
			if (manaToUse > mana.Value) return false;
			mana.Value -= manaToUse;
			return true;
		}
		public object CaptureState()
		{
			return mana.Value;
		}
		public void RestoreState(object state)
		{
			mana.Value = (float)state;
		}
	}
}
