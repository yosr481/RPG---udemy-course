using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
	[CreateAssetMenu(fileName = "Delayed Click Targeting", menuName = "Abilities/Targeting/Delayed click", order = 0)]
	public class DelayedClickTargeting : TargetingStrategy
	{
		[SerializeField] private Texture2D cursorTexture;
		[SerializeField] private Vector2 cursorHotspot;
		[SerializeField] private LayerMask groundMask;
		[SerializeField] private int AreaAffectRadius;
		[SerializeField] private Transform targetingPrefab;

		private Transform targetingPrefabInstance = null;
		
		public override void StartTargeting(AbilityData data, Action finished)
		{
			PlayerController playerController = data.GetUser().GetComponent<PlayerController>();
			playerController.StartCoroutine(Targeting(data, playerController, finished));
		}

		private IEnumerator Targeting(AbilityData data, PlayerController playerController, Action finished)
		{
			playerController.enabled = false;
			
			if(targetingPrefabInstance == null)
				targetingPrefabInstance = Instantiate(targetingPrefab);
			else
				targetingPrefabInstance.gameObject.SetActive(true);

			targetingPrefabInstance.localScale = new Vector3(AreaAffectRadius * 2, 1, AreaAffectRadius * 2);
			
			// Run every frame as MonoBehaviour.Update()
			while (!data.IsCancelled())
			{
				Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
				if (Physics.Raycast(PlayerController.GetMouseRay(), out var raycastHit, Mathf.Infinity, groundMask))
				{
					targetingPrefabInstance.position = raycastHit.point;
					if (Input.GetMouseButtonDown(0))
					{
						// Absorb the whole mouse click
						yield return new WaitUntil(() => Input.GetMouseButton(0));

						data.SetTargetedPoint(raycastHit.point);
						data.SetTargets(GetGameObjectsInRadius(raycastHit.point));

						break;
					}
				}
				yield return null;
			}
			
			targetingPrefabInstance.gameObject.SetActive(false);
			playerController.enabled = true;
			finished();
		}
		
		private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point)
		{
			RaycastHit[] hits = Physics.SphereCastAll(point, AreaAffectRadius, Vector3.up, 0);
			foreach (var hit in hits)
			{
				yield return hit.collider.gameObject;
			}
		}
	}
}
