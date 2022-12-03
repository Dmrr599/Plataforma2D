using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    { 
    if (collision.CompareTag("Player"))
    {
        AsignarItem();
    }

}

private void AsignarItem()
{
    if(gameObject.CompareTag("Moneda"))
    {
        GameManager.instance.ActualizarContadorMonedas();
    }else if (gameObject.CompareTag("PowerUp"))
    {
        GameManager.instance.player.DarInmortalidad();

    }
    Destroy(gameObject);

    }
}