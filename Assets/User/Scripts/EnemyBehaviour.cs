using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

//Visibility and distance between object - add to the cost term
//Normalise values between 0 and 1
//Find good range of values of detection range and speed - try and bring all of them between 0 and 1

public class EnemyBehaviour : MonoBehaviour
{
    private NavMeshAgent enemyAgent;
    private float enemyPlayerDistance;

    public float enemyRangeON;
    public float enemySpeedON;
    public float enemyGapON;

    public Transform playerTarget;
    public Transform zoneRender;
    public GameObject overseer;

    public Collider[] hitColliders;
    public LayerMask layermask;

    private void Awake()
    {
        overseer = GameObject.Find("overseer");
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        //zoneRender.localScale = new Vector3(enemyRangeON, enemyRangeON, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.Find("Player(Clone)"))
        {
            playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        }
        else
        {
            playerTarget = null;
        }

        hitColliders = Physics.OverlapSphere(this.transform.position, enemyRangeON, layermask, QueryTriggerInteraction.UseGlobal);

        if (hitColliders.Length != 0)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {

                if (hitColliders[i].gameObject.tag == "Player")
                {
                    enemyAgent.transform.LookAt(playerTarget);
                    enemyAgent.SetDestination(playerTarget.position);
                    enemyAgent.speed = enemySpeedON;
                }
            }
        }

        //if(Vector3.Distance(enemyAgent.transform.position, playerTarget.position) < 0.1f)
        //{
        //    print("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.tag == "Player" && collision.gameObject.GetComponent<PlayerControls>().invincible == false)
        {
            //print("here");
            GameObject.Destroy(collision.gameObject.gameObject);
            GameObject.Destroy(this.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, enemyRangeON);
    }
}
