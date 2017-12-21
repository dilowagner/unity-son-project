using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class FPSPersonagem : NetworkBehaviour {

	// Vars de Transform
	private Transform fpsView;
	private Transform fpsCamera;
	private Vector3 fpsRotation = Vector3.zero;

	//Vars de Velocidade
	public float velocidadeAndando = 6.7f;
	public float velocidadeCorrendo = 10.0f;
	public float alturaPulo = 8f;
	public float gravidade = 20f;

	private float velocidade;

	// Vars de teclado
	private float inputX, inputY;
	private float inputXSet, inputYSet;
	private float fator;
	private bool limitarVelocidadeDiagonal = true;
	private float antiToque = 0.75f;

	// Vars de logica
	private bool noChao, seMovendo;

	// Vars de Controle
	private CharacterController charController;
	private Vector3 direcaoMovimento = Vector3.zero;

	// Pulo, Corrida e Agachamento
	public LayerMask camadaDoChao;
	private float rayDistance;
	private float alturaPadrao;
	private Vector3 camPosPadrao;
	private float camAltura;
	private bool estaAgachado;
	private float velocidadeAgachado = 3.15f;


	// Vars Animacoes
	private Animator animator;
	private GameObject arma;
    private AudioSource audioSrc;

    // Multiplayer
    public GameObject personagem, braco;
    public GameObject mao;
    public GameObject[] armasView;
    private Camera mainCam;
    public FPSCamera[] fpsCameras;

    public GerenciadorDeArmas ga;

	// Use this for initialization
	void Start () {

		fpsView = transform.Find ("FPS Visao").transform;
		charController = GetComponent<CharacterController> ();
		velocidade = velocidadeAndando;
		seMovendo = false;

		animator = transform.Find("Modelo").gameObject.GetComponent<Animator>();
		arma = transform.Find ("FPS Visao").transform.Find ("Main Camera").transform.Find ("Arma").gameObject;
        audioSrc = GetComponent<AudioSource>();

		rayDistance = charController.height * 0.5f + charController.radius;
		alturaPadrao = charController.height;
		camPosPadrao = fpsView.localPosition;

        // Eu
        if(isLocalPlayer)
        {
            personagem.layer = LayerMask.NameToLayer("Jogador");
            foreach(Transform filho in personagem.transform) {
                filho.gameObject.layer = LayerMask.NameToLayer("Jogador");
            }

            braco.layer = LayerMask.NameToLayer("Inimigo");
            foreach (Transform filho in braco.transform)
            {
                filho.gameObject.layer = LayerMask.NameToLayer("Inimigo");
            }

            mao.layer = LayerMask.NameToLayer("Inimigo");
            foreach (Transform filho in mao.transform)
            {
                filho.gameObject.layer = LayerMask.NameToLayer("Inimigo");
            }

            for (int i = 0; i < armasView.Length; i++) 
            {
                armasView[i].layer = LayerMask.NameToLayer("Jogador");
            }
        }

        // Outro jogador
        if (!isLocalPlayer)
        {
            personagem.layer = LayerMask.NameToLayer("Inimigo");
            foreach (Transform filho in personagem.transform)
            {
                filho.gameObject.layer = LayerMask.NameToLayer("Inimigo");
            }

            braco.layer = LayerMask.NameToLayer("Jogador");
            foreach (Transform filho in braco.transform)
            {
                filho.gameObject.layer = LayerMask.NameToLayer("Jogador");
            }

            mao.layer = LayerMask.NameToLayer("Jogador");
            foreach (Transform filho in mao.transform)
            {
                filho.gameObject.layer = LayerMask.NameToLayer("Jogador");
            }

            for (int i = 0; i < armasView.Length; i++)
            {
                armasView[i].layer = LayerMask.NameToLayer("Inimigo");
            }
        }

        if (!isLocalPlayer)
        {
            for (int i = 0; i < fpsCameras.Length; i++) 
            {
                fpsCameras[i].enabled = false;
            }
        }

        mainCam = transform.Find("FPS Visao").Find("Main Camera").GetComponent<Camera>();
        mainCam.gameObject.SetActive(false);
	}

    public override void OnStartLocalPlayer()
    {
        tag = "Player";
    }
	
	// Update is called once per frame
	void Update () {

        if (isLocalPlayer)
        {
            if(!mainCam.gameObject.activeInHierarchy) {
                mainCam.gameObject.SetActive(true);
            }
        }

        if(!isLocalPlayer)
        {
            return;
        }

        if(Input.GetButton("Fire1"))
        {
            Dano();
        }

		Movimento ();
		AnimacoesFPS ();
        Pegadas();
	}

	void Movimento() {
		// Detectando movimento no Eixo Y
		if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S)) {

			if (Input.GetKey (KeyCode.W)) {
				inputYSet = 1f;
			} else {
				inputYSet = -1f;
			}
		} else {
			inputYSet = 0f;
		}

		// Detectando movimento no Eixo X
		if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.D)) {
			if (Input.GetKey (KeyCode.D)) {
				inputXSet = 1f;
			} else {
				inputXSet = -1f;
			}
		} else {
			inputXSet = 0f;
		}

		inputX = Mathf.Lerp (inputX, inputXSet, Time.deltaTime * 20f);
		inputY = Mathf.Lerp (inputY, inputYSet, Time.deltaTime * 20f);
		fator = Mathf.Lerp (fator, (inputY != 0 && inputXSet != 0 && limitarVelocidadeDiagonal) ? 0.75f : 1.0f, Time.deltaTime * 20f);

		fpsRotation = Vector3.Lerp (fpsRotation, Vector3.zero, Time.deltaTime * 5f);
		fpsView.localEulerAngles = fpsRotation;

		if (noChao) {
			AgachaECorre ();
			direcaoMovimento = new Vector3 (inputX * fator, -antiToque, inputY * fator);
			direcaoMovimento = transform.TransformDirection (direcaoMovimento) * velocidade;
			Pulo ();
		}

		direcaoMovimento.y -= gravidade * Time.deltaTime;

		noChao = (charController.Move (direcaoMovimento * Time.deltaTime) & CollisionFlags.Below) != 0;
		seMovendo = charController.velocity.magnitude > 0.15f;

		AnimacoesAndando ();
		AnimacoesPulando ();
	}

	void AgachaECorre() {
		if (Input.GetKeyDown (KeyCode.C)) {
			if (!estaAgachado) {
				estaAgachado = true;
			} else {
				if (PodeSeLevantar ()) {
					estaAgachado = false;
				}
			}
		}

		StopCoroutine (MoveCamera());
		StartCoroutine (MoveCamera());

		if (estaAgachado) {
			velocidade = velocidadeAgachado;
		} else {
			if (Input.GetKey (KeyCode.LeftShift)) {
				velocidade = velocidadeCorrendo;
			} else {
				velocidade = velocidadeAndando;
			}
		}

		AnimacoesAgachado ();
	}

	bool PodeSeLevantar() {
		Ray rayTopo = new Ray (transform.position, transform.up);
		RaycastHit rayTopoHit;

		if (Physics.SphereCast (rayTopo, charController.radius + 0.05f, out rayTopoHit, rayDistance, camadaDoChao)) {
			if (Vector3.Distance (transform.position, rayTopoHit.point) < 2.79f) {
				return false;
			}
		}

		return true;
	}

	IEnumerator MoveCamera() {
		charController.height = estaAgachado ? alturaPadrao / 1.5f : alturaPadrao;
		charController.center = new Vector3 (0, charController.height / 2f, 0);
		camAltura = estaAgachado ? camPosPadrao.y / 1.5f : camPosPadrao.y;

		while(Mathf.Abs(camAltura - fpsView.localPosition.y) > 0.01f) {
			fpsView.localPosition = Vector3.Lerp (
				fpsView.localPosition, 
				new Vector3 (
					camPosPadrao.x, 
					camAltura, 
					camPosPadrao.z
				), Time.deltaTime * 11f);
		}

		yield return null;
	}

	void Pulo() {
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (estaAgachado) {
				if (PodeSeLevantar ()) {
					estaAgachado = false;
					StopCoroutine (MoveCamera());
					StartCoroutine (MoveCamera());
				}
			} else {
				direcaoMovimento.y = alturaPulo;
			}
		}

		AnimacoesAgachado ();
	}

	void AnimacoesAndando() {
		
		animator.SetFloat ("VelocidadeX", charController.velocity.magnitude);
	}

	void AnimacoesPulando() {

		animator.SetFloat ("Altura", charController.velocity.y);
	}

	void AnimacoesAgachado() {

		animator.SetBool ("Agachado", estaAgachado);
	}

	void AnimacoesFPS()
	{
		if (seMovendo) {
			arma.GetComponent<Animator> ().SetBool ("Andando", true);
		} else {
			arma.GetComponent<Animator> ().SetBool ("Andando", false);
		}
	}

    void Pegadas() {

        if (charController.isGrounded == true && seMovendo && audioSrc.isPlaying == false) 
        {
            audioSrc.volume = Random.Range(0.04f, 0.1f);
            audioSrc.pitch = Random.Range(0.8f, 1.1f);
            audioSrc.Play();
        }
    }

    [Command]
    void CmdDano(GameObject inimigo)
    {
        inimigo.GetComponent<VidaJogador>().TomarDano(ga.dano);
    }

    void Dano()
    {
        if(ga.bala.transform.tag == "inimigo") 
        {
            CmdDano(ga.bala.transform.gameObject);
        }
    }
}
