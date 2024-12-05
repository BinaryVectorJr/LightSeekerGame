using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    public GameManager gameManagerScript;
    public GameObject playerDeathParticleFX;
    private GameObject tempFX;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameManagerScript = GameObject.Find("overseer").GetComponent<GameManager>();
    }

    //Hack to not spawn inside obstacles
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            tempFX = Instantiate(playerDeathParticleFX, other.transform.position, Quaternion.identity);
            GameObject.Destroy(other.gameObject);
            GameObject.Destroy(tempFX, 2.0f);

            gameManagerScript.state = GameManager.GameState.END;
        }
        else
        {
            GameObject.Destroy(other.gameObject);
        }
    }
}
