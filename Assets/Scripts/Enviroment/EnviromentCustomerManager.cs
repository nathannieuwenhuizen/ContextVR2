using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentCustomerManager : MonoBehaviour
{
    [Range(0, 50)]
    [SerializeField]
    private int amountOfCitizens = 10;
    [SerializeField]
    private Transform[] positions;
    [SerializeField]
    private GameObject customerPrefab;


    private List<Vector3> vectorPositions;

    private List<Customer> customers;

    private void Start()
    {
        customers = new List<Customer>();
        vectorPositions = new List<Vector3>();
        for (int i = 0; i < positions.Length; i++)
        {
            vectorPositions.Add(positions[i].position);

        }
        LoadCustomers();

    }
    public void LoadCustomers()
    {
        for (int i = 0; i < amountOfCitizens; i++)
        {
            int startIndex = Random.Range(0, vectorPositions.Count);
            bool invert = Random.value > 0.5f;

            Customer tempCustomer = Instantiate(customerPrefab).GetComponent<Customer>();
            tempCustomer.transform.parent = transform; 
            tempCustomer.gameObject.name = " citizen #" + i;
            tempCustomer.canvas.gameObject.SetActive(false);
            tempCustomer.transform.position = positions[startIndex].position + transform.forward * 5f;

            tempCustomer.movement.walkSpeed = Random.Range(0.02f, 0.04f);
            //tempCustomer.movement.walkSpeed = .2f;
            StartCoroutine(tempCustomer.movement.WalkLoop(vectorPositions.ToArray(), invert, startIndex));

            customers.Add(tempCustomer);
        }
    }
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;

        for (int i = 0; i < positions.Length; i++)
        {
            Gizmos.DrawSphere(positions[i].position, 1);
            Gizmos.DrawLine(positions[i].position, positions[(i + 1) % positions.Length].position);
        }
    }
}