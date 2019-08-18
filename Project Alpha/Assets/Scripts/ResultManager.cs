using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public Text resultText;

    private SendInfo sendInfo;

    private void Start()
    {
        sendInfo = GameObject.Find("SendInfoObject").GetComponent<SendInfo>();

        resultText.text = string.Format("{0}\n{1}\n\n\nScore: {2}\nMax Combo: {3}\n\nPerfect: {4}\nNormal: {5}\nMiss: {6}", sendInfo.songTitle, string.Format("{0} {1}", sendInfo.songDifficulty, sendInfo.songDifficultyLevel), Mathf.Floor(sendInfo.results[0]), sendInfo.results[1], sendInfo.results[2], sendInfo.results[3], sendInfo.results[4]);
        resultText.text.Replace("\\n", "\n");
    }

    public void Retry()
    {
        SceneManager.LoadScene("Game");
    }

    public void Next()
    {
        SceneManager.LoadScene("SongSelect");
    }
}
