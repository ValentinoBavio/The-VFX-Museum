using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DestroyOnLayer : MonoBehaviour
{
    [Header("Layer que rompe")]
    public string targetLayerName = "RompePociones";
    private int targetLayer;

    [Header("Charco (Quad)")]
    public GameObject puddlePrefab;
    public Vector3 offset = new Vector3(0, 0.05f, 0);

    [Header("Variacion")]
    public float minScale = 0.6f;
    public float maxScale = 1.4f;

    [Header("Audio - Romper pocion")]
    public AudioClip breakSFX;
    public AudioMixerGroup sfxMixerGroup;
    [Range(0f, 1f)] public float breakVolume = 1f;
    public float breakPitchMin = 0.95f;
    public float breakPitchMax = 1.08f;

    private bool alreadyBroken = false;

    private void Start()
    {
        targetLayer = LayerMask.NameToLayer(targetLayerName);

        if (targetLayer == -1)
        {
            Debug.LogWarning($"La layer '{targetLayerName}' no existe.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (alreadyBroken) return;

        if (collision.gameObject.layer == targetLayer)
        {
            alreadyBroken = true;

            Vector3 hitPoint = collision.contacts.Length > 0
                ? collision.contacts[0].point
                : transform.position;

            SpawnPuddle(hitPoint);
            PlayBreakSound(hitPoint);

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyBroken) return;

        if (other.gameObject.layer == targetLayer)
        {
            alreadyBroken = true;

            Vector3 hitPoint = transform.position;

            SpawnPuddle(hitPoint);
            PlayBreakSound(hitPoint);

            Destroy(gameObject);
        }
    }

    private void SpawnPuddle(Vector3 position)
    {
        if (puddlePrefab == null) return;

        Quaternion rot = Quaternion.Euler(90f, Random.Range(0f, 360f), 0f);

        GameObject puddle = Instantiate(puddlePrefab, position + offset, rot);

        float randomScale = Random.Range(minScale, maxScale);
        puddle.transform.localScale = Vector3.one * randomScale;
    }

    private void PlayBreakSound(Vector3 position)
    {
        if (breakSFX == null) return;

        GameObject audioObj = new GameObject("Potion Break SFX");
        audioObj.transform.position = position;

        AudioSource source = audioObj.AddComponent<AudioSource>();
        source.clip = breakSFX;
        source.volume = breakVolume;
        source.pitch = Random.Range(breakPitchMin, breakPitchMax);

        source.spatialBlend = 1f;
        source.minDistance = 1f;
        source.maxDistance = 12f;
        source.dopplerLevel = 0f;

        if (sfxMixerGroup != null)
            source.outputAudioMixerGroup = sfxMixerGroup;

        source.Play();

        Destroy(audioObj, breakSFX.length / Mathf.Abs(source.pitch) + 0.2f);
    }
}