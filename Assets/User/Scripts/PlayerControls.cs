using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float distance = 0.5f;

    public GameManager gameManagerScript;
    public bool useInitialCameraDistance = false;
    public float range = 0;
    public bool canCollect = false;
    public bool invincible = false;
    public bool bursting = false;
    public bool move = true;

    public Collider[] enemyColliders;
    public LayerMask layermask;

    public Animator lightAnimator;
    public Animation lightBurst;
    public GameObject deathParticleFX;

    private float actualDistance;
    private GameObject tempFX;

    // Start is called before the first frame update
    void Start()
    {
        if(useInitialCameraDistance)
        {
            Vector3 toObjectVector = transform.position - Camera.main.transform.position;
            Vector3 linearDistanceVector = Vector3.Project(toObjectVector, Camera.main.transform.forward);
            actualDistance = linearDistanceVector.magnitude;
        }
        else
        {
            actualDistance = distance;
        }

        canCollect = true;
    }

    // Update is called once per frame
    void Update()
    {
        gameManagerScript = GameObject.Find("overseer").GetComponent<GameManager>();

        if(move == true)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = actualDistance;
            transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        }

        range = this.GetComponentInChildren<Light>().range;

        enemyColliders = Physics.OverlapSphere(this.transform.position, (range + 1), layermask, QueryTriggerInteraction.UseGlobal);

        if (gameManagerScript.burstReady == false)
        {
            gameManagerScript.activePlayer.GetComponentInChildren<Light>().color = Color.white;
        }
        else
        {
            gameManagerScript.activePlayer.GetComponentInChildren<Light>().color = Color.yellow;
            canCollect = false;
        }


        if (Input.GetMouseButtonDown(1) && gameManagerScript.burstReady == true)
        {
            //print("here");
            canCollect = false;
            Explode();
        }

        //if (enemyColliders.Length != 0)
        //{
        //    for (int i = 0; i < enemyColliders.Length; i++)
        //    {
        //        if (enemyColliders[0].gameObject.tag == "Enemy" && bursting == true)
        //        {
        //            tempFX = Instantiate(deathParticleFX, enemyColliders[0].transform.position, Quaternion.identity);

        //            GameObject.Destroy(enemyColliders[0].gameObject);
        //            GameObject.Destroy(tempFX, 2.0f);
        //        }
        //    }
        //}
    }

    public void RightClickUp()
    {
        lightAnimator.SetBool("RightClicked", false);
        canCollect = true;
        bursting = false;
    }

    void Explode()
    {
        enemyColliders = Physics.OverlapSphere(this.transform.position, range/5, layermask, QueryTriggerInteraction.UseGlobal);

        gameManagerScript.activePlayer.GetComponentInChildren<Light>().intensity = 1;
        gameManagerScript.burstReady = false;

        lightAnimator.SetBool("RightClicked", true);

        if (lightAnimator.GetBool("RightClicked") == true)
        {
            lightBurst.Play();
            Invoke("RightClickUp", 1.0f);
            bursting = true;
        }

        foreach (Collider enemy in enemyColliders)
        {
            if(enemy.gameObject.transform.tag == "Enemy")
            {
                tempFX = Instantiate(deathParticleFX, enemy.transform.position, Quaternion.identity);
                gameManagerScript.currentPoints += 10;

                GameObject.Destroy(enemy.gameObject);
                GameObject.Destroy(tempFX, 2.0f);
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 9)
        {
            move = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, range/5);
    }
}
