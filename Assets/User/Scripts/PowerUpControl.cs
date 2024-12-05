using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerUpControl : MonoBehaviour
{
    public GameManager gameManagerScript;

    public bool selectThis = false;
    public bool VisibleToPlayer = false;

    RaycastHit hitInfo;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameObject.Find("overseer").GetComponent<GameManager>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gameManagerScript = GameObject.Find("overseer").GetComponent<GameManager>();

        if(gameManagerScript.activePlayer != null)
        {
            if (Physics.Linecast(transform.position, gameManagerScript.activePlayer.transform.position, out hitInfo))
            {
                if (hitInfo.transform == gameManagerScript.activePlayer.transform)
                {
                    Debug.DrawLine(transform.position, gameManagerScript.activePlayer.transform.position);
                    VisibleToPlayer = true;
                    gameManagerScript.visiblePowerUps.Add(this.gameObject);
                    gameManagerScript.visiblePowerUps = gameManagerScript.visiblePowerUps.Distinct().ToList();
                    //print(transform.name + " Visible");
                }
                else
                {
                    VisibleToPlayer = false;
                    gameManagerScript.visiblePowerUps.Remove(this.gameObject);
                    //print(transform.name + " NOT Visible");

                }
            }
        }    
    }

    private void OnTriggerEnter(Collider other)
    {
        selectThis = true;
        gameManagerScript.selectedPowerUp = this.gameObject;

        if ((other.transform.tag == "Player") && (other.GetComponent<PlayerControls>().canCollect == true))
        {

            if(other.GetComponentInChildren<Light>().intensity <= 5)
            {
                other.GetComponentInChildren<Light>().intensity += 1;
                gameManagerScript.currentPoints += 5;
            }
            else
            {
                other.GetComponentInChildren<Light>().intensity = 6;
                gameManagerScript.burstReady = true;
            }

            gameManagerScript.UpdatePowerUpPos();

            if (selectThis == true && VisibleToPlayer == true)
            {
                gameManagerScript.UpdatePowerUpPos();
                selectThis = false;
            }
        }        
    }
}
