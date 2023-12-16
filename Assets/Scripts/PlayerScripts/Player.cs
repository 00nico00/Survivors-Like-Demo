using System;
using UnityEngine;

namespace PlayerScripts
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float runSpeed;

        private Rigidbody2D _rb;
        private bool _isWalking;

        public bool IsWalking => _isWalking;
        public bool IsFacingRight => transform.localScale.x > 0f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        private void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            var moveDirection = GameInput.Instance.GetMovementNormalized();
            _isWalking = moveDirection != Vector2.zero;
            _rb.velocity = runSpeed * moveDirection;
            HandleFaceDirection(moveDirection);
        }

        private void HandleFaceDirection(Vector2 moveDirection)
        {
            if (moveDirection.x != 0f)
            {
                var scaleX = moveDirection.x > 0f ? 1 : -1;
                transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
            }
        }
    }
}
