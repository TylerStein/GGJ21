using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobUIController : MonoBehaviour
{
    public Canvas canvas;
    public UIHealthBar healthBar;
    public MobController mobController;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.CreateUI(mobController.maxHitPoints);
    }

    public void OnHit() {
        healthBar.SetHealth(mobController.hitPoints);
    }

    public void HideUI() {
        canvas.enabled = false;
    }

    public void ShowUI() {
        canvas.enabled = true;
    }
}
