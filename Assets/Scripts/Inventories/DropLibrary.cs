using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RPG.Inventories
{
	[CreateAssetMenu(fileName = "New Drop Library", menuName = "Inventory/Drop Library", order = 0)]
	public class DropLibrary : ScriptableObject
	{
		[SerializeField] private DropConfig[] potentialDrops;
		[SerializeField] private float[] dropChancePercentage;
		[SerializeField] private int[] minDrops;
		[SerializeField] private int[] maxDrops;

		[Serializable]
		private class DropConfig
		{
			public InventoryItem item;
			public float[] relativeChance;
			public int[] minNumber;
			public int[] maxNumber;
			
			public int GetRandomNumber(int level)
			{
				int min = GetByLevel(minNumber, level);
				int max = GetByLevel(maxNumber, level);
				return !item.IsStackable() ? 1 : Random.Range(min, max + 1);
			}
		}

		public struct Dropped
		{
			public InventoryItem item;
			public int number;
		}

		public IEnumerable<Dropped> GetRandomDrops(int level)
		{
			if (!ShouldRandomDrop(level)) yield break;
			
			for (int i = 0; i < GetRandomNumberOfDrops(level); i++)
			{
				yield return GetRandomDrop(level);
			}
		}
		
		private Dropped GetRandomDrop(int level)
		{
			var drop = SelectRandomItem(level);
			var result = new Dropped
			{
				item = drop.item,
				number = drop.GetRandomNumber(level)
			};
			return result;
		}
		
		private int GetRandomNumberOfDrops(int level)
		{
			int min = GetByLevel(minDrops, level);
			int max = GetByLevel(maxDrops, level);
			return Random.Range(min, max);
		}

		private bool ShouldRandomDrop(int level)
		{
			return Random.Range(0,100) < GetByLevel(dropChancePercentage, level);
		}

		private DropConfig SelectRandomItem(int level)
		{
			float totalChance = GetTotalChance(level);
			float randomRoll = Random.Range(0, totalChance);
			float chanceTotal = 0;
			
			foreach (var drop in potentialDrops)
			{
				chanceTotal += GetByLevel(drop.relativeChance, level);

				if (chanceTotal > randomRoll)
				{
					return drop;
				}
			}

			return null;
		}

		private float GetTotalChance(int level) => potentialDrops.Sum(drop => GetByLevel(drop.relativeChance, level));

		private static T GetByLevel<T>(T[] values, int level)
		{
			if (values.Length == 0)
				return default;
			if (level > values.Length)
				return values[values.Length - 1];
			if (level <= 0)
				return default;
			return values[level - 1];
		}
	}
}
