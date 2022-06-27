using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Core
{
    [Serializable]
    public class Condition
    {
        [SerializeField] private Disjunction[] and;

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            foreach (var disjunction in and)
            {
                if (!disjunction.Check(evaluators))
                {
                    return false;
                }
            }
            return true;
        }
        
        [Serializable]
        public class Disjunction
        {
            [SerializeField] private Predicate[] or;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (var predicate in or)
                {
                    if (predicate.Check(evaluators))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        
        [Serializable]
        public class Predicate
        {
            [SerializeField] private EPredicate predicate;
            [SerializeField] private string[] parameters;
            [SerializeField] private bool negate;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (var evaluator in evaluators)
                {
                    bool? result = evaluator.Evaluate(predicate, parameters);
                    if (result == null)
                        continue;
                    if (result == negate) 
                        return false;
                }
                return true;
            }
        }
    }
}
