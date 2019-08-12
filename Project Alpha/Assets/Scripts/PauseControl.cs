using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseControl : MonoBehaviour
{
    public GameObject pausePanel;
    public AudioSource audioSource;

    private float fixedDeltaTime = 0;

    public void Pause()
    {
        Time.timeScale = 0;
        fixedDeltaTime = Time.fixedDeltaTime;
        Time.fixedDeltaTime = float.PositiveInfinity;
        audioSource.Pause();
        pausePanel.SetActive(true);
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        audioSource.Play();
        Time.timeScale = 1;
        Time.fixedDeltaTime = fixedDeltaTime;
    }

    public void Retry()
    {
        StopAllCoroutines();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        // TODO: 메인메뉴 만들기 & 연결
        StopAllCoroutines();
        Time.timeScale = 1;
        SceneManager.LoadScene("SongSelect");
    }
}
