using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class ProximityAudio : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform listenerTarget;
    [SerializeField] private bool useMainCameraIfEmpty = true;

    [Header("Distance")]
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 8f;

    [Header("Audio")]
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.6f;

    [Header("Pitch opcional")]
    [SerializeField] private bool useRandomPitch = false;
    [SerializeField] private float pitchMin = 1f;
    [SerializeField] private float pitchMax = 1f;

    [Header("Cooldown opcional")]
    [SerializeField] private bool useCooldown = false;
    [SerializeField] private float cooldown = 3f;

    private AudioSource _audioSource;
    private float _cooldownTimer;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (clip != null)
            _audioSource.clip = clip;

        if (sfxMixerGroup != null)
            _audioSource.outputAudioMixerGroup = sfxMixerGroup;

        _audioSource.playOnAwake = false;
        _audioSource.loop = !useCooldown;
        _audioSource.spatialBlend = 1f;
        _audioSource.minDistance = minDistance;
        _audioSource.maxDistance = maxDistance;
        _audioSource.dopplerLevel = 0f;
        _audioSource.volume = 0f;
        _audioSource.pitch = 1f;
    }

    private void Update()
    {
        FindListenerIfNeeded();

        if (listenerTarget == null)
            return;

        float distance = Vector3.Distance(transform.position, listenerTarget.position);
        bool isNear = distance <= maxDistance;

        float distanceVolume = GetDistanceVolume(distance);

        if (useCooldown)
            HandleCooldownAudio(isNear, distanceVolume);
        else
            HandleNormalLoop(isNear, distanceVolume);
    }

    private void HandleNormalLoop(bool isNear, float distanceVolume)
    {
        _audioSource.loop = true;

        if (isNear)
        {
            if (!_audioSource.isPlaying)
                PlaySound();

            _audioSource.volume = volume * distanceVolume;
        }
        else
        {
            if (_audioSource.isPlaying)
                _audioSource.Stop();

            _audioSource.volume = 0f;
        }
    }

    private void HandleCooldownAudio(bool isNear, float distanceVolume)
    {
        _audioSource.loop = false;

        if (!isNear)
        {
            if (_audioSource.isPlaying)
                _audioSource.Stop();

            _audioSource.volume = 0f;
            _cooldownTimer = 0f;
            return;
        }

        _audioSource.volume = volume * distanceVolume;

        if (_audioSource.isPlaying)
            return;

        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
            return;
        }

        PlaySound();

        float clipDuration = GetClipDurationWithPitch();
        _cooldownTimer = clipDuration + cooldown;
    }

    private void PlaySound()
    {
        if (_audioSource.clip == null)
            return;

        if (useRandomPitch)
            _audioSource.pitch = Random.Range(pitchMin, pitchMax);
        else
            _audioSource.pitch = 1f;

        _audioSource.Play();
    }

    private float GetClipDurationWithPitch()
    {
        if (_audioSource.clip == null)
            return 0f;

        float pitch = Mathf.Abs(_audioSource.pitch);

        if (pitch <= 0.01f)
            pitch = 1f;

        return _audioSource.clip.length / pitch;
    }

    private float GetDistanceVolume(float distance)
    {
        if (distance <= minDistance)
            return 1f;

        if (distance >= maxDistance)
            return 0f;

        return 1f - Mathf.InverseLerp(minDistance, maxDistance, distance);
    }

    private void FindListenerIfNeeded()
    {
        if (listenerTarget != null)
            return;

        if (!useMainCameraIfEmpty)
            return;

        Camera mainCamera = Camera.main;

        if (mainCamera != null)
            listenerTarget = mainCamera.transform;
    }
}