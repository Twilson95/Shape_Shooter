using UnityEngine;
using System.Collections;
// using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    public float initialSpeed = 5f;
    public float speed = 5f; // the speed at which the enemy moves
    public float bossSpeedMultiplier = 1f;
    public float raycastDistance = 10f; // the distance that the raycasts will travel
    public float raycastAngleStep = 10f; // the angle between each raycast
    public float recalculateInterval = 5f; // the time interval for recalculating the path

    private Transform player; // a reference to the player object
    private LevelManager levelManager;
    private Vector2 targetPosition; // the position of the target (either the player or the obstacle edge)
    public bool hasObstacle; // a flag indicating whether an obstacle is in the way
    public LayerMask obstacleLayer;
    public bool pastBinaryDirection = false; //left is 0, right is 1
    public Vector2 pastDirection;
    public bool isFrozen;


    void Awake()
    {
        // StartCoroutine(FindLevelManager());
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // find the player object in the scene
        InvokeRepeating("RecalculatePath", 0f, recalculateInterval);
        // levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    void OnEnable() 
    {
        StartCoroutine(ResetStatsCoroutine());
        // ResetStats();
    }

    IEnumerator ResetStatsCoroutine()
    {
        while(levelManager == null)
        {
            // yield return new WaitForSeconds(0.1f);
            yield return null;
        }
        ResetStats();
    }

    void FixedUpdate()
    {
        if(isFrozen)
        {
            return;
        }
        HandleMotion();
    }

    private void HandleMotion()
    {
        if(!hasObstacle)
        {
            targetPosition = player.position;
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, (Vector2) (player.position - transform.position), Vector2.Distance(transform.position, player.position), obstacleLayer);
            if(hit.collider == null)
            {
                hasObstacle = false;
            }
        }
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.fixedDeltaTime); // move the enemy towards the target
        // pastDirection = targetPosition;
        if((Vector2) transform.position == targetPosition)
        {
            RecalculatePath();
            // pastDirection = (Vector2) player.position;
        }
    }

    public IEnumerator FreezeMovement(float delayTime)
    {
        isFrozen = true;
        yield return new WaitForSeconds(delayTime);
        isFrozen = false;
    }

    // private void Update()
    // {
    //     if(!hasObstacle)
    //     {
    //         targetPosition = player.position;
    //     }
    //     else
    //     {
    //         RaycastHit2D hit = Physics2D.Raycast(transform.position, (Vector2) (player.position - transform.position), Vector2.Distance(transform.position, player.position), obstacleLayer);
    //         if(hit.collider == null)
    //         {
    //             hasObstacle = false;
    //         }
    //     }
    //     transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime); // move the enemy towards the target
    //     // pastDirection = targetPosition;
    //     if((Vector2) transform.position == targetPosition)
    //     {
    //         RecalculatePath();
    //         // pastDirection = (Vector2) player.position;
    //     }
    // }

    private void RecalculatePath()
    {
        Vector2 direction = (player.position - transform.position).normalized; // the direction towards the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastDistance, obstacleLayer); // check if there is an obstacle in the way
        if (hit.collider != null)
        {
            Vector2 obstacleEdge1 = FindObstacleEdge(hit.point, direction, 1); // the first edge of the obstacle
            Vector2 obstacleEdge2 = FindObstacleEdge(hit.point, direction, -1); // the second edge of the obstacle
            
            RaycastHit2D secondHit1 = Physics2D.Raycast(obstacleEdge1, (Vector2) player.position - obstacleEdge1, 1, obstacleLayer);
            RaycastHit2D secondHit2 = Physics2D.Raycast(obstacleEdge2, (Vector2) player.position - obstacleEdge2, 1, obstacleLayer);
            
            //if both directions are clear, choose shortest path
            if(secondHit1.collider == null && secondHit2.collider == null && 
                Vector2.Angle(obstacleEdge1, (Vector2) player.position) < 135 && 
                Vector2.Angle(obstacleEdge2, (Vector2) player.position) < 135)
            {

                float distance1 = Vector2.Distance(transform.position, obstacleEdge1) + Vector2.Distance(obstacleEdge1, player.position); // the distance of the first path
                float distance2 = Vector2.Distance(transform.position, obstacleEdge2) + Vector2.Distance(obstacleEdge2, player.position); // the distance of the second path
                
                if(distance1 < distance2)
                {
                    targetPosition = obstacleEdge1;
                    pastBinaryDirection = false;
                }
                else
                {
                    targetPosition = obstacleEdge2;
                    pastBinaryDirection = true;
                }
                // targetPosition = (distance1 < distance2) ? obstacleEdge1 : obstacleEdge2; // choose the shorter path
            }
            //if from the left direction a path to the player is clear go that way
            else if(secondHit1.collider == null && secondHit2.collider != null && Vector2.Angle(obstacleEdge1, (Vector2) player.position) < 135)
            {   
                targetPosition = obstacleEdge1;
                pastBinaryDirection = false;
            }
            //if from the right direction a path to the player is clear go that way
            else if(secondHit2.collider == null && secondHit1.collider != null && Vector2.Angle(obstacleEdge2, (Vector2) player.position) < 135)
            {
                targetPosition = obstacleEdge2;
                pastBinaryDirection = true;
            }
            //if both directions don't lead to the player then continue in the same direction as before
            else 
            {
                //pick same direction as before
                if(pastBinaryDirection == false)
                {
                    targetPosition = obstacleEdge1;
                }
                else
                {
                    targetPosition = obstacleEdge2;
                }
            }
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
        float sideRaycastDistance = raycastDistance;
        while ((hit = Physics2D.Raycast(transform.position, Quaternion.AngleAxis(sign * angle, Vector3.forward) * direction, sideRaycastDistance, obstacleLayer)).collider != null && angle < 360f)
        {
            distance = Vector2.Distance(transform.position, hit.point);
            
            edgePoint = hit.point;
            angle += raycastAngleStep;

            Debug.DrawLine(edgePoint, edgePoint + (Vector2) (Quaternion.AngleAxis(sign * angle, Vector3.forward) * direction) * sideRaycastDistance, Color.yellow);

            sideRaycastDistance = distance + 0.5f;
        }
        edgePoint = transform.position + Quaternion.AngleAxis(sign * (angle + 5), Vector3.forward) * direction * (distance + 0.5f);
        Debug.DrawLine(transform.position, edgePoint, Color.blue);
        return edgePoint;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = hasObstacle ? Color.red : Color.black; // visualize the obstacle with a red color
        Gizmos.DrawLine(transform.position, transform.position + ((Vector3) targetPosition - transform.position).normalized * raycastDistance); // draw a line towards the target
    }

    private void ResetStats()
    {
        isFrozen = false;
        float speedScaler = 1.0f + (levelManager.level - 1) * 0.05f;
        speed = initialSpeed * speedScaler;
    }

    public void SetBoss()
    {
        speed *= bossSpeedMultiplier;
    }

}