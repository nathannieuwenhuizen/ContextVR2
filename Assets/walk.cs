using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walk : MonoBehaviour {

    public float speed;

    void Update() {
        transform.position += new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical")) * speed;
    }
}
