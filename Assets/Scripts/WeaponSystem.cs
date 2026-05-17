using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSystem : MonoBehaviour
{
	[Header("Cấu hình Vũ khí")]
	[SerializeField] private float damage = 10f;
	[SerializeField] private float fireRate = 0.08f; // 0.08 giây/viên -> Sấy cực nhanh
	[SerializeField] private float range = 100f;
	[SerializeField] private LayerMask enemyMask;

	[Header("Cơ chế Quá Nhiệt (Tự Thương)")]
	[SerializeField][Range(0f, 100f)] private float backfireChance = 7f; // 7% tỉ lệ lông bay ngược vào mặt khi spam
	[SerializeField] private float selfDamage = 8f; // Sát thương người chơi tự gánh

	[Header("Góc nhìn")]
	[SerializeField] private Transform camTransform;

	private PlayerControls controls;
	private float nextTimeToFire = 0f;
	private bool isFiring = false; // Biến kiểm tra người chơi có đang giữ chuột không

	private void Awake()
	{
		controls = new PlayerControls();
		if (camTransform == null) camTransform = Camera.main.transform;
	}

	private void OnEnable()
	{
		// Khi người chơi vừa nhấn giữ chuột trái
		controls.Player.Fire.started += ctx => isFiring = true;
		// Khi người chơi thả chuột trái ra
		controls.Player.Fire.canceled += ctx => isFiring = false;

		controls.Player.Enable();
	}

	private void OnDisable()
	{
		controls.Player.Disable();
		isFiring = false; // Tắt sấy nếu súng bị cất đi
	}

	private void Update()
	{
		// Nếu đang giữ chuột thì liên tục gọi hàm bắn theo nhịp cooldown
		if (isFiring)
		{
			AttemptFire();
		}
	}

	private void AttemptFire()
	{
		if (Time.time < nextTimeToFire) return;
		nextTimeToFire = Time.time + fireRate;

		// --- TÍNH TỈ LỆ ĐẠN PHẢN PHÁO (TỰ ĐÂM PLAYER) ---
		if (Random.Range(0f, 100f) < backfireChance)
		{
			TriggerSelfDamage();
			return; // Viên đạn này bị lỗi, nổ ngay tại họng súng, không bay ra ngoài nữa
		}

		// --- LOGIC BẮN RAYCAST BÌNH THƯỜNG ---
		RaycastHit hit;
		if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, range, enemyMask))
		{
			if (hit.transform.TryGetComponent<EnemyHealth>(out EnemyHealth enemy))
			{
				enemy.TakeDamage(damage);
				enemy.AddFurStack();
			}
		}
	}

	private void TriggerSelfDamage()
	{
		// Tạm thời xuất thông báo lỗi bựa lên Console, Phase sau có hệ thống máu Player sẽ trừ thẳng vào đó
		Debug.LogWarning($"⚠️ SÚNG QUÁ NHIỆT! Lông Cậu Vàng bay ngược cắm vào mặt Player! Tự mất {selfDamage} máu.");
	}
}