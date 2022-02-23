using UnityEngine;
using RPG.Inventories;
using RPG.Movement;

namespace RPG.Control
{
    [RequireComponent(typeof(Pickup))]
    public class ClickablePickup : MonoBehaviour, IRaycastable
    {
        private Pickup pickup;

        private void Awake()
        {
            pickup = GetComponent<Pickup>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.CompareTag("Player"))
            {
                pickup.PickupItem();
            }
        }

        public CursorType GetCursorType()
        {
            return pickup.CanBePickedUp() ? CursorType.Pickup : CursorType.FullPickup;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Mover>().StartMoveAction(transform.position, 1);
            }
            return true;
        }
    }
}