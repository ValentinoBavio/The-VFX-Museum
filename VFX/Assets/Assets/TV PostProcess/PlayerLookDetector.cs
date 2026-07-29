using UnityEngine;

public class PlayerLookDetector : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private LayerMask interactLayer;

    private LookTarget currentTarget;

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);


        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactLayer))
        {
            LookTarget target = hit.collider.GetComponent<LookTarget>();

            if (target != null)
            {
                if (currentTarget != target)
                {
                    if (currentTarget != null)
                        currentTarget.OnLookExit();

                    currentTarget = target;
                    currentTarget.OnLookEnter();
                }

                return;
            }
        }

        if (currentTarget != null)
        {
            currentTarget.OnLookExit();
            currentTarget = null;
        }
    }
}
