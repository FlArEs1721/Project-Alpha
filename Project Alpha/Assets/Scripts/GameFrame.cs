using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFrame : MonoBehaviour
{
    public GameObject bgSprite;
    public GameObject noteLayer;
    public GameObject judgementLayer;
    public GameObject judgementLine;

    /// <summary>
    /// 게임프레임의 위치
    /// </summary>
    [HideInInspector]
    public Vector2 position = Vector2.zero;
    /// <summary>
    /// 게임프레임의 회전 각도
    /// </summary>
    [HideInInspector]
    public float rotation = 0;
    /// <summary>
    /// 게임프레임의 크기
    /// </summary>
    [HideInInspector]
    public Vector2 frameSize = Vector2.zero;
    private Vector2 preFrameSize = Vector2.zero;
    /// <summary>
    /// 노트의 x크기
    /// </summary>
    [HideInInspector]
    public float noteXSize = 0;

    /// <summary>
    /// 판정선의 너비
    /// </summary>
    [HideInInspector]
    public const float JudgementLineWidth = 13f;

    /// <summary>
    /// 게임프레임에 있는 노트들의 리스트
    /// </summary>
    [HideInInspector]
    public List<Note> noteList = new List<Note>();

    private bool isCreating = true;
    private bool moveFrameEnabled = false;
    private bool rotateFrameEnabled = false;
    private bool resizeFrameEnabled = false;

    private IEnumerator createFrameCoroutine;
    private IEnumerator moveFrameCoroutine;
    private IEnumerator rotateFrameCoroutine;
    private IEnumerator resizeFrameCoroutine = null;

    private float preTime = 5;

    /// <summary>
    /// 매 프레임마다 실행
    /// </summary>
    private void Update()
    {
        // 게임프레임 위치 동기화
        this.transform.localPosition = position;

        // 게임프레임 회전 각도 동기화
        this.transform.localRotation = Quaternion.Euler(0, 0, rotation);

        // 게임프레임 크기 동기화
        bgSprite.transform.localScale = new Vector3(frameSize.x, frameSize.y, 1);
        judgementLine.transform.localPosition = new Vector3(0, -(frameSize.y / 2f) + (JudgementLineWidth / 2), -1);
        judgementLine.transform.localScale = new Vector3(frameSize.x, JudgementLineWidth, 1);
        noteLayer.transform.localPosition = new Vector3(0, -(frameSize.y / 2f) + (JudgementLineWidth / 2), -0.5f);

#if UNITY_ANDROID
        // 터치를 받아와 noteList에 있는 노트들 처리
        int touchCount = Input.touchCount;
        if (touchCount > 0 && Time.timeScale != 0)
        {
            for (int i = 0; i < touchCount; i++)
            {
                Touch tempTouch = Input.GetTouch(i);

                try
                {
                    switch (tempTouch.phase)
                    {
                        // 터치 노트 처리
                        case TouchPhase.Began:
                            List<Note> touchedNoteList1 = new List<Note>();
                            int targetIndex1 = 0;
                            // 터치 가능한 노트를 리스트에 담음
                            foreach (Note note in noteList)
                            {
                                // 터치 노트
                                if (note.noteType == NoteType.Touch && IsTouchedNote(NoteType.Touch, note, tempTouch.position) && note.Judgement(NoteType.Touch) != JudgementType.Ignore)
                                    touchedNoteList1.Add(note);
                                // 슬라이드 노트
                                if (note.noteType == NoteType.Slide && IsTouchedNote(NoteType.Slide, note, tempTouch.position) && note.Judgement(NoteType.Slide) != JudgementType.Ignore)
                                    touchedNoteList1.Add(note);
                                // 롱 노트
                                if (note.noteType == NoteType.Long && IsTouchedNote(NoteType.Long, note, tempTouch.position) && note.Judgement(NoteType.Long) != JudgementType.Ignore)
                                    touchedNoteList1.Add(note);
                                // 위 플릭 노트
                                if (note.noteType == NoteType.UpFlick && IsTouchedNote(NoteType.UpFlick, note, tempTouch.position) && note.Judgement(NoteType.UpFlick) != JudgementType.Ignore)
                                    touchedNoteList1.Add(note);
                                // 아래 플릭 노트
                                if (note.noteType == NoteType.DownFlick && IsTouchedNote(NoteType.DownFlick, note, tempTouch.position) && note.Judgement(NoteType.DownFlick) != JudgementType.Ignore)
                                    touchedNoteList1.Add(note);
                            }

                            // 리스트에 담은 노트 중 가장 아래에 있는 노트를 처리
                            for (int j = 1; j < touchedNoteList1.Count; j++)
                                if (touchedNoteList1[j].yPosition >= -JudgementLineWidth && touchedNoteList1[targetIndex1].yPosition > touchedNoteList1[j].yPosition)
                                    targetIndex1 = j;

                            // 노트 처리
                            JudgementType judgement1 = touchedNoteList1[targetIndex1].Judgement(touchedNoteList1[targetIndex1].noteType);
                            if (judgement1 == JudgementType.Ignore) break;
                            if (touchedNoteList1.Count > 0)
                            {
                                // 처리 완료된 노트는 삭제
                                //Destroy(touchedNoteList[targetIndex].gameObject);
                                switch (touchedNoteList1[targetIndex1].noteType)
                                {
                                    case NoteType.Touch:
                                        // 노트 처리
                                        ProcessNote(judgement1);
                                        noteList.Remove(touchedNoteList1[targetIndex1]);
                                        touchedNoteList1[targetIndex1].DisplayJudgement(judgement1);
                                        touchedNoteList1[targetIndex1].gameObject.SetActive(false);
                                        break;
                                    case NoteType.Slide:
                                        // 노트 처리
                                        if (!touchedNoteList1[targetIndex1].isProcessed) ProcessNote(judgement1);
                                        noteList.Remove(touchedNoteList1[targetIndex1]);
                                        touchedNoteList1[targetIndex1].DisplayJudgement(judgement1);
                                        touchedNoteList1[targetIndex1].gameObject.SetActive(false);
                                        break;
                                    case NoteType.Long:
                                        if (!touchedNoteList1[targetIndex1].isProcessed) ProcessNote(judgement1);
                                        // 처리 완료된 노트는 삭제
                                        //Destroy(touchedNoteList[targetIndex].gameObject);
                                        //noteList.Remove(touchedNoteList2[targetIndex2]);
                                        //touchedNoteList2[targetIndex2].gameObject.SetActive(false);
                                        touchedNoteList1[targetIndex1].isProcessed = true;
                                        break;
                                    case NoteType.UpFlick:
                                        // 노트 처리
                                        if (!touchedNoteList1[targetIndex1].isFlicking)
                                        {
                                            touchedNoteList1[targetIndex1].isFlicking = true;
                                            touchedNoteList1[targetIndex1].touchIndex = tempTouch.fingerId;
                                            touchedNoteList1[targetIndex1].touchScreenPos = tempTouch.position;
                                        }
                                        break;
                                    case NoteType.DownFlick:
                                        // 노트 처리
                                        if (!touchedNoteList1[targetIndex1].isFlicking)
                                        {
                                            touchedNoteList1[targetIndex1].isFlicking = true;
                                            touchedNoteList1[targetIndex1].touchIndex = tempTouch.fingerId;
                                            touchedNoteList1[targetIndex1].touchScreenPos = tempTouch.position;
                                        }
                                        break;
                                }
                            }
                            break;

                        // 슬라이드 노트 처리
                        case TouchPhase.Moved:
                        case TouchPhase.Stationary:
                            List<Note> touchedNoteList2 = new List<Note>();
                            //List<Note> touchedNoteList3 = new List<Note>();
                            int targetIndex2 = 0;
                            //int targetIndex3 = 0;
                            // 터치 가능한 노트를 리스트에 담음
                            foreach (Note note in noteList)
                            {
                                if (note.noteType == NoteType.Slide && IsTouchedNote(NoteType.Slide, note, tempTouch.position) && note.Judgement(NoteType.Slide) != JudgementType.Ignore)
                                    touchedNoteList2.Add(note);
                                if (note.noteType == NoteType.Long && IsTouchedNote(NoteType.Long, note, tempTouch.position) && note.Judgement(NoteType.Long) != JudgementType.Ignore)
                                    touchedNoteList2.Add(note);
                                if (note.noteType == NoteType.UpFlick && IsTouchedNote(NoteType.UpFlick, note, tempTouch.position) && note.Judgement(NoteType.UpFlick) != JudgementType.Ignore)
                                    touchedNoteList2.Add(note);
                                if (note.noteType == NoteType.DownFlick && IsTouchedNote(NoteType.DownFlick, note, tempTouch.position) && note.Judgement(NoteType.DownFlick) != JudgementType.Ignore)
                                    touchedNoteList2.Add(note);
                            }

                            // 리스트에 담은 노트 중 가장 아래에 있는 노트를 처리
                            for (int j = 1; j < touchedNoteList2.Count; j++)
                                if (touchedNoteList2[j].yPosition >= -(JudgementLineWidth / 2) && touchedNoteList2[targetIndex2].yPosition > touchedNoteList2[j].yPosition)
                                    targetIndex2 = j;
                            /*
                            for (int j = 1; j < touchedNoteList3.Count; j++)
                                if (touchedNoteList3[j].yPosition >= -(JudgementLineWidth / 2) && touchedNoteList3[targetIndex3].yPosition > touchedNoteList3[j].yPosition)
                                    targetIndex3 = j;
                                    */
                                                                
                            if (touchedNoteList2.Count > 0)
                            {
                                switch (touchedNoteList2[targetIndex2].noteType)
                                {
                                    case NoteType.Slide:
                                    case NoteType.Long:
                                        // 노트 처리
                                        JudgementType judgement2 = touchedNoteList2[targetIndex2].Judgement(touchedNoteList2[targetIndex2].noteType);
                                        if (judgement2 == JudgementType.Ignore) break;
                                        if (!touchedNoteList2[targetIndex2].isProcessed) ProcessNote(judgement2);
                                        // 처리 완료된 노트는 삭제
                                        //Destroy(touchedNoteList[targetIndex].gameObject);
                                        //noteList.Remove(touchedNoteList2[targetIndex2]);
                                        //touchedNoteList2[targetIndex2].gameObject.SetActive(false);
                                        touchedNoteList2[targetIndex2].isProcessed = true;
                                        break;

                                    case NoteType.UpFlick:
                                        if (touchedNoteList2[targetIndex2].isFlicking)
                                        {
                                            if (transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(tempTouch.position)).y > transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(touchedNoteList2[targetIndex2].touchScreenPos)).y)
                                            {
                                                if (transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(tempTouch.position)).y > transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(touchedNoteList2[targetIndex2].touchScreenPos)).y + GameFrame.JudgementLineWidth)
                                                {
                                                    JudgementType judgement3 = touchedNoteList2[targetIndex2].Judgement(touchedNoteList2[targetIndex2].noteType);
                                                    if (judgement3 == JudgementType.Ignore) break;

                                                    ProcessNote(judgement3);
                                                    noteList.Remove(touchedNoteList2[targetIndex2]);
                                                    touchedNoteList2[targetIndex2].DisplayJudgement(judgement3);
                                                    touchedNoteList2[targetIndex2].gameObject.SetActive(false);
                                                }
                                            }
                                            else
                                            {
                                                touchedNoteList2[targetIndex2].touchScreenPos = tempTouch.position;
                                            }
                                        }
                                        else
                                        {
                                            touchedNoteList2[targetIndex2].isFlicking = true;
                                            touchedNoteList2[targetIndex2].touchIndex = tempTouch.fingerId;
                                            touchedNoteList2[targetIndex2].touchScreenPos = tempTouch.position;
                                        }
                                        break;

                                    case NoteType.DownFlick:
                                        if (touchedNoteList2[targetIndex2].isFlicking)
                                        {
                                            if (transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(tempTouch.position)).y < transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(touchedNoteList2[targetIndex2].touchScreenPos)).y)
                                            {
                                                if (transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(tempTouch.position)).y < transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(touchedNoteList2[targetIndex2].touchScreenPos)).y - GameFrame.JudgementLineWidth)
                                                {
                                                    JudgementType judgement3 = touchedNoteList2[targetIndex2].Judgement(touchedNoteList2[targetIndex2].noteType);
                                                    if (judgement3 == JudgementType.Ignore) break;

                                                    ProcessNote(judgement3);
                                                    noteList.Remove(touchedNoteList2[targetIndex2]);
                                                    touchedNoteList2[targetIndex2].DisplayJudgement(judgement3);
                                                    touchedNoteList2[targetIndex2].gameObject.SetActive(false);
                                                }
                                            }
                                            else
                                            {
                                                touchedNoteList2[targetIndex2].touchScreenPos = tempTouch.position;
                                            }
                                        }
                                        else
                                        {
                                            touchedNoteList2[targetIndex2].isFlicking = true;
                                            touchedNoteList2[targetIndex2].touchIndex = tempTouch.fingerId;
                                            touchedNoteList2[targetIndex2].touchScreenPos = tempTouch.position;
                                        }
                                        break;
                                }
                            }
                            break;

                        // 터치 뗄 때 처리
                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            break;
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
#endif
#if UNITY_EDITOR
        try
        {
            if (Time.timeScale != 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    List<Note> touchedNoteList1 = new List<Note>();
                    int targetIndex1 = 0;
                    // 터치 가능한 노트를 리스트에 담음
                    foreach (Note note in noteList)
                    {
                        // 터치 노트
                        if (note.noteType == NoteType.Touch && IsTouchedNote(NoteType.Touch, note, Input.mousePosition) && note.Judgement(NoteType.Touch) != JudgementType.Ignore)
                            touchedNoteList1.Add(note);
                        // 슬라이드 노트
                        if (note.noteType == NoteType.Slide && IsTouchedNote(NoteType.Slide, note, Input.mousePosition) && note.Judgement(NoteType.Slide) != JudgementType.Ignore)
                            touchedNoteList1.Add(note);
                        // 롱 노트
                        if (note.noteType == NoteType.Long && IsTouchedNote(NoteType.Long, note, Input.mousePosition) && note.Judgement(NoteType.Long) != JudgementType.Ignore)
                            touchedNoteList1.Add(note);
                        // 위 플릭 노트
                        if (note.noteType == NoteType.UpFlick && IsTouchedNote(NoteType.UpFlick, note, Input.mousePosition) && note.Judgement(NoteType.UpFlick) != JudgementType.Ignore)
                            touchedNoteList1.Add(note);
                        // 아래 플릭 노트
                        if (note.noteType == NoteType.DownFlick && IsTouchedNote(NoteType.DownFlick, note, Input.mousePosition) && note.Judgement(NoteType.DownFlick) != JudgementType.Ignore)
                            touchedNoteList1.Add(note);
                    }
                    // 리스트에 담은 노트 중 가장 아래에 있는 노트를 처리
                    for (int j = 1; j < touchedNoteList1.Count; j++)
                        if (touchedNoteList1[j].yPosition >= -JudgementLineWidth && touchedNoteList1[targetIndex1].yPosition > touchedNoteList1[j].yPosition)
                            targetIndex1 = j;
                    // 노트 처리
                    JudgementType judgement1 = JudgementType.Ignore;
                    if (touchedNoteList1.Count > 0)
                    {
                        judgement1 = touchedNoteList1[targetIndex1].Judgement(touchedNoteList1[targetIndex1].noteType);
                        //if (judgement1 == JudgementType.Ignore) break;
                        // 처리 완료된 노트는 삭제
                        //Destroy(touchedNoteList[targetIndex].gameObject);
                        switch (touchedNoteList1[targetIndex1].noteType)
                        {
                            case NoteType.Touch:
                                // 노트 처리
                                ProcessNote(judgement1);
                                noteList.Remove(touchedNoteList1[targetIndex1]);
                                touchedNoteList1[targetIndex1].DisplayJudgement(judgement1);
                                touchedNoteList1[targetIndex1].gameObject.SetActive(false);
                                break;
                            case NoteType.Slide:
                                // 노트 처리
                                if (!touchedNoteList1[targetIndex1].isProcessed) ProcessNote(judgement1);
                                noteList.Remove(touchedNoteList1[targetIndex1]);
                                touchedNoteList1[targetIndex1].DisplayJudgement(judgement1);
                                touchedNoteList1[targetIndex1].gameObject.SetActive(false);
                                break;
                            case NoteType.Long:
                                //longTouchDictionary.Add(0, touchedNoteList1[targetIndex1]);
                                //longTouchTimeDictionary.Add(0, 0);
                                //touchedNoteList1[targetIndex1].isTouching = true;
                                if (!touchedNoteList1[targetIndex1].isProcessed) ProcessNote(judgement1);
                                // 처리 완료된 노트는 삭제
                                //Destroy(touchedNoteList[targetIndex].gameObject);
                                //noteList.Remove(touchedNoteList2[targetIndex2]);
                                //touchedNoteList2[targetIndex2].gameObject.SetActive(false);
                                touchedNoteList1[targetIndex1].isProcessed = true;
                                break;
                            case NoteType.UpFlick:
                                // 노트 처리
                                if (!touchedNoteList1[targetIndex1].isFlicking)
                                {
                                    touchedNoteList1[targetIndex1].isFlicking = true;
                                    touchedNoteList1[targetIndex1].touchIndex = 0;
                                    touchedNoteList1[targetIndex1].touchScreenPos = Input.mousePosition;
                                }
                                break;
                            case NoteType.DownFlick:
                                // 노트 처리
                                if (!touchedNoteList1[targetIndex1].isFlicking)
                                {
                                    touchedNoteList1[targetIndex1].isFlicking = true;
                                    touchedNoteList1[targetIndex1].touchIndex = 0;
                                    touchedNoteList1[targetIndex1].touchScreenPos = Input.mousePosition;
                                }
                                break;
                        }
                    }
                }
                else if (Input.GetMouseButton(0))
                {
                    List<Note> touchedNoteList2 = new List<Note>();
                    List<Note> touchedNoteList3 = new List<Note>();
                    int targetIndex2 = 0;
                    int targetIndex3 = 0;
                    // 터치 가능한 노트를 리스트에 담음
                    foreach (Note note in noteList)
                    {
                        if (note.noteType == NoteType.Slide && IsTouchedNote(NoteType.Slide, note, Input.mousePosition) && note.Judgement(NoteType.Slide) != JudgementType.Ignore)
                            touchedNoteList2.Add(note);
                        if (note.noteType == NoteType.Long && IsTouchedNote(NoteType.Long, note, Input.mousePosition) && note.Judgement(NoteType.Long) != JudgementType.Ignore)
                            touchedNoteList2.Add(note);
                        if (note.noteType == NoteType.UpFlick && IsTouchedNote(NoteType.UpFlick, note, Input.mousePosition) && note.Judgement(NoteType.UpFlick) != JudgementType.Ignore)
                            touchedNoteList3.Add(note);
                        if (note.noteType == NoteType.DownFlick && IsTouchedNote(NoteType.DownFlick, note, Input.mousePosition) && note.Judgement(NoteType.DownFlick) != JudgementType.Ignore)
                            touchedNoteList3.Add(note);
                    }

                    // 리스트에 담은 노트 중 가장 아래에 있는 노트를 처리
                    for (int j = 1; j < touchedNoteList2.Count; j++)
                        if (touchedNoteList2[j].yPosition >= -(JudgementLineWidth / 2) && touchedNoteList2[targetIndex2].yPosition > touchedNoteList2[j].yPosition)
                            targetIndex2 = j;
                    for (int j = 1; j < touchedNoteList3.Count; j++)
                        if (touchedNoteList3[j].yPosition >= -(JudgementLineWidth / 2) && touchedNoteList3[targetIndex3].yPosition > touchedNoteList3[j].yPosition)
                            targetIndex3 = j;

                    // 노트 처리
                    JudgementType judgement2 = JudgementType.Ignore;
                    if (touchedNoteList2.Count > 0)
                    {
                        judgement2 = touchedNoteList2[targetIndex2].Judgement(touchedNoteList2[targetIndex2].noteType);
                        //if (judgement2 == JudgementType.Ignore) break;
                        if (!touchedNoteList2[targetIndex2].isProcessed) ProcessNote(judgement2);
                        // 처리 완료된 노트는 삭제
                        //Destroy(touchedNoteList[targetIndex].gameObject);
                        //noteList.Remove(touchedNoteList2[targetIndex2]);
                        //touchedNoteList2[targetIndex2].gameObject.SetActive(false);
                        touchedNoteList2[targetIndex2].isProcessed = true;
                    }

                    if (touchedNoteList3.Count > 0 && touchedNoteList3[targetIndex3].touchIndex == 0)
                    {
                        switch (touchedNoteList3[targetIndex3].noteType)
                        {
                            case NoteType.UpFlick:
                                if (touchedNoteList3[targetIndex3].isFlicking)
                                {
                                    if (transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)).y > transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(touchedNoteList3[targetIndex3].touchScreenPos)).y + GameFrame.JudgementLineWidth)
                                    {
                                        JudgementType judgement3 = touchedNoteList3[targetIndex3].Judgement(touchedNoteList3[targetIndex3].noteType);
                                        if (judgement3 == JudgementType.Ignore) break;

                                        ProcessNote(judgement3);
                                        noteList.Remove(touchedNoteList3[targetIndex3]);
                                        touchedNoteList3[targetIndex3].DisplayJudgement(judgement3);
                                        touchedNoteList3[targetIndex3].gameObject.SetActive(false);
                                    }
                                }
                                else
                                {
                                    touchedNoteList3[targetIndex3].isFlicking = true;
                                    touchedNoteList3[targetIndex3].touchIndex = 0;
                                    touchedNoteList3[targetIndex3].touchScreenPos = Input.mousePosition;
                                }
                                break;
                            case NoteType.DownFlick:
                                if (touchedNoteList3[targetIndex3].isFlicking)
                                {
                                    if (transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)).y < transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(touchedNoteList3[targetIndex3].touchScreenPos)).y - GameFrame.JudgementLineWidth)
                                    {
                                        JudgementType judgement3 = touchedNoteList3[targetIndex3].Judgement(touchedNoteList3[targetIndex3].noteType);
                                        if (judgement3 == JudgementType.Ignore) break;

                                        ProcessNote(judgement3);
                                        noteList.Remove(touchedNoteList3[targetIndex3]);
                                        touchedNoteList3[targetIndex3].DisplayJudgement(judgement3);
                                        touchedNoteList3[targetIndex3].gameObject.SetActive(false);
                                    }
                                }
                                else
                                {
                                    touchedNoteList3[targetIndex3].isFlicking = true;
                                    touchedNoteList3[targetIndex3].touchIndex = 0;
                                    touchedNoteList3[targetIndex3].touchScreenPos = Input.mousePosition;
                                }
                                break;
                        }
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {

                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
#endif
        //Debug.Log(Input.mousePosition + ", " + Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public bool IsTouchedNote(NoteType noteType, Note note, Vector2 touch)
    {
        Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch);
        switch (noteType)
        {
            case NoteType.Touch:
                if (note.gameFrame.transform.InverseTransformPoint(touchPos).x < frameSize.x / 2 + GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).x > -frameSize.x / 2 - GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).x < note.xPosition + noteXSize / 2 + GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).x > note.xPosition - noteXSize / 2 - GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).y < judgementLine.transform.localPosition.y + (JudgementLineWidth / 2) + GamePlayManager.NoteTouchExtraSizeY 
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).y > judgementLine.transform.localPosition.y - (JudgementLineWidth / 2) - GamePlayManager.NoteTouchExtraSizeY)
                    return true;
                else return false;
            case NoteType.Slide:
                if (note.gameFrame.transform.InverseTransformPoint(touchPos).x < frameSize.x / 2 + GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).x > -frameSize.x / 2 - GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).x < note.xPosition + noteXSize + GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).x > note.xPosition - noteXSize - GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).y < judgementLine.transform.localPosition.y + (JudgementLineWidth / 2) + GamePlayManager.NoteTouchExtraSizeY
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).y > judgementLine.transform.localPosition.y - (JudgementLineWidth / 2) - GamePlayManager.NoteTouchExtraSizeY)
                    return true;
                else return false;
            case NoteType.Long:
                if (note.gameFrame.transform.InverseTransformPoint(touchPos).x < frameSize.x / 2 + GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).x > -frameSize.x / 2 - GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).x < note.xPosition + noteXSize / 2 + GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).x > note.xPosition - noteXSize / 2 - GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).y < judgementLine.transform.localPosition.y + (JudgementLineWidth / 2) + GamePlayManager.NoteTouchExtraSizeY
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).y > judgementLine.transform.localPosition.y - (JudgementLineWidth / 2) - GamePlayManager.NoteTouchExtraSizeY)
                    return true;
                else return false;
            case NoteType.UpFlick:
                if (note.gameFrame.transform.InverseTransformPoint(touchPos).x < frameSize.x / 2 + GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).x > -frameSize.x / 2 - GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).x < note.xPosition + noteXSize + GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).x > note.xPosition - noteXSize - GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).y < judgementLine.transform.localPosition.y + (JudgementLineWidth / 2) + GamePlayManager.NoteTouchExtraSizeY
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).y > judgementLine.transform.localPosition.y - (JudgementLineWidth / 2) - GamePlayManager.NoteTouchExtraSizeY)
                    return true;
                else return false;
            case NoteType.DownFlick:
                if (note.gameFrame.transform.InverseTransformPoint(touchPos).x < frameSize.x / 2 + GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).x > -frameSize.x / 2 - GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).x < note.xPosition + noteXSize + GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).x > note.xPosition - noteXSize - GamePlayManager.NoteTouchExtraSizeX
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).y < judgementLine.transform.localPosition.y + (JudgementLineWidth / 2) + GamePlayManager.NoteTouchExtraSizeY
                    && note.gameFrame.transform.InverseTransformPoint(touchPos).y > judgementLine.transform.localPosition.y - (JudgementLineWidth / 2) - GamePlayManager.NoteTouchExtraSizeY)
                    return true;
                else return false;
        }
        return false;
    }

    /// <summary>
    /// 게임 화면 상에 게임프레임을 생성한다.
    /// </summary>
    /// <param name="position">(화면 중앙 기준) 게임프레임의 위치</param>
    /// <param name="rotation">게임프레임의 회전 각도</param>
    /// <param name="frameSize">게임프레임의 크기</param>
    /// <param name="noteSize">노트의 크기</param>
    /// <param name="createTime">생성 시간</param>
    /// <param name="lerp">선형 보간 종류</param>
    public void CreateFrame(Vector2 position, float rotation, Vector2 frameSize, float noteXSize, float createTime, LerpType lerp)
    {
        // 이미 생성되었다면 실행되지 않음
        if (!isCreating) return;

        this.position = position;
        this.rotation = rotation;
        this.frameSize = Vector2.zero;
        this.noteXSize = noteXSize;

        //touchManager.frameList.Add(this);

        createFrameCoroutine = CreateFrameCoroutine(frameSize, createTime, lerp);
        StartCoroutine(createFrameCoroutine);
    }

    private IEnumerator CreateFrameCoroutine(Vector2 destination, float time, LerpType lerp)
    {
        //Debug.Log(preTime.ToString() + ", " + time.ToString() + ", " + (2 * preTime - time).ToString() + " Start Create");
        yield return new WaitForSeconds(preTime - time + GamePlayManager.Instance.calibration);
        //Debug.Log(preTime.ToString() + ", " + time.ToString() + ", " + (2 * preTime - time).ToString() + " End Create");

        Vector2 originalFrameSize = Vector2.zero;
        float tempTime = 0;

        while (true)
        {
            if (!GamePlayManager.Instance.isPaused)
            {
                if (tempTime <= time)
                {
                    Vector2 temp = Vector2.Lerp(originalFrameSize, destination, Utility.GetLerp(lerp, tempTime, time));
                    this.frameSize = temp;

                    tempTime += Time.deltaTime;
                }
                else
                {
                    this.frameSize = destination;
                    isCreating = false;

                    //StopCoroutine(createFrameCoroutine);
                    yield break;
                }
            }
            yield return null;
        }
    }

    public void DestroyFrame()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 게임프레임의 위치를 이동시킨다.
    /// </summary>
    /// <param name="destination">최종 위치</param>
    /// <param name="moveTime">이동 시간</param>
    /// <param name="lerp">선형 보간 종류</param>
    public void MoveFrame(Vector2 destination, float moveTime, LerpType lerp)
    {
        if (moveFrameEnabled)
        {
            // 이미 이동 중이라면 이동 중지
            moveFrameEnabled = false;
            StopCoroutine(moveFrameCoroutine);
        }

        // 이동 코루틴 작동
        moveFrameCoroutine = MoveFrameCoroutine(destination, moveTime, lerp);
        StartCoroutine(moveFrameCoroutine);
    }

    private IEnumerator MoveFrameCoroutine(Vector2 destination, float time, LerpType lerp)
    {
        //yield return new WaitForSeconds(preTime + GamePlayManager.Instance.calibration);
        float tmp = 0;
        while (tmp < preTime + GamePlayManager.Instance.calibration)
        {
            tmp += Time.deltaTime;
            yield return null;
        }

        moveFrameEnabled = true;

        Vector2 originalPosition = this.position;
        float tempTime = 0;

        while (true)
        {
            if (!GamePlayManager.Instance.isPaused)
            {
                if (tempTime <= time)
                {
                    Vector2 temp = Vector2.Lerp(originalPosition, destination, Utility.GetLerp(lerp, tempTime, time));
                    this.position = temp;

                    tempTime += Time.deltaTime;
                }
                else
                {
                    this.position = destination;
                    moveFrameEnabled = false;

                    yield break;
                    //StopCoroutine(moveFrameCoroutine);
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// 게임프레임의 회전 각도를 변경시킨다.
    /// </summary>
    /// <param name="destination">최종 각도</param>
    /// <param name="rotateTime">회전 시간</param>
    /// <param name="lerp">선형 보간 종류</param>
    public void RotateFrame(float destination, float rotateTime, LerpType lerp)
    {
        if (rotateFrameEnabled)
        {
            // 이미 회전 중이라면 회전 중지
            rotateFrameEnabled = false;
            StopCoroutine(rotateFrameCoroutine);
        }

        // 회전 코루틴 작동
        rotateFrameCoroutine = RotateFrameCoroutine(destination, rotateTime, lerp);
        StartCoroutine(rotateFrameCoroutine);
    }

    private IEnumerator RotateFrameCoroutine(float destination, float time, LerpType lerp)
    {
        //yield return new WaitForSeconds(preTime + GamePlayManager.Instance.calibration);
        float tmp = 0;
        while (tmp < preTime + GamePlayManager.Instance.calibration)
        {
            tmp += Time.deltaTime;
            yield return null;
        }

        //Debug.Log(destination);

        rotateFrameEnabled = true;

        float originalRotation = this.rotation;
        float tempTime = 0;

        while (true)
        {
            if (!GamePlayManager.Instance.isPaused)
            {
                if (tempTime <= time)
                {
                    float temp = Mathf.Lerp(originalRotation, destination, Utility.GetLerp(lerp, tempTime, time));
                    this.rotation = temp;

                    //Debug.Log(Utility.GetLerp(lerp, tempTime, time) + ", " + temp);

                    tempTime += Time.deltaTime;
                }
                else
                {
                    this.rotation = destination;
                    rotateFrameEnabled = false;

                    yield break;
                    //StopCoroutine(rotateFrameCoroutine);
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// 게임프레임의 크기를 변경시킨다.
    /// </summary>
    /// <param name="destination">최종 크기</param>
    /// <param name="resizeTime">크기 변경 시간</param>
    /// <param name="lerp">선형 보간 종류</param>
    public void ResizeFrame(Vector2 destination, float resizeTime, LerpType lerp)
    {
        if (resizeFrameEnabled)
        {
            // 이미 크기 변경 중이라면 변경 중지
            resizeFrameEnabled = false;
            preFrameSize = this.frameSize;
            StopCoroutine(resizeFrameCoroutine);

            //Debug.Log("ddd");
        }

        // 크기 변경 코루틴 작동
        resizeFrameCoroutine = ResizeFrameCoroutine(destination, resizeTime, lerp);
        StartCoroutine(resizeFrameCoroutine);
    }

    private IEnumerator ResizeFrameCoroutine(Vector2 destination, float time, LerpType lerp)
    {
        //yield return new WaitForSeconds(preTime);
        float tmp = 0;
        while (tmp < preTime + GamePlayManager.Instance.calibration)
        {
            tmp += Time.deltaTime;
            yield return null;
        }

        //if (resizeFrameCoroutine != null)
        //{
        //    // 이미 크기 변경 중이라면 변경 중지
        //    resizeFrameEnabled = false;
        //    preFrameSize = this.frameSize;
        //    StopCoroutine(resizeFrameCoroutine);
        //    resizeFrameCoroutine = null;
        //}

        resizeFrameEnabled = true;

        Vector2 originalFrameSize = this.frameSize;
        float tempTime = 0;

        while (true)
        {
            if (!GamePlayManager.Instance.isPaused)
            {
                preFrameSize = this.frameSize;
                if (tempTime <= time)
                {
                    Vector2 temp = Vector2.Lerp(originalFrameSize, destination, Utility.GetLerp(lerp, tempTime, time));
                    this.frameSize = temp;

                    tempTime += Time.deltaTime;
                }
                else
                {
                    this.frameSize = destination;
                    resizeFrameEnabled = false;
                    preFrameSize = destination;
                    if (destination.x == 0 && destination.y == 0) DestroyFrame();
                    break;
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// 화면 위에서 노트를 생성한다.
    /// </summary>
    public void CreateNote(NoteType noteType, float xPosition, float beatLength = 0)
    {
        if (noteType != NoteType.Long)
        {
            Note note = GamePlayManager.Instance.PullNote(noteType, noteLayer).GetComponent<Note>();
            note.gameFrame = this;
            note.noteType = noteType;
            note.xPosition = xPosition;
            note.Initiate();
        }
        else
        {
            LongNoteDummy longNoteDummy = GamePlayManager.Instance.PullLongNoteDummy(noteLayer).GetComponent<LongNoteDummy>();
            longNoteDummy.gameFrame = this;
            longNoteDummy.xPosition = xPosition;
            longNoteDummy.beatLength = beatLength;
            longNoteDummy.Initiate();

            StartCoroutine(CreateLongNoteCoroutine(longNoteDummy, xPosition, beatLength));
        }
    }

    private IEnumerator CreateLongNoteCoroutine(LongNoteDummy longNoteDummy, float xPosition, float beatLength)
    {
        for (int i = 0; i < Mathf.CeilToInt(beatLength / 4); i++)
        {
            Note note = GamePlayManager.Instance.PullNote(NoteType.Long, noteLayer).GetComponent<Note>();
            note.gameFrame = this;
            note.noteType = NoteType.Long;
            note.xPosition = xPosition;
            note.dummyNote = longNoteDummy;
            note.index = i + 1;
            longNoteDummy.mainNoteList.Add(note);
            note.Initiate();

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// 노트를 판정에 따라 처리한다.
    /// </summary>
    /// <param name="judgementType">판정 결과</param>
    public void ProcessNote(JudgementType judgementType)
    {
        switch (judgementType)
        {
            case JudgementType.Perfect:
                // 판정
                //GamePlayManager.Instance.score += 920000f / GamePlayManager.Instance.maxNoteCount;
                GamePlayManager.Instance.perfectCount++;
                // 콤보
                GamePlayManager.Instance.currentCombo++;
                if (GamePlayManager.Instance.currentCombo > GamePlayManager.Instance.maxCombo) GamePlayManager.Instance.maxCombo = GamePlayManager.Instance.currentCombo;
                break;
            case JudgementType.Good:
                // 판정
                //GamePlayManager.Instance.score += 522000f / GamePlayManager.Instance.maxNoteCount;
                GamePlayManager.Instance.normalCount++;
                // 콤보
                GamePlayManager.Instance.currentCombo++;
                if (GamePlayManager.Instance.currentCombo > GamePlayManager.Instance.maxCombo) GamePlayManager.Instance.maxCombo = GamePlayManager.Instance.currentCombo;
                break;
            case JudgementType.Miss:
                GamePlayManager.Instance.missCount++;
                if (GamePlayManager.Instance.currentCombo > 0) GamePlayManager.Instance.comboCountList.Add(GamePlayManager.Instance.currentCombo);
                GamePlayManager.Instance.currentCombo = 0;
                break;
        }
    }
}

/// <summary>
/// 노트 종류
/// </summary>
public enum NoteType
{
    /// <summary>
    /// 터치(단타) 노트
    /// </summary>
    Touch,
    /// <summary>
    /// 슬라이드 노트
    /// </summary>
    Slide,
    /// <summary>
    /// 롱 노트
    /// </summary>
    Long,
    /// <summary>
    /// 위 방향 플릭 노트
    /// </summary>
    UpFlick,
    /// <summary>
    /// 아래 방향 플릭 노트
    /// </summary>
    DownFlick
}

/// <summary>
/// 선형 보간 종류
/// </summary>
public enum LerpType
{
    None,
    SmoothIn,
    SmoothOut,
    Smooth
}
