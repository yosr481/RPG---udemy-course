using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public interface IPredicateEvaluator
    {
        public enum EPredicate
        {
            Select,
            HasQuest,
            CompletedObjective,
            CompletedQuest,
            HasLevel,
            MinimumTrait,
            HasItem,
            HasItems,
            HasItemEquipped
        }
        
        bool? Evaluate(string predicate, string[] parameters);
    }
}
