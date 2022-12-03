using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caida : MonoBehaviour
{
    public GameObject checkpoint;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.transform.position = checkpoint.transform.position;
            collision.GetComponent<PlayerController>().RecibirDaño();
        }
    }
}
