using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    // Use this for initialization
    GameObject timer;
    int unitSeconds, tenSeconds, unitMins, tenMins;
    float counter;
    void Start ()
    {
        counter = 0;
        unitSeconds = 0;
        tenSeconds = 0;
        unitMins = 0;
        tenMins = 0;
        timer = GameObject.FindGameObjectWithTag("Timer").GetComponentInChildren<Text>().gameObject;
	}
	
	// Update is called once per frame
	void Update ()
    {
        timer.GetComponent<Text>().text = tenMins.ToString() + unitMins.ToString() + ":" + tenSeconds.ToString() + unitSeconds.ToString();
        Timing();
    }

    void Timing()
    {
        
        counter += Time.deltaTime;
        if(counter>9)
        {
            tenSeconds++;
            counter = 0;
        }
        if(tenSeconds>=6)
        {
            unitMins++;
            tenSeconds = 0;
        }
        if(unitMins>9)
        {
            tenMins++;
            unitMins = 0;
        }
        unitSeconds = (int)counter ;

    }
}
