using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    //void Awake() { instance = this; }

    public Spawner towerSpawner;
    public HealthSystem health;
    public CurrencySystem currency;

    public static int currentLevelIndex = 0;
    public int[] enemiesToWinPerLevel = { 15, 25, 35 };

    public string[] levelSceneNames = { "Level1", "Level2", "Level3" };

    private int enemiesKilled = 0;
    private int enemiesToWin;

    public GameObject winPanel;

    public AudioClip winSoundEffect;             
    private AudioSource sfxAudioSource;          
    public AudioSource backgroundMusicPlayer;

    void Awake()
    {
        // Pola Singleton
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); 
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Dapatkan komponen AudioSource pada GameObject ini untuk SFX
        sfxAudioSource = GetComponent<AudioSource>();
        if (sfxAudioSource != null)
        {
            sfxAudioSource.playOnAwake = false; // Pastikan tidak play on awake
            sfxAudioSource.loop = false;        // Pastikan tidak loop
        }
        else
        {
            Debug.LogError("GameManager memerlukan komponen AudioSource untuk SFX, tetapi tidak ditemukan!", this);
        }

        // Muat level index saat ini
        currentLevelIndex = PlayerPrefs.GetInt("CurrentLevelIndex", 0);
        if (currentLevelIndex >= levelSceneNames.Length)
        {
            currentLevelIndex = 0;
            PlayerPrefs.SetInt("CurrentLevelIndex", currentLevelIndex);
        }

        if (winPanel != null) winPanel.SetActive(false);
        Time.timeScale = 1f; // Pastikan waktu berjalan normal saat scene dimulai
    }

    void Start()
    {
        // Inisialisasi berdasarkan level saat ini
        if (currentLevelIndex < enemiesToWinPerLevel.Length)
        {
            enemiesToWin = enemiesToWinPerLevel[currentLevelIndex]; // Benar: set target menang untuk level ini
        }
        else
        {
            Debug.LogError("Pengaturan enemiesToWinPerLevel tidak cukup untuk level saat ini: " + currentLevelIndex);
            enemiesToWin = 9999;
        }

        // BENARKAN BARIS INI: Reset jumlah musuh yang SUDAH dikalahkan, bukan target untuk menang.
        enemiesKilled = 0; // Reset hitungan musuh yang sudah dikalahkan untuk level baru

        Debug.Log("Memulai Level: " + (currentLevelIndex + 1) + ". Target Musuh: " + enemiesToWin + " (Musuh dikalahkan: " + enemiesKilled + ")");

        // Inisialisasi sistem lain (mengambil dari bagian bawah skrip Anda yang tidak di-comment)
        if (health != null) health.Init();
        else if (GetComponent<HealthSystem>() != null) GetComponent<HealthSystem>().Init();
        else Debug.LogError("HealthSystem tidak ditemukan/di-assign pada GameManager!");

        if (currency != null) currency.Init();
        else if (GetComponent<CurrencySystem>() != null) GetComponent<CurrencySystem>().Init();
        else Debug.LogError("CurrencySystem tidak ditemukan/di-assign pada GameManager!");

        StartCoroutine(WaveStartDelay());
    }

    IEnumerator WaveStartDelay()
    {
        yield return new WaitForSeconds(2f);

        // Memulai spawning musuh
        // Pastikan referensi ke Spawner benar.
        // Jika 'spawner' (field publik) adalah EnemySpawner yang Anda maksud dan sudah di-assign:
        if (EnemySpawner.instance != null) // Mengasumsikan 'Spawner' adalah base class atau interface, dan Anda perlu EnemySpawner
        {
            EnemySpawner.instance.StartSpawning();
        }
        else
        {
            Debug.LogError("EnemySpawner tidak ditemukan");
        }
    }

    // --- METHOD UNTUK KONDISI MENANG ---

    // Method ini akan dipanggil dari Enemy.cs setiap kali musuh dikalahkan
    public void RegisterEnemyKilled()
    {
        enemiesKilled++;
        Debug.Log("Musuh dikalahkan! Total dikalahkan: " + enemiesKilled + " / " + enemiesToWin);

        if (enemiesKilled >= enemiesToWin)
        {
            TriggerWinCondition();
        }
    }

    void TriggerWinCondition()
    {
        Debug.Log("LEVEL " + (currentLevelIndex + 1) + " SELESAI!");
        Time.timeScale = 0f; // Hentikan permainan

        // --- HENTIKAN MUSIK LATAR ---
        if (backgroundMusicPlayer != null)
        {
            backgroundMusicPlayer.Stop();
            Debug.Log("Musik latar dihentikan.");
        }
        else
        {
            Debug.LogWarning("Referensi 'backgroundMusicPlayer' (AudioSource) belum di-assign di GameManager. Tidak bisa menghentikan musik latar.");
        }
        // -----------------------------

        // --- MAINKAN EFEK SUARA KEMENANGAN ---
        if (sfxAudioSource != null && winSoundEffect != null)
        {
            sfxAudioSource.PlayOneShot(winSoundEffect);
        }
        else
        {
            if (sfxAudioSource == null) Debug.LogWarning("sfxAudioSource pada GameManager tidak ditemukan untuk memainkan suara kemenangan.");
            if (winSoundEffect == null) Debug.LogWarning("AudioClip 'winSoundEffect' belum di-assign di GameManager Inspector.");
        }
        // -----------------------------------

        // Tampilkan panel kemenangan
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Win Panel belum di-assign di GameManager Inspector!");
        }

        // Hentikan spawning musuh
        if (EnemySpawner.instance != null)
        {
            EnemySpawner.instance.StopSpawning(); // Pastikan method ini ada di EnemySpawner.cs
        }
        else
        {
            Debug.LogError("EnemySpawner.instance tidak ditemukan untuk menghentikan spawning!");
        }
    }
    public void GoToNextLevel()
    {
        Time.timeScale = 1f; // Selalu kembalikan Time.timeScale sebelum pindah scene
        currentLevelIndex++;
        if (currentLevelIndex < levelSceneNames.Length)
        {
            PlayerPrefs.SetInt("CurrentLevelIndex", currentLevelIndex);
            PlayerPrefs.Save(); // Simpan perubahan PlayerPrefs
            SceneManager.LoadScene(levelSceneNames[currentLevelIndex]);
        }
        else
        {
            Debug.Log("Semua level telah selesai! Kembali ke Menu Utama.");
            // Mungkin ada layar "Game Tamat" sebelum ke menu utama
            GoToMainMenu(); // Kembali ke menu jika sudah level terakhir
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("CurrentLevelIndex", 0); // Reset ke level 1 untuk sesi berikutnya
        PlayerPrefs.Save();
        SceneManager.LoadScene("Menu"); // Ganti "MainMenu" dengan nama scene menu utama Anda
    }


    // Opsional: Method untuk mereset hitungan jika ada fitur restart level
    public void ResetKills()
    {
        enemiesKilled = 0;
        // Mungkin juga reset state lain jika perlu
    }

}
