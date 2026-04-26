using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public Transform holdPoint; // lugar donde se sostiene el objeto
    public float pickupRadius = 2f;

    private GameObject currentObject;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentObject == null)
            {
                TryPickup();
            }
            else
            {
                Drop();
            }
        }
    }

    void TryPickup()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Grabbable"))
            {
                currentObject = hit.gameObject;

                // Desactivar física
                Rigidbody rb = currentObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }

                // Parentar al player
                currentObject.transform.SetParent(holdPoint);
                currentObject.transform.localPosition = Vector3.zero;
                currentObject.transform.localRotation = Quaternion.identity;

                break;
            }
        }
    }

    void Drop()
    {
        Rigidbody rb = currentObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        currentObject.transform.SetParent(null);
        currentObject = null;
    }
}
