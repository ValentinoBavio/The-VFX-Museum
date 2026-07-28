using System.Collections;
using UnityEngine;

public class FlintlockWeapon : MonoBehaviour
{
    [Header("Arma FPS")]
    [SerializeField] private GameObject weaponRoot;
    [SerializeField] private Transform recoilTransform;
    [SerializeField] private Transform muzzlePoint;

    [Header("Cámara y apuntado")]
    [SerializeField] private Camera fpsCamera;
    [SerializeField] private LayerMask aimMask = ~0;
    [SerializeField] private float maximumAimDistance = 200f;

    [Header("Proyectiles")]
    [SerializeField] private GameObject normalProjectilePrefab;
    [SerializeField] private GameObject empoweredProjectilePrefab;

    [Header("Fogonazos")]
    [SerializeField] private GameObject normalMuzzleFlashPrefab;
    [SerializeField] private GameObject empoweredMuzzleFlashPrefab;
    [SerializeField] private float muzzleFlashLifetime = 1f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shotClip;

    [Range(0f, 1f)]
    [SerializeField] private float shotVolume = 1f;

    [Header("Recoil")]
    [SerializeField] private Vector3 recoilPosition =
        new Vector3(0f, 0.015f, -0.12f);

    [SerializeField] private Vector3 recoilEuler =
        new Vector3(-8f, 0f, 0f);

    [Min(0f)]
    [SerializeField] private float recoilKickDuration = 0.07f;

    [Min(0f)]
    [SerializeField] private float recoilReturnDuration = 0.22f;

    [Header("Equipamiento")]
    [SerializeField] private Vector3 equipStartOffset =
        new Vector3(0f, -0.35f, 0.1f);

    [SerializeField] private Vector3 equipStartEuler =
        new Vector3(25f, 0f, 8f);

    [Min(0f)]
    [SerializeField] private float equipDuration = 0.35f;

    [Header("Camerashake")]
    [SerializeField] private WeaponCameraShake weaponCameraShake;

    [Min(0f)]
    [SerializeField] private float shakeDuration = 0.07f;

    [Min(0f)]
    [SerializeField] private float shakeMagnitude = 0.015f;

    [Header("Sistemas")]
    [SerializeField] private WeaponComboSystem comboSystem;
    [SerializeField] private WeaponShaderController shaderController;

    private Vector3 restingLocalPosition;
    private Quaternion restingLocalRotation;

    private bool isEquipped;
    private bool isEmpowered;
    private bool isAnimating;

    private Coroutine weaponAnimationCoroutine;

    public bool IsEquipped => isEquipped;
    public bool IsEmpowered => isEmpowered;

