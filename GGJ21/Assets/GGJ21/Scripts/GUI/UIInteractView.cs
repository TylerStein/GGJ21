using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInteractView : MonoBehaviour
{
    public TextMeshProUGUI text;
    public CanvasGroup group;

    public void ShowUI() {
        text.enabled = true;
        group.alpha = 1f;
    }

    public void SetMessage(string message) {
        text.text = message;
    }

    public void HideUI() {
        text.enabled = false;
        group.alpha = 0f;
    }
}
