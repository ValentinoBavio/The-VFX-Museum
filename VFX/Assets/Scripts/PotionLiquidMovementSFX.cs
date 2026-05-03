using UnityEngine;
using UnityEngine.Audio;

public class PotionLiquidMovementSFX : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip liquidMoveSFX;
    public AudioMixerGroup sfxMixerGroup;

    [Header("Volumen")]
    [Range(0f, 1f)] public float volume = 0.45f;

    [Header("Pitch random")]
    public float pitchMin = 0.92f;
    public float pitchMax = 1.08f;

    [Header("Para que suene debe moverse bastante")]
    public float minDistanceMoved = 0.18f;
    public float minRotationMoved = 28f;
    public float checkInterval = 0.25f;
    public float cooldown = 1.1f;

    [Header("3D Audio")]
    public float minDistance = 1f;
    public float maxDistance = 8f;

    private AudioSource audioSource;

    private Vector3 lastCheckPosition;
    private Quaternion lastCheckRotation;

    private float checkTimer;
    private float nextAllowedTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.dopplerLevel = 0f;

        if (sfxMixerGroup != null)
            audioSource.outputAudioMixerGroup = sfxMixerGroup;

        lastCheckPosition = transform.position;
        lastCheckRotation = transform.rotation;
    }

    private void Update()
    {
        if (liquidMoveSFX == null) return;

        checkTimer += Time.deltaTime;

        if (checkTimer < checkInterval)
            return;

        checkTimer = 0f;

        float distanceMoved = Vector3.Distance(transform.position, lastCheckPosition);
        float rotationMoved = Quaternion.Angle(transform.rotation, lastCheckRotation);

        lastCheckPosition = transform.position;
        lastCheckRotation = transform.rotation;

        if (Time.time < nextAllowedTime)
            return;

        bool movedEnough = distanceMoved >= minDistanceMoved;
        bool rotatedEnough = rotationMoved >= minRotationMoved;

        if (movedEnough || rotatedEnough)
        {
            PlayLiquidSound(distanceMoved, rotationMoved);
            nextAllowedTime = Time.time + cooldown;
        }
    }

    private void PlayLiquidSound(float distanceMoved, float rotationMoved)
    {
        audioSource.pitch = Random.Range(pitchMin, pitchMax);

        float movementIntensity = Mathf.InverseLerp(minDistanceMoved, minDistanceMoved * 3f, distanceMoved);
        float rotationIntensity = Mathf.InverseLerp(minRotationMoved, minRotationMoved * 3f, rotationMoved);

        float intensity = Mathf.Max(movementIntensity, rotationIntensity);

        audioSource.volume = volume * Mathf.Lerp(0.55f, 1f, intensity);
        audioSource.PlayOneShot(liquidMoveSFX);
    }
}