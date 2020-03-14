using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HairData
{
    public PrimitiveType meshType = PrimitiveType.Cube;

    public string id;

    public int parentIndex;
    public Vector3 localposition;

    public Quaternion rotation;

    public Vector3 scale;

    public Color color;
}
