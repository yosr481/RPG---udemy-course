using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Abilities
{
	public class AbilityData : IAction
	{
		private readonly GameObject user;
		private Vector3 targetedPoint;
		private IEnumerable<GameObject> targets;
		private bool cancelled = false;

		public AbilityData(GameObject user)
		{
			this.user = user;
		}
		
		// Getters
		public GameObject GetUser() => user;
		public Vector3 GetTargetedPoint => targetedPoint;
		public IEnumerable<GameObject> GetTargets() => targets;
		
		// Setters
		public void SetTargetedPoint(Vector3 targetedPoint)
		{
			this.targetedPoint = targetedPoint;
		}
		public void SetTargets(IEnumerable<GameObject> targets)
		{
			this.targets = targets;
		}

		public void StartCoroutine(IEnumerator coroutine)
		{
			user.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
		}
		public void Cancel()
		{
			cancelled = true;
		}

		public bool IsCancelled() => cancelled;
	}
}
