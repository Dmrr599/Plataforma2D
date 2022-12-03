using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeteccionFinal : MonoBehaviour
{

    public bool avanzando;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("DETECTA JUGADOR");
            GameManager.instance.ActivarPanelTransicion();   
            GameManager.instance.avanzandoNivel = avanzando; 
            StartCoroutine(EsperarCambioPosicion());
        }
    }
    private IEnumerator EsperarCambioPosicion()
    {
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.CambiarPosicionJugador();
        if(avanzando)
            GameManager.instance.nivelActual++;
        else
            GameManager.instance.nivelActual--;

    }
}

