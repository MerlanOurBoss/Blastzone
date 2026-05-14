using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
   [Header("Передвижение")]
    public float moveSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 5f;
    public float groundDrag = 6f;
    public float airDrag = 1f;

    [Header("Проверка земли")]
    public Transform groundCheck;
    public float checkRadius = 0.3f;
    public LayerMask groundLayer;

    [Header("Камера")]
    public Transform cameraTransform;

    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Блокируем вращение физикой
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        CheckGround();
        HandleInput();
        ControlDrag();
        LimitSpeed();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    // ─── Проверка земли ───────────────────────────────────────────
    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, groundLayer);
    }

    // ─── Обработка ввода ──────────────────────────────────────────
    void HandleInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // Направление относительно камеры
        Vector3 forward = cameraTransform.forward;
        Vector3 right   = cameraTransform.right;
        forward.y = 0f;
        right.y   = 0f;

        moveDirection = (forward.normalized * z + right.normalized * x).normalized;

        // Прыжок
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity  = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Сброс Y
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Поворот игрока по направлению движения
        if (moveDirection != Vector3.zero)
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * 15f);
    }

    // ─── Движение через физику ────────────────────────────────────
    void MovePlayer()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;

        if (isGrounded)
            rb.AddForce(moveDirection * (currentSpeed * 10f), ForceMode.Force);
        else
            rb.AddForce(moveDirection * (currentSpeed * 1f), ForceMode.Force); // Меньше контроля в воздухе
    }

    // ─── Сопротивление (торможение) ───────────────────────────────
    void ControlDrag()
    {
        rb.drag = isGrounded ? groundDrag : airDrag;
    }

    // ─── Ограничение максимальной скорости ────────────────────────
    void LimitSpeed()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;

        Vector3 flatVelocity = new Vector3(rb.velocity .x, 0f, rb.velocity .z);

        if (flatVelocity.magnitude > currentSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * currentSpeed;
            rb.velocity  = new Vector3(limitedVelocity.x, rb.velocity .y, limitedVelocity.z);
        }
    }

    // ─── Отладка в редакторе ──────────────────────────────────────
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}
