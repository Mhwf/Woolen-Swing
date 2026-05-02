using WoolenSwing.CM3Juice;
using WoolenSwing.Input;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows;

namespace WoolenSwing.Mechanics
{
    public class GrappleSystem : MonoBehaviour
    {
        [SerializeField] private InputHandler input;
        [Header("Detection Settings")]
        [SerializeField] private float maxRange = 25f;
        [SerializeField] private LayerMask nailLayer;
        [Range(0, 1)][SerializeField] private float forwardThreshold = 0.5f;

        [Header("Physics Settings")]
        [SerializeField] private float springForce = 4.5f;
        [SerializeField] private float damper = 4f;
        [SerializeField] private float massScale = 4.5f;

        [Header("Visuals")]
        [SerializeField] private LineRenderer lineRenderer;

        private SpringJoint _joint;
        private Transform _currentTarget;
        private Camera _mainCam;

        [Header("Boost Settings")]
        [SerializeField] private float initialPullForce = 20f;
        [SerializeField] private WoolenSwing.CM3Juice.CM3Juice Cam;

        void Start()
        {
            _mainCam = Camera.main;
            if (lineRenderer) lineRenderer.enabled = false;
        }

        void Update()
        {
            if (input.IsGrappleDown) TryGrapple();
            if (input.IsGrappleUp) ReleaseGrapple();
        }

        void LateUpdate()
        {
            DrawThread();
        }

        private void TryGrapple()
        {
            Nail bestNail = FindBestNail();

            if (bestNail != null)
            {
                AttachToNail(bestNail.transform);
            }
        }

        private Nail FindBestNail()
        {
            //check for nails in range
            Collider[] hits = Physics.OverlapSphere(transform.position, maxRange, nailLayer);
            List<Nail> candidates = new List<Nail>();

            Vector3 camForward = _mainCam.transform.forward;

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out Nail nail))
                {
                    Vector3 dirToNail = (nail.transform.position - transform.position).normalized;

                    float dot = Vector3.Dot(camForward, dirToNail);

                    if (dot > forwardThreshold)
                    {
                        candidates.Add(nail);
                    }
                }
            }

            
            return candidates
                .OrderBy(n => Vector3.Distance(transform.position, n.transform.position))
                .FirstOrDefault();
        }

        // this for catch the closest nail
        private void AttachToNail(Transform nailTransform)
        {
            _currentTarget = nailTransform;

            //add a boost to the ball
            Vector3 pullDir = (nailTransform.position - transform.position).normalized;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(pullDir * initialPullForce, ForceMode.Impulse);

            if (Cam) Cam.TriggerFOV(true);// camera boost effect


            //spring joint
            _joint = gameObject.AddComponent<SpringJoint>();
            _joint.autoConfigureConnectedAnchor = false;
            _joint.connectedAnchor = nailTransform.position;

            float distance = Vector3.Distance(transform.position, nailTransform.position);

            //add flexiblity
            _joint.maxDistance = distance * 0.8f;
            _joint.minDistance = distance * 0.1f;

            _joint.spring = springForce;
            _joint.damper = damper;
            _joint.massScale = massScale;

            if (lineRenderer) lineRenderer.enabled = true;
        }

        private void ReleaseGrapple()
        {
            if (_joint) Destroy(_joint);
            _currentTarget = null;
            if (lineRenderer) lineRenderer.enabled = false;
            if (Cam) Cam.TriggerFOV(false);
        }

        private void DrawThread()
        {
            if (!_currentTarget || !lineRenderer) return;
            lineRenderer.transform.position = Vector3.zero;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, _currentTarget.position);
        }
    }
}