using UnityEngine;
using WoolenSwing.Input;
using System.Collections;
using WoolenSwing.Management;

namespace WoolenSwing.Phys
{
    public class YarnBallLogic : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 15f;
        public float jumpForce = 12f;
        public LayerMask groundLayer;

        [Header("Hierarchy")]
        public Transform ballMesh;
        public Transform basePivot;

        [Header("Animation Settings")]
        public AnimationCurve jumpCurve;
        public AnimationCurve landCurve;
        public float animDuration = 0.5f;
        [Range(0, 20)] public float restoreSpeed = 10f;

        private Rigidbody rb;
        private InputHandler input;
        private Vector3 meshOriginalPos;
        private bool isAnimating = false;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            input = GetComponent<InputHandler>();
            meshOriginalPos = ballMesh.localPosition;
        }

        void Update()
        {
            if (input.IsJumpPressed && IsGrounded() && GameManager.Instance.CurrentState == GameState.Playing)
            {
                ApplyJump();
            }

            if (!isAnimating && basePivot.localScale != Vector3.one)
            {
                basePivot.localScale = Vector3.Lerp(basePivot.localScale, Vector3.one, Time.deltaTime * restoreSpeed);
            }
        }

        void FixedUpdate()
        {
            if(GameManager.Instance.CurrentState == GameState.Playing)
                ApplyMovement();
        }

        private void ApplyMovement()
        {
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;
            camForward.y = 0; camRight.y = 0;
            Vector3 dir = (camForward.normalized * input.Vertical + camRight.normalized * input.Horizontal).normalized;

            rb.AddForce(dir * moveSpeed);
            rb.AddTorque(Vector3.Cross(Vector3.up, dir) * moveSpeed);
        }

        private void ApplyJump()
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            StartCoroutine(PlaySqueezeStretch(new Vector3(0.8f, 1.4f, 0.8f), jumpCurve));
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.relativeVelocity.magnitude > 3f)
            {
                StartCoroutine(PlaySqueezeStretch(new Vector3(1.3f, 0.7f, 1.3f), landCurve));
            }
        }

        private IEnumerator PlaySqueezeStretch(Vector3 targetScale, AnimationCurve curve)
        {
            isAnimating = true;

            basePivot.rotation = Quaternion.identity;
            ballMesh.SetParent(basePivot);

            Vector3 startScale = basePivot.localScale;
            float time = 0;

            while (time < 1f)
            {
                time += Time.deltaTime / animDuration;
                float curveValue = curve.Evaluate(time);

                basePivot.localScale = Vector3.Lerp(Vector3.one, targetScale, curveValue);

                yield return null;
            }


            float finishTime = 0;
            while (finishTime < 0.2f)
            {
                finishTime += Time.deltaTime;
                basePivot.localScale = Vector3.Lerp(basePivot.localScale, Vector3.one, finishTime / 0.2f);
                yield return null;
            }


            basePivot.localScale = Vector3.one;
            ballMesh.SetParent(transform);
            ballMesh.localPosition = meshOriginalPos;

            isAnimating = false;
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position, Vector3.down, 0.6f, groundLayer);
        }
    }
}