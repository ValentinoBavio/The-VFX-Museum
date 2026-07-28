using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class WeaponPickup : MonoBehaviour
{
    [Header("Objetos")]
    [SerializeField] private Transform visual;

    [Tooltip("Objeto completo que debe desaparecer al recoger el arma.")]
    [SerializeField] private GameObject pickupObjectToRemove;

    [Header("Flotación")]
    [SerializeField] private float bobHeight = 0.15f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float rotationSpeed = 45f;

    [Header("Sonido")]
    [SerializeField] private AudioClip pickupClip;

    [Range(0f, 1f)]
    [SerializeField] private float pickupVolume = 1f;

    private Vector3 initialVisualPosition;
    private bool wasPickedUp;

    private void Awake()
    {
        if (visual == null)
        {
            visual = transform;
        }

        if (pickupObjectToRemove == null)
        {
            pickupObjectToRemove = gameObject;
        }

        initialVisualPosition = visual.localPosition;

        Collider pickupCollider = GetComponent<Collider>();
        pickupCollider.isTrigger = true;

        Rigidbody pickupRigidbody = GetComponent<Rigidbody>();
        pickupRigidbody.isKinematic = true;
        pickupRigidbody.useGravity = false;
    }

    private void Update()
    {
        if (wasPickedUp)
        {
            return;
        }

        AnimatePickup();
    }

    private void AnimatePickup()
    {
        float verticalOffset =
            Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        visual.localPosition =
            initialVisualPosition +
            Vector3.up * verticalOffset;

        visual.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime,
            Space.World
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (wasPickedUp)
        {
            return;
        }

        FlintlockWeapon weapon =
            other.GetComponentInParent<FlintlockWeapon>();

        if (weapon == null)
        {
            return;
        }

        bool equippedSuccessfully = weapon.Equip();

        if (!equippedSuccessfully)
        {
            return;
        }

        wasPickedUp = true;

        Vector3 pickupPosition = transform.position;

        if (pickupClip != null)
        {
            AudioSource.PlayClipAtPoint(
                pickupClip,
                pickupPosition,
                pickupVolume
            );
        }

        // Desaparece inmediatamente.
        if (visual != null)
        {
            visual.gameObject.SetActive(false);
        }

        Collider pickupCollider =
            GetComponent<Collider>();

        if (pickupCollider != null)
        {
            pickupCollider.enabled = false;
        }

        GameObject objectToRemove =
            pickupObjectToRemove != null
                ? pickupObjectToRemove
                : gameObject;

        objectToRemove.SetActive(false);
        Destroy(objectToRemove);
    }
}