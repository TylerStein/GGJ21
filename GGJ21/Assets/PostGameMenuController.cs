using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PostGameMenuController : MonoBehaviour
{
    public int mainMenuSceneIndex = 0;

    public Button mainMenuButton;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuButton.onClick.AddListener(() => {
            SceneManager.LoadScene(mainMenuSceneIndex);
        });
    }
}
