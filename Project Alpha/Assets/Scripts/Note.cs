using UnityEngine;

public class Note : MonoBehaviour
{
    [HideInInspector]
    public GameFrame gameFrame;

    /// <summary>
    /// 노트 타입
    /// </summary>
    public NoteType noteType;

    /// <summary>
    /// 노트 크기
    /// </summary>
    [HideInInspector]
    public float noteXSize;
    [HideInInspector]
    public float noteYSize;
    /// <summary>
    /// 노트의 생성 위치 x좌표
    /// </summary>
    [HideInInspector]
    public float xPosition;
    /// <summary>
    /// 노트의 y좌표
    /// </summary>
    [HideInInspector]
    public float yPosition;
    /// <summary>
    /// (롱노트에만 해당) 노트의 박자 단위 길이
    /// </summary>
    [HideInInspector]
    public float beatLength;

    [HideInInspector]
    public bool isCreated = false;

    [HideInInspector]
    public bool isProcessed = false;

    [HideInInspector]
    public bool isTouching = false;

    /// <summary>
    /// 노트 초기화 (생성 후 gameFrame 할당한 뒤 반드시 실행)
    /// </summary>
    public void Initiate()
    {
        noteXSize = gameFrame.noteXSize;
        noteYSize = (beatLength == 0) ? GameFrame.JudgementLineWidth : beatLength * ((60 / GamePlayManager.Instance.bpm) / 8) * GamePlayManager.NoteSpeedConstant * GamePlayManager.Instance.noteSpeed;
        yPosition = 5f * GamePlayManager.NoteSpeedConstant * GamePlayManager.Instance.noteSpeed;
        //yPosition = 800f;
        gameFrame.noteList.Add(this);
        isCreated = true;
    }

