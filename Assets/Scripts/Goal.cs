using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public bool achieved = false;
    public Renderer rend;
    public Color baseColor;
    public Color endColor;

    void Start()
    {
        if (rend == null)
            rend = GetComponent<Renderer>();
        rend.material.color = baseColor;
    }

    protected void Activate()
    {
        achieved = true;
        rend.material.color = endColor;
    }

    protected void Desactivate()
    {
        achieved = false;
        rend.material.color = baseColor;
    }
}
