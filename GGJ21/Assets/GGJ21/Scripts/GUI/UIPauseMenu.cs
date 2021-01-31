using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPauseMenu : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject canvasRoot;

    public Button resumeButton;

    public Button muteButton;
    public TMPro.TextMeshProUGUI muteText;
    public TMPro.TextMeshProUGUI unmuteText;

    public Button exitButton;

    void Start() {
        gameManager = FindObjectOfType<GameManager>();
        muteButton.onClick.AddListener(onClickMuteButton);
        exitButton.onClick.AddListener(onClickExitButton);
        resumeButton.onClick.AddListener(onClickResumeButton);
        canvasRoot.SetActive(gameManager.isPaused);
    }

    void OnDestroy() {
        muteButton.onClick.RemoveListener(onClickMuteButton);
        exitButton.onClick.RemoveListener(onClickExitButton);
        resumeButton.onClick.RemoveListener(onClickResumeButton);
    }

    void onClickMuteButton() {
        gameManager.ToggleMute();
        muteText.enabled = !gameManager.isMuted;
        unmuteText.enabled = gameManager.isMuted;
    }

    void onClickExitButton() {
        gameManager.QuitToMain();
    }

    void onClickResumeButton() {
        gameManager.TogglePause();
    }

    public void ShowUI() {
        canvasRoot.SetActive(true);
    }
    
    public void HideUI() {
        canvasRoot.SetActive(false);
    }
}
