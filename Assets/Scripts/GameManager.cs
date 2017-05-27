using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadScene(int scene) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
        Time.timeScale = 1;
    }
}
