using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{

    [Header("Shader")]
    [SerializeField] private Material mat_DisCOLUMNA;

    private int _cutHeightID;

    [Header("Interaction")]
    [SerializeField] private float targetCutValue = 1f;
    [SerializeField] private float duration = 3f;

    private bool _isNear = false;

    private Coroutine _currentRoutine;

    // Referencia objeto 
    public MeshCollider _currentMeshCollider;

    private void Start()
    {
        _cutHeightID = Shader.PropertyToID("_CutHeight");
    }

    private void Update()
    {
        if (_isNear && Input.GetKeyDown(KeyCode.E))
        {
            if (_currentRoutine != null)
                StopCoroutine(_currentRoutine);

            _currentRoutine = StartCoroutine(AnimateCut());
        }
    }

    private IEnumerator AnimateCut()
    {
        float startValue = mat_DisCOLUMNA.GetFloat(_cutHeightID);
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float value = Mathf.Lerp(startValue, targetCutValue, t);
            mat_DisCOLUMNA.SetFloat(_cutHeightID, value);

            yield return null;
        }

        
        mat_DisCOLUMNA.SetFloat(_cutHeightID, targetCutValue);

       
        if (_currentMeshCollider != null)
        {
            _currentMeshCollider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactuable"))
        {
            _isNear = true;

            
            _currentMeshCollider = other.GetComponent<MeshCollider>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactuable"))
        {
            _isNear = false;

            _currentMeshCollider = null;
        }
    }
}
