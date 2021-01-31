using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
    public GameManager gameManager;
    public AudioSource source;

    public AudioClip death;
    public AudioClip pain;
    public AudioClip impact;

    void Start() {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.mutedEvent.AddListener(onChangeMute);
    }

    void OnDestroy() {
        gameManager.mutedEvent.RemoveListener(onChangeMute);
    }

    void onChangeMute(bool muted) {
        source.mute = muted;
    }

    public void OnDeath() {
        if (death) source.PlayOneShot(death);
    }

    public void OnPain() {
        if (pain) source.PlayOneShot(pain);
    }

    public void OnMetalImpact() {
        if (impact) source.PlayOneShot(impact);
    }
}
