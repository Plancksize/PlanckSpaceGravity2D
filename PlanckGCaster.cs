using System.Collections.Generic;
using UnityEngine;

namespace Plancksize
{
    public class PlanckGCaster : MonoBehaviour
    {
        public float attractorRadius;
        public float gravityForce = -10;
        public LayerMask gEffectLayer;

        [HideInInspector]
        public Transform attractorTransform;
        [SerializeField]
        public List<Collider2D> gPool = new List<Collider2D>();

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, attractorRadius);
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
            return new List<Collider2D>(Physics2D.OverlapCircleAll(attractorTransform.position, attractorRadius, gEffectLayer));
        }

        //Call for attracted object force application
        private void ApplyGForce(List<Collider2D> objectPool)
        {
            for(int i = 0; i < objectPool.Count; i++)
            {
                if (objectPool[i].GetComponent<PlanckGUser>() == null)
                    objectPool[i].gameObject.AddComponent<PlanckGUser>();
            
                objectPool[i].GetComponent<PlanckGUser>().GPull(this);
            }
        }
    }
}
