using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    [SerializeField] private FullScreenPassRendererFeature blurFeature;
    [SerializeField] private Material blurMaterial;

    private float targetBlend = 1f;
    [SerializeField] private float duration;

    private Coroutine blendRoutine;

    void Start()
    {
        blurFeature.SetActive(false);
        blurMaterial.SetFloat("_Blend", 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        blurFeature.SetActive(true);

        if (blendRoutine != null)
            StopCoroutine(blendRoutine);

        blendRoutine = StartCoroutine(BlendRoutine());
    }






    IEnumerator BlendRoutine()
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float blend = Mathf.Lerp(0f, targetBlend, t / duration);                        

            blurMaterial.SetFloat("_Blend", blend);
            yield return null;
        }

        t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float blend = Mathf.Lerp(targetBlend, 0f, t / duration);                       

            blurMaterial.SetFloat("_Blend", blend);
            yield return null;
        }

        blurMaterial.SetFloat("_Blend", 0f);
        blurFeature.SetActive(false);

        // Cargar la nueva escena
        SceneManager.LoadScene("DisplayScene");
    }
}