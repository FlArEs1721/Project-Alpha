using System.Collections.Generic;
using UnityEngine;

public static class Utility {
    public static float GetLerp(LerpType lerp, float currentTime, float time) {
        float t = currentTime / time;
        switch (lerp) {
            case LerpType.None:
                return t;
            case LerpType.SmoothIn:
                return 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
            case LerpType.SmoothOut:
                return Mathf.Sin(t * Mathf.PI * 0.5f);
            case LerpType.Smooth:
                //return Mathf.SmoothStep(0, 1, t);
                return t * t * t * (t * (6f * t - 15f) + 10f);
            default:
                return t;
        }
    }

    public static float GetMistakeTime(float noteSpeed, float yPosition) {
        // 노트의 실제 속도 계산 (pixel/ms)
        float realSpeed = (noteSpeed * GamePlayManager.NoteSpeedConstant) / 1000f;

        // yPosition의 절댓값을 노트의 실제 속도로 나눈 값을 반환
        return Mathf.Abs(yPosition) / realSpeed;
    }
}
