using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactuable : MonoBehaviour
{
    private bool puedeInteractuar;
    private BoxCollider2D bc;
    private SpriteRenderer sp;
    private GameObject indicadorInteractuable;
    private Animator anim;

    public UnityEvent evento;

    public GameObject[] objetos;
    public bool esCofre;
    public bool esPalanca;
    public bool palancaAccionada;
    public bool esCheckPoint;
    public bool esSelector;

    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        if (transform.GetChild(0) != null)
        indicadorInteractuable = transform.GetChild(0).gameObject;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            puedeInteractuar = true;
            indicadorInteractuable.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            puedeInteractuar = false;
            indicadorInteractuable.SetActive(false);
        }
    }
    private void Cofre()
    {
        if (esCofre)
        {
            Instantiate(objetos[Random.Range(0, objetos.Length)], transform.position, Quaternion.identity);
            anim.SetBool("abrir", true);
            bc.enabled = false;

        }
    }

    private void Palanca()
    {
        if(esPalanca && !palancaAccionada)
        {
            anim.SetBool("activar", true);
            palancaAccionada = true;
            evento.Invoke();
            indicadorInteractuable.SetActive(false);
            bc.enabled = false;
            this.enabled = false;
        }
        
    }

    private void CheckPoint()
    {
        if (esCheckPoint)
        {
            evento.Invoke();
        }

    }

    private void SeleccionarNivel()
    {
        if (esSelector)
        {
            evento.Invoke();
        }

    }



    private void Update()
    {
        if(puedeInteractuar && Input.GetKeyDown(KeyCode.C))
        {
            Cofre();
            Palanca();
            CheckPoint();
            SeleccionarNivel();
        }
    }
}
