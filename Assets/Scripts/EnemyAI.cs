using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
	[Header("Cấu hình Di chuyển")]
	[SerializeField] private float moveSpeed = 3.5f;
	[SerializeField] private float attackRange = 2.5f; // Khoảng cách để bắt đầu cắn Ly

	[Header("Cấu hình Tấn công")]
	[SerializeField] private float damage = 15f;
	[SerializeField] private float attackRate = 1.0f; // 1 giây cắn 1 phát

	private Transform targetCore;
	private Rigidbody rb;
	private float nextTimeToAttack = 0f;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();

		// Tự động đi tìm vật thể có Tag là "Core" trên bản đồ
		GameObject coreObj = GameObject.FindGameObjectWithTag("Core");
		if (coreObj != null)
		{
			targetCore = coreObj.transform;
		}
		else
		{
			Debug.LogError("❌ KHÔNG TÌM THẤY LY RAU MÁ NÀO CÓ TAG 'Core' TRÊN MAP!");
		}
	}

	private void FixedUpdate()
	{
		if (targetCore == null) return;

		// Tính khoảng cách từ quái đến Ly Rau Má
		float distanceToCore = Vector3.Distance(transform.position, targetCore.position);

		if (distanceToCore > attackRange)
		{
			// --- LOGIC DI CHUYỂN ---
			// Quay mặt về phía Ly Rau Má
			Vector3 direction = (targetCore.position - transform.position).normalized;
			direction.y = 0; // Không cho quái bị chúi đầu xuống đất

			Quaternion targetRotation = Quaternion.LookRotation(direction);
			rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f));

			// Di chuyển tới bằng Rigidbody để không bị xuyên tường/xuyên quái khác
			rb.MovePosition(transform.position + direction * moveSpeed * Time.fixedDeltaTime);
		}
		else
		{
			// --- LOGIC TẤN CÔNG ---
			if (Time.time >= nextTimeToAttack)
			{
				AttackCore();
			}
		}
	}

	private void AttackCore()
	{
		nextTimeToAttack = Time.time + attackRate;

		// Gọi hàm trừ máu của Ly Rau Má
		if (targetCore.TryGetComponent<RauMaCore>(out RauMaCore core))
		{
			core.TakeDamage(damage);
		}
	}
}