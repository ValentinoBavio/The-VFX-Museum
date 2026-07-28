using System.Collections;
using UnityEngine;

public class WeaponCameraShake : MonoBehaviour
{
    private Vector3 initialLocalPosition;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        initialLocalPosition = transform.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(
            ShakeRoutine(duration, magnitude)
        );
    }

    private IEnumerator ShakeRoutine(
        float duration,
        float magnitude
    )
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float normalizedTime =
                Mathf.Clamp01(elapsedTime / duration);

            float currentMagnitude =
                Mathf.Lerp(magnitude, 0f, normalizedTime);

            Vector2 randomOffset =
                Random.insideUnitCircle * currentMagnitude;

            transform.localPosition =
                initialLocalPosition +
                new Vector3(
                    randomOffset.x,
                    randomOffset.y,
                    0f
                );

            yield return null;
        }

        transform.localPosition = initialLocalPosition;
        shakeCoroutine = null;
    }

    private void OnDisable()
    {
        transform.localPosition = initialLocalPosition;
    }
}