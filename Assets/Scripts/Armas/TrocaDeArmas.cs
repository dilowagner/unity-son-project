using UnityEngine;

public class TrocaDeArmas : MonoBehaviour {

    public int armaSelecionada = 0;
    public GerenciadorDeArmas gerenciadorDeArmas;

	// Use this for initialization
	void Start () {
        SelecionarArma();
	}
	
	// Update is called once per frame
	void Update () {

        if(gerenciadorDeArmas.estaRecarregando || gerenciadorDeArmas.estaScope) 
        {
            return;
        }

        int armaAnterior = armaSelecionada;
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) {
            if(armaSelecionada >= transform.childCount - 1) 
            {
                armaSelecionada = 0;
            } 
            else 
            {
                armaSelecionada++;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (armaSelecionada <= 0)
            {
                armaSelecionada = transform.childCount - 1;
            }
            else
            {
                armaSelecionada--;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (armaSelecionada >= transform.childCount - 1)
            {
                armaSelecionada = 0;
            }
            else
            {
                armaSelecionada++;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (armaSelecionada <= 0)
            {
                armaSelecionada = transform.childCount - 1;
            }
            else
            {
                armaSelecionada--;
            }
        }

        if(armaAnterior != armaSelecionada) {
            SelecionarArma();
        }
	}

    void SelecionarArma() {

        int i = 0;
        foreach(Transform arma in transform) {

            if(i == armaSelecionada) {
                arma.gameObject.SetActive(true);
            } else {
                arma.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
