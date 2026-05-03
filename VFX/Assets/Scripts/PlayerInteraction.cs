using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Shader")]
    [SerializeField] private string cutHeightProperty = "_CutHeight";

    [Header("Interaction")]
    [SerializeField] private float visibleCutValue = 0f;
    [SerializeField] private float targetCutValue = 5f;
    [SerializeField] private float duration = 3f;

    [Header("Input")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Texto flotante")]
    [SerializeField] private GameObject pressETextObject;
    [SerializeField] private bool showTextOnlyWhenNear = true;
    [SerializeField] private bool hideTextOnInteract = true;

    [Header("Audio - Dissolve")]
    [SerializeField] private AudioClip dissolveStartSFX;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    [Range(0f, 1f)]
    [SerializeField] private float dissolveVolume = 1f;

    [SerializeField] private float dissolvePitchMin = 0.95f;
    [SerializeField] private float dissolvePitchMax = 1.05f;

    [Header("Audio 3D")]
    [SerializeField] private float audioMinDistance = 1f;
    [SerializeField] private float audioMaxDistance = 12f;

    private int _cutHeightID;
    private bool _isNear = false;
    private bool _isDissolving = false;

    private GameObject _currentTarget;

    private readonly HashSet<GameObject> _usedTargets = new HashSet<GameObject>();

    private MaterialPropertyBlock _propertyBlock;

    private void Start()
    {
        _cutHeightID = Shader.PropertyToID(cutHeightProperty);
        _propertyBlock = new MaterialPropertyBlock();

        ResetAllInteractuables();

        if (pressETextObject != null && showTextOnlyWhenNear)
        {
            pressETextObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!_isNear) return;
        if (_isDissolving) return;
        if (_currentTarget == null) return;
        if (_usedTargets.Contains(_currentTarget)) return;

        if (Input.GetKeyDown(interactKey))
        {
            StartCoroutine(AnimateCut(_currentTarget));
        }
    }

    private IEnumerator AnimateCut(GameObject target)
    {
        if (target == null)
            yield break;

        _isDissolving = true;
        _usedTargets.Add(target);

        if (hideTextOnInteract && pressETextObject != null)
        {
            pressETextObject.SetActive(false);
        }

        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        Collider[] colliders = target.GetComponentsInChildren<Collider>();

        PlayDissolveSound(target.transform.position);

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;
            float value = Mathf.Lerp(visibleCutValue, targetCutValue, t);

            SetCutHeight(renderers, value);

            yield return null;
        }

        SetCutHeight(renderers, targetCutValue);

        foreach (Collider col in colliders)
        {
            if (col != null)
                col.enabled = false;
        }

        foreach (Renderer rend in renderers)
        {
            if (rend != null)
                rend.enabled = false;
        }

        if (_currentTarget == target)
        {
            _currentTarget = null;
            _isNear = false;
        }

        _isDissolving = false;
    }

    private void PlayDissolveSound(Vector3 position)
    {
        if (dissolveStartSFX == null) return;

        GameObject audioObj = new GameObject("Dissolve Start SFX");
        audioObj.transform.position = position;

        AudioSource source = audioObj.AddComponent<AudioSource>();

        source.clip = dissolveStartSFX;
        source.volume = dissolveVolume;
        source.pitch = Random.Range(dissolvePitchMin, dissolvePitchMax);

        source.spatialBlend = 1f;
        source.minDistance = audioMinDistance;
        source.maxDistance = audioMaxDistance;
        source.dopplerLevel = 0f;

        if (sfxMixerGroup != null)
            source.outputAudioMixerGroup = sfxMixerGroup;

        source.Play();

        Destroy(audioObj, dissolveStartSFX.length / Mathf.Abs(source.pitch) + 0.2f);
    }

    private void SetCutHeight(Renderer[] renderers, float value)
    {
        foreach (Renderer rend in renderers)
        {
            if (rend == null) continue;

            rend.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(_cutHeightID, value);
            rend.SetPropertyBlock(_propertyBlock);
        }
    }

    private void SetCutHeight(Renderer rend, float value)
    {
        if (rend == null) return;

        rend.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetFloat(_cutHeightID, value);
        rend.SetPropertyBlock(_propertyBlock);
    }

    private void ResetAllInteractuables()
    {
        GameObject[] interactuables = GameObject.FindGameObjectsWithTag("Interactuable");

        foreach (GameObject obj in interactuables)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

            foreach (Renderer rend in renderers)
            {
                SetCutHeight(rend, visibleCutValue);
                rend.enabled = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Interactuable")) return;

        GameObject target = other.gameObject;

        if (_usedTargets.Contains(target)) return;
        if (_isDissolving) return;

        _isNear = true;
        _currentTarget = target;

        if (pressETextObject != null && showTextOnlyWhenNear)
        {
            pressETextObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Interactuable")) return;

        GameObject target = other.gameObject;

        if (_currentTarget == target && !_isDissolving)
        {
            _isNear = false;
            _currentTarget = null;

            if (pressETextObject != null && showTextOnlyWhenNear)
            {
                pressETextObject.SetActive(false);
            }
        }
    }
}