    /// <summary>
    /// 매 프레임마다 실행
    /// </summary>
    private void Update()
    {
        if (isCreated)
        {
            // x좌표 & 크기 조정
            if (xPosition - (noteXSize / 2) < -(gameFrame.frameSize.x / 2) && xPosition + (noteXSize / 2) > (gameFrame.frameSize.x / 2))
            {
                // 노트의 양쪽 모두 게임프레임을 벗어날 경우
                // 노트의 중심을 게임프레임의 중심(로컬 x좌표 0)으로 설정
                this.transform.localPosition = new Vector3(0, this.transform.localPosition.y, 0);
                // 노트의 크기를 게임프레임의 크기로 설정
                this.transform.localScale = new Vector3(gameFrame.frameSize.x, this.transform.localScale.y, 1);
            }
            else if (xPosition - (noteXSize / 2) < -(gameFrame.frameSize.x / 2))
            {
                if (xPosition + (noteXSize / 2) < -(gameFrame.frameSize.x / 2))
                {
                    // 노트가 왼쪽으로 게임프레임을 전부 벗어날 경우
                    // 노트의 중심을 게임프레임의 가장 왼쪽으로 설정
                    this.transform.localPosition = new Vector3(-(gameFrame.frameSize.x / 2), this.transform.localPosition.y, 0);
                    // 노트의 크기를 0으로 설정
                    this.transform.localScale = new Vector3(0, this.transform.localScale.y, 1);
                }
                else
                {
                    // 노트의 왼쪽만 게임프레임을 벗어날 경우
                    // 노트의 중심을 게임프레임의 가장 왼쪽과 노트의 가장 오른쪽의 중심으로 설정
                    this.transform.localPosition = new Vector3((-(gameFrame.frameSize.x / 2) + (xPosition + (noteXSize / 2))) / 2, this.transform.localPosition.y, 0);
                    // 노트의 크기를 게임프레임의 가장 왼쪽과 노트의 가장 오른쪽 간의 거리로 설정
                    this.transform.localScale = new Vector3((gameFrame.frameSize.x / 2) + (xPosition + (noteXSize / 2)), this.transform.localScale.y, 1);
                }
            }
            else if (xPosition + (noteXSize / 2) > (gameFrame.frameSize.x / 2))
            {
                if (xPosition - (noteXSize / 2) > (gameFrame.frameSize.x / 2))
                {
                    // 노트가 오른쪽으로 게임프레임을 전부 벗어날 경우
                    // 노트의 중심을 게임프레임의 가장 오른쪽으로 설정
                    this.transform.localPosition = new Vector3(gameFrame.frameSize.x / 2, this.transform.localPosition.y, 0);
                    // 노트의 크기를 0으로 설정
                    this.transform.localScale = new Vector3(0, this.transform.localScale.y, 1);
                }
                else
                {
                    // 노트의 오른쪽만 게임프레임을 벗어날 경우
                    // 노트의 중심을 게임프레임의 가장 오른쪽과 노트의 가장 왼쪽의 중심으로 설정
                    this.transform.localPosition = new Vector3(((gameFrame.frameSize.x / 2) + (xPosition - (noteXSize / 2))) / 2, this.transform.localPosition.y, 0);
                    // 노트의 크기를 게임프레임의 가장 오른쪽과 노트의 가장 왼쪽 간의 거리로 설정
                    this.transform.localScale = new Vector3((gameFrame.frameSize.x / 2) - (xPosition - (noteXSize / 2)), this.transform.localScale.y, 1);
                }
            }
            else
            {
                // 노트가 게임프레임을 벗어나지 않을 경우
                // 기존 위치 그대로 적용
                this.transform.localPosition = new Vector3(xPosition, this.transform.localPosition.y, 0);
                // 기존 크기 그대로 적용
                this.transform.localScale = new Vector3(noteXSize, this.transform.localScale.y, 1);
            }

            // y좌표 & 크기 조정
            if (yPosition + noteYSize > gameFrame.frameSize.y && yPosition < 0)
            {
                // 노트의 양쪽 모두 게임프레임을 벗어날 경우
                this.transform.localPosition = new Vector3(this.transform.localPosition.x, gameFrame.frameSize.y / 2, 0);
                this.transform.localScale = new Vector3(this.transform.localScale.x, gameFrame.frameSize.y, 1);
            }
            else if (yPosition + noteYSize > gameFrame.frameSize.y)
            {
                if (yPosition > gameFrame.frameSize.y)
                {
                    // 노트가 위쪽으로 게임프레임을 전부 벗어난 경우
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x, gameFrame.frameSize.y, 0);
                    this.transform.localScale = new Vector3(this.transform.localScale.x, 0, 1);
                }
                else
                {
                    // 노트의 위쪽만 게임프레임을 벗어날 경우
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x, (gameFrame.frameSize.y + yPosition) / 2, 0);
                    this.transform.localScale = new Vector3(this.transform.localScale.x, gameFrame.frameSize.y - yPosition, 1);
                }
            }
            else if (yPosition < 0)
            {
                if (yPosition + noteYSize < 0)
                {
                    // 노트가 아래쪽으로 게임프레임을 전부 벗어났을 경우
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x, 0, 0);
                    this.transform.localScale = new Vector3(this.transform.localScale.x, 0, 1);
                }
                else
                {
                    // 노트의 아래쪽만 게임프레임을 벗어날 경우
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x, (yPosition + noteYSize) / 2, 0);
                    this.transform.localScale = new Vector3(this.transform.localScale.x, yPosition + noteYSize, 1);
                }
            }
            else
            {
                // 노트가 게임프레임을 벗어나지 않을 경우
                // 기존 위치 그대로 적용
                this.transform.localPosition = new Vector3(this.transform.localPosition.x, yPosition + (noteYSize / 2), 0);
                // 기존 크기 그대로 적용
                this.transform.localScale = new Vector3(this.transform.localScale.x, noteYSize, 1);
            }

            this.transform.localRotation = Quaternion.Euler(0, 0, 0);

            // 노트가 너무 내려갔을때 Miss로 처리하고 삭제
            if (yPosition < (-0.75f) * (GamePlayManager.NoteSpeedConstant * GamePlayManager.Instance.noteSpeed) && !isTouching)
            {
                gameFrame.ProcessNote(JudgementType.Miss);
                gameFrame.noteList.Remove(this);
                //Destroy(gameObject);
                gameObject.SetActive(false);
            }

            if (yPosition <= 5f && isProcessed)
            {
                gameFrame.noteList.Remove(this);
                gameObject.SetActive(false);
            }

            // 노트 낙하
            yPosition -= GamePlayManager.NoteSpeedConstant * GamePlayManager.Instance.noteSpeed * Time.deltaTime;
        }
    }

    public JudgementType Judgement(NoteType noteType)
    {
        // 오차 시간 (ms 단위)
        float mistakeTime = Utility.GetMistakeTime(GamePlayManager.Instance.noteSpeed, yPosition);

        // bpm이 160보다 크다면 160으로 조정
        // bpm이 120보다 작다면 120으로 조정
        float bpm = GamePlayManager.Instance.bpm;
        if (bpm > 160) bpm = 160;
        else if (bpm < 120) bpm = 120;

        // x = 16분음표의 길이 시간 (ms 단위)
        float x = ((60 / bpm) / 4) * 1000;

        switch (noteType)
        {
            case NoteType.Touch:
                // 오차 시간이 (5/8)x 이하인 경우 Perfect
                // 오차 시간이 (2)x 이하인 경우 Normal
                // 오차 시간이 그 초과인 경우 Miss
                // 오차 시간이 (4)x 이상인 경우 해당 입력은 무시
                //float absYPosition = Mathf.Abs(yPosition);
                if (mistakeTime > 4f * x) return JudgementType.Ignore;
                else if (mistakeTime > 2f * x) return JudgementType.Miss;
                else if (mistakeTime > (5f / 8f) * x) return JudgementType.Normal;
                else return JudgementType.Perfect;
            case NoteType.Slide:
                // 오차 시간이 (0.75)x 이상인 경우 해당 입력은 무시
                // 아니면 Perfect
                if (mistakeTime > 0.75f * x) return JudgementType.Ignore;
                else return JudgementType.Perfect;
            case NoteType.Long:
                // 오차 시간이 (5/8)x 이하인 경우 Perfect
                // 오차 시간이 (2)x 이하인 경우 Normal
                // 오차 시간이 그 초과인 경우 Miss
                // 오차 시간이 (4)x 이상인 경우 해당 입력은 무시
                //float absYPosition = Mathf.Abs(yPosition);
                if (mistakeTime > 4f * x) return JudgementType.Ignore;
                else if (mistakeTime > 2f * x) return JudgementType.Miss;
                else if (mistakeTime > (5f / 8f) * x) return JudgementType.Normal;
                else return JudgementType.Perfect;
        }

        return JudgementType.Ignore;
    }

    public JudgementType JudgementPerfect()
    {
        if (Mathf.Abs(yPosition) < 2) return JudgementType.Perfect;
        else return JudgementType.Ignore;
    }
}

/// <summary>
/// 노트 판정 종류
/// </summary>
public enum JudgementType
{
    /// <summary>
    /// Perfect 판정
    /// </summary>
    Perfect,
    /// <summary>
    /// Normal 판정 (타 게임의 Good과 동일)
    /// </summary>
    Normal,
    /// <summary>
    /// Miss 판정
    /// </summary>
    Miss,
    /// <summary>
    /// 너무 일찍 터치한 경우 터치하지 않은 것으로 간주(무시)
    /// </summary>
    Ignore
}
