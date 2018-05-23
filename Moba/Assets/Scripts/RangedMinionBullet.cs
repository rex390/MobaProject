using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMinionBullet : MonoBehaviour {

    int damage; // the damage of this bullet ( since different minion but have different strength of bullets)
    GameObject target; //the target the bullet is going for
    GameObject minionShooting; // the minio that is shooting it
	// Intialise all the variables
	public void Initialise(int damageOfBullet,GameObject targetOfBullet,GameObject minionWhoShot)
    {
        damage = damageOfBullet;
        target = targetOfBullet;
        minionShooting = minionWhoShot;
        InvokeRepeating("BulletCode", 0.0f, 0.01f);
    }
    // code that is looped for the bullets
    void BulletCode()
    {
        //if the target is not dead then move towards the target
        if (target != null)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, 1.0f * Time.deltaTime);
        }
        //if the target is dead then destroy the bullet
        else if (target == null)
        {
            Destroy(this.gameObject);
        }
    }
    //checks if target is destroyed and if so then stop firing 
    void OnDestroy()
    {
        if(target==null)
        {
            if(minionShooting != null)
            {
                minionShooting.GetComponent<MinionCode>().TurnOffFire();
            }
        }
    }
    //when the bullet collides with the enemy then destroy bullet which also remove the bullet script
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
