using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourBeta : MonoBehaviour
{
    public Material colourBetaMat;

    public void Yellow()
    {
        colourBetaMat.color = Color.yellow;
    }

    public void Blue()
    {
        colourBetaMat.color = Color.blue;
    }

    public void Red()
    {
        colourBetaMat.color = Color.red;
    }

    public void Magenta()
    {
        colourBetaMat.color = Color.magenta;
    }
}
