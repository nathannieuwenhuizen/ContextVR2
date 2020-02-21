using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour {

    [SerializeField] GameObject SpawnableObject;
    Transform spawnObject() {
        return GameObject.Instantiate(SpawnableObject,transform.position,transform.rotation,transform).transform;
    }
}
