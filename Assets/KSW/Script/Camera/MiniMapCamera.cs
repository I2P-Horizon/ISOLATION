using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    private void LateUpdate()
    {
        if (!Player.Instance) return;
        transform.position = new Vector3(Player.Instance.transform.position.x, 30, Player.Instance.transform.position.z);
    }
}