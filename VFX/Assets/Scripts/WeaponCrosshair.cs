using UnityEngine;
using UnityEngine.UI;

public class WeaponCrosshair : MonoBehaviour
{
    [Header("Crosshair estilo CS 1.6")]
    [SerializeField] private Color color = Color.green;

    [SerializeField] private float thickness = 2f;
    [SerializeField] private float length = 9f;
    [SerializeField] private float gap = 5f;

    private GameObject canvasObject;
    private GameObject crosshairRoot;

    private void Awake()
    {
        CreateCrosshair();
        Hide();
    }

    private void CreateCrosshair()
    {
        // =========================
        // CANVAS
        // =========================

        canvasObject = new GameObject(
            "WeaponCrosshairCanvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler)
        );

        Canvas canvas = canvasObject.GetComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler =
            canvasObject.GetComponent<CanvasScaler>();

        scaler.uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;

        scaler.referenceResolution =
            new Vector2(1920f, 1080f);

        scaler.matchWidthOrHeight = 0.5f;

        // =========================
        // CROSSHAIR ROOT
        // =========================

        crosshairRoot = new GameObject(
            "Crosshair",
            typeof(RectTransform)
        );

        crosshairRoot.transform.SetParent(
            canvasObject.transform,
            false
        );

        RectTransform rootRect =
            crosshairRoot.GetComponent<RectTransform>();

        rootRect.anchorMin = new Vector2(0.5f, 0.5f);
        rootRect.anchorMax = new Vector2(0.5f, 0.5f);

        rootRect.pivot = new Vector2(0.5f, 0.5f);
        rootRect.anchoredPosition = Vector2.zero;

        // =========================
        // 4 LÍNEAS
        // =========================

        CreateLine(
            "Top",
            new Vector2(thickness, length),
            new Vector2(
                0f,
                gap + length * 0.5f
            )
        );

        CreateLine(
            "Bottom",
            new Vector2(thickness, length),
            new Vector2(
                0f,
                -(gap + length * 0.5f)
            )
        );

        CreateLine(
            "Left",
            new Vector2(length, thickness),
            new Vector2(
                -(gap + length * 0.5f),
                0f
            )
        );

        CreateLine(
            "Right",
            new Vector2(length, thickness),
            new Vector2(
                gap + length * 0.5f,
                0f
            )
        );
    }

    private void CreateLine(
        string lineName,
        Vector2 size,
        Vector2 position
    )
    {
        GameObject line = new GameObject(
            lineName,
            typeof(RectTransform),
            typeof(Image)
        );

        line.transform.SetParent(
            crosshairRoot.transform,
            false
        );

        RectTransform rect =
            line.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);

        rect.pivot = new Vector2(0.5f, 0.5f);

        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        Image image = line.GetComponent<Image>();

        image.color = color;
        image.raycastTarget = false;
    }

    public void Show()
    {
        if (crosshairRoot != null)
        {
            crosshairRoot.SetActive(true);
        }
    }

    public void Hide()
    {
        if (crosshairRoot != null)
        {
            crosshairRoot.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (canvasObject != null)
        {
            Destroy(canvasObject);
        }
    }
}