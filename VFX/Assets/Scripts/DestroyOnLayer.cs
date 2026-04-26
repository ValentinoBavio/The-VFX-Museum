using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnLayer : MonoBehaviour
{
    [Header("Layer que rompe")]
    public string targetLayerName = "RompePociones";
    private int targetLayer;

    [Header("Charco (Quad)")]
    public GameObject puddlePrefab;
    public Vector3 offset = new Vector3(0, 0.05f, 0); 

    [Header("Variacion")]
    public float minScale = 0.6f;
    public float maxScale = 1.4f;

    private void Start()
    {
        targetLayer = LayerMask.NameToLayer(targetLayerName);

        if (targetLayer == -1)
        {
            Debug.LogWarning($"La layer '{targetLayerName}' no existe.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == targetLayer)
        {
            
            Vector3 hitPoint = collision.contacts.Length > 0
                ? collision.contacts[0].point
                : transform.position;

            SpawnPuddle(hitPoint);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == targetLayer)
        {
            Destroy(gameObject);
            SpawnPuddle(transform.position);
           
        }
    }

    private void SpawnPuddle(Vector3 position)
    {
        if (puddlePrefab == null) return;

        
        Quaternion rot = Quaternion.Euler(90f, Random.Range(0f, 360f), 0f);

        GameObject puddle = Instantiate(puddlePrefab, position + offset, rot);

        
        float randomScale = Random.Range(minScale, maxScale);
        puddle.transform.localScale = Vector3.one * randomScale;
    }
}
