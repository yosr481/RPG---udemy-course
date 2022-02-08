using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    public class MouseItemDropper : ItemDropper
    {
        protected override Vector3 GetDropLocation()
        {

            bool hasHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit);

            if (!hasHit) return transform.position;

            bool hasCastToNavMesh = NavMesh.SamplePosition(
                hit.point, out var navMeshHit, 1, NavMesh.AllAreas);
            if (!hasCastToNavMesh) return transform.position;

            var point = navMeshHit.position;

            return point;
        }
    }
}