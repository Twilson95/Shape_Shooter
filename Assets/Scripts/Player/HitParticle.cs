using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitParticle : MonoBehaviour
{
    private ParticleSystem particles;
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer spriteRenderer = transform.parent.gameObject.GetComponent<SpriteRenderer>();
        particles = this.GetComponent<ParticleSystem>();
        particles.Stop();
        var main = particles.main;
        main.startColor = spriteRenderer.color;
    }

    public void StartParticles()
    {
        particles.Play();
    }

    public void EmitParticles(int number)
    {
        particles.Emit(number);
    }

    public void StopParticles()
    {
        particles.Stop();
    }

    public void SetPosition(Vector3 location)
    {
        transform.position = location;
    }
}
