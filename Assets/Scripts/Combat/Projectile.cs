using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float projectileSpeed = 1;
        [SerializeField] private GameObject hitEffect = null;
        [SerializeField] private float maxLifetime = 10;
        [SerializeField] private GameObject[] hitToDestroy = null;
        [FormerlySerializedAs("maxDestroyImact")][SerializeField]
        private float maxDestroyImpact = 0.2f;
        [SerializeField] private UnityEvent onHit;

        private Health target = null;
        private Vector3 targetPoint;
        private GameObject investigator;
        private float damage = 0;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        private void Update()
        {
            if (target != null && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
            
            transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject investigator, float damage)
        {
            SetTarget(investigator, damage, target);
        }

        public void SetTarget(Vector3 targetPoint, GameObject investigator, float damage)
        {
            SetTarget(investigator,damage, null, targetPoint);
        }

        public void SetTarget(GameObject investigator, float damage, Health target = null, Vector3 targetPoint = default)
        {
            this.target = target;
            this.targetPoint = targetPoint;
            this.damage = damage;
            this.investigator = investigator;

            Destroy(gameObject, maxLifetime);
        }

        private Vector3 GetAimLocation()
        {
            if (target == null)
            {
                return targetPoint;
            }
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null) return target.transform.position;
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            var health = other.GetComponent<Health>();
            if (target != null && health != target) return;
            if(health == null || health.IsDead()) return;
            if(other.gameObject == investigator) return;
            
            onHit.Invoke();
            health.TakeDamage(investigator, damage);
            projectileSpeed = 0;

            if(hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in hitToDestroy)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, maxDestroyImpact);
        }
    }
}
