using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{

    [SerializeField]
    private GameObject[] props;

    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private int price = 5;

    private bool pressed = false;


    private void SpawnProp()
    {
        //play cash register sound
        AudioManager.instance?.Play3DSound(AudioEffect.vendingMachine, 1, spawnPoint.position);

        int index = Mathf.FloorToInt(Random.Range(0, props.Length));
        GameObject newProp = Instantiate(props[index]);
        newProp.transform.position = spawnPoint.position;
        //newProp.name = "Prop from vendingMachine" ;
        newProp.transform.localScale = new Vector3(2,2,2);

        StartCoroutine(resetting());
    }
    IEnumerator resetting()
    {
        yield return new WaitForSeconds(.5f);
        pressed = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!pressed)
        {
            pressed = true;
            SpawnProp();
        }
    }

}
