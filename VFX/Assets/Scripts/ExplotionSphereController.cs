using UnityEngine;

public class ExplosionSphereController : MonoBehaviour
{
    [Header("Camera Shake")]
    [SerializeField] private float shakeDuration = 0.18f;
    [SerializeField] private float shakeStrength = 0.08f;

    [Tooltip("Distancia máxima desde la que se siente la explosión.")]
    [SerializeField] private float maxShakeDistance = 20f;

    private Renderer sphereRenderer;
    private WeaponCameraShake weaponCameraShake;

    private void Awake()
    {
        sphereRenderer = GetComponent<Renderer>();

        weaponCameraShake =
            FindFirstObjectByType<WeaponCameraShake>();
    }

    private void Start()
    {
        ExplosionShake();
    }

    private void ExplosionShake()
    {
        if (weaponCameraShake == null)
        {
            Debug.LogWarning(
                "La explosión no encontró WeaponCameraShake en la escena.",
                this
            );

            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            weaponCameraShake.transform.position
        );

        if (distance > maxShakeDistance)
        {
            Debug.Log(
                $"Explosión demasiado lejos para shake. Distancia: {distance}",
                this
            );

            return;
        }

        float distanceMultiplier =
            1f - Mathf.Clamp01(distance / maxShakeDistance);

        float finalStrength =
            shakeStrength * distanceMultiplier;

        Debug.Log(
            $"EXPLOSION SHAKE | Distancia: {distance:F2} | Fuerza: {finalStrength:F3}",
            this
        );

        weaponCameraShake.Shake(
            shakeDuration,
            finalStrength
        );
    }

    // Este SÍ se llama mediante Animation Event
    // al final de la animación de la esfera.
    public void HideSphere()
    {
        if (sphereRenderer != null)
        {
            sphereRenderer.enabled = false;
        }
    }
}