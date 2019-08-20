using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Parser : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject gameFramePrefab;
    public GameObject touchNotePrefab;
    public GameObject slideNotePrefab;
    public GameObject longNotePrefab;
    public GameObject judgementObjectPrefab;
    public TextAsset scoreData;

    private Dictionary<int, GameFrame> gameFrameList = new Dictionary<int, GameFrame>();

    private List<List<PlayData>> playDataList = new List<List<PlayData>>();

    private AudioClip audioClip;

    private SendInfo sendInfo;

    private float audioPlayTime = 0;
    private float tempTime = 0;

    private bool isScoreFinished = false;

    private int i = 0;

    private void Awake()
    {
        //Time.fixedDeltaTime = float.PositiveInfinity;
        
    }

    private void Start()
    {
        sendInfo = GameObject.Find("SendInfoObject").GetComponent<SendInfo>();
        GamePlayManager.Instance.noteSpeed = sendInfo.noteSpeed;
        GamePlayManager.Instance.calibration = sendInfo.calibration;
        scoreData = Resources.Load(sendInfo.songTitle + "_" + sendInfo.songDifficulty, typeof(TextAsset)) as TextAsset;
        //Destroy(sendSongInfo.gameObject);
        InitiateScore();
    }

    private void Update()
    {
        audioPlayTime = audioSource.time + tempTime;

        if (!audioSource.isPlaying && isScoreFinished)
        {
            sendInfo.results = new float[5] { GamePlayManager.Instance.GetScore(), GamePlayManager.Instance.maxCombo, GamePlayManager.Instance.perfectCount, GamePlayManager.Instance.normalCount, GamePlayManager.Instance.missCount};
            SceneManager.LoadScene("Result");
        }
    }

    public void InitiateScore()
    {
        StringReader sr = new StringReader(scoreData.text);

        string source = sr.ReadLine();

        string[] infos = source.Split(',');
        GamePlayManager.Instance.bpm = float.Parse(infos[0]);
        float audioPreTime = float.Parse(infos[1]);
        audioClip = Resources.Load<AudioClip>(infos[2]);
        //Debug.Log(audioClip.name + ", " + GamePlayManager.Instance.bpm);

        // 32분음표 단위
        Time.fixedDeltaTime = (60 / GamePlayManager.Instance.bpm) / 8;

        source = sr.ReadLine();

        for (int i = 0; source != null; i++)
        {
            // *로 시작하는 줄은 무시 (주석)
            if (source.Substring(0, 1).Equals("*"))
            {
                source = sr.ReadLine();
                continue;
            }

            playDataList.Add(new List<PlayData>());

            string[] values = source.Split('/');
            if (values.Length == 0)
            {
                sr.Close();
                return;
            }

            for (int j = 0; j < values.Length; j++)
            {
                playDataList[i].Add(new PlayData());
                string[] tempArray = values[j].Split(',');
                try
                {
                    switch (int.Parse(tempArray[0]))
                    {
                        case 0:
                            playDataList[i][j].playType = PlayType.None;
                            break;
                        case 1:
                            playDataList[i][j].playType = PlayType.CreateFrame;
                            // 프레임 번호
                            playDataList[i][j].data[0] = float.Parse(tempArray[1]);
                            // 생성 위치 x
                            playDataList[i][j].data[1] = float.Parse(tempArray[2]);
                            // 생성 위치 y
                            playDataList[i][j].data[2] = float.Parse(tempArray[3]);
                            // 회전 각도
                            playDataList[i][j].data[3] = float.Parse(tempArray[4]);
                            // 게임프레임 크기 x
                            playDataList[i][j].data[4] = float.Parse(tempArray[5]);
                            // 게임프레임 크기 y
                            playDataList[i][j].data[5] = float.Parse(tempArray[6]);
                            // 노트 x크기
                            playDataList[i][j].data[6] = float.Parse(tempArray[7]);
                            // 생성 시간
                            playDataList[i][j].data[7] = float.Parse(tempArray[8]);
                            // Lerp 종류
                            playDataList[i][j].data[8] = float.Parse(tempArray[9]);
                            break;
                        case 2:
                            playDataList[i][j].playType = PlayType.MoveFrame;
                            // 프레임 번호
                            playDataList[i][j].data[0] = float.Parse(tempArray[1]);
                            // 목적 좌표 x
                            playDataList[i][j].data[1] = float.Parse(tempArray[2]);
                            // 목적 좌표 y
                            playDataList[i][j].data[2] = float.Parse(tempArray[3]);
                            // 이동 시간
                            playDataList[i][j].data[3] = float.Parse(tempArray[4]);
                            // Lerp 종류
                            playDataList[i][j].data[4] = float.Parse(tempArray[5]);
                            break;
                        case 3:
                            playDataList[i][j].playType = PlayType.RotateFrame;
                            // 프레임 번호
                            playDataList[i][j].data[0] = float.Parse(tempArray[1]);
                            // 목적 회전 각도
                            playDataList[i][j].data[1] = float.Parse(tempArray[2]);
                            // 회전 시간
                            playDataList[i][j].data[2] = float.Parse(tempArray[3]);
                            // Lerp 종류
                            playDataList[i][j].data[3] = float.Parse(tempArray[4]);
                            break;
                        case 4:
                            playDataList[i][j].playType = PlayType.ResizeFrame;
                            // 프레임 번호
                            playDataList[i][j].data[0] = float.Parse(tempArray[1]);
                            // 목적 크기 x
                            playDataList[i][j].data[1] = float.Parse(tempArray[2]);
                            // 목적 크기 y
                            playDataList[i][j].data[2] = float.Parse(tempArray[3]);
                            // 크기 변경 시간
                            playDataList[i][j].data[3] = float.Parse(tempArray[4]);
                            // Lerp 종류
                            playDataList[i][j].data[4] = float.Parse(tempArray[5]);
                            break;
                        case 5:
                            playDataList[i][j].playType = PlayType.CreateNote;
                            // 프레임 번호
                            playDataList[i][j].data[0] = float.Parse(tempArray[1]);
                            // 노트 종류
                            playDataList[i][j].data[1] = float.Parse(tempArray[2]);
                            // 생성 x좌표
                            playDataList[i][j].data[2] = float.Parse(tempArray[3]);

                            for (int k = 0; k < j; k++)
                                if (playDataList[i][j].data[1] == playDataList[i][k].data[1] && playDataList[i][j].data[3] == playDataList[i][k].data[3])
                                    continue;

                            switch (int.Parse(tempArray[2]))
                            {
                                case 0:
                                    GamePlayManager.Instance.CreateNote(NoteType.Touch, touchNotePrefab, judgementObjectPrefab);
                                    break;
                                case 1:
                                    GamePlayManager.Instance.CreateNote(NoteType.Slide, slideNotePrefab, judgementObjectPrefab);
                                    break;
                                case 2:
                                    GamePlayManager.Instance.CreateNote(NoteType.Long, longNotePrefab, judgementObjectPrefab);
                                    // 박자단위 길이
                                    playDataList[i][j].data[3] = float.Parse(tempArray[4]);
                                    break;
                            }

                            GamePlayManager.Instance.maxNoteCount++;
                            break;
                        case 6:
                            playDataList[i][j].playType = PlayType.DeleteFrame;
                            // 프레임 번호
                            playDataList[i][j].data[0] = float.Parse(tempArray[1]);
                            break;
                    }
                }
                catch
                {
                    Debug.LogError(i);
                }
            }

            source = sr.ReadLine();
        }

        //Debug.Log(GamePlayManager.Instance.maxNoteCount);

        StartCoroutine(PlayAudioCoroutine(audioPreTime));
        //StartCoroutine(PlayScoreCoroutine(audioPreTime));

        i = 0;
    }

    public IEnumerator PlayAudioCoroutine(float audioPreTime)
    {
        audioSource.clip = audioClip;
        audioSource.mute = true;
        audioSource.Play();

        yield return new WaitForSeconds(audioPreTime);

        tempTime = audioPreTime;
        audioSource.mute = false;
        audioSource.Stop();
        audioSource.Play();
    }

    private void FixedUpdate()
    {
        //Debug.Log(i);
        if (i >= playDataList.Count)
        {
            isScoreFinished = true;
            return;
        }
        else
        {
            for (int j = 0; j < playDataList[i].Count; j++)
            {
                PlayData temp = playDataList[i][j];
                switch (temp.playType)
                {
                    case PlayType.None:
                        break;
                    case PlayType.CreateNote:
                        switch (temp.data[1])
                        {
                            // 단타노트, 슬라이드 노트
                            case 0:
                            case 1:
                                gameFrameList[(int)temp.data[0]].CreateNote((NoteType)temp.data[1], temp.data[2]);
                                break;
                            // 롱노트
                            case 2:
                                gameFrameList[(int)temp.data[0]].CreateNote((NoteType)temp.data[1], temp.data[2], temp.data[3]);
                                break;
                        }
                        break;
                    case PlayType.CreateFrame:
                        if (!gameFrameList.ContainsKey((int)temp.data[0]))
                        {
                            gameFrameList.Add((int)temp.data[0], Instantiate(gameFramePrefab).GetComponent<GameFrame>());
                            gameFrameList[(int)temp.data[0]].CreateFrame(new Vector2(temp.data[1], temp.data[2]), temp.data[3], new Vector2(temp.data[4], temp.data[5]), temp.data[6], temp.data[7], (LerpType)temp.data[8]);
                        }
                        else Debug.LogWarning("이미 존재하는 게임프레임 번호입니다. 다른 번호를 할당해주세요.");
                        break;
                    case PlayType.MoveFrame:
                        gameFrameList[(int)temp.data[0]].MoveFrame(new Vector2(temp.data[1], temp.data[2]), temp.data[3], (LerpType)temp.data[4]);
                        break;
                    case PlayType.RotateFrame:
                        gameFrameList[(int)temp.data[0]].RotateFrame(temp.data[1], temp.data[2], (LerpType)temp.data[3]);
                        break;
                    case PlayType.ResizeFrame:
                        gameFrameList[(int)temp.data[0]].ResizeFrame(new Vector2(temp.data[1], temp.data[2]), temp.data[3], (LerpType)temp.data[4]);
                        break;
                    case PlayType.DeleteFrame:
                        gameFrameList[(int)temp.data[0]].Invoke("DestroyFrame", GamePlayManager.Instance.calibration);
                        break;
                }
            }
            i++;
        }
        //Debug.Log("fixedupdate");
    }
}

public class PlayData
{
    public PlayType playType;
    public float[] data = new float[9];
    // 0번 데이터는 게임프레임의 번호
}

public enum PlayType
{
    None,
    CreateFrame,
    MoveFrame,
    RotateFrame,
    ResizeFrame,
    CreateNote,
    DeleteFrame
}
