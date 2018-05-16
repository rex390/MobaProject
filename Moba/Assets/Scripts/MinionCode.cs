using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Threading;


enum Type {Melee,Ranged,Cannon}
public enum Lane { Bot,Mid,Top}
enum State { Move,Attack}

//~(1 << 8) //All but 8

//and you can specify multiple layers

// (1<<8) || (1<<11) // None but 8 and 11

public class MinionCode : MonoBehaviour {

    // Use this for initialization
    string minionSide;
    Type minionType;
    LayerMask EnemylayerMask; //layer for the minion to be looking for
    State minionState; //what is the state of the minion
    float maxHealth = 100.0f;
    float health;
    int damage = 20;
    public GameObject target;
    bool RangedFire = false;
    GameObject healthBar;
    Lane minionLane;
    NavMeshAgent agent;
    Transform goalPos;
    Vector3 targetPosition;
    public int startPath;

    //TEST LEADER PART and position list
    public GameObject leader;
    LayerMask TeamlayerMask; //layer for the minion to be looking for his team
    bool findNewLeader = false;
    List<Transform> pathfindingList = new List<Transform>();
    public void SetLeader(GameObject leaderOfMinion)
    {
        leader = leaderOfMinion;
    }

    public void SetPathList(List<Transform> listPath)
    {
        pathfindingList = listPath;
    }
    void Start()
    {
        string thisLane = this.name.Remove(0, this.name.IndexOf(' ') + 1);
        if (thisLane == "Mid")
        {
            minionLane = Lane.Mid;
        }
        else if (thisLane == "Bot")
        {
            minionLane = Lane.Bot;
        }
        else if (thisLane == "Top")
        {
            minionLane = Lane.Top;
        }
        healthBar = this.gameObject.transform.GetChild(0).GetChild(0).gameObject;
        health = maxHealth;
        minionState = State.Move; //set it to be moving at start
        minionSide = this.name.Remove(this.name.IndexOf(' ')); //set the side of the minion by cutting part of the original gameobjectse name
        if (minionSide == "Red")
        {
            startPath = 3;
            targetPosition = GameObject.Find("BlueInhibitor").transform.position;
            EnemylayerMask = 1 << LayerMask.NameToLayer("Blue"); //searching only for layers which is blue and ignores rest
            TeamlayerMask = 1 << LayerMask.NameToLayer("Red"); //searching only for layers which is blue and ignores rest
        }
        if (minionSide == "Blue")
        {
            startPath = 0;
            targetPosition = GameObject.Find("RedInhibitor").transform.position;
            EnemylayerMask = 1 << LayerMask.NameToLayer("Red"); //searching only for layers which is red and ignores rest
            TeamlayerMask = 1 << LayerMask.NameToLayer("Blue"); //searching only for layers which is blue and ignores rest
        }
        //setting up navigation and set up the starting path to take
        agent = GetComponent<NavMeshAgent>();
        goalPos = pathfindingList[startPath];

        if(leader.gameObject == this.gameObject)
        {
            GetComponent<NavMeshAgent>().enabled = true;
            agent.destination = goalPos.position;         
        }
        //set a invoke to check if enemies are within range
        InvokeRepeating("EnemyInVision", 0.0f, 0.1f);
    }

