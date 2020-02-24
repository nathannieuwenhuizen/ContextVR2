using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour {

    [SerializeField] GameObject SpawnableObject;
    public GameObject spawnObject() {
        return GameObject.Instantiate(SpawnableObject,transform.position,transform.rotation,transform);
    }
}