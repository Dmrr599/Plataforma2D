using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
   
    private int direccionX;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 direccion;
    private CinemachineVirtualCamera cm;
    private Vector2 direccionMovimiento;
    private Vector2 direccionDaño;
    private bool bloqueado;
    private GrayCamera gc;
    private SpriteRenderer sprite;
    
    [Header("Estadisticas")]
    public float velocidadDeMovimiento = 10;
    public float fuerzaDeSalto = 5;
    public float velocidadDash = 20;
    public float velocidadDeslizar;
    public int vidas = 3;
    public float tiempoInmortalidad;

    [Header("Colisiones")]
    public LayerMask layerPiso;
    public float radioDeColision;
    public Vector2 abajo, derecha, izquierda;
    

    [Header("Booleanos")]
    public bool puedeMover = true;
    public bool enSuelo = true;
    public bool puedeDash;
    public bool haciendoDash;
    public bool tocadoPiso;
    public bool haciendoShake;
    public bool estaAtacando;
    public bool enMuro;
    public bool muroDerecho;
    public bool muroIzquierdo;
    public bool agarrarse;
    public bool saltarDeMuro;
    public bool esInmortal;
    public bool aplicarFuerza;
    public bool terminandoMapa;


    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        gc = Camera.main.GetComponent<GrayCamera>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void SetBloqueadoTrue()
    {
        bloqueado = true;
    }

    public void Morir()
    {
        if (vidas > 0)
        return;
        
        GameManager.instance.GameOver();
        this.enabled =false;
    }

    public void RecibirDaño()
    {
        StartCoroutine(ImpactoDaño(Vector2.zero));        
    }

    public void RecibirDaño(Vector2 direccionDaño)
    {
        StartCoroutine(ImpactoDaño(direccionDaño));
    }

   private IEnumerator ImpactoDaño(Vector2 direccionDaño)
    {
        if (!esInmortal)
        {
            StartCoroutine(Inmortalidad());
            vidas--;
            gc.enabled = true;
            float velocidadAuxiliar = velocidadDeMovimiento;
            this.direccionDaño = direccionDaño;
            aplicarFuerza = true;
            Time.timeScale = 0.4f;
            FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
            StartCoroutine(AgitarCamara());
            yield return new WaitForSeconds(0.2f);
            Time.timeScale = 1;
            gc.enabled = false;

            ActualizarVidasUI(1);
            
            velocidadDeMovimiento = velocidadAuxiliar;
            Morir();

        }
    }

        
    public void ActualizarVidasUI(int vidasADescontar)
    {
        int vidasDescontadas = vidasADescontar;
            for(int i = GameManager.instance.vidasUI.transform.childCount - 1; i>= 0; i--)
            {
                if(GameManager.instance.vidasUI.transform.GetChild(i).gameObject.activeInHierarchy && vidasDescontadas != 0)
                {
                   GameManager.instance.vidasUI.transform.GetChild(i).gameObject.SetActive(false);
                   vidasDescontadas--;                   
                }
                else
                {
                    if (vidasDescontadas == 0)
                    break;

                }
            }
            

    }

    private void FixedUpdate()
    {
        if(aplicarFuerza)
        {
            velocidadDeMovimiento = 0;
            rb.velocity = Vector2.zero;
            rb.AddForce(-direccionDaño * 25, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }

    public void DarInmortalidad()
    {
        StartCoroutine(Inmortalidad());
    }

    private IEnumerator Inmortalidad()
    {
        esInmortal = true;

        float tiempoTranscurrido = 0;

        while (tiempoTranscurrido < tiempoInmortalidad)
        {
            sprite.color = new Color(1, 1, 1, .5f);
            yield return new WaitForSeconds(tiempoInmortalidad / 20);
            sprite.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(tiempoInmortalidad / 20);
            tiempoTranscurrido += tiempoInmortalidad / 10;
        }
        esInmortal = false;
    }

    public void MovimientoFinalMapa(int direccionX)
    {
        terminandoMapa = true;
        this.direccionX = direccionX;
        anim.SetBool("caminar", true);

        if(this.direccionX < 0 && transform.localScale.x > 0)
        {                        
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if(this.direccionX > 0 && transform.localScale.x < 0)
        {                        
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); 
        }
        
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!terminandoMapa)
        {
            Movimiento();
            Agarres(); 
        }else
        {
            rb.velocity = (new Vector2(direccionX * velocidadDeMovimiento, rb.velocity.y));
        }
       
    }

    private void Atacar(Vector2 direccion)
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!estaAtacando && !haciendoDash)
            {
                estaAtacando = true;

                anim.SetFloat("ataqueX", direccion.x);
                anim.SetFloat("ataqueY", direccion.y);

                anim.SetBool("atacar", true);
            }
        }
    }

    public void FinalizarAtaque()
    {
        anim.SetBool("atacar", false);
        bloqueado = false;
        estaAtacando = false;
    }

    private Vector2 DireccionAtaque(Vector2 direccionMovimiento, Vector2 direccion)
    {
        if(rb.velocity.x == 0 && direccion.y != 0)
            return new Vector2(0,direccion.y);

        return new Vector2(direccionMovimiento.x, direccion.y);
    }

    private IEnumerator AgitarCamara(){
        haciendoShake = true;
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
        yield return new WaitForSeconds(0.3f);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
        haciendoShake = false;
    }

    private IEnumerator AgitarCamara(float tiempo){
        haciendoShake = true;
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
        yield return new WaitForSeconds(tiempo);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
        haciendoShake = false;
    }

    private void Dash(float x, float y){ // Para controlar el efecto de las ondas
        anim.SetBool("dash", true);
        Vector3 posicionjugador = Camera.main.WorldToViewportPoint(transform.position); // Obtiene la posicion de acuerdo al movimiento de la camara
        Camera.main.GetComponent<RippleEffect>().Emit(posicionjugador); // Emite las ondas de acuerdo a la posicion en cuanto a la camara
        StartCoroutine(AgitarCamara());

        puedeDash = true;
        rb.velocity = Vector2.zero; // Aplica una velocidad cero cuando vaya a hacer el dash
        rb.velocity += new Vector2(x, y).normalized * velocidadDash; // Devuelve las velocidad
        StartCoroutine(PrepararDash());
    }

    // Crea una nueva rutina
    private IEnumerator PrepararDash(){

        StartCoroutine(DashSuelo());

        rb.gravityScale = 0;
        haciendoDash = true;

        yield return new WaitForSeconds(0.3f); // Espera 0.3 segundos

        rb.gravityScale = 3;
        haciendoDash = false;
        FinalizarDash();
    }

    private IEnumerator DashSuelo(){
        yield return new WaitForSeconds(0.15f);

        if (enSuelo)
        puedeDash = false;
    }

    public void FinalizarDash(){
        anim.SetBool("dash", false);
    }

    private void TocarPiso(){
        puedeDash = false;
        haciendoDash = false;
        anim.SetBool("saltar", false);
    }

    private void Movimiento ()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // Parámetros que se le daran al dash
        // Con GetAxisRaw puede tomar valores entre el cero y el -1
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        direccion = new Vector2(x, y);
        Vector2 direccionRaw = new Vector2(xRaw, yRaw);

        Caminar();
        Atacar(DireccionAtaque(direccionMovimiento, direccionRaw));

        if (enSuelo && !haciendoDash)
        {
            saltarDeMuro = false;
        }

        agarrarse = enMuro && Input.GetKey(KeyCode.LeftShift);

        if(agarrarse && !enSuelo)
        {
            anim.SetBool("escalar", true);
            if(rb.velocity == Vector2.zero)
            {
                anim.SetFloat("velocidad", 0);
            }
            else
            {
                anim.SetFloat("velocidad", 1);
            }
        }
        else
        {
            anim.SetBool("escalar", false);
            anim.SetFloat("velocidad", 0);
        }

        if(agarrarse && !haciendoDash)
        {
            rb.gravityScale = 0;
            if (x > 0.2f || x < -0.2f)
                rb.velocity = new Vector2(rb.velocity.x, 0);

            float modificadorVelocidad = y > 0 ? 0.5f : 1;
            rb.velocity = new Vector2(rb.velocity.x, y * (velocidadDeMovimiento * modificadorVelocidad));

            if (muroIzquierdo && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }else if (muroDerecho && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            rb.gravityScale = 3;
        }

        if (enMuro && !enSuelo)
        {
            anim.SetBool("escalar", true);

            if (x != 0 && !agarrarse)
                DeslizarPared();
        }

        MejorarSalto();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (enSuelo)
            {
                anim.SetBool("saltar", true);
                 Saltar();
            }

            if (enMuro && !enSuelo)
            {
                anim.SetBool("escalar", false);
                anim.SetBool("saltar", true);
                SaltarDesdeMuro();
            }
           
        }
        
        if(Input.GetKeyDown(KeyCode.X) && !haciendoDash && !puedeDash)
        { 
            if(xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }

        if(enSuelo && !tocadoPiso)
        {
            anim.SetBool("escalar", false);

            TocarPiso();
            tocadoPiso = true;
        }

        if(!enSuelo && !tocadoPiso)
            tocadoPiso = false;

        // Controlar el salto
        float velocidad; // Para que la velocidad solo pueda ser 0 o 1 porque "y" es de valor muy variante
            if(rb.velocity.y > 0)
                velocidad = 1;
            else
                velocidad = -1;

        if (!enSuelo)
        {
            // anim.SetBool("caer", false);

            anim.SetFloat("velocidadVertical", velocidad);
        } else { // Si esta en el suelo finaliza el salto
            if(velocidad == -1)
                FinalizarSalto();
        }
       
    }

    private void DeslizarPared() 
    {
        if (puedeMover)
            rb.velocity = new Vector2(rb.velocity.x, -velocidadDeslizar);
    }

    private void SaltarDesdeMuro()
    {
        StopCoroutine(DeshabilitarMovimiento(0));
        StartCoroutine(DeshabilitarMovimiento(0.1f));

        Vector2 direccionMuro = muroDerecho ? Vector2.left : Vector2.right;

        if(direccionMuro.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if(direccionMuro.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        anim.SetBool("saltar", true);
        anim.SetBool("escalar",false);
        Saltar((Vector2.up + direccionMuro), true);

        saltarDeMuro = true;
    }

    private IEnumerator DeshabilitarMovimiento(float tiempo)
    {
        puedeMover = false;
        yield return new WaitForSeconds(tiempo);
        puedeMover = true;
    }

    public void FinalizarSalto()
        {
            anim.SetBool("saltar", false);
            // anim.SetBool("caer", false);
        }

    private void MejorarSalto()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.5f - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.0f -1) * Time.deltaTime;
        }
    }

    private void Agarres()
    {
        enSuelo = Physics2D.OverlapCircle((Vector2)transform.position + abajo, radioDeColision, layerPiso);
        Collider2D collisionDerecha = Physics2D.OverlapCircle((Vector2)transform.position + derecha, radioDeColision, layerPiso);
        Collider2D collisionIzquierda = Physics2D.OverlapCircle((Vector2)transform.position + izquierda, radioDeColision, layerPiso);

        if(collisionDerecha != null)
        {
            enMuro = !collisionDerecha.CompareTag("Plataforma");
        }else if(collisionIzquierda != null)
        {
            enMuro = !collisionIzquierda.CompareTag("Plataforma");            
        }
        else
        {
            enMuro = false;
        }


        muroDerecho = Physics2D.OverlapCircle((Vector2)transform.position + derecha, radioDeColision, layerPiso);
        muroIzquierdo = Physics2D.OverlapCircle((Vector2)transform.position + izquierda, radioDeColision, layerPiso); 
               

    }

    private void Saltar()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up * fuerzaDeSalto;

    }

    private void Saltar(Vector2 direccionSalto, bool muro)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += direccionSalto * fuerzaDeSalto;

    }

    private void Caminar()
    {
        if(puedeMover && !haciendoDash && !estaAtacando)
        {
            if(saltarDeMuro)
            {
                rb.velocity = Vector2.Lerp(rb.velocity, 
                (new Vector2(direccion.x * velocidadDeMovimiento, rb.velocity.y)), Time.deltaTime / 2);
            }
            else
            {
                if(direccion != Vector2.zero && !agarrarse)
                {
                    if (!enSuelo)
                    {
                        // anim.SetBool("caer", true);
                        anim.SetBool("saltar", true);
                    }
                    else
                    {
                        anim.SetBool("caminar", true);
                    }

                    rb.velocity = (new Vector2(direccion.x * velocidadDeMovimiento, rb.velocity.y));

                    if(direccion.x < 0 && transform.localScale.x > 0)
                    {
                        direccionMovimiento = DireccionAtaque(Vector2.left, direccion);
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    }
                    else if(direccion.x > 0 && transform.localScale.x < 0)
                    {
                        direccionMovimiento = DireccionAtaque(Vector2.right, direccion);
                        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); 
                    }

                }
                else
                {
                    if (direccion.y > 0 && direccion.x == 0)
                    {
                        direccionMovimiento = DireccionAtaque(direccion, Vector2.up);
                    }
                    anim.SetBool("caminar", false);
                }    
            }
            }
            else
            {
                if(bloqueado)
                {
                    FinalizarAtaque();
                }
            }

        }
        
    }
