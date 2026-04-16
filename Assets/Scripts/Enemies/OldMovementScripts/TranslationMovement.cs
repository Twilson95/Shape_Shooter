using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslationMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        // transform.Translate(direction * speed * Time.deltaTime);
        Vector3 direction3d = new Vector3(direction.x, 0, direction.y);
        transform.position += direction3d * speed * Time.deltaTime;
    }
}
