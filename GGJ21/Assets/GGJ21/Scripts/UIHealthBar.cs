using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public GameObject container;
    public GameObject prefab;

    public List<GameObject> hitContainers = new List<GameObject>();
    public List<Image> hitImages = new List<Image>();

    public void ClearUI() {
        foreach (GameObject obj in hitContainers) {
            Destroy(obj);
        }

        hitContainers.Clear();
        hitImages.Clear();
    }

    public void CreateUI(int count) {
        ClearUI();
        for (int i = 0; i < count; i++) {
            GameObject obj = Instantiate(prefab, container.transform);
            Image surfaceImage = obj.transform.GetChild(0).GetComponent<Image>();
            hitContainers.Add(obj);
            hitImages.Add(surfaceImage);
        }
    }

    public void SetHealth(int health) {
        for (int i = 0; i < hitContainers.Count; i++) {
            if (i < health) {
                hitImages[i].enabled = true;
            } else {
                hitImages[i].enabled = false;
            }

        }
    }
}
