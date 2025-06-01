using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class Menu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject creditsPanel;
    public GameObject tutorialPanel;

    public AudioClip buttonClickSound;
    private AudioSource sfxAudioSource;

    void Awake()
    {
        sfxAudioSource = GetComponent<AudioSource>();
        if (sfxAudioSource == null)
        {
            Debug.LogError("AudioSource untuk SFX tidak ditemukan pada _MenuManager!", this);
        }
    }

    void Start()
    {
        ActivateCorrectPanel(mainMenuPanel);
    }

    private void ActivateCorrectPanel(GameObject panelToActivate)
    {
        // Nonaktifkan semua panel yang mungkin aktif terlebih dahulu
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);

        // Kemudian aktifkan panel yang diinginkan
        if (panelToActivate != null)
        {
            panelToActivate.SetActive(true);
        }
    }

    public void ButtonClickSound()
    {
        if (sfxAudioSource != null && buttonClickSound != null)
        {
            sfxAudioSource.PlayOneShot(buttonClickSound);
        }
        else
        {
            if (sfxAudioSource == null) Debug.LogWarning("sfxAudioSource belum di-assign/ditemukan di Menu.cs");
            if (buttonClickSound == null) Debug.LogWarning("AudioClip 'buttonClickSound' belum di-assign di Menu.cs Inspector");
        }
    }

    public void StartGame()
    {
        ButtonClickSound();
        PlayerPrefs.SetInt("CurrentLevelIndex", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Level1");
    }

    public void ShowTutorialPanel()
    {
        ButtonClickSound();
        ActivateCorrectPanel(tutorialPanel);

    }
    public void ShowCreditsPanel() // Nama sebelumnya: OpenCredits
    {
        ButtonClickSound();
        ActivateCorrectPanel(creditsPanel);
    }

    public void ShowMainMenu()
   {
        ButtonClickSound();
        ActivateCorrectPanel(mainMenuPanel);
    }
}
