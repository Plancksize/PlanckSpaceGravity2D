using System.Collections.Generic;
using UnityEngine;

namespace Plancksize
{
    public class PlanckGCaster : MonoBehaviour
    {
        public float gravityRadius = 1;
        [Space(10)]
        public bool useMass = false;
        public float gravityConstant = -10;
        [Tooltip("Ignored if not using Mass")]
        public float mass = 1;
        [Space(10)]
        public LayerMask gEffectLayer;

        [HideInInspector]
        public Transform attractorTransform;
        [HideInInspector]
        public List<Collider2D> gPool = new List<Collider2D>();

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, gravityRadius);
        }

        private void Awake()
        {
            attractorTransform = GetComponent<Transform>();
        }

        private void Update()
        {
            if (SetGPoolObjects().Count > 0)
            {
                gPool = SetGPoolObjects();
            }
            else
                gPool.Clear();
        }

        private void FixedUpdate()
        {
            if (gPool.Count > 0)
                ApplyGForce(gPool);
        }

        //Detect valid objects in Gravity Wheel zone
        private List<Collider2D> SetGPoolObjects()
        {
            return new List<Collider2D>(Physics2D.OverlapCircleAll(attractorTransform.position, gravityRadius, gEffectLayer));
        }

        //Call for attracted object force application
        private void ApplyGForce(List<Collider2D> objectPool)
        {
            for(int i = 0; i < objectPool.Count; i++)
            {
                if (objectPool[i].GetComponent<PlanckGUser>() == null)
                    objectPool[i].gameObject.AddComponent<PlanckGUser>();
            
                objectPool[i].GetComponent<PlanckGUser>().GPull(this, useMass);
            }
        }
    }
}
