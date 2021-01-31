using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class PlayerUIController : MonoBehaviour
{
    public PlayerController playerController;
    public UIHealthBar healthBar;
    public UIInteractView interactView;
    public UIPauseMenu pauseMenu;

    public CanvasGroup postGameMessageGroup;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.CreateUI(playerController.maxHitPoints);
    }

    public void OnHit() {
        healthBar.SetHealth(playerController.hitPoints);
    }

    public void ShowInteractMessage(string message) {
        interactView.SetMessage(message);
        interactView.ShowUI();
    }

    public void HideInteractMessage() {
        interactView.HideUI();
    }

    public void SetPaused(bool paused) {
        if (paused) pauseMenu.ShowUI();
        else pauseMenu.HideUI();
    }

    public void ShowPostGameMessage() {
        postGameMessageGroup.alpha = 1f;
    }

    public void HidePostGameMessage() {
        postGameMessageGroup.alpha = 0f;
    }
}
