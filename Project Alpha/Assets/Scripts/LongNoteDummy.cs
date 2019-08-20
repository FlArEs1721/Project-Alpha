using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNoteDummy : MonoBehaviour
{
    [HideInInspector]
    public GameFrame gameFrame;

    [HideInInspector]
    public float xPosition;
    [HideInInspector]
    public float yPosition;
    [HideInInspector]
    public float noteXSize;
    [HideInInspector]
    public float noteYSize;

    [HideInInspector]
    public float beatLength;

    [HideInInspector]
    public List<Note> mainNoteList = new List<Note>();

    [HideInInspector]
    public bool isCreated = false;

    public void Initiate()
    {
        noteXSize = gameFrame.noteXSize;
        noteYSize = beatLength * ((60 / GamePlayManager.Instance.bpm) / 8) * GamePlayManager.NoteSpeedConstant * GamePlayManager.Instance.noteSpeed;
        yPosition = (5f + GamePlayManager.Instance.calibration) * GamePlayManager.NoteSpeedConstant * GamePlayManager.Instance.noteSpeed;
        isCreated = true;
    }

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

            // 노트 낙하
            yPosition -= GamePlayManager.NoteSpeedConstant * GamePlayManager.Instance.noteSpeed * Time.deltaTime;
        }
    }

    public void DeleteNote()
    {
        foreach (Note note in mainNoteList)
        {
            if (note.gameObject.activeSelf)
            {
                gameFrame.ProcessNote(JudgementType.Miss);
                gameFrame.noteList.Remove(note);
                note.gameObject.SetActive(false);
            }
        }
        gameObject.SetActive(false);
    }
}
