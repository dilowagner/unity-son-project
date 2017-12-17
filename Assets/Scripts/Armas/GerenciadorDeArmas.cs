using UnityEngine;
using UnityEngine.UI;

public class GerenciadorDeArmas : MonoBehaviour {

	public float distancia = 100f;
	public int balasPorPente = 30;
	public int balasDisparadas;
	public int balasRestantes;
    public int balasReservas = 100;
	public float taxaDeDisparo = 0.1f;
	public float contador;
	public Transform pontoRayCaster;
	public ParticleSystem efeitoDeFogo;
    private bool estaRecarregando;
    public Text textoMunicao;

	private Animator anim;
    private AudioSource fonteDeSom;
    public AudioClip somDeDisparo;
    public AudioClip somDeRecarga;

	// Use this for initialization
	void Start () {
		balasRestantes = balasPorPente;
		anim = GetComponent<Animator> ();
        fonteDeSom = GetComponent<AudioSource>();

        textoMunicao.text = balasRestantes + "/" + balasReservas;
	}
	
	// Update is called once per frame
	void Update () {
		Disparo ();
	}

	private void FixedUpdate() {
		AnimatorStateInfo informacao = anim.GetCurrentAnimatorStateInfo (0);
		if (informacao.IsName ("Atirando")) {
			anim.SetBool ("Atirando", false);
		}

        estaRecarregando = informacao.IsName("Recarga");
	}

	void Disparo()
	{
        if (Input.GetButton ("Fire1")) {
            if(balasRestantes > 0) {
                Tiro();    
            } else {
                Recarregar();
            }   
        }

        if(Input.GetKey(KeyCode.R)) {
            Recarregar();
        }

		if (contador < taxaDeDisparo) {
			contador += Time.deltaTime;
		}
	}

	void Tiro()
	{
        if (contador < taxaDeDisparo || balasRestantes <= 0 || estaRecarregando) {
			return;
		}

		RaycastHit bala;

		if (Physics.Raycast (pontoRayCaster.position, pontoRayCaster.transform.forward, out bala, distancia)) {
			Debug.Log ("Tocou em: " + bala.transform.name);
		}

		efeitoDeFogo.Play ();
        EfeitoSonoro();
		anim.CrossFadeInFixedTime ("Atirando", 0.1f);

        balasRestantes--;

		contador = 0.0f;

        AtualizarTextoMunicao();
	}

    void Recarregar() 
    {
        if(balasReservas <= 0) {
            return;
        }

        int QtdBalas = balasPorPente - balasRestantes;
        int QtdReduzir;

        if (balasReservas >= QtdBalas) {
            QtdReduzir = QtdBalas;    
        } else {
            QtdReduzir = balasReservas;    
        }

        RecargaAnimacao();

        balasReservas -= QtdReduzir;
        balasRestantes += QtdReduzir;

        AtualizarTextoMunicao();
    }

    void RecargaAnimacao()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("Recarga")) {
            return;
        }

        fonteDeSom.clip = somDeRecarga;
        fonteDeSom.PlayDelayed(0.7f);

        anim.CrossFadeInFixedTime("Recarga", 0.01f);
    }

    void EfeitoSonoro()
    {
        fonteDeSom.clip = somDeDisparo;
        fonteDeSom.Play();
    }

    void AtualizarTextoMunicao()
    {
        textoMunicao.text = balasRestantes + "/" + balasReservas;
    }
}
