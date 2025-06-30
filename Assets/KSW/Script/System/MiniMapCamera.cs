using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    [SerializeField] private Transform player;

    private float height = 50f;

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPos = player.position;
            newPos.y = height;
            transform.position = newPos;
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}