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
            else if(collision.name == "Skeleton")
            {
                collision.GetComponent<Skeleton>().RecibirDaño();
            }else if(collision.name == "Spider")
            {
                collision.GetComponent<Waypoints>().RecibirDaño();
            }
        }
        else if (collision.CompareTag("Destruible"))
            {
                collision.GetComponent<Animator>().SetBool("destruir", true);
            }
    }
}


