using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitching : MonoBehaviour
{
	[Header("Cấu hình Vũ khí")]
	[SerializeField] private int selectedWeapon = 0; // 0 là Slot súng, 1 là Slot cận chiến

	private PlayerControls controls;

	private void Awake()
	{
		controls = new PlayerControls();
	}

	private void OnEnable()
	{
		// Đăng ký sự kiện: Bấm phím 1 gán index = 0, bấm phím 2 gán index = 1
		controls.Player.Weapon1.performed += ctx => SetSelectedWeapon(0);
		controls.Player.Weapon2.performed += ctx => SetSelectedWeapon(1);

		controls.Player.Enable();
	}

	private void OnDisable()
	{
		controls.Player.Disable();
	}

	private void Start()
	{
		SelectWeapon(); // Bật vũ khí mặc định khi vừa vào game
	}

	private void SetSelectedWeapon(int index)
	{
		// Nếu người chơi bấm phím của vũ khí đang cầm trên tay thì không làm gì cả
		if (selectedWeapon == index) return;

		// Phòng hờ lỗi: Nếu bấm số vượt quá số lượng vũ khí đang có trong WeaponHolder thì bỏ qua
		if (index >= transform.childCount) return;

		selectedWeapon = index;
		SelectWeapon(); // Tiến hành ẩn hiện vũ khí
	}

	private void SelectWeapon()
	{
		int i = 0;

		// Vòng lặp duyệt qua ĐÚNG thứ tự các object con nằm trong WeaponHolder từ trên xuống dưới
		foreach (Transform weapon in transform)
		{
			if (i == selectedWeapon)
			{
				weapon.gameObject.SetActive(true); // Hiện vũ khí được chọn (Gồm cả việc kích hoạt Script của súng/kiếm đó)
				Debug.Log($"🛡️ Trang bị: {weapon.name}");
			}
			else
			{
				weapon.gameObject.SetActive(false); // Ẩn các vũ khí còn lại đi
			}
			i++;
		}
	}
}