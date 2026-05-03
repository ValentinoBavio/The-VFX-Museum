using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        // Esto resetea todos los objetos interactuables al iniciar el Play
        // sin tocar el material original del proyecto.
        ResetAllInteractuables();
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

        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        Collider[] colliders = target.GetComponentsInChildren<Collider>();

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

        // Al terminar el efecto, apagamos colliders para que puedas pasar.
        foreach (Collider col in colliders)
        {
            if (col != null)
                col.enabled = false;
        }

        // Y apagamos los renderers para que desaparezca de verdad.
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
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Interactuable")) return;

        GameObject target = other.gameObject;

        if (_currentTarget == target && !_isDissolving)
        {
            _isNear = false;
            _currentTarget = null;
        }
    }
}