using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeSystem : MonoBehaviour
{
	[Header("Chỉ số Cây Rau Má")]
	[SerializeField] private float damage = 120f;      // Sát thương cực đại (Gấp 12 lần súng)
	[SerializeField] private float attackRate = 1.2f;   // 1.2 giây mới đập được một phát (Tốc độ chậm)
	[SerializeField] private float attackRange = 3f;     // Tầm quét vòng cung (bán kính)
	[SerializeField] private float knockbackForce = 15f; // Lực đẩy lùi quái

	[Header("Điểm Quét Đòn (Attack Point)")]
	[SerializeField] private Transform attackPoint;     // Vị trí tâm của cú vung cây
	[SerializeField] private LayerMask enemyMask;       // Layer của quái vật

	private PlayerControls controls;
	private float nextTimeToAttack = 0f;

	private void Awake()
	{
		controls = new PlayerControls();

		// Nếu quên tạo Attack Point, tự động lấy chính vị trí họng cây rau má
		if (attackPoint == null) attackPoint = this.transform;
	}

	private void OnEnable()
	{
		// Dùng chung nút Chuột Trái (Fire Action) luôn cho tiện, vì khi đổi vũ khí, script súng tắt thì script này bật
		controls.Player.Fire.performed += ctx => SwingRauMa();
		controls.Player.Enable();
	}

	private void OnDisable()
	{
		controls.Player.Disable();
	}

	private void SwingRauMa()
	{
		if (Time.time < nextTimeToAttack) return;
		nextTimeToAttack = Time.time + attackRate;

		Debug.Log("🌿 VUNG CÂY RAU MÁ THẦN THÁNH!");

		// --- CƠ CHẾ QUÉT VÒNG CUNG (OVERLAP SPHERE) ---
		// Tạo một quả cầu vật lý vô hình tại attackPoint để bắt va chạm diện rộng
		Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyMask);

		foreach (Collider enemyCollider in hitEnemies)
		{
			// 1. Gây sát thương cực lớn lên quái
			if (enemyCollider.TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth))
			{
				enemyHealth.TakeDamage(damage);
				Debug.Log($"💥 Đập chao đảo trúng: {enemyCollider.name}");
			}

			// 2. Tác dụng lực đẩy lùi (Knockback) nếu quái có Rigidbody vật lý
			if (enemyCollider.TryGetComponent<Rigidbody>(out Rigidbody rb))
			{
				// Tính toán hướng đẩy: Từ người chơi hướng thẳng đến con quái
				Vector3 knockbackDirection = (enemyCollider.transform.position - transform.position).normalized;
				knockbackDirection.y = 0.2f; // Hất nhẹ quái lên trời nhìn cho đã mắt

				// Đẩy quái văng ra xa
				rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
				Debug.Log($"💨 Đã hất văng {enemyCollider.name} ra xa!");
			}
		}
	}

	// Vẽ vòng tròn tầm đánh trong Scene để dễ căn chỉnh
	private void OnDrawGizmosSelected()
	{
		if (attackPoint == null) return;
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(attackPoint.position, attackRange);
	}
}