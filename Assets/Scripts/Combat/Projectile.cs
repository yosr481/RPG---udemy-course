using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float projectileSpeed = 1;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifetime = 10;
        [SerializeField] GameObject[] hitToDestroy = null;
        [SerializeField] float maxDestroyImact = 0.2f;
        [SerializeField] UnityEvent onHit;

        Health target = null;
        GameObject investigator;
        float damage = 0;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        private void Update()
        {
            if (target == null) return;
            
            transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
        }

        public void SetTarget(Health _target, GameObject _investigator, float _damage)
        {
            target = _target;
            damage = _damage;
            investigator = _investigator;

            Destroy(gameObject, maxLifetime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null) return target.transform.position;
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target) return;
            if (target.IsDead()) return;
            onHit.Invoke();
            target.TakeDamage(investigator, damage);
            projectileSpeed = 0;

            if(hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in hitToDestroy)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, maxDestroyImact);
        }
    }
}
