using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAtaque : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("Enemigo"))
        {
            if(collision.name == "Bat")
            {
                collision.GetComponent<Bat>().RecibirDaño();
            }
        }
    }
}
