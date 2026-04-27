using System.Collections;
using TMPro;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    [Header("Input")]
    public KeyCode startKey = KeyCode.E;

    [Header("Modelo del título")]
    public GameObject titleModel;
    public bool ocultarTituloAlEmpezar = true;

    [Header("Texto UI")]
    public GameObject pressEPanel;
    public TMP_Text pressEText;
    public string textoInicio = "Press E to Start";

    [Header("Jugador")]
    public Rigidbody playerRb;

    [Tooltip("Scripts del jugador que se desactivan hasta apretar E. Ej: movimiento, cámara, wallrun, grapple, etc.")]
    public MonoBehaviour[] scriptsDelJugador;

    [Header("Animación opcional")]
    public bool animarAparicionTitulo = true;
    public float duracionAparicion = 0.35f;
    public float duracionSalida = 0.25f;

    private RigidbodyConstraints rbConstraintsOriginales;
    private bool[] estadosOriginalesScripts;
    private Vector3 escalaOriginalTitulo;

    private bool yaEmpezo;

    private void Awake()
    {
        if (playerRb != null)
            rbConstraintsOriginales = playerRb.constraints;

        if (titleModel != null)
            escalaOriginalTitulo = titleModel.transform.localScale;
    }

    private void Start()
    {
        BloquearJugador();
        MostrarPantallaInicio();

        if (animarAparicionTitulo && titleModel != null)
            StartCoroutine(AparecerTitulo());
    }

    private void Update()
    {
        if (yaEmpezo) return;

        if (Input.GetKeyDown(startKey))
        {
            yaEmpezo = true;
            StartCoroutine(EmpezarJuego());
        }
    }

    private void MostrarPantallaInicio()
    {
        if (titleModel != null)
            titleModel.SetActive(true);

        if (pressEPanel != null)
            pressEPanel.SetActive(true);

        if (pressEText != null)
        {
            pressEText.gameObject.SetActive(true);
            pressEText.text = textoInicio;
        }
    }

    private void BloquearJugador()
    {
        if (playerRb != null)
        {
            playerRb.velocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.constraints = RigidbodyConstraints.FreezeAll;
        }

        estadosOriginalesScripts = new bool[scriptsDelJugador.Length];

        for (int i = 0; i < scriptsDelJugador.Length; i++)
        {
            if (scriptsDelJugador[i] == null) continue;

            estadosOriginalesScripts[i] = scriptsDelJugador[i].enabled;
            scriptsDelJugador[i].enabled = false;
        }
    }

    private void DesbloquearJugador()
    {
        if (playerRb != null)
            playerRb.constraints = rbConstraintsOriginales;

        for (int i = 0; i < scriptsDelJugador.Length; i++)
        {
            if (scriptsDelJugador[i] == null) continue;

            scriptsDelJugador[i].enabled = estadosOriginalesScripts[i];
        }
    }

    private IEnumerator AparecerTitulo()
    {
        Vector3 escalaFinal = escalaOriginalTitulo;
        titleModel.transform.localScale = Vector3.zero;

        float t = 0f;

        while (t < duracionAparicion)
        {
            t += Time.deltaTime;

            float p = t / duracionAparicion;
            p = Mathf.SmoothStep(0f, 1f, p);

            titleModel.transform.localScale = Vector3.Lerp(Vector3.zero, escalaFinal, p);

            yield return null;
        }

        titleModel.transform.localScale = escalaFinal;
    }

    private IEnumerator EmpezarJuego()
    {
        if (pressEPanel != null)
            pressEPanel.SetActive(false);

        if (pressEText != null)
            pressEText.gameObject.SetActive(false);

        if (ocultarTituloAlEmpezar && titleModel != null)
        {
            Vector3 escalaInicial = titleModel.transform.localScale;

            float t = 0f;

            while (t < duracionSalida)
            {
                t += Time.deltaTime;

                float p = t / duracionSalida;
                p = Mathf.SmoothStep(0f, 1f, p);

                titleModel.transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, p);

                yield return null;
            }

            titleModel.SetActive(false);
            titleModel.transform.localScale = escalaOriginalTitulo;
        }

        DesbloquearJugador();
    }
}