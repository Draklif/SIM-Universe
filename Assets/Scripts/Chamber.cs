using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chamber : MonoBehaviour
{
    public float GravityMag;
    public float FrictionMag;
    public float Steps;

    public GameObject[] Particles;
    public bool isActive = false;
    public bool isSimple = true;

    public void Initialize()
    {
        foreach (GameObject Particle in Particles)
        {
            Particle.SetActive(true);
            Particle.GetComponent<Particle>().RemoteStart();
        }
        isActive = true;
    }

    void Update()
    {
        if (isActive)
        {
            if (isSimple)
            {
                for (int i = 0; i < Particles.Length; i++)
                {
                    Particles[i].GetComponent<Particle>().Shoot(Steps, FrictionMag, GravityMag);
                    for (int j = 0; j < Particles.Length; j++)
                    {
                        if (Particles[i] != Particles[j])
                        {
                            float radA = Particles[i].transform.localScale.x / 2;
                            float radB = Particles[j].transform.localScale.x / 2;
                            float distance = Mathf.Sqrt(
                                Mathf.Pow(Particles[j].transform.position.x - Particles[i].transform.position.x, 2) +
                                Mathf.Pow(Particles[j].transform.position.y - Particles[i].transform.position.y, 2) +
                                Mathf.Pow(Particles[j].transform.position.z - Particles[i].transform.position.z, 2)
                            );
                            if (distance < (radA + radB))
                            {
                                Particles[i].GetComponent<Particle>().ParticleCollide(Particles[j].GetComponent<Particle>());
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Particles.Length; i++)
                {
                    Particles[i].GetComponent<Particle>().Boing(Steps, FrictionMag, GravityMag);
                }
            }
        }
    }
}
