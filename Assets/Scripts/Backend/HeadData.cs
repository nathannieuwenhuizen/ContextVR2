using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeadData
{

    private static HeadData _current;
    public static HeadData current
    {
        get
        {
            if (_current == null)
            {
                _current = new HeadData();
            }
            return _current;
        }
        set
        {
            _current = value;
        }
    }

    public int test = 5;
    public List<HairData> hairObjects;
}
