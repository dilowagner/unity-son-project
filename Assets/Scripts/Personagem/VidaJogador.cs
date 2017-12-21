using UnityEngine;
using UnityEngine.Networking;

public class VidaJogador : NetworkBehaviour
{
    [SyncVar]
    public float vida = 100f;

    private NetworkStartPosition[] spawnPoints;

    void Start()
    {
        if(isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();    
        }
    }

    public void TomarDano(float dano)
    {
        if(! isServer)
        {
            return;
        }

        vida -= dano;

        Debug.Log("Tomou Dano");

        if(vida <= 0f) 
        {
            Vector3 ponto = Vector3.zero;
            if(spawnPoints != null && spawnPoints.Length > 0) 
            {
                ponto = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;   
            }

            transform.position = ponto;
            vida = 100f;
        }
    }
}
