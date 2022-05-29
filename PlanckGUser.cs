using UnityEngine;
using System;
using System.Collections;

namespace Plancksize
{
    public class PlanckGUser : MonoBehaviour
    {
        public bool lookAt = true;
        public float lookSmoothness;
        public float rotationOffset;
        public float mass = 1;

        [Space(20)]
        [SerializeField]
        private PlanckGCaster gCaster;

        [SerializeField]
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


        private void Update()
        {
            
        }
        private void FixedUpdate()
        {
            if (gCaster != null && !gCaster.gPool.Contains(bodyCollider))
                gCaster = null;
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
                appliedForce = -(caster.mass * mass * -(caster.gravityConstant));
            else
                appliedForce = caster.gravityConstant;

            Vector2 gForceDirection = caster.attractorTransform.position - bodyTransform.position;
            bodyRB.AddForce(gForceDirection.normalized * -appliedForce * 10 * gScale(caster) * Time.fixedDeltaTime);

            if (lookAt)
                StartCoroutine(SmoothLookAtGravity(caster, lookSmoothness));
        }

        //Apply rotation towards G-Force Center
        /*
        private void RotateWithGravity(PlanckGCaster caster)
        {
            Vector2 distanceVector = caster.attractorTransform.position - bodyTransform.position;
            float gAngle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
            bodyTransform.rotation = Quaternion.AngleAxis(gAngle + rotationOffset, Vector3.forward);
        }
        */

        //Apply rotation towards G-Force Center with controlled smoothness
        private IEnumerator SmoothLookAtGravity(PlanckGCaster caster, float smoothness = 60)
        {
            Vector2 distanceVector = caster.attractorTransform.position - bodyTransform.position;
            float gAngle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
            if(smoothness > 0)
                bodyTransform.rotation = Quaternion.Lerp(bodyTransform.rotation, Quaternion.AngleAxis(gAngle + rotationOffset, Vector3.forward), smoothness * Time.fixedDeltaTime);
            else
                bodyTransform.rotation = Quaternion.AngleAxis(gAngle + rotationOffset, Vector3.forward);
            Debug.Log("rotate - "+ bodyTransform.rotation);

            yield return null;
        }
    }

}