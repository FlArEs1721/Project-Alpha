using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseControl : MonoBehaviour
{
    public Parser parser;
    public GameObject pausePanel;
    public AudioSource audioSource;

    private float fixedDeltaTime = 0;

    public void Pause()
    {
        Time.timeScale = 0;
        //fixedDeltaTime = Time.fixedDeltaTime;
        //Time.fixedDeltaTime = float.PositiveInfinity;
        parser.enabled = false;
        audioSource.Pause();
        pausePanel.SetActive(true);
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        audioSource.Play();
        Time.timeScale = 1;
        //Time.fixedDeltaTime = fixedDeltaTime;
        parser.enabled = true;
    }

    public void Retry()
    {
        StopAllCoroutines();
        Time.timeScale = 1;
        //Time.fixedDeltaTime = fixedDeltaTime;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        StopAllCoroutines();
        Time.timeScale = 1;
        //Time.fixedDeltaTime = fixedDeltaTime;
        SceneManager.LoadScene("SongSelect");
    }
}
