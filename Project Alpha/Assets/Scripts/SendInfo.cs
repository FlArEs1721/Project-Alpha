using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendInfo : MonoBehaviour
{
    [HideInInspector]
    public string songTitle = "";

    [HideInInspector]
    public string songDifficulty = "";

    [HideInInspector]
    public int currentPackIndex = 0;
    [HideInInspector]
    public int currentSongIndex = 0;
    [HideInInspector]
    public SongDifficulty currentDifficulty = SongDifficulty.Hard;
    [HideInInspector]
    public bool songPlayed = false;

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
