using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveSpawner : MonoBehaviour
{
	[Header("Mẫu Quái Vật (Prefab)")]
	[SerializeField] private GameObject enemyPrefab; // Kéo file Prefab quái màu xanh vào đây

	[Header("Điểm Sinh Quái (Spawn Points)")]
	[SerializeField] private Transform[] spawnPoints; // Danh sách các vị trí quái xuất hiện

	[Header("Thời gian giãn cách")]
	[SerializeField] private float timeBetweenWaves = 5f; // Thời gian nghỉ giữa 2 wave
	private float waveCountdown;

	private int currentWave = 0;
	private List<GameObject> activeEnemies = new List<GameObject>();
	private bool isSpawning = false;

	private void Start()
	{
		waveCountdown = timeBetweenWaves;
	}

	private void Update()
	{
		// Loại bỏ những con quái đã bị người chơi tiêu diệt (bị Destroy thành null) khỏi danh sách quản lý
		activeEnemies.RemoveAll(item => item == null);

		// Nếu đang sinh quái hoặc vẫn còn quái sống sót trên map thì không đếm ngược wave tiếp theo
		if (isSpawning || activeEnemies.Count > 0) return;

		if (waveCountdown <= 0f)
		{
			StartCoroutine(SpawnWaveRoutine());
			waveCountdown = timeBetweenWaves;
		}
		else
		{
			waveCountdown -= Time.deltaTime;
		}
	}

	private IEnumerator SpawnWaveRoutine()
	{
		isSpawning = true;
		currentWave++;
		Debug.LogWarning($"🏁 --- START WAVE {currentWave} ---");

		// Công thức tăng số lượng quái chuyên nghiệp: Wave 1 = 3 con, Wave 2 = 5 con, Wave 3 = 7 con...
		int enemiesToSpawn = 1 + (currentWave * 2);

		for (int i = 0; i < enemiesToSpawn; i++)
		{
			SpawnEnemy();
			yield return new WaitForSeconds(0.5f); // Cách 0.5 giây sinh 1 con để quái không bị dính chùm vào nhau
		}

		isSpawning = false;
	}

	private void SpawnEnemy()
	{
		if (spawnPoints.Length == 0) return;

		// Chọn ngẫu nhiên 1 điểm trong danh sách các vị trí Spawn Points
		Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

		// Sinh quái ra map
		GameObject enemy = Instantiate(enemyPrefab, randomPoint.position, randomPoint.rotation);

		// Thêm con quái mới sinh vào List để theo dõi
		activeEnemies.Add(enemy);
	}
}