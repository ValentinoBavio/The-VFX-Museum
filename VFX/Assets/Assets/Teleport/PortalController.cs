using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PortalController : MonoBehaviour
{
    [SerializeField] private Material fullscreenMaterial;
    [SerializeField] private string sceneName;
    [SerializeField] private float duration = 1f;

    private bool activated;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;
            StartCoroutine(Transition());
        }
    }

    IEnumerator Transition()
    {
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            fullscreenMaterial.SetFloat("_Progress", t / duration);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}