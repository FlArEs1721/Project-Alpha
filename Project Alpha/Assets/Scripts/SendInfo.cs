using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendInfo : MonoBehaviour
{
    [HideInInspector]
    public string songTitle = "";

    [HideInInspector]
    public string songDifficulty = "";

    private void Start()
    {
        if (GameObject.Find("SendInfoObject") != null)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        gameObject.name = "SendInfoObject";
    }
}
