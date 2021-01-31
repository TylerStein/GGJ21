using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInteractView : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image textBackground;

    public void ShowUI() {
        text.enabled = true;
        textBackground.enabled = true;
    }

    public void SetMessage(string message) {
        text.text = message;
    }

    public void HideUI() {
        text.enabled = false;
        textBackground.enabled = false;
    }
}
