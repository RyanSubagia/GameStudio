using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Pagination : MonoBehaviour
{
    public List<GameObject> pages;
    public Button previousButton;
    public Button nextButton;
    public TextMeshProUGUI pageIndicatorText;

    public AudioClip pageTurnSound;
    private AudioSource sfxAudioSource;

    private int currentPageIndex = 0;

    void Awake() 
    {
        sfxAudioSource = GetComponent<AudioSource>();

        if (sfxAudioSource != null)
        {
            sfxAudioSource.playOnAwake = false;
            sfxAudioSource.loop = false;       
            sfxAudioSource.spatialBlend = 0f;  
        }
        else
        {
            Debug.LogError("Pagination script pada " + gameObject.name + " memerlukan AudioSource, tapi tidak bisa ditemukan/dibuat.", this);
        }
    }

    void OnEnable()
    {
        currentPageIndex = 0; // Selalu mulai dari halaman pertama saat panel aktif
        ShowPage(currentPageIndex);
        UpdateNavigationButtons();
        UpdatePageIndicator();
    }

    void ShowPage(int index)
    {
        if (pages == null || pages.Count == 0) return;

        for (int i = 0; i < pages.Count; i++)
        {
            if (pages[i] != null)
            {
                pages[i].SetActive(i == index); // Aktifkan hanya halaman saat ini
            }
        }
    }

    private void PlayPageSound()
    {
        if (sfxAudioSource != null && pageTurnSound != null)
        {
            sfxAudioSource.PlayOneShot(pageTurnSound);
        }
        else
        {
            if (sfxAudioSource == null) Debug.LogWarning("AudioSource tidak ditemukan pada Pagination component di " + gameObject.name);
            if (pageTurnSound == null) Debug.LogWarning("AudioClip 'Page Turn Sound' belum di-assign pada Pagination component di " + gameObject.name);
        }
    }

    public void NextPage()
    {
        if (pages == null || pages.Count == 0) return;

        if (currentPageIndex < pages.Count - 1)
        {
            PlayPageSound();
            currentPageIndex++;
            ShowPage(currentPageIndex);
            UpdateNavigationButtons();
            UpdatePageIndicator();
        }
    }

    public void PreviousPage()
    {
        if (pages == null || pages.Count == 0) return;

        if (currentPageIndex > 0)
        {
            PlayPageSound();
            currentPageIndex--;
            ShowPage(currentPageIndex);
            UpdateNavigationButtons();
            UpdatePageIndicator();
        }
    }

    void UpdateNavigationButtons()
    {
        if (previousButton != null)
        {
            previousButton.interactable = (currentPageIndex > 0); // Nonaktifkan tombol Previous di halaman pertama
        }
        if (nextButton != null)
        {
            nextButton.interactable = (currentPageIndex < pages.Count - 1); // Nonaktifkan tombol Next di halaman terakhir
        }
    }

    void UpdatePageIndicator()
    {
        if (pageIndicatorText != null && pages != null && pages.Count > 0)
        {
            pageIndicatorText.text = "Halaman " + (currentPageIndex + 1) + " / " + pages.Count;
        }
        else if (pageIndicatorText != null)
        {
            pageIndicatorText.text = ""; // Kosongkan jika tidak ada halaman
        }
    }
}
