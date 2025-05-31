using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;
    //void Awake(){instance=this;}

    //Enemy prefabs
    public List<GameObject> prefabs;
    //Enemy spawn root points
    public List<Transform> spawnPoints;
    //Enemy spawn interval
    public float spawnInterval = 2f;

    public float[] spawnIntervalsPerLevel = { 2.0f, 1.5f, 1.0f }; // Interval per level
    private float currentSpawnInterval;

    private Coroutine spawningCoroutine;

    void Awake()
    {
        if (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }

        SetupForCurrentLevel();
    }

    void SetupForCurrentLevel()
    {
        int levelIdx = GameManager.currentLevelIndex;

        if (levelIdx < spawnIntervalsPerLevel.Length)
        {
            currentSpawnInterval = spawnIntervalsPerLevel[levelIdx];
        }
        else
        {
            // Fallback ke interval terakhir jika level index di luar jangkauan array
            currentSpawnInterval = spawnIntervalsPerLevel[spawnIntervalsPerLevel.Length - 1];
        }

        // Pengecekan apakah list prefabs sudah diisi di Inspector untuk scene ini
        if (prefabs == null || prefabs.Count == 0)
        {
            Debug.LogError("List 'prefabs' di EnemySpawner belum di-assign atau kosong untuk level ini! Periksa Inspector.", gameObject);
        }

        Debug.Log("EnemySpawner setup untuk Level " + (levelIdx + 1) + ". Interval: " + currentSpawnInterval);
    }

    public void StartSpawning()
    {
        // Cek list 'prefabs' utama yang dikonfigurasi untuk scene ini
        if (prefabs == null || prefabs.Count == 0)
        {
            Debug.LogWarning("Tidak bisa memulai spawning, tidak ada prefab musuh di List 'prefabs' untuk level ini. Periksa Inspector EnemySpawner.", gameObject);
            return;
        }

        // Hentikan coroutine sebelumnya jika ada (untuk mencegah duplikasi jika StartSpawning dipanggil lagi)
        if (spawningCoroutine != null)
        {
            StopCoroutine(spawningCoroutine);
        }

        // Mulai coroutine baru dan simpan referensinya
        spawningCoroutine = StartCoroutine(SpawnLoop()); // Pastikan Anda memiliki method IEnumerator SpawnLoop()
        Debug.Log("Enemy spawning dimulai.");
    }

    public void StopSpawning()
    {
        if (spawningCoroutine != null)
        {
            StopCoroutine(spawningCoroutine);
            spawningCoroutine = null; // Reset referensi
            Debug.Log("Enemy spawning stopped.");
        }
    }

    IEnumerator SpawnLoop() // Mengganti nama dari SpawnDelay dan mengubah logika loop
    {
        while (true) // Loop ini akan berjalan terus sampai coroutine dihentikan
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator SpawnDelay()
    {
        //Call the spawn method
        SpawnEnemy();
        //Wait spawn interval
        yield return new WaitForSeconds(spawnInterval);
        //Recall the same coroutine
        StartCoroutine(SpawnDelay());
    }

    void SpawnEnemy()
    {
        // Pastikan prefabs dan spawnPoints sudah di-assign dan tidak kosong
        if (prefabs == null || prefabs.Count == 0 || spawnPoints == null || spawnPoints.Count == 0)
        {
            // Debug.LogWarning("Prefabs atau SpawnPoints belum di-assign atau kosong di EnemySpawner.", this);
            return;
        }

        int randomPrefabID = Random.Range(0, prefabs.Count); // Gunakan prefabs
        int randomSpawnPointID = Random.Range(0, spawnPoints.Count);

        GameObject spawnedEnemyObj = Instantiate(prefabs[randomPrefabID], spawnPoints[randomSpawnPointID].position, spawnPoints[randomSpawnPointID].rotation);

        Enemy enemyScript = spawnedEnemyObj.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.InitializeStatsForLevel(GameManager.currentLevelIndex);
        }

    }
}
