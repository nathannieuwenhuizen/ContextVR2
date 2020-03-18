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
            if (scaleObjects[i] == null) {
                scaleObjects.Remove(scaleObjects[i]);
                break;
            }
            scaleObjects[i].localScale -= new Vector3(scaleSpeed,scaleSpeed,scaleSpeed);
            if (scaleObjects[i].localScale.x < deleteThershold) {
                GameObject dest = scaleObjects[i].gameObject;
                scaleObjects.Remove(scaleObjects[i]);
                GameObject.Destroy(dest);
            }
        }
    }

    void OnCollisionEnter(Collision collision) {
        //play prop hitting ground or othero object at head sound
        if (collision.gameObject.tag == Tags.GRABABLE)
        {
            AudioManager.instance?.Play3DSound(AudioEffect.propHitGround, .1f, collision.transform.position);
        }

        scaleObjects.Add(collision.transform);
    }
    
    void OnCollisionExit(Collision collision) {
        scaleObjects.Remove(collision.transform);
    }
}
