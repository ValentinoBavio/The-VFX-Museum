using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RainAreaCrossfade : MonoBehaviour
{
    [Header("Fuentes de audio")]
    [SerializeField] private AudioSource outsideRain;
    [SerializeField] private AudioSource insideRain;

    [Header("Volúmenes máximos")]
    [Range(0f, 1f)]
    [SerializeField] private float outsideVolume = 0.8f;

    [Range(0f, 1f)]
    [SerializeField] private float insideVolume = 0.8f;

    [Header("Transición")]
    [Min(0.01f)]
    [SerializeField] private float fadeDuration = 2f;

    [Tooltip("Activarlo si el jugador comienza la escena dentro de la bóveda.")]
    [SerializeField] private bool playerStartsInside;

    private Coroutine fadeCoroutine;
    private int playerCollidersInside;

    private void Awake()
    {
        Collider zoneCollider = GetComponent<Collider>();
        zoneCollider.isTrigger = true;

        PrepareAudioSource(outsideRain);
        PrepareAudioSource(insideRain);

        SetVolumesImmediately(playerStartsInside);
    }

    private void PrepareAudioSource(AudioSource source)
    {
        if (source == null)
            return;

        source.loop = true;
        source.spatialBlend = 0f;

        if (!source.isPlaying)
            source.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other))
            return;

        playerCollidersInside++;

        // Evita ejecutar varias veces si el jugador tiene más de un collider.
        if (playerCollidersInside == 1)
            StartCrossfade(isInside: true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other))
            return;

        playerCollidersInside = Mathf.Max(0, playerCollidersInside - 1);

        if (playerCollidersInside == 0)
            StartCrossfade(isInside: false);
    }

    private bool IsPlayer(Collider other)
    {
        return other.CompareTag("Player") ||
               other.transform.root.CompareTag("Player");
    }

    private void StartCrossfade(bool isInside)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(CrossfadeRoutine(isInside));
    }

    private IEnumerator CrossfadeRoutine(bool isInside)
    {
        if (outsideRain == null || insideRain == null)
            yield break;

        float startingOutsideVolume = outsideRain.volume;
        float startingInsideVolume = insideRain.volume;

        float targetOutsideVolume = isInside ? 0f : outsideVolume;
        float targetInsideVolume = isInside ? insideVolume : 0f;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsedTime / fadeDuration);

            // Suaviza el comienzo y el final de la transición.
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            outsideRain.volume = Mathf.Lerp(
                startingOutsideVolume,
                targetOutsideVolume,
                smoothProgress
            );

            insideRain.volume = Mathf.Lerp(
                startingInsideVolume,
                targetInsideVolume,
                smoothProgress
            );

            yield return null;
        }

        outsideRain.volume = targetOutsideVolume;
        insideRain.volume = targetInsideVolume;

        fadeCoroutine = null;
    }

    private void SetVolumesImmediately(bool isInside)
    {
        if (outsideRain == null || insideRain == null)
            return;

        outsideRain.volume = isInside ? 0f : outsideVolume;
        insideRain.volume = isInside ? insideVolume : 0f;
    }
}