using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DifuseSceneTransition : MonoBehaviour
{
    [SerializeField] private Material fullscreenMaterial;
    [SerializeField] private FullScreenPassRendererFeature blurFeature;
    [SerializeField] private float startValue = 1f;
    [SerializeField] private float endValue = 0f;
    [SerializeField] private float duration = 2f;

    private void Start()
    {
        fullscreenMaterial.SetFloat("_DissolveAmount", startValue);
        StartCoroutine(DissolveDown());
    }

    private IEnumerator DissolveDown()
    {
        blurFeature.SetActive(true);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float value = Mathf.Lerp(startValue, endValue, elapsed / duration);
            fullscreenMaterial.SetFloat("_DissolveAmount", value);

            yield return null;
        }

        fullscreenMaterial.SetFloat("_DissolveAmount", endValue);

        blurFeature.SetActive(false);
    }
}
