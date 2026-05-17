using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
	[Header("Chỉ số cơ bản")]
	[SerializeField] private float maxHealth = 500f;
	private float currentHealth;

	[Header("Giao diện Máu (UI)")]
	[SerializeField] private Slider healthSlider;

	[Header("Cơ chế Tĩnh Điện Cậu Vàng")]
	[SerializeField] private int maxStacks = 10;
	[SerializeField] private float stackTimeout = 3f;
	[SerializeField] private float explosionDamage = 150f;
	[SerializeField] private float explosionRadius = 5f;

	private int currentStacks = 0;
	private float lastHitTime;
	private bool isParalyzed = false;
	private Transform mainCamTransform;

	private void Start()
	{
		currentHealth = maxHealth;
		mainCamTransform = Camera.main.transform;

		if (healthSlider != null)
		{
			healthSlider.maxValue = maxHealth;
			healthSlider.value = maxHealth;
		}
	}

	private void Update()
	{
		if (currentStacks > 0 && Time.time - lastHitTime > stackTimeout)
		{
			ResetStacks();
		}

		if (healthSlider != null)
		{
			// Hiệu ứng Billboard xoay thanh máu về Camera
			healthSlider.transform.LookAt(healthSlider.transform.position + mainCamTransform.forward);
		}
	}

	public void TakeDamage(float damage)
	{
		if (currentHealth <= 0) return;

		currentHealth -= damage;

		if (healthSlider != null)
		{
			healthSlider.value = currentHealth;
		}

		if (currentHealth <= 0)
		{
			Die();
		}
	}

	public void AddFurStack()
	{
		if (currentHealth <= 0 || isParalyzed) return;

		currentStacks++;
		lastHitTime = Time.time;

		Debug.Log($"🎯 Găm lông! {transform.name} đang có {currentStacks}/{maxStacks} Stack");

		if (currentStacks >= maxStacks)
		{
			TriggerFurExplosion();
		}
	}

	private void TriggerFurExplosion()
	{
		Debug.Log("💥 BÙM! TĨNH ĐIỆN PHÁT NỔ DIỆN RỘNG! 💥");

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

		foreach (var collider in hitColliders)
		{
			if (collider.gameObject != this.gameObject && collider.CompareTag("Enemy"))
			{
				if (collider.TryGetComponent<EnemyHealth>(out EnemyHealth enemy))
				{
					enemy.TakeDamage(explosionDamage);
				}
			}
		}

		TakeDamage(explosionDamage * 1.5f);
		StartCoroutine(ParalyzeRoutine());
		ResetStacks();
	}

	// ĐÂY LÀ HÀM BỊ LỖI LƯỢT TRƯỚC - BẢN NÀY ĐÃ FIX CHUẨN
	private IEnumerator ParalyzeRoutine()
	{
		isParalyzed = true;
		Debug.Log($"🛑 {transform.name} bị TÊN LIỆT trong 2 giây!");

		yield return new WaitForSeconds(2f); // Dòng lệnh trả về giá trị bắt buộc phải có

		isParalyzed = false;
		Debug.Log($"🏃‍♂️ {transform.name} hết tê liệt.");
	}

	private void ResetStacks()
	{
		if (currentStacks > 0) Debug.Log("🍃 Lông rụng hết, reset stack về 0.");
		currentStacks = 0;
	}

	private void Die()
	{
		if (healthSlider != null) Destroy(healthSlider.gameObject);
		Debug.Log($"💀 {transform.name} đã hẻo!");
		Destroy(gameObject);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, explosionRadius);
	}
}