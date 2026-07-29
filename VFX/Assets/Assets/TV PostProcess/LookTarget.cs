using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LookTarget : MonoBehaviour
{
    [SerializeField] private ScriptableRendererFeature fullscreenPass;

    private Coroutine disableCoroutine;

    public void OnLookEnter()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
            disableCoroutine = null;
        }

        fullscreenPass.SetActive(true);
    }

    public void OnLookExit()
    {
        disableCoroutine = StartCoroutine(DisableAfterDelay());
    }

    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);

        fullscreenPass.SetActive(false);
        disableCoroutine = null;
    }
}