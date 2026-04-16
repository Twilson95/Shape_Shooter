using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillation : MonoBehaviour
{
    public float amplitude = 1f;
    public float frequency = 1f;
    public float rotationSpeed = 90f;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    public Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.localPosition;
        player = GameObject.FindWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 perpendicular = new Vector2(direction.y, -direction.x);

        float oscillationX = Mathf.Sin(Time.time * frequency) * amplitude;
        // Vector2 oscillationVector = new Vector2(oscillationX, 0f);

        Vector2 parentLocation = new Vector2(transform.parent.position.x, transform.parent.position.z);
        
        // rb.MovePosition(parentLocation + perpendicular * oscillationX);
        // rb.MoveTowards(parentLocation + perpendicular * oscillationX, Time.deltaTime * speed);
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, perpendicular * oscillationX, Time.deltaTime * frequency);

    }
}
