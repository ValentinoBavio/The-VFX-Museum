using System;
using UnityEngine;
using UnityEngine.VFX;

public class WeaponShaderController : MonoBehaviour
{
    [Serializable]
    private class MaterialSwap
    {
        [Tooltip("Renderer al que se le cambiará el material.")]
        public Renderer targetRenderer;

        [Tooltip("Posición del material dentro del Renderer. Generalmente 0.")]
        [Min(0)]
        public int materialIndex;

        [Tooltip("Material que utilizará durante el modo Empowered.")]
        public Material empoweredMaterial;

        [NonSerialized]
        public Material originalMaterial;
    }

    [Header("Cambio de materiales")]
    [SerializeField] private MaterialSwap[] materialSwaps;

    [Header("VFX del modo Empowered")]
    [Tooltip("VFX que estarán activos durante el modo Empowered.")]
    [SerializeField] private VisualEffect[] empoweredVfx;

    private bool originalsCached;
    private bool isEmpowered;

    private void Awake()
    {
        CacheOriginalMaterials();

        // El arma comienza con sus materiales normales.
        ApplyMaterials(false);

        // El VFX comienza apagado.
        SetVfxActive(false);
    }

    private void CacheOriginalMaterials()
    {
        if (originalsCached)
        {
            return;
        }

        foreach (MaterialSwap swap in materialSwaps)
        {
            if (swap == null || swap.targetRenderer == null)
            {
                continue;
            }

            Material[] materials =
                swap.targetRenderer.sharedMaterials;

            if (swap.materialIndex < 0 ||
                swap.materialIndex >= materials.Length)
            {
                Debug.LogWarning(
                    $"El Renderer '{swap.targetRenderer.name}' " +
                    $"no tiene un material en el índice {swap.materialIndex}.",
                    swap.targetRenderer
                );

                continue;
            }

            swap.originalMaterial =
                materials[swap.materialIndex];
        }

        originalsCached = true;
    }

    public void SetEmpowered(bool empowered)
    {
        if (isEmpowered == empowered)
        {
            return;
        }

        isEmpowered = empowered;

        CacheOriginalMaterials();
        ApplyMaterials(empowered);
        SetVfxActive(empowered);
    }

    private void ApplyMaterials(bool empowered)
    {
        foreach (MaterialSwap swap in materialSwaps)
        {
            if (swap == null || swap.targetRenderer == null)
            {
                continue;
            }

            Material[] materials =
                swap.targetRenderer.sharedMaterials;

            if (swap.materialIndex < 0 ||
                swap.materialIndex >= materials.Length)
            {
                continue;
            }

            Material selectedMaterial = empowered
                ? swap.empoweredMaterial
                : swap.originalMaterial;

            if (selectedMaterial == null)
            {
                continue;
            }

            materials[swap.materialIndex] =
                selectedMaterial;

            swap.targetRenderer.sharedMaterials =
                materials;
        }
    }

    private void SetVfxActive(bool active)
    {
        if (empoweredVfx == null)
        {
            return;
        }

        foreach (VisualEffect vfx in empoweredVfx)
        {
            if (vfx == null)
            {
                continue;
            }

            if (active)
            {
                // Activa el componente, limpia el estado anterior
                // y reproduce el efecto desde el principio.
                vfx.enabled = true;
                vfx.Reinit();
                vfx.Play();
            }
            else
            {
                // Detiene y limpia las partículas existentes.
                vfx.Stop();
                vfx.Reinit();
                vfx.enabled = false;
            }
        }
    }

    private void OnDisable()
    {
        if (!originalsCached)
        {
            return;
        }

        isEmpowered = false;
        ApplyMaterials(false);
        SetVfxActive(false);
    }
}