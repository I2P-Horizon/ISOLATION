using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    [SerializeField] private Transform player;

    void LateUpdate()
    {
        if (!player) return;
        transform.position = new Vector3(player.position.x, 20, player.position.z);
    }
}