using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
	[Header("Cấu hình Chuột")]
	[SerializeField] private float mouseSensitivity = 15f;
	[SerializeField] private Transform playerBody; // Kéo vật thể Cha (Player) vào đây

	private float xRotation = 0f;
	private PlayerControls controls;
	private Vector2 lookInput;

	private void Awake()
	{
		controls = new PlayerControls();

		// Khóa con trỏ chuột vào giữa màn hình game cho chuẩn vibe FPS
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void OnEnable()
	{
		controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
		controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;
		controls.Player.Enable();
	}

	private void OnDisable()
	{
		controls.Player.Disable();
	}

	private void Update()
	{
		HandleLook();
	}

	private void HandleLook()
	{
		// Đọc độ dời của chuột theo thời gian thực
		float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
		float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

		// Xoay Camera theo chiều dọc (Ngước lên / Cúi xuống) và giới hạn góc 90 độ tránh bị lật cổ
		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);

		transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

		// Xoay toàn bộ cơ thể Player theo chiều ngang (Quay trái / Quay phải)
		playerBody.Rotate(Vector3.up * mouseX);
	}
}