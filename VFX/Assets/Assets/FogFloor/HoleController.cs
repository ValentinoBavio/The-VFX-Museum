using UnityEngine;

public class HoleController : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private Transform player;

    void Update()
    {
        material.SetVector("_PlayerPos", player.position);
    }
}
