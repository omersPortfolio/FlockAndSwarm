using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Flock MyFlock;
    public float Speed;
    public float TurnSpeed;
    public float NeighborRadius;
    public float FOV;
    public Vector3 Dir;
    public Vector3 alignment;
    public Vector3 coherence;
    public Vector3 avoidance;
    public int NeighborCount;
    public float AvoidDistance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TurnSpeed = MyFlock.TurnSpeed;
        Speed = MyFlock.Speed;
        // use motivations to update direction
        alignment = Vector3.zero;
        coherence = Vector3.zero;
        avoidance = Vector3.zero;
        NeighborCount = 0;
        foreach (Boid b in MyFlock.boids)
        {
            if (b == this)
            {
                continue;
            }
            if (Vector3.Distance(transform.position,b.transform.position)> NeighborRadius)
            {
                continue;
            }
            if (Vector3.Angle(transform.forward,b.transform.position-transform.position)> FOV)
            {
                continue;
            }
            NeighborCount++;
            alignment += b.transform.forward;
            coherence += b.transform.position;
            if (Vector3.Distance(transform.position, b.transform.position) < AvoidDistance)
            {
                avoidance += transform.position - b.transform.position;
            }
        }
        if (NeighborCount == 0)
        {
            // give up and go home;
            Debug.Log("heading home");
            Dir = MyFlock.transform.position - transform.position;
        }
        else
        {
            alignment = alignment.normalized;
            avoidance = avoidance.normalized;
            coherence = coherence / NeighborCount;
            coherence -= transform.position;
            coherence = coherence.normalized;
            Vector3 ran = UnityEngine.Random.insideUnitSphere;
            Vector3 SwarmDir = MyFlock.target.transform.position - transform.position;
            Dir = (SwarmDir *0.2f + ran + avoidance*1.5f + coherence + 0.36f* alignment).normalized;

        }
    }

    private void LateUpdate()
    {
        // turn towards direction
        Quaternion facing = Quaternion.LookRotation(Dir, transform.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, facing, TurnSpeed * Time.deltaTime);
        // move forward
        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    internal void SetFlock(Flock flock)
    {
        MyFlock = flock;
        Speed = flock.Speed;
        TurnSpeed = flock.TurnSpeed;
        NeighborRadius = flock.NeighborRadius;
        FOV = flock.FOV;
        AvoidDistance = flock.AvoidDistance;
    }
}
