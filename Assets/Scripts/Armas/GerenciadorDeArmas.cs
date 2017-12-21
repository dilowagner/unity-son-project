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
    public bool estaRecarregando;
    public Text textoMunicao;

	private Animator anim;
    private AudioSource fonteDeSom;
    public AudioClip somDeDisparo;
    public AudioClip somDeRecarga;


    private float[] distanciaArmas = { 100f, 120f, 40f, 300f };
    private int[] balasPorPenteArmas = { 30, 35, 12, 10 };
    private int[] balasReservasArmas = { 100, 120, 40, 25 };
    private float[] taxaDeDisparoArmas = { 0.1f, 0.1f, 0.98f, 1.8f };
    private int[] balasRestantesArmas = { 30, 35, 12, 10 };
    public float dano;
    private float[] danoArmas = { 1.2f, 0.9f, 3f, 8f };
    public AudioClip[] somDeDisparoArmas = {};

    public TrocaDeArmas trocaDeArmas;
    private int armaSelecionada;
    public GameObject efeitoImpacto;
    public GameObject rayCastCamera;

    public GameObject scope;
    public GameObject scopeCam;
    public bool estaScope;

    public GameObject mira;
    public RaycastHit bala;

	// Use this for initialization
	void Start () {

        dano = danoArmas[trocaDeArmas.armaSelecionada];
        scope.SetActive(false);
        estaScope = false;
        armaSelecionada = trocaDeArmas.armaSelecionada;

        balasRestantesArmas[armaSelecionada] = balasPorPenteArmas[armaSelecionada];
		anim = GetComponent<Animator> ();
        fonteDeSom = GetComponent<AudioSource>();

        textoMunicao.text = balasRestantesArmas[armaSelecionada] + "/" + balasReservasArmas[armaSelecionada];
	}
	
	// Update is called once per frame
	void Update () {
        Scope();
        dano = danoArmas[trocaDeArmas.armaSelecionada];
        armaSelecionada = trocaDeArmas.armaSelecionada;
		Disparo ();
        AtualizarTextoMunicao();
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
            if(balasRestantesArmas[armaSelecionada] > 0) {
                Tiro();    
            } else {
                Recarregar();
            }   
        }

        if(Input.GetKey(KeyCode.R)) {
            Recarregar();
        }

        if (contador < taxaDeDisparoArmas[armaSelecionada]) {
			contador += Time.deltaTime;
		}
	}

	void Tiro()
	{
        if (contador < taxaDeDisparoArmas[armaSelecionada] || balasRestantesArmas[armaSelecionada] <= 0 || estaRecarregando) {
			return;
		}

		

        if (Physics.Raycast (rayCastCamera.transform.position, rayCastCamera.transform.forward, out bala, distanciaArmas[armaSelecionada])) {
			Debug.Log ("Tocou em: " + bala.transform.name);
		}

		efeitoDeFogo.Play ();
        EfeitoSonoro();
		anim.CrossFadeInFixedTime ("Atirando", 0.1f);

        GameObject efeitoImpactoClone = Instantiate(efeitoImpacto, bala.point, Quaternion.FromToRotation(Vector3.forward, bala.normal));
        balasRestantesArmas[armaSelecionada]--;

		contador = 0.0f;

        AtualizarTextoMunicao();
	}

    void Recarregar() 
    {
        if(balasReservasArmas[armaSelecionada] <= 0) {
            return;
        }

        int QtdBalas = balasPorPenteArmas[armaSelecionada] - balasRestantesArmas[armaSelecionada];
        int QtdReduzir;

        if (balasReservasArmas[armaSelecionada] >= QtdBalas) {
            QtdReduzir = QtdBalas;    
        } else {
            QtdReduzir = balasReservasArmas[armaSelecionada];    
        }

        RecargaAnimacao();

        balasReservasArmas[armaSelecionada] -= QtdReduzir;
        balasRestantesArmas[armaSelecionada] += QtdReduzir;

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
        fonteDeSom.clip = somDeDisparoArmas[armaSelecionada];
        fonteDeSom.Play();
    }

    void AtualizarTextoMunicao()
    {
        textoMunicao.text = balasRestantesArmas[armaSelecionada] + "/" + balasReservasArmas[armaSelecionada];
    }

    void Scope() 
    {
        if(estaRecarregando) 
        {
            scope.SetActive(false);
            scopeCam.GetComponent<Camera>().fieldOfView = 60f;
            estaScope = false;
        }

        if(armaSelecionada != 3 || estaRecarregando) 
        {
            mira.SetActive(true);
            return;
        }

        mira.SetActive(false);

        if(Input.GetMouseButtonDown(1)) 
        {
            if(estaScope) 
            {
                scope.SetActive(false);
                scopeCam.GetComponent<Camera>().fieldOfView = 60f;
                estaScope = false;
            }
            else 
            {
                scope.SetActive(true);
                scopeCam.GetComponent<Camera>().fieldOfView = 10f;
                estaScope = true;
            }
        }
    }
}
