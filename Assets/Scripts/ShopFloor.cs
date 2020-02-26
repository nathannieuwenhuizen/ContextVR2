using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopFloor : MonoBehaviour {
    
    [SerializeField] float scaleSpeed = .01f;
    [SerializeField] float deleteThershold = .01f;
    List<Transform> scaleObjects;

    void Start() {
        scaleObjects = new List<Transform>();
    }

    void Update() {
        for (int i = 0; i < scaleObjects.Count; i++) {
            scaleObjects[i].localScale -= new Vector3(scaleSpeed,scaleSpeed,scaleSpeed);
            if (scaleObjects[i].localScale.x < deleteThershold) {
                GameObject.Destroy(scaleObjects[i].gameObject);
                scaleObjects.Remove(scaleObjects[i]);
            }
        }
    }

    void OnCollisionEnter(Collision collision) {
        scaleObjects.Add(collision.transform);
    }
    
    void OnCollisionExit(Collision collision) {
        scaleObjects.Remove(collision.transform);
    }
}
