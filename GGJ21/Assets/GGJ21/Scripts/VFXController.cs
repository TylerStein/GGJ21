using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    public GameObject dashParticlesPrefab;
    public float dashParticlesDuration = 1f;

    public GameObject bloodParticlesPrefab;
    public float bloodParticlesDuration = 1f;

    public GameObject impactParticlesPrefab;
    public float impactParticlesDuration = 1f;

    public GameObject blockParticlesPrefab;
    public float blockParticlesDuration = 1f;

    public void PlayDash(Vector3 position, Quaternion rotation) {
        SpawnEmitter(dashParticlesPrefab, position, rotation, dashParticlesDuration);
    }

    public void PlayBlood(Vector3 position, Quaternion rotation) {
        SpawnEmitter(bloodParticlesPrefab, position, rotation, bloodParticlesDuration);
    }

    public void PlayMetalImpact(Vector3 position, Quaternion rotation) {
        SpawnEmitter(impactParticlesPrefab, position, rotation, impactParticlesDuration);
    }

    public void PlayBlockImpact(Vector3 position, Quaternion rotation) {
        SpawnEmitter(blockParticlesPrefab, position, rotation, blockParticlesDuration);
    }

    public void SpawnEmitter(GameObject prefab, Vector3 position, Quaternion rotation, float duration) {
        GameObject obj = Instantiate(prefab, position, rotation, null);
        Destroy(obj, duration);
    }
}
