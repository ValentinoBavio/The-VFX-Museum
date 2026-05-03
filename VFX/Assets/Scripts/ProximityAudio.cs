using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class ProximityAudio : MonoBehaviour
{
    [Header("Referencia")]
    public Transform listener; // Arrastrá acá el Player o el objeto que tenga el AudioListener

    [Header("Distancias")]
    public float minDistance = 2f;   // Dentro de esta distancia suena al máximo
    public float maxDistance = 15f;  // Fuera de esta distancia no suena

    [Header("Volumen")]
    [Range(0f, 1f)] public float maxVolume = 0.3f;
    public float fadeSpeed = 6f;

    [Header("Audio")]
    public bool playOnStart = true;
    public bool stopWhenFar = true;
    public AudioMixerGroup outputMixerGroup;

    private AudioSource audioSource;
    private float targetVolume;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;

        // Si querés que respete tu AudioMixer
        if (outputMixerGroup != null)
            audioSource.outputAudioMixerGroup = outputMixerGroup;

        // Busca automáticamente el AudioListener si no lo asignaste
        if (listener == null)
        {
            AudioListener foundListener = FindObjectOfType<AudioListener>();

            if (foundListener != null)
                listener = foundListener.transform;
        }
    }

    private void Start()
    {
        if (playOnStart && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    private void Update()
    {
        if (listener == null || audioSource.clip == null)
            return;

        float distance = Vector3.Distance(transform.position, listener.position);

        if (distance <= minDistance)
        {
            targetVolume = maxVolume;

            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else if (distance >= maxDistance)
        {
            targetVolume = 0f;

            if (stopWhenFar && audioSource.volume <= 0.01f && audioSource.isPlaying)
                audioSource.Stop();
        }
        else
        {
            float t = Mathf.InverseLerp(maxDistance, minDistance, distance);
            targetVolume = maxVolume * t;

            if (!audioSource.isPlaying)
                audioSource.Play();
        }

        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, Time.deltaTime * fadeSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}