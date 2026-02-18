using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog 
{
    public Sprite sprite;
    [TextArea(1, 2)] public string text;
    public bool showImage = true;
    public float typingSpeed = 0.05f;
}