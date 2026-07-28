using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [Header("Movimiento")]
    [Min(0f)]
    [SerializeField] private float speed = 45f;

    [Min(0.1f)]
    [SerializeField] private float lifetime = 5f;

    [Header("Impacto")]
    [SerializeField] private GameObject impactVfxPrefab;

    [Min(0f)]
    [SerializeField] private float impactVfxLifetime = 3f;

    private Rigidbody projectileRigidbody;
    private Collider projectileCollider;

    private FlintlockWeapon ownerWeapon;
    private bool hasImpacted;

    private void Awake()
    {
        projectileRigidbody = GetComponent<Rigidbody>();
        projectileCollider = GetComponent<Collider>();

        projectileRigidbody.useGravity = false;

        projectileRigidbody.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Initialize(FlintlockWeapon weapon)
    {
        ownerWeapon = weapon;

        IgnoreOwnerColliders();

        projectileRigidbody.linearVelocity =
            transform.forward * speed;
    }

    private void IgnoreOwnerColliders()
    {
        if (ownerWeapon == null || projectileCollider == null)
        {
            return;
        }

        Collider[] ownerColliders =
            ownerWeapon.GetComponentsInParent<Collider>(true);

        foreach (Collider ownerCollider in ownerColliders)
        {
            if (ownerCollider == null)
            {
                continue;
            }

            Physics.IgnoreCollision(
                projectileCollider,
                ownerCollider,
                true
            );
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(
            $"PROYECTIL IMPACTÓ CONTRA: {collision.collider.name}",
            collision.collider
        );

        if (hasImpacted)
        {
            return;
        }

        hasImpacted = true;

        Vector3 impactPoint = transform.position;
        Vector3 impactNormal = -transform.forward;

        if (collision.contactCount > 0)
        {
            ContactPoint contact = collision.GetContact(0);

            impactPoint = contact.point;
            impactNormal = contact.normal;
        }

        TargetDummy dummy =
            collision.collider.GetComponentInParent<TargetDummy>();

        if (dummy == null)
        {
            Debug.LogWarning(
                $"Impactó contra '{collision.collider.name}', " +
                "pero no encontró TargetDummy en ese objeto ni en sus padres.",
                collision.collider
            );
        }
        else
        {
            Debug.Log(
                $"TARGET RECONOCIDO: {dummy.name}",
                dummy
            );

            dummy.ReceiveHit(
                impactPoint,
                transform.forward
            );

            if (ownerWeapon == null)
            {
                Debug.LogError(
                    "El Target fue reconocido, pero ownerWeapon es NULL. " +
                    "El proyectil no fue inicializado correctamente.",
                    gameObject
                );
            }
            else
            {
                Debug.Log(
                    "Enviando impacto al sistema de combo.",
                    ownerWeapon
                );

                ownerWeapon.RegisterSuccessfulHit(dummy);
            }
        }

        SpawnImpactVfx(
            impactPoint,
            impactNormal
        );

        projectileRigidbody.linearVelocity =
            Vector3.zero;

        projectileRigidbody.angularVelocity =
            Vector3.zero;

        projectileRigidbody.isKinematic = true;
        projectileCollider.enabled = false;

        Destroy(gameObject);
    }

    private void SpawnImpactVfx(
        Vector3 position,
        Vector3 normal
    )
    {
        if (impactVfxPrefab == null)
        {
            return;
        }

        Quaternion impactRotation =
            Quaternion.LookRotation(normal);

        GameObject impactVfx = Instantiate(
            impactVfxPrefab,
            position,
            impactRotation
        );

        if (impactVfxLifetime > 0f)
        {
            Destroy(
                impactVfx,
                impactVfxLifetime
            );
        }
    }
}