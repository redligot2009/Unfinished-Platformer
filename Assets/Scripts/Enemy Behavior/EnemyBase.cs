using UnityEngine;
using System.Collections;

public abstract class EnemyBase : MonoBehaviour {
    public Controller2D myController;
    public Vector3 velocity;
    public Vector3 maxVelocity;
    public bool dead = false;
    [HideInInspector]
    public HealthComponent healthComponent;

    public virtual void resetEnemy()
    {
        healthComponent.health = healthComponent.originalHealth;
        dead = false;
        myController.ResetPos();
    }

    public virtual void Start () {
        myController = GetComponent<Controller2D>();
        healthComponent = transform.Find("killCollider").GetComponent<HealthComponent>();
    }
}
