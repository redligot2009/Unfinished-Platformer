using UnityEngine;
using System.Collections;

public class Jeep : MonoBehaviour
{

    Rigidbody2D myRigidBody;
    public Vector2 velocity;
    public Vector2 maxVelocity;
    public Vector2 trueMaxVelocity;
    Vector2 originalMaxVelocity;
    public Vector3 angleVelocity;
    public float maxAngleVelocity;
    public Vector3 angle;
    public float movementSpeed = 0.5f;
    // Use this for initialization
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        originalMaxVelocity = maxVelocity;
        myRigidBody.centerOfMass = new Vector2(0, transform.FindChild("pivot").GetComponent<Transform>().position.y);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        myRigidBody.AddForce(transform.up * velocity.y, ForceMode2D.Force);
        //transform.eulerAngles = angle;
        myRigidBody.AddTorque(angleVelocity.z, ForceMode2D.Force);
        //myRigidBody.MoveRotation(angle.z);
        angle.z += angleVelocity.z;
        if (Input.GetKey(KeyCode.W))
        {
            velocity.y += movementSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            velocity.y -= movementSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            angleVelocity.z += 0.5f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            angleVelocity.z -= 0.5f;
        }
        if (Mathf.Abs(myRigidBody.velocity.y) >= originalMaxVelocity.normalized.y)
        {
            maxVelocity.y += Time.deltaTime * 2f;
        }
        else
        {
            maxVelocity.y = Mathf.Lerp(maxVelocity.y, originalMaxVelocity.y, 0.05f);
        }
        maxVelocity.y = Mathf.Clamp(maxVelocity.y, -trueMaxVelocity.y, trueMaxVelocity.y);
        velocity.y = Mathf.Clamp(velocity.y, -maxVelocity.y, maxVelocity.y);
        angleVelocity.z = Mathf.Clamp(angleVelocity.z, -maxAngleVelocity, maxAngleVelocity);
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            angleVelocity.z = Mathf.Lerp(angleVelocity.z, 0, 0.5f);
        }
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            velocity.y = Mathf.Lerp(velocity.y, 0, Time.deltaTime * 8f);
        }
        //transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + (angleVelocity.z));
        //rotateRigidBodyAroundPointBy(myRigidBody, new Vector3(transform.position.x, transform.position.y - 0.5f), new Vector3(0, 0, 1.0f), myRigidBody.transform.eulerAngles.z);
    }
}
