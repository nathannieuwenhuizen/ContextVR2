using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSpawner : MonoBehaviour
{

    [SerializeField] GameObject SpawnableObject;
    [SerializeField] HSVColorPanel colorPanel;
    MeshRenderer renderer;

    private void Start()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        renderer.material.color = colorPanel.color;
    }

    public void spawnObject()
    {
        GameObject obj = GameObject.Instantiate(this.gameObject, transform.position, transform.rotation);
        Destroy(obj.GetComponent<spawner>());
        obj.GetComponent<HairObject>().idleMaterial = GetComponent<HairObject>().idleMaterial;
        //GameObject obj = GameObject.Instantiate(SpawnableObject, transform.position, transform.rotation);
        obj.GetComponent<MeshRenderer>().material.color = colorPanel.color;
    }
}