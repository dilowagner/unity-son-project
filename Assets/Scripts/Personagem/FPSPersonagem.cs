﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPersonagem : MonoBehaviour {

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

	// Use this for initialization
	void Start () {

		fpsView = transform.Find ("FPS Visao").transform;
		charController = GetComponent<CharacterController> ();
		velocidade = velocidadeAndando;
		seMovendo = false;

		animator = transform.Find("Modelo").gameObject.GetComponent<Animator>();

		rayDistance = charController.height * 0.5f + charController.radius;
		alturaPadrao = charController.height;
		camPosPadrao = fpsView.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		Movimento ();
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
	}

	void AnimacoesAndando() {
		
		animator.SetFloat ("VelocidadeX", charController.velocity.magnitude);
	}

	void AnimacoesPulando() {

		animator.SetFloat ("Altura", charController.velocity.y);
	}
}
