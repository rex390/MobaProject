using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMinionBullet : MonoBehaviour {

    int damage;
    GameObject target;
    GameObject minionShooting;
	// Use this for initialization
	void Start () {
		
	}
	public void Initialise(int damageOfBullet,GameObject targetOfBullet,GameObject minionWhoShot)
    {
        damage = damageOfBullet;
        target = targetOfBullet;
        minionShooting = minionWhoShot;
        InvokeRepeating("BulletCode", 0.0f, 0.01f);
    }

    void BulletCode()
    {
        if (target != null)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, 1.0f * Time.deltaTime);
        }
        else if (target == null)
        {
            Destroy(this.gameObject);
        }
    }
    void OnDestroy()
    {
        if(minionShooting!=null)
        minionShooting.GetComponent<MinionCode>().TurnOffFire();
    }
    void OnTriggerEnter(Collider col)
    {
        if (target != null)
        {
            if (col.gameObject == target.gameObject)
            {
                col.gameObject.GetComponent<MinionCode>().DecreaseHealth(damage);
                Destroy(this.gameObject);
            }
        }


    }
}
