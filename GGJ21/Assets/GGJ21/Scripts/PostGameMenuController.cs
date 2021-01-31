using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PostGameMenuController : MonoBehaviour
{
    public int mainMenuSceneIndex = 0;
    public int mainGameSceneIndex = 1;

    public Button mainMenuButton;
    public Button tryAgainButton;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuButton.onClick.AddListener(() => {
            SceneManager.LoadScene(mainMenuSceneIndex);
        });

        if (tryAgainButton) {
            tryAgainButton.onClick.AddListener(() => {
                SceneManager.LoadScene(mainGameSceneIndex);
            });
        }
    }
}
