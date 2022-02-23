using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
	public class RandomDropper: ItemDropper
	{
		[Tooltip("How far an the pickups be scattered from the dropper.")]
		[SerializeField] private float scatterDistance = 1;
		[SerializeField] private DropLibrary dropLibrary;
		
		private const int Attempts = 30;

		public void RandomDrop()
		{
			var baseStats = GetComponent<BaseStats>();
			var level = baseStats.GetLevel();
			
			var drops = dropLibrary.GetRandomDrops(level);
			foreach (var drop in drops)
			{
				DropItem(drop.item, drop.number);
			}
		}
		
		protected override Vector3 GetDropLocation()
		{
			for (int i = 0; i < Attempts; i++)
			{
				Vector3 randomPoint = transform.position + Random.insideUnitSphere * scatterDistance;
				if(NavMesh.SamplePosition(randomPoint, out var hit, 0.1f, NavMesh.AllAreas))
				{
					return hit.position;
				}
			}
			return transform.position;
		}
	}
}
