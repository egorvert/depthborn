using System;
using UnityEngine;

public class PlatformRenderer : MonoBehaviour {
    Color[] colors = { Color.red, Color.blue, Color.yellow, Color.green };
    System.Random rnd = new System.Random();


    void Start() {
        Renderer platformRenderer = GetComponent<Renderer>();

        int cR = rnd.Next(0, colors.Length);
        platformRenderer.material.SetColor("_Color", colors[cR]);
    }
    
}
