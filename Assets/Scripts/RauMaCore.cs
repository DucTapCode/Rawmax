using UnityEngine;
using UnityEngine.UI; // Để dùng Slider hiển thị máu của Ly nếu cần

public class RauMaCore : MonoBehaviour
{
	[Header("Chỉ số Ly Rau Má")]
	[SerializeField] private float maxHealth = 2000f; // Máu của Ly trâu hơn người chơi rất nhiều
	private float currentHealth;

	[Header("Giao diện (Tùy chọn)")]
	[SerializeField] private Slider coreHealthSlider; // Thanh máu của Ly trên UI Canvas chính

	private bool isDestroyed = false;

	private void Start()
	{
		currentHealth = maxHealth;

		if (coreHealthSlider != null)
		{
			coreHealthSlider.maxValue = maxHealth;
			coreHealthSlider.value = maxHealth;
		}
	}

	// Hàm để quái vật gọi khi chúng tiếp cận và tấn công Ly
	public void TakeDamage(float damage)
	{
		if (isDestroyed) return;

		currentHealth -= damage;
		Debug.LogWarning($"🚨 LY RAU MÁ BỊ TẤN CÔNG! Máu còn lại: {currentHealth}/{maxHealth}");

		if (coreHealthSlider != null)
		{
			coreHealthSlider.value = currentHealth;
		}

		if (currentHealth <= 0)
		{
			CoreDestroyed();
		}
	}

	private void CoreDestroyed()
	{
		isDestroyed = true;
		Debug.LogError("💀 LY RAU MÁ ĐÃ VỠ! GAME OVER!");

		// Ngưng đọng thời gian game khi thua cuộc
		Time.timeScale = 0f;

		// Sau này ở Phase 5-6 chúng ta sẽ bật màn hình Game Over hiển thị nút Replay ở đây
	}
}