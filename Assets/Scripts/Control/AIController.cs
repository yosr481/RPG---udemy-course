using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using GameDevTV.Utils;
using System;
using UnityEngine.AI;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float suspicionTime = 3f;
        [SerializeField] private float agroCoolDownTime = 5f;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private float waypointTolerance = 1f;
        [SerializeField] private float waypointDwellTime = 2f;
        [Range(0,1)]
        [SerializeField]
        private float patrolSpeedFraction = 0.3f;
        [SerializeField] private float shoutDistance = 5;

        private Fighter fighter;
        private Mover mover;
        private GameObject player;
        private Health health;

        private LazyValue<Vector3> guardPosition;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceArriveAtWaypoint = Mathf.Infinity;
        private float timeSinceAggrevated = Mathf.Infinity;
        private int currentWaypointIndex = 0;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            player = GameObject.FindWithTag("Player");
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
            guardPosition.ForceInit();
        }

        private void Start()
        {
            guardPosition.ForceInit();
        }

        private void Update()
        {
            if (health.IsDead()) return;

            if (IsAggrevated() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }


        public void Reset()
        {
            var agent = GetComponent<NavMeshAgent>();
            agent.Warp(guardPosition.Value);
            timeSinceLastSawPlayer = Mathf.Infinity;
            timeSinceArriveAtWaypoint = Mathf.Infinity;
            timeSinceAggrevated = Mathf.Infinity;
            currentWaypointIndex = 0;
        }

        private void Aggravate()
        {
            timeSinceAggrevated = 0;
        }
        
        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArriveAtWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.Value;

            if(patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArriveAtWaypoint = 0;
                    CycleWaypoint();
                }

                nextPosition = GetCurrentWaypoint();
            }

            if(timeSinceArriveAtWaypoint > waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);

            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach(RaycastHit hit in hits)
            {
                AIController ai = hit.transform.GetComponent<AIController>();
                if (ai == null) continue;
                ai.Aggravate();
            }
        }

        private bool IsAggrevated()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance  || timeSinceAggrevated < agroCoolDownTime;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
