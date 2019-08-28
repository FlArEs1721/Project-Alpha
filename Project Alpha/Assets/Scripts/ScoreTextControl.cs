using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTextControl : MonoBehaviour {
    private Text scoreText;

    private void Awake() {
        scoreText = gameObject.GetComponent<Text>();
    }

    private void Update() {
        scoreText.text = GetScoreText();
        //Debug.Log(GamePlayManager.Instance.GetScore() + ", " + GamePlayManager.Instance.perfectCount);
    }

    private string GetScoreText() {
        int score = Mathf.FloorToInt(GamePlayManager.Instance.GetScore());
        //Debug.Log(GamePlayManager.Instance.perfectCount + ", " + GamePlayManager.Instance.maxNoteCount + ", " + (GamePlayManager.Instance.perfectCount / GamePlayManager.Instance.maxNoteCount));
        if (score >= 1000000) return "1000000";
        else if (score >= 100000) return score.ToString();
        else if (score >= 10000) return "0" + score;
        else if (score >= 1000) return "00" + score;
        else if (score >= 100) return "000" + score;
        else if (score >= 10) return "0000" + score;
        else if (score >= 1) return "00000" + score;
        else return "000000";
    }
}
