using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        private NavMeshAgent agent;
        private Animator animator;
        private Health health;

        private float maxSpeed = 16.32f;
        private static readonly int ForwardSpeed = Animator.StringToHash("forwardSpeed");

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            health = GetComponent<Health>();
        }

        private void Update()
        {
            if(health)
                agent.enabled = !health.IsDead();
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            agent.destination = destination;
            agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            agent.isStopped = false;
        }

        public void Cancel()
        {
            agent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            animator.SetFloat(ForwardSpeed, speed);
        }

        public object CaptureState()
        {
            return new SerializibleVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializibleVector3 position = (SerializibleVector3)state;
            GetComponent<NavMeshAgent>().Warp(position.ToVector());
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}