    void Update()
    {
        if(leader!= this.gameObject)
        {
            if(leader ==null)
            {
                if(findNewLeader==false)
                {
                    findNewLeader = true;
                    Invoke("ResetLeader", 0.0f);
                }
                
            }

        }
    }
    void FixedUpdate()
    {
        //does different actions based on the state of the minion
        //if the minion state is move then call the move command
        if(minionState == State.Move)
        {
            PathUpdating();
            if(leader==this.gameObject)
            {               
                //MinionMovement();
            }
            else
            {
                //makes sure we only follow the leader if he is still alive
                if(leader!=null)
                {
                    this.transform.position = Vector3.MoveTowards(this.transform.position, leader.transform.position, 1.0f * Time.deltaTime);
                }                
            }
        }
        //if the minion state is attack then attack
        else if (minionState == State.Attack)
        {
            //if the target is not null then we can attack again
            if(target!=null)
            {
                Attacking();
            }
            //else the target is dead and we change back to moving
            else
            {
                minionState = State.Move;
                if(leader==this.gameObject)
                {
                    this.GetComponent<NavMeshAgent>().enabled = true;
                    agent.destination = goalPos.position;
                }
            }           
        }     
    }
    //when the leader dies
    private void ResetLeader()
    {

            //make a array of colliders and set it to null for start
            Collider[] colliders = null;
            float minDistance;
            float Distance;
            //fill the array of colliders up with all the gameobjects found from the overlapsphere  and only look for ones that are from the enemy layer
            colliders = Physics.OverlapSphere(this.transform.position, 3.0f, TeamlayerMask);
            Distance = Vector3.Distance(this.transform.position, targetPosition); // the distance will be calculated to this minion and target
            minDistance = Distance; //set the minimum distance to the distance found
            leader = this.gameObject; //set the leader for now to this object
           //if object are found then find the distance between them all and the target
            if (colliders.Length > 0)
            {
                for (int colliderArray = 1; colliderArray < colliders.Length; colliderArray++)
                {
                    if(minionLane == colliders[colliderArray].GetComponent<MinionCode>().GetLane())
                    {
                        Distance = Vector3.Distance(colliders[colliderArray].transform.position, targetPosition); // the distance will be calculated to this minion and target
                        if (Distance < minDistance)
                        {
                            minDistance = Distance;
                            leader = colliders[colliderArray].gameObject;
                        }
                    }
                }
            }
        findNewLeader = false;
    }
    //getter for the lane
    public Lane GetLane()
    {
        return minionLane;
    }
    //setter for minion type
    public void SetType(int type_)
    {
        minionType = (Type)type_;
    }
    //Replaced by navmesh movement
    private void PathUpdating()
    {

        //makes the minions go to the right base based on their enemy color 
        float distanceFromPoint = Vector3.Distance(this.transform.position, pathfindingList[startPath].position);
        if(distanceFromPoint<=1.4f)
        {
            //Debug.Log("Reached");
            if (minionSide == "Red")
            {
                startPath--;
            }
            if (minionSide == "Blue")
            {
                startPath++;
            }
            goalPos = pathfindingList[startPath];
            if(leader == this.gameObject)
            {
                agent.destination = goalPos.position;
            }
        }
        //if (minionSide == "Red")
        //{
        //    //agent.destination = goalPos.position;
        //    //this.transform.position = Vector3.MoveTowards(this.transform.position, GameObject.Find("BlueInhibitor").transform.position, 1.0f * Time.deltaTime);
        //}
        //if (minionSide == "Blue")
        //{
        //    //agent.destination = goalPos.position;
        //    //this.transform.position = Vector3.MoveTowards(this.transform.position, GameObject.Find("RedInhibitor").transform.position, 1.0f * Time.deltaTime);
        //}
    }
    //code which runs when the minion has found a target
    private void Attacking()
    {
        //get the distance between the minion position and the target position
        float distance = Vector3.Distance(this.transform.position, target.transform.position);
        //if the minion is melee move him closer to target and then fire the attack across to the target
        if(minionType == Type.Melee)
        {
            if (distance >0.2f)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, 0.5f * Time.deltaTime);
            }
            else if (distance <= 0.2f)
            {
                MeleeAttackSent();
                //Invoke("AttackSent", 0.2f); //acts as the animation delay of a minion attacking
            }
        }
        //if the minion is ranged, move it within ranged attack distance then fire a projectile that will go to the target
        else if (minionType == Type.Ranged)
        {
            if (distance > 1.2f)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, 0.5f * Time.deltaTime);
            }
            else if (distance <= 1.2f)
            {
                //this will stop the minion rapid firing shots which was one of the issues 
                if(RangedFire==false)
                {
                    RangedFire = true;//make it true so minion is shotting correctly
                    InvokeRepeating("RangedAttackSent", 0.0f,10.0f); //make the delay between each shot 10 seconds
                }
            }
        }
    }
    //turns off fire so minion can fire again  ( when the target dies)
    public void TurnOffFire()
    {
        RangedFire = false;
    }
    //creates the ranged bullet and sets the target and the damage and the owner of the shot
    private void RangedAttackSent()
    {
        //for ranged minion then make this bullet
        if(minionType == Type.Ranged)
        {
            GameObject bullet = Instantiate(Resources.Load("Prefabs/Bullet"), this.transform.position, Quaternion.identity, this.transform) as GameObject;
            bullet.name = "RangedShot";
            bullet.AddComponent<RangedMinionBullet>().Initialise(damage,target,this.gameObject);
        }
        
    }
    //attack the target and decrease their health
    private void MeleeAttackSent()
    {
        //if the target is not null then we can lower its hp
        if(target!=null)
        {
            target.GetComponent<MinionCode>().DecreaseHealth(damage);
        }       
    }

    //function which decrease the health 
    public void DecreaseHealth(int damage)
    {
        //if the health is greater than zero then decrease it and also set the new health bar
        if(health>0)
        {
            health -= damage;
            //SET THE NEW LIFE BAR IN UI
            Vector3 scale = healthBar.transform.localScale;
            scale.x = (float)(health / maxHealth);          
            healthBar.transform.localScale = new Vector3(scale.x,scale.y,scale.z);
        }
        //if the new health value is zero then destroy the object
        if(health == 0)
        {
            Destroy(this.gameObject);
        }

    }
    //function which determines if there is any enemies within a range
    private void EnemyInVision()
    {
        //if the minion is not attacking then we are looking for a enemies within a area
        if(minionState!=State.Attack)
        {
            //make a array of colliders and set it to null for start
            Collider[] colliders = null;
            //fill the array of colliders up with all the gameobjects found from the overlapsphere  and only look for ones that are from the enemy layer
            colliders = Physics.OverlapSphere(this.transform.position, 1.5f, EnemylayerMask);
            //if the array is not null then a target has been found
            if(colliders!=null)
            {
                if (colliders.Length > 0)
                {                   
                    minionState = State.Attack; //change state to attack since no enemies anymore ( might be altered to instead check if no enemies are present)
                    GetComponent<NavMeshAgent>().enabled=false;
                    //make it attack the closest target in the array
                    if (minionSide == "Red")
                    {
                        // it is the max side - 1 beacuse max size is out of bound of array and also because the last gameobject in the array is the first target found                
                        target = colliders[colliders.Length - 1].gameObject;
                    }
                    else if (minionSide == "Blue")
                    {
                        // the first target found will be the first gameobject in the array
                        target = colliders[0].gameObject; //setting the first enemy to the target 
                    }                                     
                }
                else
                {
                    //if the the collider is null and the object was attacking then set it back to moving since there are no targets within range
                    if(minionState == State.Attack)
                    {
                        minionState = State.Move;
                        if (leader == this.gameObject)
                        {
                            this.GetComponent<NavMeshAgent>().enabled = true;
                            agent.destination = goalPos.position;
                        }
                    }
                }
            }

        }
    }

}
