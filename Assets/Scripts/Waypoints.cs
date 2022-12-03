using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    private Vector3 direccion;
    private PlayerController player;
    private CinemachineVirtualCamera cm;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private int indiceActual = 0;
    private bool aplicarFuerza;

    public int vidas = 3;
    public Vector2 posicionCabeza;
    public float velocidadDesplazamiento;
    public List<Transform> puntos = new List<Transform>();
    public bool esperando;
    public float tiempoDeEspera;

    private void Awake()
    {
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Start()
    {
        if(gameObject.CompareTag("Enemigo"))
            gameObject.name = "Spider";
    }

    private void FixedUpdate()
    {
        MovimientoWaypoints();
        if (gameObject.CompareTag("Enemigo"))
        {
            CambiarEscalaEnemigo();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Enemigo"))
        {
            if(player.transform.position.y - 0.7f > transform.position.y + posicionCabeza.y)
            {
                player.GetComponent<Rigidbody2D>().velocity = Vector2.up * player.fuerzaDeSalto;

                Destroy(this.gameObject, 0.2f);
            }
            else
            {
                player.RecibirDaño(-(player.transform.position - transform.position).normalized);                
            }
        } else if(collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Plataforma"))
        {
            if(player.transform.position.y - 0.7f > transform.position.y)
            {
                player.transform.parent = transform;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Plataforma"))
        {
            player.transform.parent = null;
        }
    }

    private void CambiarEscalaEnemigo()
    {
        if(direccion.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if(direccion.x > 0 && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
    }

    private void MovimientoWaypoints()
    {
        direccion = (puntos[indiceActual].position - transform.position).normalized;

        if(!esperando)
        transform.position = (Vector2.MoveTowards(transform.position, puntos[indiceActual].position, velocidadDesplazamiento * Time.deltaTime));

        if(Vector2.Distance(transform.position, puntos[indiceActual].position)<= 0.7f)
    {
        if (!esperando)
        {
                 StartCoroutine(Espera());
        }

    }
        
        
    }

    private IEnumerator Espera()
    {
        esperando = true;
        yield return new WaitForSeconds(tiempoDeEspera); 
        esperando = false;
        indiceActual++;        

        if (indiceActual >= puntos.Count)
            indiceActual = 0;
    }

    public void RecibirDaño()
    {
        if(vidas > 0)
        {
            StartCoroutine(EfectoDaño());
            StartCoroutine(AgitarCamara(0.1f));
            aplicarFuerza =  true;
            vidas--;
        }
        else
        {
            StartCoroutine(AgitarCamara(0.1f));
            velocidadDesplazamiento = 0;
            rb.velocity = Vector2.zero;
            Destroy(this.gameObject, 0.2f);
        }

    }

    private IEnumerator AgitarCamara(float tiempo)
    {       
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
        yield return new WaitForSeconds(tiempo);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;       
    }
    private IEnumerator EfectoDaño()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sp.color = Color.white;
    }
}
