using UnityEngine;
using System.Collections;

public class BasicSideToSideEnemy : EnemyBase {
    //move speed of enemy
    public float moveSpeed = 5f;

    public override void Start () {
        base.Start();
        velocity.x = moveSpeed;
    }
	
    public override void resetEnemy()
    {
        base.resetEnemy();
        velocity.x = moveSpeed;
    }

	void Update () {
        //reset
        if (GlobalVars.resetTimer > 0)
        {
            resetEnemy();
        }
        if (!GlobalVars.paused)
        {
            //gravity
            velocity.y -= 1f;
            if (!dead)
            {
                if (velocity.x > moveSpeed)
                {
                    velocity.x = Mathf.Lerp(velocity.x, moveSpeed, Time.deltaTime * 2f);
                }
                if (velocity.x < -moveSpeed)
                {
                    velocity.x = Mathf.Lerp(velocity.x, moveSpeed, Time.deltaTime * 2f);
                }
                if (myController.collisions.left)
                {
                    velocity.x = moveSpeed;
                }
                if (myController.collisions.right)
                {
                    velocity.x = -moveSpeed;
                }
            }
            else
            {
                velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * 2f);
                velocity.y = 0;
            }
            if (healthComponent.health <= 0)
            {
                dead = true;
            }
            myController.Move(velocity * Time.deltaTime);
            velocity.x = Mathf.Clamp(velocity.x, -maxVelocity.x, maxVelocity.x);
            velocity.y = Mathf.Clamp(velocity.y, -maxVelocity.y, maxVelocity.y);
        }
    }
}
