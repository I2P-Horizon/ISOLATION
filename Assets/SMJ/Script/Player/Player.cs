using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance {  get; private set; }

    public PlayerState State {  get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerInteraction Interaction { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        State = GetComponent<PlayerState>();
        Movement = GetComponent<PlayerMovement>();
        Interaction = GetComponent<PlayerInteraction>();
    }
}
