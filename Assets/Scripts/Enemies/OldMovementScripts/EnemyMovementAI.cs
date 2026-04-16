using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementAI : MonoBehaviour
{
    public Transform playerTransform;
    public float speed = 2.0f;
    public float raycastDistance = 1.0f;
    public LayerMask obstacleLayer;
    public float obstacleAvoidanceDistance = 1.0f;
    public int angle = 1;
    public float timer;
    public float checkInterval;
    public bool countdown;
    public Vector2 direction;
    public int angleInterval;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        if(countdown == true)
        {
            timer += Time.deltaTime;
            if(timer > checkInterval)
            {
                angle = 1;
                timer = 0;
                countdown = false;
            }
        }
    
    }

    private void FixedUpdate()
    {
        // Move towards the player
        direction = (playerTransform.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        // Check for obstacles in front and to the sides
        RaycastHit2D hitFront = Physics2D.Raycast(transform.position, direction, raycastDistance, obstacleLayer);

        // If there's an obstacle in front, try to avoid it
        if (hitFront.collider != null)
        {   
            Vector2 normal = hitFront.normal;
            Vector2 leftDirection = Quaternion.Euler(0, 0, angle) * direction;
            Vector2 rightDirection = Quaternion.Euler(0, 0, -angle) * direction;

            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, leftDirection, raycastDistance, obstacleLayer);
            RaycastHit2D hitRight = Physics2D.Raycast(transform.position, rightDirection, raycastDistance, obstacleLayer);
            // If we can turn left or right, do so
            if (hitLeft.collider == null)
            {
                countdown = true;
                 //if angle direction and left direction > 90 travel in direction of left direction else perpendicular
                if(Vector3.Angle(leftDirection, direction) > 90)
                {
                    direction = leftDirection;
                }
                else
                {
                    float sign = Mathf.Sign(normal.y * leftDirection.x - normal.x * leftDirection.y);
                    Vector2 perpendicular = new Vector2(normal.y, -normal.x) * sign;
                    direction = perpendicular;
                }
            }
            else if (hitRight.collider == null)
            {
                countdown = true;
                //if angle direction and left direction > 90 travel in direction of left direction else perpendicular
                if(Vector3.Angle(rightDirection, direction) > 90)
                {
                    direction = rightDirection;
                }
                else
                {
                    float sign = Mathf.Sign(normal.y * rightDirection.x - normal.x * rightDirection.y);
                    Vector2 perpendicular = new Vector2(normal.y, -normal.x) * sign;
                    direction = perpendicular;
                }
            }
            // Otherwise, try to follow the obstacle around
            else
            {
                angle += angleInterval;
                raycastDistance += 0.05f;
            }
        }
        else
        {
            angle = 1;
            raycastDistance = 1f;
        }
        

        // Set the new velocity
        rb.linearVelocity = direction * speed;
    }


    private void OnDrawGizmos()
    {
        RaycastHit2D hitFront = Physics2D.Raycast(transform.position, direction, raycastDistance, obstacleLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, angle) * direction, raycastDistance, obstacleLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -angle) * direction, raycastDistance, obstacleLayer);

        if (hitFront.collider != null)
        {
            Debug.DrawLine(transform.position, hitFront.point, Color.red);
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + (Vector3) direction * raycastDistance, Color.green);
        }

        if (hitLeft.collider != null)
        {
            Debug.DrawLine(transform.position, hitLeft.point, Color.red);
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(0, 0, angle) * direction * raycastDistance, Color.green);
        }

        if (hitRight.collider != null)
        {
            Debug.DrawLine(transform.position, hitRight.point, Color.red);
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(0, 0, -angle) * direction * raycastDistance, Color.green);
        }
    }
}
