using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance; // Singleton untuk akses mudah

    private Vector3 originalPosition;
    private Coroutine currentShakeCoroutine;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Hancurkan instance duplikat
        }
    }

    void Start()
    {
        originalPosition = transform.localPosition; // Simpan posisi awal kamera
    }

    public void StartShake(float duration, float magnitude)
    {
        // Hentikan guncangan sebelumnya jika ada
        if (currentShakeCoroutine != null)
        {
            StopCoroutine(currentShakeCoroutine);
            transform.localPosition = originalPosition; // Kembalikan ke posisi awal dulu
        }
        currentShakeCoroutine = StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null; // Tunggu frame berikutnya
        }

        transform.localPosition = originalPosition; // Kembalikan kamera ke posisi semula setelah selesai
        currentShakeCoroutine = null;
    }
}