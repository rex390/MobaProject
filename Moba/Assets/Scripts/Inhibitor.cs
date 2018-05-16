using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LaneRoute
{
    List<Transform> Lane;

    public void Init()
    {
        Lane = new List<Transform>();
    }
    public void AddLane(Transform lane_)
    {
        Lane.Add(lane_);
    }
    public List<Transform> GetLane()
    {
        return Lane;
    }
}

public class Inhibitor : MonoBehaviour {

    // Use this for initialization
    int counter = 1; //represent which wave ( every 3 is cannon)
    int hp = 1000; //represent the inhibitor total hp (test value is 100 for now)
    List<GameObject> spawnPoints; //the points the minions will spawn from
    GameObject[] leaders = new GameObject[3] ;
    float MinionSpawnStart = 5.0f;
    float MinionSpawnRepeatDelay = 20.0f;
    //array List to hold all the routes for a lane 
    LaneRoute[] Lanes = new LaneRoute[3];
    //Lanes[0].
    void Start ()
    {
        //just setting the color correct for inhibitor based on name
        if(this.name == "BlueInhibitor")
        {
            this.gameObject.GetComponent<Renderer>().material.color = Color.blue;
        }
        //initialise the list 
        spawnPoints = new List<GameObject>();
        //make a array of all the spawn point
        GameObject[] items = GameObject.FindGameObjectsWithTag("SpawnPoint");
        //go through the array of spawn points
        foreach (GameObject item in items)
        {
            //if the spawn point parent is this inhibitor then add it to the list
            if(item.transform.parent.gameObject == this.transform.gameObject)
            {
                spawnPoints.Add(item.gameObject);
            }
        }

        //Set up the list of Paths CONTINUE ON FROM HERE FIX THE LANES ROUTE BEING INTIALISED
        int laneCount = 0;
        GameObject turretLane = GameObject.Find("TopPath");
        SetUpLane(turretLane, laneCount);
        laneCount++;
        turretLane = GameObject.Find("BotPath");
        SetUpLane(turretLane, laneCount);
        laneCount++;
        turretLane = GameObject.Find("MidPath");
        SetUpLane(turretLane, laneCount);
        //invoke repeat the code to start spawning the minions after 5 seconds and repeat every 5 seconds       
        InvokeRepeating("StartMinionCreation", MinionSpawnStart, MinionSpawnRepeatDelay);
    }

    // Set up lane variables
    void SetUpLane(GameObject turretLane,int laneCount_)
    {
        Lanes[laneCount_].Init();
        for (int countVar = 0; countVar < turretLane.transform.childCount; countVar++)
        {
            Lanes[laneCount_].AddLane(turretLane.transform.GetChild(countVar).transform);
        }
    }
    private void StartMinionCreation()
    {
        StartCoroutine(SpawnMinion("Top"));
        StartCoroutine(SpawnMinion("Bot"));
        StartCoroutine(SpawnMinion("Mid"));
        //controls when the counter should reset or if it should increment
        if (counter == 3)
        {
            counter = 1;
        }
        else
        {
            counter++;
        }
        

        
    }

    private void CreateMinion(int minionNumber, GameObject spawnPoint,int minionSpawnNumber, string minionLane)
    {
        int counter = 0;
        string Side = this.name.Remove(this.name.IndexOf('I'), 9);
        GameObject minion = Instantiate(Resources.Load("Prefabs/Minion"), spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform) as GameObject;

        //set up the pathfinding route minions will take
        if(minionLane=="Top")
        {
            minion.GetComponent<MinionCode>().SetPathList(Lanes[0].GetLane());
        }
        else if(minionLane=="Bot")
        {
            minion.GetComponent<MinionCode>().SetPathList(Lanes[1].GetLane());
        }
        else if(minionLane == "Mid")
        {
            minion.GetComponent<MinionCode>().SetPathList(Lanes[2].GetLane());
        }
        //make its name equal to the side it is on
        minion.name = Side + " " + spawnPoint.name;
        //change color to the correct side color
        if (Side == "Blue")
        {
            minion.GetComponent<Renderer>().material.color = Color.blue;
            minion.layer = 9; //the 9th layer is blue side ( allow for the minion to know who their allies and enemies are)
        }
        else
        {
            minion.layer = 8; //the 8th layer is red side ( allow for the minion to know who their allies and enemies are)
        }
        //CLEAN THIS UP BUT IT SETS What type of minion this will be from 0 = melee 1= ranged 2 = cannon;
        if (minionNumber < 3)
        {
            minion.GetComponent<MinionCode>().SetType(0);

        }
        else if (minionNumber < 6)
        {
            minion.GetComponent<MinionCode>().SetType(1);
        }
        else if (minionNumber == 6)
        {
            minion.GetComponent<MinionCode>().SetType(2);
        }
        if(minionSpawnNumber==0)
        {
            counter = 0;
            foreach(GameObject leaderOfLane in leaders)
            {
                if(leaders[counter]==null)
                {
                    leaders[counter] = minion;
                    minion.GetComponent<MinionCode>().SetLeader(minion);
                    break;
                }
                else
                {
                    counter++;
                } 
                      
            }         
        }
        if(minionSpawnNumber > 0)
        {
            counter = 0;
            foreach(GameObject leaderOfLane in leaders)
            {
                if (leaders[counter].name == minion.name)
                {
                    minion.GetComponent<MinionCode>().SetLeader(leaders[counter]);
                }
                counter++;
            }           
        }
    }
    IEnumerator SpawnMinion(string Lane)
    {
        //yield return new WaitForSeconds(3.0f);
        //this represents how many minions are in the wave
        int Wave;
        //if it is not the third wave then there are 6 minions in the wave else spawn a cannon minion alongside the 6 minions 
        if(counter<3 )
        {
            Wave = 6;
        }
        else
        {
            Wave = 7;
        }
        //go through each spawn point and spawn a minion 
        foreach (GameObject spawnPoint in spawnPoints)
        {
            //spawn set number of minion based on which wave it is
            if(spawnPoint.name == Lane)
            {
                //go through a for loop for how many minions there are in this wave
                 for(int minionNumber =0;minionNumber < Wave; minionNumber++)
                {                    
                    //spawn a minion
                    CreateMinion(minionNumber, spawnPoint,minionNumber,Lane);
                    yield return new WaitForSeconds(0.5f); //delay the next spawn of minion by 0.5 seconds
                }      
            }                     
        }
        //we set all the leaders up correctly and also have gave every minion a leader to follow so now we can clean them up
        leaders = null;
        leaders = new GameObject[3];

    }

    void OnCollisionEnter(Collision col)
    {
        if(this.name == "RedInhibitor")
        {
            if(col.gameObject.tag == "Blue")
            {
                Debug.Log("inhibitor damaged");
            }
        }
        if (this.name == "BlueInhibitor")
        {
            if (col.gameObject.tag == "Red")
            {
                Debug.Log("inhibitor damaged");
            }
        }
    }
}
