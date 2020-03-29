using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraShowRotate : MonoBehaviour
{
    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localEulerAngles += new Vector3(0, rotationSpeed*Time.deltaTime, 0); 
    }
}
