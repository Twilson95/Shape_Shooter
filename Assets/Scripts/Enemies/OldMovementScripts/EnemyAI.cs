// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class EnemyAI : MonoBehaviour
// {
//     public Transform playerTransform;
//     public float moveSpeed = 2.0f;
//     public float raycastDistance = 1.0f;
//     public LayerMask obstacleLayer;
//     public float obstacleAvoidanceDistance = 1.0f;

//     private Rigidbody2D rb;

//     private void Start()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         playerTransform = GameObject.FindWithTag("Player").transform;
//     }

//     private void FixedUpdate()
//     {
//         // Move towards the player
//         Vector2 direction = (playerTransform.position - transform.position).normalized;
//         rb.velocity = direction * moveSpeed;

//         // Check for obstacles in front and to the sides
//         RaycastHit2D hitFront = Physics2D.Raycast(transform.position, direction, raycastDistance, obstacleLayer);
//         RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, 45) * direction, raycastDistance, obstacleLayer);
//         RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -45) * direction, raycastDistance, obstacleLayer);

//         // If there's an obstacle in front, try to avoid it
//         if (hitFront.collider != null)
//         {
//             // If we can turn left or right, do so
//             if (hitLeft.collider == null)
//             {
//                 direction = Quaternion.Euler(0, 0, 45) * direction;
//             }
//             else if (hitRight.collider == null)
//             {
//                 direction = Quaternion.Euler(0, 0, -45) * direction;
//             }
//             // Otherwise, try to follow the obstacle around
//             else
//             {
//                 Vector2 leftDirection = Quaternion.Euler(0, 0, 45) * direction;
//                 Vector2 rightDirection = Quaternion.Euler(0, 0, -45) * direction;
//                 float leftDistance = CheckObstacleDistance(transform.position + (Vector3)leftDirection * obstacleAvoidanceDistance);
//                 float rightDistance = CheckObstacleDistance(transform.position + (Vector3)rightDirection * obstacleAvoidanceDistance);
//                 if (leftDistance > rightDistance)
//                 {
//                     direction = leftDirection;
//                 }
//                 else
//                 {
//                     direction = rightDirection;
//                 }
//             }
//         }

//         // Set the new velocity
//         rb.velocity = direction * moveSpeed;
//     }

//     private float CheckObstacleDistance(Vector2 position)
//     {
//         RaycastHit2D hit = Physics2D.Raycast(position, (Vector2) playerTransform.position - position, obstacleAvoidanceDistance, obstacleLayer);
//         if (hit.collider == null)
//         {
//             return obstacleAvoidanceDistance;
//         }
//         else
//         {
//             return hit.distance;
//         }
//     }

//     OnDrawGizmos()
//     {
//         RaycastHit2D hitFront = Physics2D.Raycast(transform.position, direction, raycastDistance, obstacleLayer);
//         RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, 45) * direction, raycastDistance, obstacleLayer);
//         RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, -45) * direction, raycastDistance, obstacleLayer);
//     }
// }

using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 5f; // the speed at which the enemy moves
    public float raycastDistance = 10f; // the distance that the raycasts will travel
    public float raycastAngleStep = 10f; // the angle between each raycast
    public float recalculateInterval = 5f; // the time interval for recalculating the path

    private Transform player; // a reference to the player object
    private Vector2 targetPosition; // the position of the target (either the player or the obstacle edge)
    public bool hasObstacle; // a flag indicating whether an obstacle is in the way
    public LayerMask obstacleLayer;
    // public Vector2 obstacleEdge1;
    // public Vector2 obstacleEdge2;
    // public float distance1;
    // public float distance2;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // find the player object in the scene
        InvokeRepeating("RecalculatePath", 0f, recalculateInterval);
    }

    private void Update()
    {
        if(!hasObstacle)
        {
            targetPosition = player.position;
        }
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime); // move the enemy towards the target
    }

    private void RecalculatePath()
    {
        Vector2 direction = (player.position - transform.position).normalized; // the direction towards the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastDistance, obstacleLayer); // check if there is an obstacle in the way
        if (hit.collider != null)
        {
            Vector2 obstacleEdge1 = FindObstacleEdge(hit.point, direction, 1); // the first edge of the obstacle
            Vector2 obstacleEdge2 = FindObstacleEdge(hit.point, direction, -1); // the second edge of the obstacle
            float distance1 = Vector2.Distance(transform.position, obstacleEdge1) + Vector2.Distance(obstacleEdge1, player.position); // the distance of the first path
            float distance2 = Vector2.Distance(transform.position, obstacleEdge2) + Vector2.Distance(obstacleEdge2, player.position); // the distance of the second path
            targetPosition = (distance1 < distance2) ? obstacleEdge1 : obstacleEdge2; // choose the shorter path
            hasObstacle = true;
        }
        else
        {
            targetPosition = player.position; // move directly towards the player if there is no obstacle
            hasObstacle = false;
        }
    }

    private Vector2 FindObstacleEdge(Vector2 startPoint, Vector2 direction, int sign)
    {
        Vector2 edgePoint = startPoint;
        float angle = 0f;
        RaycastHit2D hit;
        float distance = 1;
        while ((hit = Physics2D.Raycast(transform.position, Quaternion.AngleAxis(sign * angle, Vector3.forward) * direction, raycastDistance, obstacleLayer)).collider != null)
        {
            distance = Vector2.Distance(transform.position, hit.point);
            edgePoint = hit.point;
            angle += raycastAngleStep;
            Debug.DrawLine(edgePoint, edgePoint + (Vector2) (Quaternion.AngleAxis(sign * angle, Vector3.forward) * direction) * raycastDistance, Color.yellow);
        }
        edgePoint = transform.position + Quaternion.AngleAxis(sign * (angle + 5), Vector3.forward) * direction * (distance + 0.5f);
        Debug.DrawLine(startPoint, edgePoint, Color.green);
        return edgePoint;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = hasObstacle ? Color.red : Color.green; // visualize the obstacle with a red color
        Gizmos.DrawLine(transform.position, transform.position + ((Vector3) targetPosition - transform.position).normalized * raycastDistance); // draw a line towards the target
    }

}
