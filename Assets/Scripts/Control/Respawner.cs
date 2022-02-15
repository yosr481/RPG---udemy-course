using System;
using System.Collections;
using Cinemachine;
using RPG.Attributes;
using RPG.SceneManagment;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
	public class Respawner : MonoBehaviour
	{
		[SerializeField] private Transform respawnLocation;
		[SerializeField] private float respawnDelay = 3f;
		[SerializeField] private float fadeTime = 0.8f;
		[SerializeField] private float healthRegenPercentage = 20f;
		[SerializeField] private float enemyHealthRegenPercentage = 20f;
		
		private void Awake()
		{
			GetComponent<Health>().onDie.AddListener(Respawn);
		}

		private void Start()
		{
			if (GetComponent<Health>().IsDead())
			{
				Respawn();
			}
		}

		private void Respawn()
		{
			StartCoroutine(RespawnRoutine());
		}

		private IEnumerator RespawnRoutine()
		{
			var savingWrapper = FindObjectOfType<SavingWrapper>();
			savingWrapper.Save();
			yield return new WaitForSeconds(respawnDelay);
			var fader = FindObjectOfType<Fader>();
			yield return fader.FadeOut(fadeTime);
			RespawnPlayer();
			ResetEnemies();
			savingWrapper.Save();
			yield return fader.FadeIn(fadeTime);
		}
		private void ResetEnemies()
		{
			foreach (var enemyController in FindObjectsOfType<AIController>())
			{
				var health = enemyController.GetComponent<Health>();
				if (health && !health.IsDead())
				{
					enemyController.Reset();
					health.Heal(health.GetMaxHealthPoints() * enemyHealthRegenPercentage / 100);
				}
			}
		}
		private void RespawnPlayer()
		{
			var position = respawnLocation.position;
			Vector3 positionDelta = position - transform.position;
			GetComponent<NavMeshAgent>().Warp(position);
			var health = GetComponent<Health>();
			health.Heal(health.GetMaxHealthPoints() * healthRegenPercentage / 100);
			var activeVirtualCamera = FindObjectOfType<CinemachineBrain>().ActiveVirtualCamera;
			if (activeVirtualCamera.Follow == transform)
			{
				activeVirtualCamera.OnTargetObjectWarped(transform, positionDelta);
			}
		}
	}
}
