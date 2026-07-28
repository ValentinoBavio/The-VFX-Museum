using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TargetDummy : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Transform visual;

    [Header("Reacción")]
    [SerializeField] private float hitTiltAngle = 10f;
    [SerializeField] private float hitReactionDuration = 0.15f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitClip;

    [Range(0f, 1f)]
    [SerializeField] private float hitVolume = 1f;

    [Header("Evento opcional")]
    [SerializeField] private UnityEvent onHit;

    private Quaternion restingRotation;
    private Coroutine reactionCoroutine;

    private void Awake()
    {
        if (visual == null)
        {
            visual = transform;
        }

        restingRotation = visual.localRotation;
    }

    public void ReceiveHit(
        Vector3 hitPoint,
        Vector3 shotDirection
    )
    {
        onHit?.Invoke();

        if (audioSource != null && hitClip != null)
        {
            audioSource.PlayOneShot(
                hitClip,
                hitVolume
            );
        }

        if (reactionCoroutine != null)
        {
            StopCoroutine(reactionCoroutine);
        }

        reactionCoroutine =
            StartCoroutine(HitReactionRoutine());
    }

    private IEnumerator HitReactionRoutine()
    {
        Quaternion hitRotation =
            restingRotation *
            Quaternion.Euler(
                -hitTiltAngle,
                0f,
                0f
            );

        float halfDuration =
            hitReactionDuration * 0.5f;

        yield return RotateVisual(
            visual.localRotation,
            hitRotation,
            halfDuration
        );

        yield return RotateVisual(
            visual.localRotation,
            restingRotation,
            halfDuration
        );

        visual.localRotation = restingRotation;
        reactionCoroutine = null;
    }

    private IEnumerator RotateVisual(
        Quaternion startRotation,
        Quaternion targetRotation,
        float duration
    )
    {
        if (duration <= 0f)
        {
            visual.localRotation = targetRotation;
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float normalizedTime =
                Mathf.Clamp01(elapsedTime / duration);

            normalizedTime =
                normalizedTime *
                normalizedTime *
                (3f - 2f * normalizedTime);

            visual.localRotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    normalizedTime
                );

            yield return null;
        }

        visual.localRotation = targetRotation;
    }
}