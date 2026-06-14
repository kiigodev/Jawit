using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDelay : MonoBehaviour
{
    [SerializeField] private string sceneToLoad; // Ketik nama scene tujuan di Inspector
    [SerializeField] private float delayInSeconds = 3f; // Waktu tunggu sebelum pindah

    void Start()
    {
        // Jalankan timer otomatis pas scene mulai
        StartCoroutine(LoadSceneAfterDelay());
    }

    void Update()
    {
        // Kalau pencet ESC, langsung skip tanpa nunggu timer
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadTargetScene();
        }
    }

    IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayInSeconds);
        LoadTargetScene();
    }

    void LoadTargetScene()
    {
        // Biar ga error pas manggil scene
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Bree, nama scene-nya belum lu isi di Inspector!");
        }
    }
}