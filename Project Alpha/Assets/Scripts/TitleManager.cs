using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            // 씬 넘어가기
            SceneManager.LoadScene("SongSelect");
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
            SceneManager.LoadScene("SongSelect");
#endif
    }
}
