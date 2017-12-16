using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GerenciadorDeArmas : MonoBehaviour {

	public float distancia = 100f;
	public int balasPorPente = 30;
	public int balasDisparadas;
	public int balasRestantes;
	public float taxaDeDisparo = 0.1f;
	public float contador;
	public Transform pontoRayCaster;

	// Use this for initialization
	void Start () {
		balasRestantes = balasPorPente;
	}
	
	// Update is called once per frame
	void Update () {
		Disparo ();
	}

	void Disparo()
	{
		if (Input.GetButton ("Fire1")) {
			Tiro ();
		}

		if (contador < taxaDeDisparo) {
			contador += Time.deltaTime;
		}
	}

	void Tiro()
	{
		if (contador < taxaDeDisparo) {
			return;
		}

		RaycastHit bala;

		if (Physics.Raycast (pontoRayCaster.position, pontoRayCaster.transform.forward, out bala, distancia)) {
			Debug.Log ("Tocou em: " + bala.transform.name);
		}

		contador = 0.0f;
	}
}
