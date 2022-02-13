using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Abilities.Filters
{
	[CreateAssetMenu(fileName = "Tag Filter", menuName = "Abilities/Filters/Tag", order = 0)]
	public class TagFilter : FilterStrategy
	{
		[SerializeField] private string tagToFilter;
		public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectsToFilter)
		{
			return objectsToFilter.Where(gameObject => gameObject.CompareTag(tagToFilter));
		}
	}
}
