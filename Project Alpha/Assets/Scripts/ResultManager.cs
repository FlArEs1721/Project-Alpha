using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public Text resultText;

    private SendInfo sendInfo;

    private void Start()
    {
        sendInfo = GameObject.Find("SendInfoObject").GetComponent<SendInfo>();

        resultText.text = string.Format("Score: {0}\nMax Combo: {1}\n\nPerfect: {2}\nNormal: {3}\nMiss: {4}", Mathf.Floor(sendInfo.results[0]), sendInfo.results[1], sendInfo.results[2], sendInfo.results[3], sendInfo.results[4]);
        resultText.text.Replace("\\n", "\n");
    }
}
