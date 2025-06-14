using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{    
    //The UI text for the health count
    public Text txt_healthCount;
    //The default value of the health count (used for init)
    public int defaultHealthCount;
    //Current health count
    public int healthCount;
    //[SerializeField] private TMP_Text txtTitle;
    //[SerializeField] private GameObject gameOverCanvas;

    public AudioClip baseDamageSound; // Assign suara damage di Inspector
    private AudioSource sfxAudioSource; // Referensi ke AudioSource

    void Awake()
    {
        // Dapatkan komponen AudioSource yang terpasang pada GameObject yang sama
        sfxAudioSource = GetComponent<AudioSource>();

        // Lakukan konfigurasi untuk AudioSource agar cocok untuk SFX
        if (sfxAudioSource != null)
        {
            sfxAudioSource.playOnAwake = false;
            sfxAudioSource.loop = false;
        }
        else
        {
            // Seharusnya tidak pernah terjadi jika [RequireComponent] digunakan, tapi ini sebagai pengaman
            Debug.LogError("HealthSystem.cs: Komponen AudioSource tidak ditemukan!", this.gameObject);
        }
    }
    //Init the health system (reset the health count)
    public void Init()
    {
        healthCount = defaultHealthCount;
        txt_healthCount.text = healthCount.ToString();
    }

    //Lose health count
    public void LoseHealth()
    {
        if (healthCount < 1)
            return;

        healthCount--;
        txt_healthCount.text = healthCount.ToString();

        if (sfxAudioSource != null && baseDamageSound != null)
        {
            sfxAudioSource.PlayOneShot(baseDamageSound);
            Debug.Log("SFX Health Base Damage");
        }
        Debug.Log("Health berkurang " + healthCount);

        CheckHealthCount();
    }

    //Check health count for losing
    void CheckHealthCount()
    {
        if (healthCount < 1)
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.TriggerLoseCondition();
            }
            else
            {
                Debug.LogError("GameManager.instance tidak ditemukan! Tidak bisa memicu kondisi kalah.");
            }
        }
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
