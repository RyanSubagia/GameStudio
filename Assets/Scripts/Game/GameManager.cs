using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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

    void Awake()
    {
        // Pola Singleton yang lebih aman
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // Jika GameManager harus ada terus antar scene
        }
        else if (instance != this) // Jika sudah ada instance lain dan itu bukan diri sendiri
        {
            Destroy(gameObject); // Hancurkan diri sendiri agar hanya ada satu instance
            return;
        }

        // Muat level index saat ini (default ke 0 jika belum ada)
        currentLevelIndex = PlayerPrefs.GetInt("CurrentLevelIndex", 0);
        // Pastikan currentLevelIndex tidak melebihi jumlah level yang ada
        if (currentLevelIndex >= levelSceneNames.Length)
        {
            currentLevelIndex = 0; // Kembali ke level pertama atau ke menu utama
            PlayerPrefs.SetInt("CurrentLevelIndex", currentLevelIndex);
            // SceneManager.LoadScene("MainMenu"); // Contoh jika melebihi batas
            // return;
        }


        if (winPanel != null) winPanel.SetActive(false);
        // if (losePanel != null) losePanel.SetActive(false);

        // Set Time.timeScale ke 1 setiap kali scene dimulai (penting jika sebelumnya di-set ke 0)
        Time.timeScale = 1f;
    }

    void Start()
    {
        // Inisialisasi HealthSystem dan CurrencySystem
        // Cara Anda memanggil GetComponent().Init() mengasumsikan HealthSystem & CurrencySystem
        // adalah komponen yang terpasang pada GameObject yang sama dengan GameManager ini.
        // Jika field publik 'health' dan 'currency' sudah Anda assign di Inspector,
        // pemanggilan GetComponent tidak lagi wajib, Anda bisa langsung:
        // if (health != null) health.Init();
        // if (currency != null) currency.Init();

        //HealthSystem hs = GetComponent<HealthSystem>();
        //if (hs != null)
        //{
        //    hs.Init();
        //}
        //else if (health != null) // Fallback jika diassign via public field
        //{
        //    health.Init();
        //}
        //else
        //{
        //    Debug.LogError("HealthSystem tidak ditemukan atau tidak di-assign pada GameManager!");
        //}

        //CurrencySystem cs = GetComponent<CurrencySystem>();
        //if (cs != null)
        //{
        //    cs.Init();
        //}
        //else if (currency != null) // Fallback jika diassign via public field
        //{
        //    currency.Init();
        //}
        //else
        //{
        //    Debug.LogError("CurrencySystem tidak ditemukan atau tidak di-assign pada GameManager!");
        //}

        //StartCoroutine(WaveStartDelay());
        // Inisialisasi berdasarkan level saat ini
        // Inisialisasi HealthSystem dan CurrencySystem
        // (Saya akan menggunakan blok kode inisialisasi yang tidak di-comment dari skrip Anda)

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
        //Debug.Log("SELAMAT! ANDA TELAH MEMENANGKAN PERMAINAN!");

        //// ----- Implementasikan logika kemenangan Anda di sini -----
        //// Contoh:
        //// 1. Menghentikan waktu permainan (pause)
        //Time.timeScale = 0f;

        //// 2. Menampilkan UI Panel Kemenangan
        //if (winPanel != null)
        //{
        //    winPanel.SetActive(true);
        //}

        //// 3. Memuat Scene Kemenangan (pastikan scene sudah ada di Build Settings)
        //// SceneManager.LoadScene("NamaSceneKemenangan");

        //// 4. Menghentikan spawning musuh lebih lanjut
        ////    Anda perlu cara untuk mengakses spawner Anda dan memberitahunya untuk berhenti.
        ////    Misalnya, jika EnemySpawner memiliki method StopSpawning():
        //if (EnemySpawner.instance != null)
        //{
        //    EnemySpawner.instance.StopSpawning();
        //}
        //else
        //{
        //    Debug.LogError("EnemySpawner.instance tidak ditemukan untuk menghentikan spawning!");
        //}

        Debug.Log("LEVEL " + (currentLevelIndex + 1) + " SELESAI!");
        Time.timeScale = 0f; // Hentikan permainan

        if (winPanel != null)
        {
            winPanel.SetActive(true);
            // Di sini Anda bisa memperbarui teks di winPanel untuk menunjukkan level mana yang selesai, dll.
            // Anda juga perlu mengkonfigurasi tombol di winPanel (lihat poin III)
        }
        else
        {
            Debug.LogWarning("Win Panel belum di-assign di GameManager Inspector!");
        }

        if (EnemySpawner.instance != null)
        {
            EnemySpawner.instance.StopSpawning();
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
