using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {
    private bool canEscape = true;

    private void Update() {
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape) && canEscape) {
            // 앱 종료
            Application.Quit();
        }

        if (Input.touchCount > 0) {
            for (int i = 0; i < Input.touchCount; i++) {
                if (Input.GetTouch(i).phase == TouchPhase.Ended) {
                    // 씬 넘어가기
                    SceneManager.LoadScene("SongSelect");
                }
            }
        }
#endif
#if UNITY_EDITOR_WIN
        if (Input.GetMouseButtonDown(0)) {
            SceneManager.LoadScene("SongSelect");
        }
#endif
    }
}
