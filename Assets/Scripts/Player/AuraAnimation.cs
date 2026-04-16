using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraAnimation : MonoBehaviour
{
    private Vector3 scaleVector = new Vector3(1,1,1);
    public float maxScale = 1f;
    public float amplitude = 1f;
    public float offset = 0.5f;
    public float degreesPerSecond = 15f;
    public float frequency;
    public Vector3 rotateDirection;

    void Start()
    {
        
    }


    void Update()
    {
        transform.Rotate(rotateDirection * degreesPerSecond * Time.deltaTime);

        transform.localScale = scaleVector * amplitude * (1 + Mathf.Sin((Time.time + offset) * frequency));
    }
}
