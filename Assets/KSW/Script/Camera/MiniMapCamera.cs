using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    [SerializeField] private Transform player;

    private void LateUpdate()
    {
        if (!player) return;
        transform.position = new Vector3(player.position.x, 30, player.position.z);
    }
}