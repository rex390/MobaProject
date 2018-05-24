using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    Vector3 prevGoalPoint;
    NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("PlayerControls", 0.0f, 0.016667f);
        prevGoalPoint = Vector3.zero;
    }



    void PlayerControls()
    {      
        if (Input.GetMouseButton(0))
        {            
            if(prevGoalPoint!= Input.mousePosition)
            {
                prevGoalPoint = Input.mousePosition;
                //this.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100) != false)
                {
                     agent.destination= hit.point;
                    //Debug.Log(hit.transform.position);
                }
            }
        }
    }

}
