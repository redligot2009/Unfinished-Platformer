using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {
    Vector3 targetPos;
    Vector3 velocity;
    Controller2D myController;
    public float speed;
    public float lifeTime;
    public float maxLifeTime = 5f;
    public float flySpeed = 30f;
    public float facing = 1;
    public int damageAmount = 20;
    float newAngle;
    float trueAngle;
    RaycastHit hitInfo;
    public LayerMask collisionLayers;

    GameObject player;
    // Use this for initialization
    void Start () {
        myController = GetComponent<Controller2D>();
        targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        player = GameObject.Find("player");
        if (targetPos.x >= player.transform.position.x)
        {
            newAngle = Mathf.Atan2(targetPos.y - player.transform.position.y, targetPos.x - player.transform.position.x);
            transform.localScale = new Vector3(transform.localScale.x * 1f, transform.localScale.y * 1, transform.localScale.z * 1);
            transform.eulerAngles = new Vector3(0, 0, newAngle * Mathf.Rad2Deg);
        }
        if (targetPos.x < player.transform.position.x)
        {
            newAngle = Mathf.Atan2(player.transform.position.y - targetPos.y, player.transform.position.x - targetPos.x);
            transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y * 1, 1 * transform.localScale.z);
            transform.eulerAngles = new Vector3(0, 0, (newAngle * Mathf.Rad2Deg));
        }
        trueAngle = Mathf.Atan2(targetPos.y - player.transform.position.y, targetPos.x - player.transform.position.x);
        lifeTime = maxLifeTime;
    }

    bool flying = false;
    bool bounced = false;
    // Update is called once per frame
    void Update () {
        if (!GlobalVars.paused)
        {
            if (lifeTime >= 0)
            {
                lifeTime -= Time.deltaTime;
            }
            if (lifeTime <= 0)
            {
                Destroy(gameObject);
            }
            transform.position += velocity * Time.deltaTime;
            if (!flying)
            {
                velocity.x = flySpeed * Mathf.Cos(trueAngle);
                velocity.y = flySpeed * Mathf.Sin(trueAngle);
                flying = true;
            }
            if (bounced)
            {
                transform.eulerAngles += new Vector3(0, 0, 10f * facing);
                velocity.y -= 1f;
            }
            Debug.DrawRay(transform.position, velocity.normalized * 1f);
        }
    }
    
    void FixedUpdate()
    {

    }

    void OnTriggerEnter2D(Collider2D info)
    {
        if (info.gameObject.layer == collisionLayers || info.gameObject.tag == "ground")
        {
            Destroy(gameObject);
        }
        if (info.gameObject.layer == LayerMask.NameToLayer("enemies"))
        {
            EnemyBase basic = info.gameObject.GetComponentInParent<EnemyBase>();
            HealthComponent healthComponent = info.gameObject.GetComponent<HealthComponent>();
            if(basic != null)
            {
                basic.velocity.x += (flySpeed/6)*facing;
            }
            healthComponent.Damage(damageAmount);
            if (!bounced)
            {
                velocity.x *= -0.25f;
                velocity.y *= -0.25f;
                bounced = true;
            }
        }
    }
}
