using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongSelectManager : MonoBehaviour
{
    public TextAsset songList;

    [Space]
    public GameObject songPackPrefab;

    [Space]
    public GameObject songPackSelectPanel;
    public Text songTitleText;
    public Text songComposerText;

    private SendInfo sendObj;

    private int currentPackIndex = 0;
    private int currentSongIndex = 0;
    private SongDifficulty currentDifficulty = SongDifficulty.Hard;
    
    private void Start()
    {
        Initiate();
        if (!sendObj.songPlayed)
        {
            GoToPackSelect();
        }
        else
        {
            currentPackIndex = sendObj.currentPackIndex;
            currentSongIndex = sendObj.currentSongIndex;
            currentDifficulty = sendObj.currentDifficulty;

            SelectPack(currentPackIndex, currentSongIndex);
        }
    }

    private void Initiate()
    {
        sendObj = GameObject.Find("SendInfoObject").GetComponent<SendInfo>();

        // 음악팩과 음악들의 정보를 받아옴
        StringReader sr = new StringReader(songList.text);
        string source = sr.ReadLine();
        for (int i = 0; source != null; i++)
        {
            string[] values = source.Split(',');
            if (values[0] == "0")
            {
                SongPack newPack = new SongPack(int.Parse(values[1]), values[2], values[3], (SongState)int.Parse(values[4]));
            }
            else if (values[0] == "1")
            {
                Song newSong = new Song(int.Parse(values[1]), int.Parse(values[2]), values[3], values[4], values[5], new string[4] { values[6], values[7], values[8], values[9] }, (SongState)int.Parse(values[10]), (SongState)int.Parse(values[11]));
            }

            source = sr.ReadLine();
        }
    }

    public void SelectPack(int packIndex, int songIndex = 0)
    {
        if (SongPack.songPackDictionary[packIndex].songDictionary.Count <= 0) return;

        currentSongIndex = songIndex;
        songPackSelectPanel.SetActive(false);
        RefreshSongInfo(packIndex, songIndex);
    }

    private void RefreshSongInfo(int packIndex, int songIndex)
    {
        songTitleText.text = SongPack.songPackDictionary[packIndex].songDictionary[songIndex].songTitle;
        songComposerText.text = SongPack.songPackDictionary[packIndex].songDictionary[songIndex].composer;
    }

    public void GoToPackSelect()
    {
        songPackSelectPanel.SetActive(true);

        Transform[] childTransforms = songPackSelectPanel.GetComponentsInChildren<Transform>();
        for (int i = childTransforms.Length - 1; i >= 0; i--)
        {
            if (childTransforms[i].Equals(songPackSelectPanel.transform)) continue;

            Destroy(childTransforms[i].gameObject);
        }

        for (int i = 0; i < SongPack.songPackDictionary.Count; i++)
        {
            GameObject songPackButton = CreateSongPackButton(i);
            Text packNameText = songPackButton.GetComponentInChildren<Text>();

            songPackButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300 + 300 * i, 0);
            packNameText.text = SongPack.songPackDictionary[i].packName;
        }
    }

    private GameObject CreateSongPackButton(int index)
    {
        GameObject songPackButton = Instantiate(songPackPrefab, songPackSelectPanel.transform);
        songPackButton.GetComponent<Button>().onClick.AddListener(delegate() { this.SelectPack(index); });
        if (!SongPack.songPackDictionary[index].illustPath.Equals(""))
            songPackButton.GetComponent<Image>().sprite = Resources.Load(SongPack.songPackDictionary[index].illustPath, typeof(Sprite)) as Sprite;

        return songPackButton;
    }

    public void NextSong()
    {
        currentSongIndex++;
        if (currentSongIndex >= SongPack.songPackDictionary[currentPackIndex].songDictionary.Count) currentSongIndex = 0;

        RefreshSongInfo(currentPackIndex, currentSongIndex);
    }

    public void PreviousSong()
    {
        currentSongIndex--;
        if (currentSongIndex < 0) currentSongIndex = SongPack.songPackDictionary[currentPackIndex].songDictionary.Count - 1;

        RefreshSongInfo(currentPackIndex, currentSongIndex);
    }

    public void SelectSong()
    {
        sendObj.songTitle = SongPack.songPackDictionary[currentPackIndex].songDictionary[currentSongIndex].songTitle;
        sendObj.songDifficulty = currentDifficulty.ToString();
        sendObj.currentPackIndex = currentPackIndex;
        sendObj.currentSongIndex = currentSongIndex;
        sendObj.currentDifficulty = currentDifficulty;
        sendObj.songPlayed = true;
        SceneManager.LoadScene("Game");
    }
}

public class SongPack
{
    public string packName = "";
    public SongState state = SongState.Playable;
    public string illustPath = "";
    public Dictionary<int, Song> songDictionary = new Dictionary<int, Song>();

    public static Dictionary<int, SongPack> songPackDictionary = new Dictionary<int, SongPack>();

    public SongPack(int index, string packName, string illustPath, SongState state)
    {
        this.packName = packName;
        this.illustPath = illustPath;
        this.state = state;

        // 존재하지 않는 인덱스라면 추가
        // 이미 존재하는 인덱스라면 이 객체로 변경
        if (!SongPack.songPackDictionary.ContainsKey(index)) SongPack.songPackDictionary.Add(index, this);
        else SongPack.songPackDictionary[index] = this;
    }
}

public class Song
{
    public string songTitle = "";
    public string composer = "";
    public string illustPath = "";
    public string[] level = new string[4];
    public SongState normalLevelState = SongState.Playable;
    public SongState specialLevelState = SongState.Playable;

    public Song(int packIndex, int index, string songTitle, string composer, string illustPath, string[] level, SongState normalLevelState, SongState specialLevelState)
    {
        this.songTitle = songTitle;
        this.composer = composer;
        this.illustPath = illustPath;
        this.level = level;
        this.normalLevelState = normalLevelState;
        this.specialLevelState = specialLevelState;

        // 존재하지 않는 인덱스라면 추가
        // 이미 존재하는 인덱스라면 이 객체로 변경
        Dictionary<int, Song> songDictionary = SongPack.songPackDictionary[packIndex].songDictionary;
        if (!songDictionary.ContainsKey(index)) SongPack.songPackDictionary[packIndex].songDictionary.Add(index, this);
        else SongPack.songPackDictionary[packIndex].songDictionary[index] = this;
    }
}

public enum SongState
{
    Playable,
    Locked,
    DLC
}

public enum SongDifficulty
{
    Easy,
    Normal,
    Hard,
    Special
}
