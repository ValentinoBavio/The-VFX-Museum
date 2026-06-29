using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class BeamImpactButton : MonoBehaviour
{
    [Header("Beam")]
    [SerializeField] private GameObject beamObject;
    [SerializeField] private VisualEffect beamVFX;
    [SerializeField] private float beamDuration = 3f;

    [Header("Sonido del Beam")]
    [SerializeField] private AudioSource beamAudioSource;
    [SerializeField] private AudioClip beamSound;
    [SerializeField, Range(0f, 1f)] private float beamVolume = 1f;

    [Header("Sonido permanente del Ground Crack")]
    [SerializeField] private AudioSource crackLoopAudioSource;
    [SerializeField] private AudioClip crackLoopSound;
    [SerializeField, Range(0f, 1f)] private float crackLoopVolume = 0.5f;

    [Tooltip("Distancia dentro de la cual se escucha al volumen máximo.")]
    [SerializeField] private float crackMinDistance = 2f;

    [Tooltip("Distancia a partir de la cual deja de escucharse.")]
    [SerializeField] private float crackMaxDistance = 12f;

    [Header("Crack permanente")]
    [SerializeField] private GameObject groundCrack;

    [Header("Objetos que desaparecen")]
    [SerializeField] private GameObject[] objectsToHide;

    [Header("Camera Shake")]
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private float shakeStrength = 0.035f;
    [SerializeField] private float shakeFrequency = 25f;

    private bool playerInside;
    private bool activated;

    private void Awake()
    {
        if (beamObject != null)
            beamObject.SetActive(false);

        if (groundCrack != null)
            groundCrack.SetActive(false);

        if (beamAudioSource != null)
        {
            beamAudioSource.playOnAwake = false;
            beamAudioSource.loop = false;
        }

        if (crackLoopAudioSource != null)
        {
            crackLoopAudioSource.playOnAwake = false;
            crackLoopAudioSource.loop = true;

            // Sonido completamente 3D.
            crackLoopAudioSource.spatialBlend = 1f;

            crackLoopAudioSource.rolloffMode =
                AudioRolloffMode.Logarithmic;

            crackLoopAudioSource.minDistance =
                crackMinDistance;

            crackLoopAudioSource.maxDistance =
                crackMaxDistance;

            crackLoopAudioSource.Stop();
        }
    }

    private void Update()
    {
        if (!playerInside || activated)
            return;

        if (Input.GetKeyDown(KeyCode.E))
            ActivateButton();
    }

    private void ActivateButton()
    {
        activated = true;

        // Desactiva los objetos de la escena.
        foreach (GameObject objectToHide in objectsToHide)
        {
            if (objectToHide != null)
                objectToHide.SetActive(false);
        }

        // Activa permanentemente el crack.
        if (groundCrack != null)
            groundCrack.SetActive(true);

        // Activa el objeto del beam.
        if (beamObject != null)
            beamObject.SetActive(true);

        // Reinicia y reproduce el VFX.
        if (beamVFX != null)
        {
            beamVFX.Reinit();
            beamVFX.Play();
        }

        // Reproduce una sola vez el sonido del beam.
        if (beamAudioSource != null && beamSound != null)
        {
            beamAudioSource.PlayOneShot(
                beamSound,
                beamVolume
            );
        }

        // Temblor durante el beam.
        if (cameraShake != null)
        {
            cameraShake.Shake(
                beamDuration,
                shakeStrength,
                shakeFrequency
            );
        }

        StartCoroutine(DisableBeamAfterDuration());
    }

    private IEnumerator DisableBeamAfterDuration()
    {
        yield return new WaitForSeconds(beamDuration);

        // Finaliza el beam.
        if (beamVFX != null)
            beamVFX.Stop();

        if (beamObject != null)
            beamObject.SetActive(false);

        // Comienza el sonido permanente del crack.
        if (crackLoopAudioSource != null &&
            crackLoopSound != null)
        {
            crackLoopAudioSource.clip = crackLoopSound;
            crackLoopAudioSource.volume = crackLoopVolume;
            crackLoopAudioSource.loop = true;
            crackLoopAudioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}