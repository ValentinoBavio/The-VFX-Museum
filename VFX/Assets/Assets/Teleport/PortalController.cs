using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    [Header("Efecto de blur")]
    [SerializeField] private FullScreenPassRendererFeature blurFeature;
    [SerializeField] private Material blurMaterial;

    [SerializeField] private float targetBlend = 1f;
    [SerializeField] private float duration = 1f;

    [Header("Sonido de teletransporte")]
    [SerializeField] private AudioClip teleportSound;

    [Range(0f, 1f)]
    [SerializeField] private float teleportVolume = 1f;

    private Coroutine blendRoutine;
    private bool portalActivated;

    private void Start()
    {
        blurFeature.SetActive(false);
        blurMaterial.SetFloat("_Blend", 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Evita que el portal se active varias veces.
        if (portalActivated)
            return;

        portalActivated = true;

        // Reproduce el sonido en un objeto que sobrevivirá
        // al cambio de escena.
        PersistentSFX.Play(
            teleportSound,
            teleportVolume
        );

        blurFeature.SetActive(true);

        if (blendRoutine != null)
            StopCoroutine(blendRoutine);

        blendRoutine = StartCoroutine(BlendRoutine());
    }

    private IEnumerator BlendRoutine()
    {
        float t = 0f;

        // Aumentar el blur.
        while (t < duration)
        {
            t += Time.deltaTime;

            float progress = Mathf.Clamp01(t / duration);

            float blend = Mathf.Lerp(
                0f,
                targetBlend,
                progress
            );

            blurMaterial.SetFloat("_Blend", blend);

            yield return null;
        }

        blurMaterial.SetFloat("_Blend", targetBlend);

        t = 0f;

        // Disminuir el blur.
        while (t < duration)
        {
            t += Time.deltaTime;

            float progress = Mathf.Clamp01(t / duration);

            float blend = Mathf.Lerp(
                targetBlend,
                0f,
                progress
            );

            blurMaterial.SetFloat("_Blend", blend);

            yield return null;
        }

        blurMaterial.SetFloat("_Blend", 0f);
        blurFeature.SetActive(false);

        // El sonido seguirá reproduciéndose aunque
        // este objeto desaparezca al cargar la escena.
        SceneManager.LoadScene("DisplayScene");
    }
}