using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponComboSystem : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private FlintlockWeapon weapon;

    [Header("Combo")]
    [Min(1)]
    [SerializeField] private int hitsRequired = 3;

    [Min(0.1f)]
    [SerializeField] private float comboWindow = 3f;

    [SerializeField] private bool requireDifferentTargets = true;

    [Header("Modo potenciado")]
    [Min(0.1f)]
    [SerializeField] private float empoweredDuration = 10f;

    [Header("Eventos opcionales")]
    [SerializeField] private UnityEvent onEmpoweredStarted;
    [SerializeField] private UnityEvent onEmpoweredEnded;

    private readonly HashSet<TargetDummy> hitTargets =
        new HashSet<TargetDummy>();

    private int currentHits;
    private float comboExpirationTime;

    private Coroutine empoweredCoroutine;

    public int CurrentHits => currentHits;

    public float RemainingComboTime
    {
        get
        {
            if (currentHits == 0)
            {
                return 0f;
            }

            return Mathf.Max(
                0f,
                comboExpirationTime - Time.time
            );
        }
    }

    private void Awake()
    {
        if (weapon == null)
        {
            weapon = GetComponent<FlintlockWeapon>();
        }
    }

    private void Update()
    {
        if (currentHits == 0)
        {
            return;
        }

        if (Time.time > comboExpirationTime)
        {
            ResetCombo();
        }
    }

    public void RegisterHit(TargetDummy target)
    {
        if (weapon == null || weapon.IsEmpowered)
        {
            return;
        }

        if (currentHits > 0 &&
            Time.time > comboExpirationTime)
        {
            ResetCombo();
        }

        if (requireDifferentTargets)
        {
            if (target == null)
            {
                return;
            }

            bool isNewTarget = hitTargets.Add(target);

            if (!isNewTarget)
            {
                return;
            }
        }

        if (currentHits == 0)
        {
            comboExpirationTime =
                Time.time + comboWindow;
        }

        if (requireDifferentTargets)
        {
            currentHits = hitTargets.Count;
        }
        else
        {
            currentHits++;
            Debug.Log(
    $"Impacto válido: {currentHits}/{hitsRequired} - Target: {target.name}",
    target
);
        }

        if (currentHits >= hitsRequired)
        {
            CompleteCombo();
        }
    }

    private void CompleteCombo()
    {
        ResetCombo();

        if (empoweredCoroutine != null)
        {
            StopCoroutine(empoweredCoroutine);
        }

        empoweredCoroutine =
            StartCoroutine(EmpoweredRoutine());
    }

    private IEnumerator EmpoweredRoutine()
    {
        weapon.SetEmpowered(true);
        onEmpoweredStarted?.Invoke();

        yield return new WaitForSeconds(empoweredDuration);

        weapon.SetEmpowered(false);
        onEmpoweredEnded?.Invoke();

        empoweredCoroutine = null;
    }

    private void ResetCombo()
    {
        currentHits = 0;
        comboExpirationTime = 0f;
        hitTargets.Clear();
    }

    private void OnDisable()
    {
        ResetCombo();

        if (weapon != null)
        {
            weapon.SetEmpowered(false);
        }
    }
}