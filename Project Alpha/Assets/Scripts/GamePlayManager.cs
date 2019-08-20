using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{
    [HideInInspector]
    public float noteSpeed = 5f;

    /// <summary>
    /// 세부 싱크 (노트가 떨어지는 타이밍 조정)
    /// -> 숫자가 커질수록 판정선에 늦게 닿음
    /// (0에서 0.5 사이)
    /// </summary>
    [HideInInspector]
    public float calibration = 0.12f;

    [HideInInspector]
    public float bpm = 160;

    [HideInInspector]
    public const float NoteSpeedConstant = 160f;

    [HideInInspector]
    public const float NoteTouchExtraSizeX = 30f;

    [HideInInspector]
    public const float NoteTouchExtraSizeY = 150f;

    [HideInInspector]
    //public float score = 0;
    public int perfectCount = 0;

    [HideInInspector]
    public int normalCount = 0;

    [HideInInspector]
    public int missCount = 0;

    [HideInInspector]
    public int maxNoteCount = 0;

    [HideInInspector]
    public int currentCombo = 0;

    [HideInInspector]
    public int maxCombo = 0;

    [HideInInspector]
    public List<int> comboCountList = new List<int>();

    [HideInInspector]
    public bool isPaused = false;

    [HideInInspector]
    public List<GameObject> touchNotePool = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> slideNotePool = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> longNotePool = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> longNoteDummyPool = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> judgementObjectPool = new List<GameObject>();

    public void CreateNote(NoteType noteType, GameObject notePrefab, GameObject judgementPrefab)
    {
        GameObject note = null;
        GameObject judgementObj = null;

        switch (noteType)
        {
            case NoteType.Touch:
                note = Instantiate(notePrefab, this.transform);
                note.SetActive(false);
                touchNotePool.Add(note);
                break;
            case NoteType.Slide:
                note = Instantiate(notePrefab, this.transform);
                note.SetActive(false);
                slideNotePool.Add(note);
                break;
            case NoteType.Long:
                note = Instantiate(notePrefab, this.transform);
                note.SetActive(false);
                longNotePool.Add(note);
                break;
        }

        judgementObj = Instantiate(judgementPrefab, this.transform);
        judgementObj.SetActive(false);
        judgementObjectPool.Add(judgementObj);
    }

    public void CreateLongDummyNote(GameObject prefab)
    {
        GameObject note = null;
        note = Instantiate(prefab, this.transform);
        note.SetActive(false);
        longNoteDummyPool.Add(note);
    }

    public GameObject PullNote(NoteType noteType, GameObject noteLayer)
    {
        GameObject note = null;
        switch (noteType)
        {
            case NoteType.Touch:
                note = touchNotePool[0];
                touchNotePool.RemoveAt(0);
                note.transform.SetParent(noteLayer.transform);
                note.SetActive(true);
                return note;
            case NoteType.Slide:
                note = slideNotePool[0];
                slideNotePool.RemoveAt(0);
                note.transform.SetParent(noteLayer.transform);
                note.SetActive(true);
                return note;
            case NoteType.Long:
                note = longNotePool[0];
                longNotePool.RemoveAt(0);
                note.transform.SetParent(noteLayer.transform);
                note.SetActive(true);
                return note;
        }
        return null;
    }

    public GameObject PullLongNoteDummy(GameObject noteLayer)
    {
        GameObject note = longNoteDummyPool[0];
        longNoteDummyPool.RemoveAt(0);
        note.transform.SetParent(noteLayer.transform);
        note.SetActive(true);
        return note;
    }

    public GameObject PullJudgementObject(GameObject judgementLayer)
    {
        GameObject judgementObj = null;
        judgementObj = judgementObjectPool[0];
        judgementObjectPool.RemoveAt(0);
        judgementObj.transform.SetParent(judgementLayer.transform);
        judgementObj.SetActive(true);
        return judgementObj;
    }

    public float GetScore()
    {
        return 920000f * ((float)perfectCount / maxNoteCount) + 522000f * ((float)normalCount / maxNoteCount) + GetComboScore();
    }

    private int Sum(int n)
    {
        int result = 0;
        for (int i = 1; i <= n; i++)
            result += i;
        return result;
    }

    private float GetComboScore()
    {
        float result = 0;
        foreach (int count in comboCountList)
        {
            result += 80000 * ((float)Sum(count) / Sum(maxNoteCount));
        }
        result += 80000 * ((float)Sum(currentCombo) / Sum(maxNoteCount));
        return result;
    }

    public static Rank GetRank(float score)
    {
        if (score >= 1000000) return Rank.AllPerfect;
        else if (score >= 950000) return Rank.S;
        else if (score >= 900000) return Rank.A;
        else if (score >= 800000) return Rank.B;
        else if (score >= 700000) return Rank.C;
        else return Rank.Failed;
    }
}

public enum Rank
{
    AllPerfect,
    S,
    A,
    B,
    C,
    Failed
}