using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongTextControl : MonoBehaviour {
    public Text titleText;
    public Text difficultyText;

    private SendInfo sendInfo;

    private void Start() {
        sendInfo = GameObject.Find("SendInfoObject").GetComponent<SendInfo>();
        titleText.text = sendInfo.songTitle;
        difficultyText.text = string.Format("{0} {1}", sendInfo.songDifficulty, sendInfo.songDifficultyLevel);
    }
}
