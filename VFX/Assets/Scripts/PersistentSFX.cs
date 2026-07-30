using System.Collections;
using UnityEngine;

public class PersistentSFX : MonoBehaviour
{
    public static void Play(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning(
                "No se asignó el sonido de teletransporte."
            );

            return;
        }

        GameObject audioObject = new GameObject(
            "Persistent SFX - " + clip.name
        );

        DontDestroyOnLoad(audioObject);

        PersistentSFX persistentSFX =
            audioObject.AddComponent<PersistentSFX>();

        persistentSFX.StartCoroutine(
            persistentSFX.PlayAndDestroy(clip, volume)
        );
    }

    private IEnumerator PlayAndDestroy(
        AudioClip clip,
        float volume)
    {
        AudioSource audioSource =
            gameObject.AddComponent<AudioSource>();

        audioSource.clip = clip;
        audioSource.volume = Mathf.Clamp01(volume);
        audioSource.spatialBlend = 0f;
        audioSource.loop = false;
        audioSource.playOnAwake = false;

        audioSource.Play();

        while (audioSource.isPlaying)
        {
            yield return null;
        }

        Destroy(gameObject);
    }
}