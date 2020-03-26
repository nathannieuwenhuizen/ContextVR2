using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PropType {
    none,
    book,
    sandwitch,
    doughnut,
    ATM
}

[System.Serializable]
public class HairData
{
    public PrimitiveType meshType = PrimitiveType.Capsule;
    public PropType propType = PropType.none;

    public string id;
    public int parentIndex;

    public Vector3 localposition;
    public Quaternion rotation;
    public Vector3 scale;

    public Color color;
}
