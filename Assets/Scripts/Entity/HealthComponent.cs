using UnityEngine;
using System.Collections;

public class HealthComponent : MonoBehaviour {
    public int health = 100;
    [HideInInspector]
    public int originalHealth;
    public int resistance = 0;
	// Use this for initialization
	void Start () {
        originalHealth = health;
	}
	
	void Update () {
        
	}

    public void Damage(int howMuch)
    {
        if ((howMuch - resistance) > 0 && health > 0)
        {
            health -= (howMuch - resistance);
        }
    }
}
