using UnityEngine;
using YesChef.Managers;

namespace YesChef.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 7.0f;
        public float rotationSpeed = 15.0f;

        private Rigidbody rb;
        private Vector3 moveInput;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
        }

        private void Update()
        {
            if (GameManager.Instance != null && !GameManager.Instance.IsPlaying())
            {
                moveInput = Vector3.zero;
                return;
            }

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            moveInput = new Vector3(horizontal, 0f, vertical).normalized;
        }

        private void FixedUpdate()
        {
            if (moveInput.sqrMagnitude > 0.01f)
            {
                Vector3 targetVelocity = moveInput * moveSpeed;
                rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

                Quaternion targetRotation = Quaternion.LookRotation(moveInput, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
            }
            else
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
        }
    }
}
