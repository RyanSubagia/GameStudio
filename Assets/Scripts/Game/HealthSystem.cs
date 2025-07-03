using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{    
    public Text txt_healthCount;
    public int defaultHealthCount;
    public int healthCount;


    public AudioClip baseDamageSound;
    private AudioSource sfxAudioSource;

    void Awake()
    {
        sfxAudioSource = GetComponent<AudioSource>();

        if (sfxAudioSource != null)
        {
            sfxAudioSource.playOnAwake = false;
            sfxAudioSource.loop = false;
        }
        else
        {
            Debug.LogError("HealthSystem.cs: Komponen AudioSource tidak ditemukan!", this.gameObject);
        }
    }
    public void Init()
    {
        healthCount = defaultHealthCount;
        txt_healthCount.text = healthCount.ToString();
    }

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
