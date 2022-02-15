using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
	public class TraitStore : MonoBehaviour, IModifierProvider, ISaveable
	{
		[SerializeField] private TraitBonus[] bonusConfig;
		[Serializable]
		private class TraitBonus
		{
			public Trait trait;
			public Stat stat;
			public float additiveBonusPerPoint = 0;
			public float percentageBonusPerPoint = 0;
		}
		
		private Dictionary<Trait, int> assignedPoints = new Dictionary<Trait, int>();
		private Dictionary<Trait, int> stagedPoints = new Dictionary<Trait, int>();

		private Dictionary<Stat, Dictionary<Trait, float>> additiveBonusCache;
		private Dictionary<Stat, Dictionary<Trait, float>> percentageBonusCache;

		private void Awake()
		{
			additiveBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();
			percentageBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();
			
			foreach (var bonus in bonusConfig)
			{
				if (!additiveBonusCache.ContainsKey(bonus.stat))
				{
					additiveBonusCache[bonus.stat] = new Dictionary<Trait, float>();
				}
				if (!percentageBonusCache.ContainsKey(bonus.stat))
				{
					percentageBonusCache[bonus.stat] = new Dictionary<Trait, float>();
				}
				
				additiveBonusCache[bonus.stat][bonus.trait] = bonus.additiveBonusPerPoint;
				percentageBonusCache[bonus.stat][bonus.trait] = bonus.percentageBonusPerPoint;
			}
		}

		public int GetProposedPoints(Trait trait) => GetPoints(trait) + GetStagedPoints(trait);

		public int GetPoints(Trait trait) => assignedPoints.ContainsKey(trait) ? assignedPoints[trait] : 0;

		public int GetStagedPoints(Trait trait) => stagedPoints.ContainsKey(trait) ? stagedPoints[trait] : 0;

		public void AssignPoints(Trait trait, int points)
		{
			if(!CanAssignPoints(trait,points)) return;
			
			stagedPoints[trait] = GetStagedPoints(trait) + points;
		}

		public bool CanAssignPoints(Trait trait, int points)
		{
			if (GetStagedPoints(trait) + points < 0) return false;
			return GetUnassignedPoints() >= points;
		}

		public int GetUnassignedPoints() => GetAssignablePoints() - GetTotalProposedPoints();
		
		private int GetTotalProposedPoints() => assignedPoints.Values.Sum() + stagedPoints.Values.Sum();

		public void Commit()
		{
			foreach (var trait in stagedPoints.Keys)
			{
				assignedPoints[trait] = GetProposedPoints(trait);
			}
			stagedPoints.Clear();
		}

		public int GetAssignablePoints()
		{
			return (int)GetComponent<BaseStats>().GetStat(Stat.TotalTraitPoints);
		}
		public IEnumerable<float> GetAdditiveModifier(Stat stat)
		{
			if(!additiveBonusCache.ContainsKey(stat)) yield break;

			foreach (var trait in additiveBonusCache[stat].Keys)
			{
				float bonus = additiveBonusCache[stat][trait];
				yield return bonus * GetPoints(trait);
			}
		}
		public IEnumerable<float> GetPercentageModifier(Stat stat)
		{
			if(!percentageBonusCache.ContainsKey(stat)) yield break;

			foreach (var trait in percentageBonusCache[stat].Keys)
			{
				float bonus = percentageBonusCache[stat][trait];
				yield return bonus * GetPoints(trait);
			}
		}
		public object CaptureState()
		{
			return assignedPoints;
		}
		public void RestoreState(object state)
		{
			assignedPoints = new Dictionary<Trait, int>((IDictionary<Trait, int>)state);
		}
	}
}
