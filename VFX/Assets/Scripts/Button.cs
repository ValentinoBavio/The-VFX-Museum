using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] private Transform sphere;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float moveSpeed = 3f;

    private bool playerNear = false;
    private bool moving = false;
    private bool goingToA = false;

    private Transform currentTarget;


    private void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            
            ToggleTarget();
        }

        if (moving && currentTarget != null)
        {
            sphere.position = Vector3.MoveTowards(sphere.position,currentTarget.position,moveSpeed * Time.deltaTime);

            if (Vector3.Distance(sphere.position, currentTarget.position) < 0.01f)
            {
                sphere.position = currentTarget.position;
                moving = false;
            }
        }
    }

    private void ToggleTarget()
    {
        goingToA = !goingToA;

        currentTarget = goingToA ? pointA : pointB;
        moving = true;
    }

    private void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = false;
    }
}
