using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class PlayerUIController : MonoBehaviour
{
    public PlayerController playerController;
    public UIHealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.CreateUI(playerController.maxHitPoints);
    }

    public void OnHit() {
        healthBar.SetHealth(playerController.hitPoints);
    }
}
