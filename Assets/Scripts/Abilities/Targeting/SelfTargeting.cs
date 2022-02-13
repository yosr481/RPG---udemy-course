using System;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
	[CreateAssetMenu(fileName = "New Self Targeting", menuName = "Abilities/Targeting/Self", order = 0)]
	public class SelfTargeting : TargetingStrategy
	{
		public override void StartTargeting(AbilityData data, Action finished)
		{
			data.SetTargets(new[] { data.GetUser() });
			data.SetTargetedPoint(data.GetUser().transform.position);
			finished();
		}
	}
}
