using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogos : MonoBehaviour
{
    public bool enDialogo;
    private bool detectando;

    public List<string> dialogo = new List<string>();
    public Text textDialogos;
    public PlayerController playerController;
    public float tiempoEntreTextos;
    public GameObject iconoDialogo;

    public Image imagenCaraDialogo;
    public Sprite imagenCara;

    private void OnTriggerEnter2D(Collider2D col)
    {
        detectando = true;
        iconoDialogo.SetActive (true);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        detectando = false;
        iconoDialogo.SetActive (false);
    }

    private void Update ()
    {
        if (detectando)
        {
            if (Input.GetKeyDown(KeyCode.C) && !enDialogo)
            {
                imagenCaraDialogo.sprite = imagenCara;
                textDialogos.transform.parent.gameObject.SetActive(true);
                enDialogo = true;
                playerController.enabled = false;
                StartCoroutine(Dialogar());
            }
        }
    }
 

    public bool GetEnDialogo()
    {
        return enDialogo;
    }

    private IEnumerator Dialogar()
    {
        for(int i = 0; i < dialogo.Count; i++)
        {
            char[] textoActual = dialogo[i].ToCharArray();
            for (int j = 0; j < textoActual.Length; j++)
            {
                textDialogos.text += textoActual[j];
                if (Input.GetKeyDown(KeyCode.C))
                {
                    textDialogos.text = dialogo[i];
                    j = textoActual.Length - 1;
                    yield return null;
                }
                else
                {
                    yield return new WaitForSeconds(tiempoEntreTextos);
                }
                
            }

            while (!Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("Continuar");
                yield return null;
            }
            textDialogos.text = string.Empty;
            yield return null;
        }
        enDialogo = false;
        textDialogos.transform.parent.gameObject.SetActive(false);
        playerController.enabled = true;
    }
}
