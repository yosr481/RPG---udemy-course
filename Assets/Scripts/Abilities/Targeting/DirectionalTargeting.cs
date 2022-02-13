using System;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
	[CreateAssetMenu(fileName = "New Directional Targeting", menuName = "Abilities/Targeting/Directional", order = 0)]
	public class DirectionalTargeting : TargetingStrategy
	{
		[SerializeField] private LayerMask groundMask;
		[SerializeField] private float groundOffset = 1;
		
		public override void StartTargeting(AbilityData data, Action finished)
		{
			var mouseRay = PlayerController.GetMouseRay();
			if (Physics.Raycast(mouseRay, out var raycastHit, Mathf.Infinity, groundMask))
			{
				data.SetTargetedPoint(raycastHit.point + mouseRay.direction * groundOffset / mouseRay.direction.y);
			}

			finished();
		}
	}
}
