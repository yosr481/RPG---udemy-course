using System;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Inventories
{
	public class Collector : MonoBehaviour, IAction
	{
		private Pickup targetPickup = null;

		public void StartCollectionAction(Pickup newPickup)
		{
			if(newPickup == targetPickup) return;
			GetComponent<ActionScheduler>().StartAction(this);
			targetPickup = newPickup;
		}

		private void Update()
		{
			if(!targetPickup) return;

			if (Vector3.Distance(transform.position, targetPickup.transform.position) > 3.0f)
			{
				GetComponent<Mover>().MoveTo(targetPickup.transform.position, 1);
			}
			else
			{
				GetComponent<Mover>().Cancel();
				targetPickup.PickupItem();
				targetPickup = null;
			}
		}

		public void Cancel()
		{
			targetPickup = null;
			GetComponent<Mover>().Cancel();
		}
	}
}
