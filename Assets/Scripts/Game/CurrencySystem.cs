using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CurrencySystem : MonoBehaviour
{
    //FIELDS
    //currency txt UI
    public Text txt_Currency;
    //default currency value
    public int defaultCurrency;
    //current currency value
    public int currency;

    public AudioClip coinGainSound; // Assign suara damage di Inspector
    public float coinGainSoundVolume = 0.8f;
    private AudioSource sfxAudioSource;

    void Awake()
    {
        // Dapatkan komponen AudioSource yang terpasang pada GameObject yang sama ini
        sfxAudioSource = GetComponent<AudioSource>();
        if (sfxAudioSource != null)
        {
            // Konfigurasi AudioSource agar cocok untuk SFX
            sfxAudioSource.playOnAwake = false;
            sfxAudioSource.loop = false;
            sfxAudioSource.spatialBlend = 0f; // Set ke 2D untuk suara UI/sistem
        }
        else
        {
            Debug.LogError("CurrencySystem.cs: Komponen AudioSource tidak ditemukan!", this.gameObject);
        }
    }
    //METHODS
    //Init (set the default values)
    public void Init()
    {
        currency = defaultCurrency;
        UpdateUI();
    }
    //Gain currency (input of value)
    public void Gain(int val)
    {
        currency += val;
        UpdateUI();

        if (sfxAudioSource != null && coinGainSound != null)
        {
            // Gunakan volume yang sudah di-set sebagai parameter kedua
            sfxAudioSource.PlayOneShot(coinGainSound, coinGainSoundVolume);
        }
    }
    //Use currency (input of value)
    public bool Use(int val)
    {
        if (EnoughCurrency(val))
        {
            currency -= val;
            UpdateUI();
            return true;
        }
        else
        {
            return false;
        }
    }
    //Check availability of currency
    public bool EnoughCurrency(int val)
    {
        //Check if the val is equal or more than currency
        if (val <= currency)
            return true;
        else
            return false;
    }
    //Update txt ui
    void UpdateUI()
    {
        txt_Currency.text = currency.ToString();
    }

    public void USE_TEST()
    {
        Debug.Log(Use(3));
    }

}
