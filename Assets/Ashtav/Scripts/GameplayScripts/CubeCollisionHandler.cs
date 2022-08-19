using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCollisionHandler : MonoBehaviour
{
    [SerializeField]
    float collisionForce = 1000.0f;
    Rigidbody rigidbody;
    PhysicMaterial physicMaterial;
    public CubeManager cubeManager;
    public int cubeNumber;
    bool collided = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.mass = 1.5f;
        physicMaterial = GetComponent<BoxCollider>().material;
        physicMaterial.staticFriction = 0.2f;
        physicMaterial.dynamicFriction = 0.2f;
        physicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
        physicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
        physicMaterial.bounciness = 0.1f;

    }

    private void OnCollisionEnter(Collision collision)
    {
        CubeCollisionHandler cubeCollisionHandler = collision.gameObject.GetComponent<CubeCollisionHandler>();

        if (cubeCollisionHandler == null)
        {
            return;
        }

        ChangePhysicsAttributes();

        if (cubeNumber == cubeCollisionHandler.cubeNumber && !collided)
        {
            collided = true;
            cubeManager.OnSameNumberedCubeCollision(gameObject, cubeNumber);
        }
        else
        {
            // check if gameover

            if (transform.position.z < 0.5f)
            {
                // game over

                ScoreManager.Instance.ShowGameOver();
            }
        }
    }

    public void ChangePhysicsAttributes()
    {
        rigidbody.mass = 7;
        physicMaterial.staticFriction = 10;
        physicMaterial.dynamicFriction = 10;
        physicMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
        physicMaterial.bounciness = 0.1f;
        physicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
    }

    public void HopOnToNextTarget()
    {
        float distance = 100;
        Vector3 nearestCubePosition = Vector3.zero;

        // get the nearest cube with same number
        for (int i = 0; i < cubeManager.cubes.Count; i++)
        {
            if (cubeManager.cubes[i].cubeNumber == cubeNumber)
            {
                float tempDistance = Vector3.Distance(cubeManager.cubes[i].transform.position, transform.position);
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    nearestCubePosition = cubeManager.cubes[i].transform.position;
                }
            }
        }

        if (distance != 100)
        {
            Vector3 moveDirection = nearestCubePosition - transform.position;

            // reset the transform
            transform.eulerAngles = Vector3.zero;
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;

            // limiting the x and z values so we get a nice jump to nearest cube
            moveDirection = LimitVector3(moveDirection, 1.0f, 1.0f);

            // jump towards the nearest cube

            // multiplyind by distance helps to reach the nearest cube more effectively
            moveDirection = moveDirection * distance * 0.25f;
            // when distance is too great, we get a big jump so limit that
            if (moveDirection.y > 2.5f)
            {
                float factor = 2.5f / moveDirection.y;
                moveDirection.y = moveDirection.y * factor;
                moveDirection.x = moveDirection.x * factor;
                moveDirection.z = moveDirection.z * factor;
            }
            rigidbody.AddForce(moveDirection * collisionForce);

            // rotate the cube
            moveDirection = LimitVector3(moveDirection, 1, 1);
            StartCoroutine(ApplyRotation(moveDirection));

        }
        else
        {
            // jump upwards
            rigidbody.AddForce(Vector3.up * collisionForce * 2.5f);

            // create a random direction to rotate
            Vector3 moveDirection = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            //moveDirection = LimitVector3(moveDirection, 0.5f, 0.5f, 0.5f);

            // rotate the cube
            StartCoroutine(ApplyRotation(moveDirection));

            // save this cube to a list
            cubeManager.cubes.Add(this);
        }

    }

    Vector3 LimitVector3(Vector3 moveDirection, float x, float z)
    {
        // if one axis is limited the next axis should be also be proportionately reduced
        // this is so that the accurate direction will be preserved.

        float largestAxisValue = Mathf.Max(Mathf.Abs(moveDirection.x), Mathf.Abs(moveDirection.z));

        if (Mathf.Abs(moveDirection.x) > x)
        {
            moveDirection.x = (Mathf.Abs(moveDirection.x) / largestAxisValue) * x * Mathf.Sign(moveDirection.x);
        }

        if (Mathf.Abs(moveDirection.z) > z)
        {
            moveDirection.z = (Mathf.Abs(moveDirection.z) / largestAxisValue) * z * Mathf.Sign(moveDirection.z);
        }

        moveDirection.y = (Mathf.Max(Mathf.Abs(moveDirection.x), Mathf.Abs(moveDirection.z))) * 2.5f;

        return moveDirection;

    }

    IEnumerator ApplyRotation(Vector3 dir)
    {
        yield return new WaitForSeconds(0.1f);
        rigidbody.AddTorque(-dir * collisionForce * 0.5f);
    }

    public void SetCombinedCubeAttributes()
    {
        StartCoroutine(SetCombined());
    }

    IEnumerator SetCombined()
    {
        yield return new WaitForEndOfFrame();
        ChangePhysicsAttributes();
        HopOnToNextTarget();
    }
}
