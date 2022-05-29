using UnityEngine;

namespace Plancksize
{
    public class PlanckGUser : MonoBehaviour
    {
        public bool gRotation = true;
        public float mass = 1;

        [Space(20)]
        [SerializeField]
        private PlanckGCaster gCaster;

        Transform bodyTransform;
        Collider2D bodyCollider;
        Rigidbody2D bodyRB;

        //[SerializeField]
        private float currentGScale;
        private float appliedForce;

        private void Awake()
        {
            bodyTransform = GetComponent<Transform>();
            bodyCollider = GetComponent<Collider2D>();
            bodyRB = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (gCaster != null)
            {
                if (!gCaster.gPool.Contains(bodyCollider))
                    gCaster = null;

                if (gRotation && gCaster != null)
                    RotateWithGravity();
            }
        }

        //Calculate G-Force scale
        private float gScale(PlanckGCaster caster)
        {
            //calculate distance between Attractor and Attracted
            float dist = (caster.attractorTransform.transform.position - bodyTransform.position).magnitude;

            //get normalized value between 0 and 1 of Gravity Force inverse Square based on distance
            if (dist <= 0) currentGScale = 1;
            else
            {
                currentGScale = Mathf.Pow(Mathf.InverseLerp(caster.gravityRadius, 0, dist), 2);
            }

            return currentGScale;
        }

        //Apply G-Force
        public void GPull(PlanckGCaster caster, bool useMasses = false)
        {
            if (useMasses)
            {
                appliedForce = -(caster.mass * mass * -(caster.gravityConstant));
            }
            else
                appliedForce = caster.gravityConstant;

            Vector2 gForceDirection = caster.attractorTransform.position - bodyTransform.position;
            bodyRB.AddForce(gForceDirection.normalized * -appliedForce * 10 * gScale(caster) * Time.fixedDeltaTime);
        }

        //Apply rotation towards G-Force Center
        private void RotateWithGravity()
        {
            Vector2 distanceVector = gCaster.attractorTransform.position - bodyTransform.position;
            float gAngle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
            bodyTransform.rotation = Quaternion.AngleAxis(gAngle + 90, Vector3.forward);
        }
    }

}