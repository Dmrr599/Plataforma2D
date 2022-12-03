using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampa : MonoBehaviour
{
    private PlayerController player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }   


private void OnTriggerEnter2D(Collider2D collision)
{
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().RecibirDaño(-(collision.transform.position - transform.position).normalized);
        }
    }
}