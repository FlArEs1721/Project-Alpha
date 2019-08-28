using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializeManager : MonoBehaviour {

    private void Start() {
        SceneManager.LoadScene("Title");
    }
}
