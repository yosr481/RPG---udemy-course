using System;
using System.Collections.Generic;
using System.IO;
using Cinemachine.Editor;
using RPG.Abilities.Effects;
using RPG.Abilities.Filters;
using RPG.Attributes;
using RPG.Core;
using RPG.Inventories;
using RPG.Stats;
using UnityEditor;
using UnityEngine;

namespace RPG.Abilities
{
	[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Ability", order = 0)]
	public class Ability : ActionItem
	{
		[SerializeField] private TargetingStrategy targetingStrategy;
		[SerializeField] private List<FilterStrategy> filterStrategies = new List<FilterStrategy>();
		[SerializeField] private List<EffectStrategy> effectStrategies = new List<EffectStrategy>();
		[SerializeField] private float cooldownTime = 0;
		[SerializeField] private float manaCost = 0;

		public override bool Use(GameObject user)
		{
			Mana mana = user.GetComponent<Mana>();
			if (mana.GetMana() < manaCost) return false;
			
			CooldownStore cooldownStore = user.GetComponent<CooldownStore>();
			if(cooldownStore.GetTimeRemaining(this) > 0) return false;
			
			AbilityData data = new AbilityData(user);
			
			var actionScheduler = user.GetComponent<ActionScheduler>();
			actionScheduler.StartAction(data);

			targetingStrategy.StartTargeting(data,
				() => TargetAcquired(data));

			return true;
		}

		private void TargetAcquired(AbilityData data)
		{
			if(data.IsCancelled()) return;
			
			Mana mana = data.GetUser().GetComponent<Mana>();
			if(!mana.UseMana(manaCost)) return;
			
			CooldownStore cooldownStore = data.GetUser().GetComponent<CooldownStore>();
			cooldownStore.StartCooldown(this, cooldownTime);
			
			foreach (var filterStrategy in filterStrategies)
			{
				data.SetTargets(filterStrategy.Filter(data.GetTargets()));
			}

			foreach (var effect in effectStrategies)
			{
				effect.StartEffect(data, EffectFinished);
			}
		}

		private void EffectFinished()
		{
			
		}

		private TargetingStrategy GetTargetingStrategy() => targetingStrategy;
		
		private FilterStrategy GetFilterStrategies(List<FilterStrategy> filterStrategiesList, int i)
		{
			return filterStrategiesList[i];
		}
		
		private EffectStrategy GetEffectStrategies(List<EffectStrategy> filterStrategiesList, int i)
		{
			return effectStrategies[i];
		}

		private float GetCooldownTime() => cooldownTime;

		private float GetManaCost() => manaCost;

#if UNITY_EDITOR
		private void SetTargetingStrategy(TargetingStrategy newTargetingStrategy)
		{
			if(newTargetingStrategy == targetingStrategy) return;
			SetUndo("Set Targeting Strategy");
			targetingStrategy = newTargetingStrategy;
			Dirty();
		}

#region FilterStrategies
private void AddFilterStrategies(List<FilterStrategy> strategiesList)
		{
			SetUndo("Add Filter Strategies");
			strategiesList?.Add(CreateInstance<TagFilter>());
			Dirty();
		}
		
		private void SetFilterStrategies(List<FilterStrategy> StrategiesList, int i, FilterStrategy strategy)
		{
			if(StrategiesList[i] == strategy) return;
			SetUndo("Set Filter Strategies");
			FilterStrategy tempStrategy = StrategiesList[i];
			tempStrategy = strategy;
			StrategiesList[i] = tempStrategy;
			Dirty();
		}

		private void RemoveFilterStrategies(List<FilterStrategy>filterStrategiesList, int index)
		{
			SetUndo("Remove Filter Strategies");
			filterStrategiesList?.RemoveAt(index);
			Dirty();
		}

		void DrawFilterStrategiesList(List<FilterStrategy> strategiesList)
		{
			int strategyToDelete = -1;
			GUIContent statLabel = new GUIContent("Filter Strategy");
			for (int i = 0; i < strategiesList.Count; i++)
			{
				ScriptableObject strategy = strategiesList[i];
				EditorGUILayout.BeginHorizontal();
				FilterStrategy currentStrategy = GetFilterStrategies(strategiesList, i);
				SetFilterStrategies(strategiesList, i,(FilterStrategy)EditorGUILayout.ObjectField("Filter Strategy", currentStrategy, typeof(FilterStrategy), false));
				if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false)))
				{
					strategyToDelete = i;
				}

				EditorGUILayout.EndHorizontal();
			}

