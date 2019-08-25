using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public Scrollbar noteSpeedScrollbar;
    public Scrollbar calibrationScrollbar;
    public Text noteSpeedText;
    public Text calibrationText;

    private SendInfo sendInfo;

    private void Start()
    {
        sendInfo = GameObject.Find("SendInfoObject").GetComponent<SendInfo>();
        noteSpeedText.text = sendInfo.noteSpeed.ToString();
        calibrationText.text = sendInfo.calibration.ToString();
        noteSpeedScrollbar.value = ((sendInfo.noteSpeed * 2) - 2) / 18f;
        calibrationScrollbar.value = sendInfo.calibration * 2;
    }

    public void IncreaseNoteSpeed()
    {
        sendInfo.noteSpeed += 0.5f;

        if (sendInfo.noteSpeed > 10)
        {
            sendInfo.noteSpeed = 10;
        }

        noteSpeedText.text = sendInfo.noteSpeed.ToString();
        noteSpeedScrollbar.value = ((sendInfo.noteSpeed * 2) - 2) / 18f;
    }

    public void DecreaseNoteSpeed()
    {
        sendInfo.noteSpeed -= 0.5f;

        if (sendInfo.noteSpeed < 1f)
        {
            sendInfo.noteSpeed = 1f;
        }

        noteSpeedText.text = sendInfo.noteSpeed.ToString();
        noteSpeedScrollbar.value = ((sendInfo.noteSpeed * 2) - 2) / 18f;
    }

    public void IncreaseCalibration()
    {
        sendInfo.calibration += 0.01f;
        sendInfo.calibration = (float)Math.Round((double)sendInfo.calibration, 2);

        if (sendInfo.calibration > 0.5f)
        {
            sendInfo.calibration = 0.5f;
        }

        calibrationText.text = sendInfo.calibration.ToString();
        calibrationScrollbar.value = sendInfo.calibration * 2;
    }

    public void DecreaseCalibration()
    {
        sendInfo.calibration -= 0.01f;
        sendInfo.calibration = (float)Math.Round((double)sendInfo.calibration, 2);

        if (sendInfo.calibration < 0f)
        {
            sendInfo.calibration = 0f;
        }

        calibrationText.text = sendInfo.calibration.ToString();
        calibrationScrollbar.value = sendInfo.calibration * 2;
    }

    public void Back()
    {
        SceneManager.LoadScene("SongSelect");
    }
}
