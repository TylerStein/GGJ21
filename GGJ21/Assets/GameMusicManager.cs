using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusicManager : MonoBehaviour
{
    public GameManager gameManager;
    public AudioSource calmAudioSource;
    public AudioSource combatAudioSource;

    public float transitionSpeed;
    public bool combatMusic = false;

    public int aggroMobCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.mutedEvent.AddListener((muted) => {
            calmAudioSource.mute = muted;
            combatAudioSource.mute = muted;
        });

        calmAudioSource.volume = 0f;
        combatAudioSource.volume = 1f;
    }

    void Update() {
        if (combatMusic) {
            if (calmAudioSource.volume > 0f) calmAudioSource.volume -= transitionSpeed * Time.deltaTime;
            if (combatAudioSource.volume < 1f) combatAudioSource.volume += transitionSpeed * Time.deltaTime;
        } else {
            if (calmAudioSource.volume < 1f) calmAudioSource.volume += transitionSpeed * Time.deltaTime;
            if (combatAudioSource.volume > 0f) combatAudioSource.volume -= transitionSpeed * Time.deltaTime;
        }
    }

    public void AddAggroMob() {
        aggroMobCount++;
        combatMusic = true;
    }

    public void RemoveAggroMob() {
        aggroMobCount--;
        combatMusic = (aggroMobCount > 0);
    }
}
