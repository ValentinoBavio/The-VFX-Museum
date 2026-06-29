using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Coroutine shakeCoroutine;
    private Vector3 currentOffset;

    public void Shake(float duration, float strength, float frequency = 25f)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            transform.localPosition -= currentOffset;
            currentOffset = Vector3.zero;
        }

        shakeCoroutine = StartCoroutine(
            ShakeRoutine(duration, strength, frequency)
        );
    }

    private IEnumerator ShakeRoutine(
        float duration,
        float strength,
        float frequency
    )
    {
        float elapsed = 0f;

        float seedX = Random.Range(0f, 100f);
        float seedY = Random.Range(100f, 200f);

        while (elapsed < duration)
        {
            // Quitamos el desplazamiento del frame anterior.
            transform.localPosition -= currentOffset;

            float x =
                Mathf.PerlinNoise(seedX, elapsed * frequency) * 2f - 1f;

            float y =
                Mathf.PerlinNoise(seedY, elapsed * frequency) * 2f - 1f;

            float fadeOut = 1f - Mathf.Clamp01(elapsed / duration);

            currentOffset = new Vector3(x, y, 0f)
                            * strength
                            * fadeOut;

            transform.localPosition += currentOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition -= currentOffset;
        currentOffset = Vector3.zero;
        shakeCoroutine = null;
    }

    private void OnDisable()
    {
        if (currentOffset != Vector3.zero)
        {
            transform.localPosition -= currentOffset;
            currentOffset = Vector3.zero;
        }
    }
}