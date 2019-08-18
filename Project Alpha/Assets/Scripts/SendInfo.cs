using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendInfo : MonoBehaviour
{
    [HideInInspector]
    public float noteSpeed = 5;
    [HideInInspector]
    public float calibration = 0.12f;

    [HideInInspector]
    public string songTitle = "";
    [HideInInspector]
    public string songDifficulty = "";
    [HideInInspector]
    public string songDifficultyLevel = "";

    [HideInInspector]
    public int currentPackIndex = 0;
    [HideInInspector]
    public int currentSongIndex = 0;
    [HideInInspector]
    public SongDifficulty currentDifficulty = SongDifficulty.Hard;
    [HideInInspector]
    public bool songPlayed = false;

    /// <summary>
    /// 결과 (점수, 콤보, 퍼펙트, 노말, 미스)
    /// </summary>
    [HideInInspector]
    public float[] results = new float[5];

    private void Start()
    {
        /*
        if (GameObject.FindGameObjectsWithTag("GameController").Length > 1)
        {
            Destroy(gameObject);
        }
        */

        DontDestroyOnLoad(gameObject);
        gameObject.name = "SendInfoObject";
    }
}
