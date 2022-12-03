using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelTransicion : MonoBehaviour
{
      
    public void AparecerJuego()

    
    {
       gameObject.GetComponent<Animator>().SetTrigger("aparecer");   
    }

    public void DefaultTransicion()
    {
        gameObject.GetComponent<Animator>().SetTrigger("default");   
    }

}
