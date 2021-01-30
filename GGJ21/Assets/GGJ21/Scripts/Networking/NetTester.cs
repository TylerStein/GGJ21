using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetTester : MonoBehaviour
{
    public InputField addressInput;
    public Button submitButton;
    public Text statusCodeText;
    public Text statusMessageText;

    public NetItemManager netItemManager;
    public string addressValue = "";

    // Start is called before the first frame update
    void Start()
    {
        netItemManager.serverHealthEvent.AddListener((NetHealth health) => {
            if (health == null) {
                statusCodeText.text = "null";
                statusMessageText.text = "null";
            } else {
                statusCodeText.text = health.status;
                statusMessageText.text = health.code.ToString();
            }
        });

        addressInput.onEndEdit.AddListener((string value) => {
            addressValue = value;
        });

        submitButton.onClick.AddListener(() => {
            netItemManager.serverAddress = addressValue;
            netItemManager.GetServerHealth();
        });

    }
}
