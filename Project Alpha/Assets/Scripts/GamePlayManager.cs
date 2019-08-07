using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{
    [HideInInspector]
    public float noteSpeed = 4f;

    [HideInInspector]
    public float bpm = 160;

    [HideInInspector]
    public const float NoteSpeedConstant = 160f;

    [HideInInspector]
    public const float NoteTouchExtraSizeX = 15f;

    [HideInInspector]
    public const float NoteTouchExtraSizeY = 100f;

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

    public void CreateNote(NoteType noteType, GameObject touchNotePrefab, GameObject slideNotePrefab)
    {
        GameObject note = null;
        switch (noteType)
        {
            case NoteType.Touch:
                note = Instantiate(touchNotePrefab, this.transform);
                note.SetActive(false);
                touchNotePool.Add(note);
                break;
            case NoteType.Slide:
                note = Instantiate(slideNotePrefab, this.transform);
                note.SetActive(false);
                slideNotePool.Add(note);
                break;
        }
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
        }
        return null;
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

    public Rank GetRank(float score)
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