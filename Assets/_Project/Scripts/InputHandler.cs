using UnityEngine;

namespace WoolenSwing.Input
{
    /// <summary>
    /// this class for input handling
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        public float Horizontal { get; private set; }
        public float Vertical { get; private set; }
        public float MouseX { get; private set; }
        public float MouseY { get; private set; }
        public bool IsJumpPressed { get; private set; }

        public bool IsGrappleDown { get; private set; }
        public bool IsGrappleUp { get; private set; }

        void Update()
        {

            Horizontal = UnityEngine.Input.GetAxis("Horizontal");
            Vertical = UnityEngine.Input.GetAxis("Vertical");
            IsJumpPressed = UnityEngine.Input.GetButtonDown("Jump");

            IsGrappleDown = UnityEngine.Input.GetMouseButtonDown(0) || UnityEngine.Input.GetMouseButtonDown(1);
            IsGrappleUp = UnityEngine.Input.GetMouseButtonUp(0) || UnityEngine.Input.GetMouseButtonUp(1);

            MouseX = UnityEngine.Input.GetAxis("Mouse X");
            MouseY = UnityEngine.Input.GetAxis("Mouse Y");
        }
    }
}