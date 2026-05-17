using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
	[Header("Cấu hình Di chuyển")]
	[SerializeField] private float moveSpeed = 6f;
	[SerializeField] private float gravity = -9.81f;
	[SerializeField] private float jumpHeight = 1.5f;

	[Header("Kiểm tra Mặt đất")]
	[SerializeField] private Transform groundCheck;
	[SerializeField] private float groundDistance = 0.4f;
	[SerializeField] private LayerMask groundMask;

	private CharacterController controller;
	private Vector3 velocity;
	private bool isGrounded;
	private Vector2 moveInput;

	// Biến quản lý file Input Action đã tạo ở Bước 1
	private PlayerControls controls;

	private void Awake()
	{
		controller = GetComponent<CharacterController>();
		controls = new PlayerControls();
	}

	private void OnEnable()
	{
		// Đăng ký sự kiện đọc phím WASD
		controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
		controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

		// Đăng ký sự kiện nút Nhảy
		controls.Player.Jump.performed += ctx => OnJump();

		controls.Player.Enable();
	}

	private void OnDisable()
	{
		controls.Player.Disable();
	}

	private void Update()
	{
		HandleMovement();
	}

	private void HandleMovement()
	{
		// 1. Kiểm tra xem nhân vật có đang đứng trên mặt đất không
		isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

		if (isGrounded && velocity.y < 0)
		{
			velocity.y = -2f; // Giữ nhân vật bám chắc vào mặt đất
		}

		// 2. Tính toán hướng đi dựa theo hướng quay mặt của chính nhân vật đó (Future-proof cho Co-op)
		Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
		controller.Move(move * moveSpeed * Time.deltaTime);

		// 3. Áp dụng trọng lực rơi tự do
		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);
	}

	private void OnJump()
	{
		if (isGrounded)
		{
			// Công thức vật lý tính toán lực nhảy vọt lên
			velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
		}
	}
}