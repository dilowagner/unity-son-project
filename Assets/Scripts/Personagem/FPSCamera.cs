using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamera : MonoBehaviour {

	// Vars de Eixos
	public enum EixosDeRotacao {MouseX, MouseY}
	public EixosDeRotacao eixos = EixosDeRotacao.MouseY;

	// Vars de Sens
	private float sensXSet = 1.5f;
	private float sensYSet = 1.5f;
	private float sensX = 1.5f;
	private float sensY = 1.5f;

	private float sensMouse = 1.5f;

	// Vars de Angulos
	private float rotacaoX, rotacaoY;

	// Vars de Limite
	private float maximumX = 360f;
	private float minimumX = -360f;
	private float maximumY = 60f;
	private float minimumY = -60f;

	// Vars de Rotacao
	private Quaternion rotacao;

	// Use this for initialization
	void Start () {
		
		rotacao = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