    private void Awake()
    {
        if (fpsCamera == null)
        {
            fpsCamera = Camera.main;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (comboSystem == null)
        {
            comboSystem = GetComponent<WeaponComboSystem>();
        }

        if (shaderController == null)
        {
            shaderController =
                GetComponent<WeaponShaderController>();
        }

        if (recoilTransform == null &&
            weaponRoot != null)
        {
            recoilTransform = weaponRoot.transform;
        }

        if (recoilTransform != null)
        {
            restingLocalPosition =
                recoilTransform.localPosition;

            restingLocalRotation =
                recoilTransform.localRotation;
        }

        if (weaponRoot != null)
        {
            weaponRoot.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isEquipped || isAnimating)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    public bool Equip()
    {
        if (isEquipped)
        {
            return false;
        }

        if (weaponRoot == null ||
            recoilTransform == null ||
            muzzlePoint == null)
        {
            Debug.LogError(
                "El Flintlock no tiene todas sus referencias asignadas.",
                this
            );

            return false;
        }

        isEquipped = true;
        weaponRoot.SetActive(true);

        if (weaponAnimationCoroutine != null)
        {
            StopCoroutine(weaponAnimationCoroutine);
        }

        weaponAnimationCoroutine =
            StartCoroutine(EquipRoutine());

        return true;
    }

    private void Fire()
    {
        GameObject selectedProjectile =
            isEmpowered
                ? empoweredProjectilePrefab
                : normalProjectilePrefab;

        if (selectedProjectile == null)
        {
            Debug.LogWarning(
                "No hay un prefab de proyectil asignado.",
                this
            );

            return;
        }

        isAnimating = true;

        Vector3 shotDirection =
            CalculateShotDirection();

        Quaternion projectileRotation =
            Quaternion.LookRotation(shotDirection);

        GameObject projectileObject = Instantiate(
            selectedProjectile,
            muzzlePoint.position,
            projectileRotation
        );

        Projectile projectile =
            projectileObject.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.Initialize(this);
        }
        else
        {
            Debug.LogWarning(
                "El prefab instanciado no tiene Projectile.",
                projectileObject
            );
        }

        SpawnMuzzleFlash();

        if (audioSource != null && shotClip != null)
        {
            audioSource.PlayOneShot(
                shotClip,
                shotVolume
            );
        }

        if (weaponCameraShake != null)
        {
            weaponCameraShake.Shake(
                shakeDuration,
                shakeMagnitude
            );
        }

        weaponAnimationCoroutine =
            StartCoroutine(RecoilRoutine());
    }

    private Vector3 CalculateShotDirection()
    {
        if (fpsCamera == null)
        {
            return muzzlePoint.forward;
        }

        Ray cameraRay = new Ray(
            fpsCamera.transform.position,
            fpsCamera.transform.forward
        );

        if (Physics.Raycast(
                cameraRay,
                out RaycastHit hit,
                maximumAimDistance,
                aimMask,
                QueryTriggerInteraction.Ignore
            ))
        {
            Vector3 directionToHit =
                hit.point - muzzlePoint.position;

            if (directionToHit.sqrMagnitude > 0.001f)
            {
                return directionToHit.normalized;
            }
        }

        Vector3 distantPoint =
            cameraRay.origin
            + cameraRay.direction
            * maximumAimDistance;

        return (
            distantPoint - muzzlePoint.position
        ).normalized;
    }

    private void SpawnMuzzleFlash()
    {
        GameObject selectedMuzzleFlash =
            isEmpowered
                ? empoweredMuzzleFlashPrefab
                : normalMuzzleFlashPrefab;

        if (selectedMuzzleFlash == null)
        {
            return;
        }

        GameObject muzzleFlash = Instantiate(
            selectedMuzzleFlash,
            muzzlePoint.position,
            muzzlePoint.rotation,
            muzzlePoint
        );

        if (muzzleFlashLifetime > 0f)
        {
            Destroy(
                muzzleFlash,
                muzzleFlashLifetime
            );
        }
    }

    private IEnumerator EquipRoutine()
    {
        isAnimating = true;

        recoilTransform.localPosition =
            restingLocalPosition + equipStartOffset;

        recoilTransform.localRotation =
            restingLocalRotation
            * Quaternion.Euler(equipStartEuler);

        yield return AnimateWeaponPose(
            restingLocalPosition,
            restingLocalRotation,
            equipDuration
        );

        isAnimating = false;
        weaponAnimationCoroutine = null;
    }

    private IEnumerator RecoilRoutine()
    {
        Vector3 kickPosition =
            restingLocalPosition + recoilPosition;

        Quaternion kickRotation =
            restingLocalRotation
            * Quaternion.Euler(recoilEuler);

        yield return AnimateWeaponPose(
            kickPosition,
            kickRotation,
            recoilKickDuration
        );

        yield return AnimateWeaponPose(
            restingLocalPosition,
            restingLocalRotation,
            recoilReturnDuration
        );

        isAnimating = false;
        weaponAnimationCoroutine = null;
    }

    private IEnumerator AnimateWeaponPose(
        Vector3 targetPosition,
        Quaternion targetRotation,
        float duration
    )
    {
        Vector3 startPosition =
            recoilTransform.localPosition;

        Quaternion startRotation =
            recoilTransform.localRotation;

        if (duration <= 0f)
        {
            recoilTransform.localPosition =
                targetPosition;

            recoilTransform.localRotation =
                targetRotation;

            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float normalizedTime =
                Mathf.Clamp01(elapsedTime / duration);

            normalizedTime =
                normalizedTime *
                normalizedTime *
                (3f - 2f * normalizedTime);

            recoilTransform.localPosition =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    normalizedTime
                );

            recoilTransform.localRotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    normalizedTime
                );

            yield return null;
        }

        recoilTransform.localPosition =
            targetPosition;

        recoilTransform.localRotation =
            targetRotation;
    }

    public void RegisterSuccessfulHit(
        TargetDummy target
    )
    {
        if (comboSystem != null)
        {
            comboSystem.RegisterHit(target);
        }
    }

    public void SetEmpowered(bool empowered)
    {
        isEmpowered = empowered;

        if (shaderController != null)
        {
            shaderController.SetEmpowered(empowered);
        }
    }
}