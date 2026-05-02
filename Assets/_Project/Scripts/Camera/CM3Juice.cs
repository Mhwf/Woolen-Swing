using UnityEngine;
using Unity.Cinemachine;

namespace WoolenSwing.CM3Juice
{
    public class CM3Juice : MonoBehaviour
    {
        private CinemachineCamera _cmCamera;
        private float _targetFOV;

        [Header("FOV Settings")]
        [SerializeField] private float normalFOV = 60f;
        [SerializeField] private float boostFOV = 80f;
        [SerializeField] private float fovSmoothSpeed = 5f;

        void Awake()
        {
            _cmCamera = GetComponent<CinemachineCamera>();
            _targetFOV = normalFOV;

            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            if (_cmCamera == null) return;
            var lens = _cmCamera.Lens;
            lens.FieldOfView = Mathf.Lerp(lens.FieldOfView, _targetFOV, Time.deltaTime * fovSmoothSpeed);
            _cmCamera.Lens = lens;
        }

        //speed effect
        public void TriggerFOV(bool boosting)
        {
            _targetFOV = boosting ? boostFOV : normalFOV;
        }
    }
}