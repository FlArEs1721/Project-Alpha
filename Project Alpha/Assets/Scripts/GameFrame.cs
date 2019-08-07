using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFrame : MonoBehaviour
{
    public GameObject bgSprite;
    public GameObject noteLayer;
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
    public const float JudgementLineWidth = 10f;

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
    private IEnumerator resizeFrameCoroutine0 = null;
    private IEnumerator resizeFrameCoroutine1 = null;

    private float preTime = 0;

    private void Awake()
    {
        //touchManager = GameObject.Find("GameManager").GetComponent<TouchManager>();
    }

    /// <summary>
    /// 오브젝트가 생성될 때 실행
    /// </summary>
    private void Start()
    {
        preTime = 800f / (GamePlayManager.NoteSpeedConstant * GamePlayManager.Instance.noteSpeed);
    }

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

        //게임프레임 크기 변화에 따른 노트들의 위치 변화 적용
        //float temp = (frameSize.y - preFrameSize.y) / 2;
        //foreach (Note note in noteList)
        //{
        //    Vector3 pos = note.transform.localPosition;
        //    note.transform.localPosition = new Vector3(pos.x, pos.y - temp, 0);
        //}
        //noteLayer.transform.localPosition = new Vector3(0, -(frameSize.y / 2), 0);

        // 터치를 받아와 noteList에 있는 노트들 처리
        int touchCount = Input.touchCount;
        if (touchCount > 0)
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
                                if (note.noteType == NoteType.Touch && IsTouchedNote(NoteType.Touch, note, tempTouch.position) && note.Judgement(NoteType.Touch) != JudgementType.Ignore)
                                    touchedNoteList1.Add(note);
                                if (note.noteType == NoteType.Slide && IsTouchedNote(NoteType.Slide, note, tempTouch.position) && note.Judgement(NoteType.Touch) != JudgementType.Ignore)
                                    touchedNoteList1.Add(note);
                            }
                            // 리스트에 담은 노트 중 가장 아래에 있는 노트를 처리
                            for (int j = 1; j < touchedNoteList1.Count; j++)
                                if (touchedNoteList1[targetIndex1].yPosition > touchedNoteList1[j].yPosition)
                                    targetIndex1 = j;
                            if (touchedNoteList1[targetIndex1].Judgement(touchedNoteList1[targetIndex1].noteType) == JudgementType.Ignore) break;
                            // 노트 처리
                            ProcessNote(touchedNoteList1[targetIndex1].Judgement(touchedNoteList1[targetIndex1].noteType));
                            // 처리 완료된 노트는 삭제
                            //Destroy(touchedNoteList[targetIndex].gameObject);
                            noteList.Remove(touchedNoteList1[targetIndex1]);
                            touchedNoteList1[targetIndex1].gameObject.SetActive(false);
                            break;

                        // 슬라이드 노트 처리
                        case TouchPhase.Moved:
                        case TouchPhase.Stationary:
                        case TouchPhase.Canceled:
                        case TouchPhase.Ended:
                            List<Note> touchedNoteList2 = new List<Note>();
                            int targetIndex2 = 0;
                            // 터치 가능한 노트를 리스트에 담음
                            foreach (Note note in noteList)
                            {
                                if (note.noteType == NoteType.Slide && IsTouchedNote(NoteType.Slide, note, tempTouch.position) && note.Judgement(NoteType.Touch) != JudgementType.Ignore)
                                    touchedNoteList2.Add(note);
                            }
                            // 리스트에 담은 노트 중 가장 아래에 있는 노트를 처리
                            for (int j = 1; j < touchedNoteList2.Count; j++)
                                if (touchedNoteList2[targetIndex2].yPosition > touchedNoteList2[j].yPosition)
                                    targetIndex2 = j;
                            if (touchedNoteList2[targetIndex2].Judgement(touchedNoteList2[targetIndex2].noteType) == JudgementType.Ignore) break;
                            // 노트 처리
                            ProcessNote(touchedNoteList2[targetIndex2].Judgement(touchedNoteList2[targetIndex2].noteType));
                            // 처리 완료된 노트는 삭제
                            //Destroy(touchedNoteList[targetIndex].gameObject);
                            noteList.Remove(touchedNoteList2[targetIndex2]);
                            touchedNoteList2[targetIndex2].gameObject.SetActive(false);
                            break;
                    }
                }
                catch
                {
                    continue;
                }
                

                //List<Note> touchedNoteList = new List<Note>();
                //int targetIndex = 0;
                //// 터치 가능한 노트를 리스트에 담음
                //foreach (Note note in noteList)
                //{
                //    if (note.noteType == NoteType.Touch && IsTouchedNote(NoteType.Touch, note.transform.position, tempTouch.position) && note.Judgement(NoteType.Touch) != JudgementType.Ignore)
                //        touchedNoteList.Add(note);
                //    if (note.noteType == NoteType.Slide && IsTouchedNote(NoteType.Slide, note.transform.position, tempTouch.position) && note.Judgement(NoteType.Touch) != JudgementType.Ignore)
                //        touchedNoteList.Add(note);
                //}
                //// 리스트에 담은 노트 중 가장 아래에 있는 노트를 처리
                //for (int j = 1; j < touchedNoteList.Count; j++)
                //    if (touchedNoteList[targetIndex].yPosition > touchedNoteList[j].yPosition)
                //        targetIndex = j;
                //if (touchedNoteList[targetIndex].Judgement(touchedNoteList[targetIndex].noteType) == JudgementType.Ignore) break;
                //if (touchedNoteList[targetIndex].noteType == NoteType.Touch && tempTouch.phase != TouchPhase.Began) break;
                //// 노트 처리
                //ProcessNote(/*touchedNoteList[targetIndex].Judgement(touchedNoteList[targetIndex].noteType)*/JudgementType.Perfect);
                //// 처리 완료된 노트는 삭제
                ////Destroy(touchedNoteList[targetIndex].gameObject);
                //noteList.Remove(touchedNoteList[targetIndex]);
                //touchedNoteList[targetIndex].gameObject.SetActive(false);

                // 슬라이드 노트 처리
                /*
                foreach (var note in noteList)
                {
                    JudgementType judgement = note.Judgement(NoteType.Slide);
                    if (note.noteType == NoteType.Slide && IsTouchedNote(NoteType.Slide, note.transform.position, tempTouch.position) && judgement != JudgementType.Ignore)
                    {
                        ProcessNote(judgement);
                        noteList.Remove(note);
                        note.gameObject.SetActive(false);
                    }
                }
                */
            }
        }

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
        yield return new WaitForSeconds(preTime);

        Vector2 originalFrameSize = Vector2.zero;
        float tempTime = 0;

        while (true)
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

                StopCoroutine(createFrameCoroutine);
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
        yield return new WaitForSeconds(preTime);

        moveFrameEnabled = true;

        Vector2 originalPosition = this.position;
        float tempTime = 0;

        while (true)
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

                StopCoroutine(moveFrameCoroutine);
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
        yield return new WaitForSeconds(preTime);

        //Debug.Log(destination);

        rotateFrameEnabled = true;

        float originalRotation = this.rotation;
        float tempTime = 0;

        while (true)
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

                StopCoroutine(rotateFrameCoroutine);
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
        //if (resizeFrameCoroutine0 != null)
        //{
        //    // 이미 크기 변경 중이라면 변경 중지
        //    resizeFrameEnabled = false;
        //    preFrameSize = this.frameSize;
        //    StopCoroutine(resizeFrameCoroutine0);

        //    Debug.Log("ddd");
        //}

        // 크기 변경 코루틴 작동
        resizeFrameCoroutine0 = resizeFrameCoroutine1;
        resizeFrameCoroutine1 = ResizeFrameCoroutine(destination, resizeTime, lerp);
        StartCoroutine(resizeFrameCoroutine1);
    }

    private IEnumerator ResizeFrameCoroutine(Vector2 destination, float time, LerpType lerp)
    {
        yield return new WaitForSeconds(preTime);

        if (resizeFrameCoroutine0 != null)
        {
            // 이미 크기 변경 중이라면 변경 중지
            resizeFrameEnabled = false;
            preFrameSize = this.frameSize;
            StopCoroutine(resizeFrameCoroutine0);
            resizeFrameCoroutine0 = null;
        }

        resizeFrameEnabled = true;

        Vector2 originalFrameSize = this.frameSize;
        float tempTime = 0;

        while (true)
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
            yield return null;
        }

        resizeFrameCoroutine1 = null;
    }

    /// <summary>
    /// 화면 위에서 노트를 생성한다.
    /// </summary>
    /// <param name="position">(게임프레임 중앙 기준) 노트 생성 위치</param>
    public void CreateNote(NoteType noteType, float xPosition)
    {
        //if (noteType == NoteType.Touch) return;
        Note note = GamePlayManager.Instance.PullNote(noteType, noteLayer).GetComponent<Note>();
        note.gameFrame = this;
        note.noteType = noteType;
        note.xPosition = xPosition;
        note.Initiate();
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
                // TODO: 콤보점수
                // 콤보
                GamePlayManager.Instance.currentCombo++;
                if (GamePlayManager.Instance.currentCombo > GamePlayManager.Instance.maxCombo) GamePlayManager.Instance.maxCombo = GamePlayManager.Instance.currentCombo;
                break;
            case JudgementType.Normal:
                // 판정
                //GamePlayManager.Instance.score += 522000f / GamePlayManager.Instance.maxNoteCount;
                GamePlayManager.Instance.normalCount++;
                // TODO: 콤보점수
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
    Slide
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