			if (strategyToDelete > -1)
			{
				RemoveFilterStrategies(strategiesList, strategyToDelete);
			}

			if (GUILayout.Button("Add Strategy"))
			{
				AddFilterStrategies(strategiesList);
			}
		}
#endregion
		
#region EffectStrategies
		private void AddEffectStrategies(List<EffectStrategy> strategiesList)
		{
			SetUndo("Add Effect Strategies");
			strategiesList?.Add(CreateInstance<HealthEffect>());
			Dirty();
		}
				
		private void SetEffectStrategies(List<EffectStrategy> strategiesList, int i, EffectStrategy strategy)
		{
			if(strategiesList[i] == strategy) return;
			SetUndo("Set Effect Strategies");
			EffectStrategy tempStrategy = strategiesList[i];
			tempStrategy = strategy;
			strategiesList[i] = tempStrategy;
			Dirty();
		}

		private void RemoveEffectStrategies(List<EffectStrategy> strategiesList, int index)
		{
			SetUndo("Remove Effect Strategies");
			strategiesList?.RemoveAt(index);
			Dirty();
		}

		void DrawEffectStrategiesList(List<EffectStrategy> strategiesList)
		{
			int strategyToDelete = -1;
			GUIContent statLabel = new GUIContent("Effect Strategy");
			for (int i = 0; i < strategiesList.Count; i++)
			{
				EffectStrategy strategy = strategiesList[i];
				EditorGUILayout.BeginHorizontal();
				EffectStrategy currentStrategy = GetEffectStrategies(strategiesList, i);
				SetEffectStrategies(strategiesList, i,(EffectStrategy)EditorGUILayout.ObjectField($"Effect Strategy", currentStrategy, typeof(EffectStrategy), false));
				if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false)))
				{
					strategyToDelete = i;
				}

				EditorGUILayout.EndHorizontal();
			}

			if (strategyToDelete > -1)
			{
				RemoveEffectStrategies(strategiesList, strategyToDelete);
			}

			if (GUILayout.Button("Add Strategy"))
			{
				AddEffectStrategies(strategiesList);
			}
		}
#endregion
		private void SetCooldownTime(float newCooldownTime)
		{
			if (FloatEquals(cooldownTime, newCooldownTime)) return;
			SetUndo("Set Weapon Range");
			cooldownTime = newCooldownTime;
			Dirty();
		}

		private void SetManaCost(float newManaCost)
		{
			if (FloatEquals(manaCost, newManaCost)) return;
			SetUndo("Set Weapon Range");
			manaCost = newManaCost;
			Dirty();
		}


		bool drawAbilityData = true;
		bool drawFilters = true;
		bool drawEffects = true;
		
		public override void DrawCustomInspector()
		{
			base.DrawCustomInspector();
			
			GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout)
			{
				fontStyle = FontStyle.Bold
			};
			
			drawAbilityData = EditorGUILayout.Foldout(drawAbilityData, "Ability Item Data", true, foldoutStyle);
			if (!drawAbilityData) return;
			
			EditorGUILayout.BeginVertical(contentStyle);
			SetTargetingStrategy((TargetingStrategy)EditorGUILayout.ObjectField("Targeting Strategy", GetTargetingStrategy(), typeof(TargetingStrategy), false));
			
			GUIStyle strategiesFoldoutStyle = new GUIStyle(EditorStyles.foldout)
			{
				fontStyle = FontStyle.Italic
			};
			
			EditorGUILayout.BeginVertical(contentStyle);
			drawFilters = EditorGUILayout.Foldout(drawFilters, "Filter Strategies", true, strategiesFoldoutStyle);
			if (drawFilters)
			{
				DrawFilterStrategiesList(filterStrategies);
			}
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical(contentStyle);
			drawEffects = EditorGUILayout.Foldout(drawEffects, "Effect Strategies", true, strategiesFoldoutStyle);
			if (drawEffects)
			{
				DrawEffectStrategiesList(effectStrategies);
			}
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical(contentStyle);
			SetCooldownTime(EditorGUILayout.IntSlider("Cooldown Time", (int)cooldownTime, 0, 60));
			SetManaCost(EditorGUILayout.IntSlider("Mana Cost", (int)manaCost, 0, 1000));
			EditorGUILayout.EndVertical();
		}
#endif
	}
}
