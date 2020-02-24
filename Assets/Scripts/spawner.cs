using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour {

    [SerializeField] GameObject SpawnableObject;
    [SerializeField] HSVColorPanel colorPanel;
    MeshRenderer renderer;

    private void Start() {
        renderer = GetComponent<MeshRenderer>();
    }

    private void Update() {
        renderer.material.color = colorPanel.color;
    }

    public GameObject spawnObject() {
        GameObject obj = GameObject.Instantiate(SpawnableObject, transform.position, transform.rotation);
        obj.GetComponent<MeshRenderer>().material.color = colorPanel.color;
        return obj;
    }
}