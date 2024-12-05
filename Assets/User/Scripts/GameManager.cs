using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gmInstance;

    public enum GameState { MAINMENU, ACTIVE, END };

    [Header("Game Settings")]
    public GameState state;
    public GameObject HUDText;
    public GameObject endCanvasObj;
    public GameObject endCanvasText;
    public int currentPoints = 0;

    [Header("Script & GameObject References")]
    //Use this script to contain references to all scripts. Then refer to this script to work between values - like a connecting bridge
    public GameObject playerObj;
    public GameObject enemyObj;
    public PlayerControls playerControlScript;
    public GameObject activePlayer;
    public GameObject activeEnemyGO;
    public GameObject activePowerUpGO;
    public GameObject powerUp;
    public GameObject selectedPowerUp;
    public Boundaries boundaryScript;
    //public GameObject prevEnemyGO;

    [Header("Lists")]
    public List<GameObject> activeEnemies = new List<GameObject>();
    public List<GameObject> activePowerUps = new List<GameObject>();
    public List<GameObject> visiblePowerUps = new List<GameObject>();

    [Header("Difficulty Settings")]
    [Range(0.1f, 1)]
    public float targetDifficulty = 1;
    public float maxRandSpeed = 10.0f;
    public float maxRandDetectionRange = 10.0f;
    public float maxEnemyGap = 10.0f;
    public float currentDifficulty = 0;
    public float proposedDifficultyPos = 0;
    public float proposedDifficulty = 0;

    //public float targetPowerDifficulty = 10;
    //public float currentPowerDifficulty = 0;
    //public float proposedPowerDifficultyPos = 0;

    [Header("Enemy Settings")]
    //public float enemyRange;
    //public float enemySpeed;
    public float enemyGap;
    private float PrandX;
    private float PrandZ;
    private Vector3 enemyRandPos;
    private Vector3 prevEnemyPos;
    private float randSpeed;
    private float randDetectionRange;
    private float randDistBetween;
    [Range(0, 10)]
    public int totalEnemies = 0;

    [Header("Power Settings")]
    private float randX;
    private float randZ;
    private Vector3 powerRandPos;
    private Vector3 powerPosVector;
    private Vector3 prevPos;
    public float powerGap;
    private int visScore;

    [Header("Simulation Settings")]
    public bool burstReady;
    public float thresholdVal = 0.0f;
    public float randomWeight = 0.0f;
    public bool annealingAccept = false;

    // Singleton Game Manager
    private void Awake()
    {
        if (gmInstance != null && gmInstance != this)
        {
            Destroy(this);
        }
        else
        {
            gmInstance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        state = GameState.ACTIVE;

        activePlayer = Instantiate(playerObj, new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity) as GameObject;

        if(activePlayer != null)
        {
            boundaryScript = activePlayer.GetComponent<Boundaries>();

            for (int i = 0; i < totalEnemies; i++)
            {
                activeEnemyGO = Instantiate(enemyObj, enemyRandPos, Quaternion.identity) as GameObject;
                activeEnemies.Add(activeEnemyGO);
            }

            Optimize(activeEnemies);

            playerControlScript = activePlayer.GetComponent<PlayerControls>();

            prevPos = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (state == GameState.ACTIVE)
        {
            HUDText.GetComponent<Text>().text = currentPoints.ToString();

            if (activePowerUps.Count < 3)
            {
                SpawnPowerUps();
            }

            selectedPowerUp = activePowerUps[Random.Range(0, activePowerUps.Count)];

            //to remove entry in list, if the object has been destroyed
            for (int i = activeEnemies.Count-1; i>=0; i--)
            {
                if(activeEnemies[i] == null)
                {
                    activeEnemies[i] = activeEnemies[activeEnemies.Count - 1];
                    activeEnemies.RemoveAt(activeEnemies.Count - 1);
                }
            }

            for (int i = activePowerUps.Count - 1; i >= 0; i--)
            {
                if (activePowerUps[i] == null)
                {
                    activePowerUps[i] = activePowerUps[activePowerUps.Count - 1];
                    activePowerUps.RemoveAt(activePowerUps.Count - 1);
                }
            }

            for (int i = visiblePowerUps.Count - 1; i >= 0; i--)
            {
                if (visiblePowerUps[i] == null)
                {
                    visiblePowerUps[i] = activePowerUps[visiblePowerUps.Count - 1];
                    visiblePowerUps.RemoveAt(visiblePowerUps.Count - 1);
                }
            }

            visScore = visiblePowerUps.Count;
        }
        else if(state == GameState.END)
        {
            endCanvasObj.SetActive(true);

            endCanvasText.GetComponent<Text>().text = HUDText.GetComponent<Text>().text.ToString();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

    }

    void SpawnPowerUps()
    {
        PrandX = Random.Range(-10.0f, 10.0f);
        PrandZ = Random.Range(-10.0f, 10.0f);
        powerPosVector = new Vector3(PrandX, 0.5f, PrandZ);

        for(int i=0; i<100; i++)
        {
            powerGap = Random.Range(0.0f, 10.0f);
        }

        if (Vector3.Distance(powerPosVector, prevPos) > powerGap)
        {
            activePowerUpGO = Instantiate(powerUp, powerPosVector, Quaternion.identity) as GameObject;
            activePowerUps.Add(activePowerUpGO);

            prevPos = powerPosVector;
        }
    }

    public void UpdatePowerUpPos()
    {
        //print(selectedPowerUp.name + " || " + selectedPowerUp.transform.position);
        print("Update run");

        //selectedPowerUp.GetComponent<PowerUpControl>().selectThis = true;

        PrandX = Random.Range(-10.0f, 10.0f);
        PrandZ = Random.Range(-10.0f, 10.0f);
        powerPosVector = new Vector3(PrandX, 0.5f, PrandZ);

        for (int i = 0; i < 100; i++)
        {
            powerGap = Random.Range(0.1f, 10.0f);
        }

        if (Vector3.Distance(powerPosVector, prevPos) > powerGap)
        {
            selectedPowerUp.transform.position = powerPosVector;
        }
    }

    //Define probablity that accepts the values; say for first 200 it there is 50% probability to accept a badly optimizzed value
    //For the next 300, we accept 25% of bad scores; After 500 it (final nos) we accept 100% of good scores.
    //Accept if its smaller or the probability tells you to accept
    //Randomly enerate probabilities

    void Optimize(List<GameObject> enemyList)
    {
        //print(enemyList.Count);

        for (int k = 0; k < enemyList.Count; k++)
        {
            for (int i = 0; i < 500; i++)
            {
                if(i >= 0 && i < 200)
                {
                    thresholdVal = 50.0f;
                }
                else if(i > 200 && i <= 400)
                {
                    thresholdVal = 75.0f;
                }
                else if(i > 400)
                {
                    thresholdVal = 100.0f;
                }

                randomWeight = Random.Range(0.0f, 100.0f);

                if(randomWeight < thresholdVal)
                {
                    annealingAccept = true;
                }
                else
                {
                    annealingAccept = false;
                }    

                randSpeed = Random.Range(1.0f, maxRandSpeed);
                randDetectionRange = Random.Range(1.0f, maxRandDetectionRange);
                enemyGap = Random.Range(0.0f, maxEnemyGap);

                randX = Random.Range(-10.0f, 10.0f);
                randZ = Random.Range(-10.0f, 10.0f);
                proposedDifficultyPos = Mathf.Abs(randX) + Mathf.Abs(randZ);

                proposedDifficulty = (randSpeed / 10.0f) + (randDetectionRange / 10.0f) + (proposedDifficultyPos / 40) + (powerGap / 10.0f) + (1 - (visScore/3));
                //print(proposedDifficulty);

                if ((Mathf.Abs(targetDifficulty - proposedDifficulty) < Mathf.Abs(targetDifficulty - currentDifficulty)) || (annealingAccept == true))
                {
                    currentDifficulty = proposedDifficulty;

                    print(Mathf.Abs(targetDifficulty - proposedDifficulty));

                    //Adjust locations of enemy spawns - higher difficulty means more spread out with larger detection range.
                    enemyList[k].transform.position = new Vector3(randX, 0.5f, randZ);

                    enemyList[k].GetComponent<EnemyBehaviour>().enemySpeedON = randSpeed;
                    enemyList[k].GetComponent<EnemyBehaviour>().enemyRangeON = randDetectionRange;

                    //break;
                }

                if (Vector3.Distance(enemyList[k].transform.position, prevEnemyPos) > enemyGap)
                {
                    enemyList[k].transform.position = new Vector3(randX, 0.5f, randZ);

                    prevEnemyPos = enemyList[k].transform.position;
                }

            }
        }
    }

    public void powerDistanceCheck(Vector3 offset)
    {
        activePowerUpGO = Instantiate(powerUp, offset, Quaternion.identity) as GameObject;

        activePowerUps.Add(activePowerUpGO);
            
        for(int i=0; i<activePowerUps.Count - 1; i++)
        {
            if(Vector3.Distance(activePowerUps[i].transform.position, activePowerUps[i+1].transform.position) < 2.0f)
            {
                activePowerUps[i + 1].transform.position += offset;
                break;
            }
        }    
    }
}
