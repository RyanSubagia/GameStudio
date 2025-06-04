using UnityEngine;
using UnityEngine.UI; // Diperlukan untuk Button
using UnityEngine.SceneManagement;
using System.Collections; // Diperlukan untuk IEnumerator

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("System References")]
    public Spawner towerSpawner;
    public HealthSystem health;
    public CurrencySystem currency;

    [Header("Level Management")]
    public static int currentLevelIndex = 0;
    public int[] enemiesToWinPerLevel = { 15, 25, 35 };
    public string[] levelSceneNames = { "Level1", "Level2", "Level3" };

    private int enemiesKilledThisLevel = 0;
    private int enemiesToWinThisLevel;

    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject gameOverPanel;

    [Header("Audio Settings")]
    public AudioClip winSoundEffect;
    public AudioClip loseSoundEffect;
    public AudioClip nuclearExplosionSound;
    public AudioSource backgroundMusicPlayer;
    private AudioSource sfxAudioSource;

    // --- Pengaturan untuk Bom Nuklir ---
    [Header("Nuclear Bomb Settings")]
    public Button nuclearBombButton;
    public int nuclearBombCost = 100;
    public GameObject nuclearExplosionAnimationPrefab; // Prefab animasi ledakan dari sprite sheet
    public Transform nuclearExplosionSpawnPoint;   // Titik munculnya animasi ledakan sprite sheet
    // public Animator screenFlashAnimator;        // << DIHAPUS
    // ------------------------------------

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        sfxAudioSource = GetComponent<AudioSource>();
        if (sfxAudioSource != null)
        {
            sfxAudioSource.playOnAwake = false;
            sfxAudioSource.loop = false;
        }
        else
        {
            Debug.LogError("GameManager memerlukan komponen AudioSource untuk SFX!", this);
        }

        if (winPanel != null) winPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        Time.timeScale = 1f;

        if (nuclearBombButton != null)
        {
            nuclearBombButton.onClick.AddListener(TryActivateNuclearBomb);
        }
    }

    void Start()
    {
        if (currentLevelIndex < 0 || currentLevelIndex >= levelSceneNames.Length)
        {
            Debug.LogError($"currentLevelIndex ({currentLevelIndex}) di luar batas untuk levelSceneNames. Reset ke 0.");
            currentLevelIndex = 0;
        }

        if (currentLevelIndex < enemiesToWinPerLevel.Length)
        {
            enemiesToWinThisLevel = enemiesToWinPerLevel[currentLevelIndex];
        }
        else
        {
            Debug.LogError("Pengaturan enemiesToWinPerLevel tidak cukup untuk level saat ini: " + currentLevelIndex + ". Default ke 9999.");
            enemiesToWinThisLevel = 9999;
        }

        enemiesKilledThisLevel = 0;
        Debug.Log("Memulai Level: " + (currentLevelIndex + 1) + " (" + SceneManager.GetActiveScene().name + "). Target Musuh: " + enemiesToWinThisLevel);

        if (health != null) health.Init();
        else Debug.LogError("HealthSystem tidak ditemukan/di-assign pada GameManager!");

        if (currency != null) currency.Init();
        else Debug.LogError("CurrencySystem tidak ditemukan/di-assign pada GameManager!");

        if (backgroundMusicPlayer != null && !backgroundMusicPlayer.isPlaying)
        {
            backgroundMusicPlayer.Play();
        }

        StartCoroutine(WaveStartDelay());
        UpdateNuclearBombButtonInteractable();
    }

    void Update()
    {
        UpdateNuclearBombButtonInteractable();
    }

    IEnumerator WaveStartDelay()
    {
        yield return new WaitForSeconds(2f);
        if (EnemySpawner.instance != null)
        {
            EnemySpawner.instance.StartSpawning();
        }
        else
        {
            Debug.LogError("EnemySpawner.instance tidak ditemukan! Tidak bisa memulai spawning musuh.");
        }
    }

    public void RegisterEnemyKilled()
    {
        if (winPanel != null && winPanel.activeSelf) return;

        enemiesKilledThisLevel++;
        if (enemiesKilledThisLevel >= enemiesToWinThisLevel)
        {
            TriggerWinCondition();
        }
    }

    void TriggerWinCondition()
    {
        if (winPanel != null && winPanel.activeSelf) return;

        Debug.Log("LEVEL " + (currentLevelIndex + 1) + " SELESAI!");
        Time.timeScale = 0f;

        if (backgroundMusicPlayer != null) backgroundMusicPlayer.Stop();
        if (sfxAudioSource != null && winSoundEffect != null) sfxAudioSource.PlayOneShot(winSoundEffect);
        if (winPanel != null) winPanel.SetActive(true);
        else Debug.LogWarning("Win Panel belum di-assign!");
        if (EnemySpawner.instance != null) EnemySpawner.instance.StopSpawning();
    }

    public void TriggerLoseCondition()
    {
        if (gameOverPanel != null && gameOverPanel.activeSelf) return;
        if (winPanel != null && winPanel.activeSelf) return;

        Debug.Log("GAME OVER!");
        Time.timeScale = 0f;

        if (backgroundMusicPlayer != null) backgroundMusicPlayer.Stop();
        if (sfxAudioSource != null && loseSoundEffect != null) sfxAudioSource.PlayOneShot(loseSoundEffect);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        else Debug.LogWarning("GameOver Panel belum di-assign!");
        if (EnemySpawner.instance != null) EnemySpawner.instance.StopSpawning();
    }

    public void GoToNextLevel()
    {
        Time.timeScale = 1f;
        currentLevelIndex++;
        if (currentLevelIndex < levelSceneNames.Length)
        {
            PlayerPrefs.SetInt("CurrentLevelIndex", currentLevelIndex);
            PlayerPrefs.Save();
            SceneManager.LoadScene(levelSceneNames[currentLevelIndex]);
        }
        else
        {
            Debug.Log("Semua level selesai! Kembali ke Menu.");
            SceneManager.LoadScene("Menu");
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    // --- Logika Bom Nuklir ---
    void UpdateNuclearBombButtonInteractable()
    {
        if (nuclearBombButton != null && currency != null)
        {
            nuclearBombButton.interactable = (Time.timeScale > 0f) && currency.EnoughCurrency(nuclearBombCost);
        }
    }

    public void TryActivateNuclearBomb()
    {
        if (currency != null && currency.Use(nuclearBombCost))
        {
            StartCoroutine(ActivateNuclearBombSequence());
        }
        else
        {
            Debug.Log("Not enough gold for Nuclear Bomb!");
        }
    }

    private IEnumerator ActivateNuclearBombSequence()
    {
        if (nuclearBombButton != null) nuclearBombButton.interactable = false;

        // Mainkan efek visual ledakan sprite sheet di lokasi tertentu
        if (nuclearExplosionAnimationPrefab != null && nuclearExplosionSpawnPoint != null)
        {
            Instantiate(nuclearExplosionAnimationPrefab, nuclearExplosionSpawnPoint.position, Quaternion.identity);
        }
        else if (nuclearExplosionAnimationPrefab != null)
        {
            Debug.LogWarning("NuclearExplosionSpawnPoint tidak di-set, ledakan muncul di (0,0,0).");
            Instantiate(nuclearExplosionAnimationPrefab, Vector3.zero, Quaternion.identity);
        }

        // Mainkan efek visual screen flash tambahan (jika ada)
        // if (screenFlashAnimator != null) // << BAGIAN INI DIHAPUS/DIKOMENTARI
        // {
        //     screenFlashAnimator.SetTrigger("Explode"); 
        // }

        // Mainkan suara ledakan
        if (sfxAudioSource != null && nuclearExplosionSound != null)
        {
            sfxAudioSource.PlayOneShot(nuclearExplosionSound);
        }

        // Guncangkan Kamera
        if (CameraShake.instance != null)
        {
            CameraShake.instance.StartShake(0.5f, 0.1f); // Sesuaikan durasi dan kekuatan
        }

        // Hancurkan semua musuh
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObject in allEnemies)
        {
            if (enemyObject != null)
            {
                Enemy enemyScript = enemyObject.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.LoseHealth(9999999);
                }
            }
        }
        yield return null;
    }
}