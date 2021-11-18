using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotCar : MonoBehaviour {
    public Catmul spline;
    public Catmul[] splines;
    // Use this for initialization
    public float speed = 0.5f;
    public float maxrotationspeed = 0.4f; // radians
    public float angle;
    public float crashLimit = 5.0f;
    public int nextwaypoint = 2;
    public int driveDir = 1;
    public int lane = 1; // 0 = left, 1 = middle, 2 = right
    public int nd;
    public float AB;
    public Vector3 AV;
    public Vector3 BV;
	void Start () {
        Vector3 pos = spline.newPoints[1];  // start one node past the waypoint 
                                            // to avoid triggering
                                            // immediatge lane change
        nextwaypoint = 3;
        gameObject.transform.position = pos;
        Vector3 next = spline.newPoints[3];
        Vector3 direction = next - pos;
        transform.rotation = Quaternion.LookRotation(direction);
        BoidSpawner bs = GetComponent<BoidSpawner>();
        bs.SpawnBoids(5); // spawn here so they start with the leader.
    }
	
	// Update is called once per frame
	void Update () {

        // try to move towards the next point
        // if close enough, select the subsequent point to get to.
        // we also need to know if we've gone to fast for the track.  For now, lets not worry about that, and just
        // animate along it.
        bool needToPickRandomLane = false;
        bool swappedSplines = false;
        Vector3 pos = gameObject.transform.position;
        Vector3 next = getPoint(nextwaypoint);
        Vector3 direction = next - pos;
        angle = Vector3.Angle(direction, gameObject.transform.forward);
        Vector3 newDir = Vector3.RotateTowards(gameObject.transform.forward, direction, maxrotationspeed, 0.0f);
        gameObject.transform.rotation = Quaternion.LookRotation(newDir);
        float distanceToMove = speed * Time.deltaTime;
        gameObject.transform.Translate(Vector3.forward * distanceToMove);

        if (Vector3.Distance(gameObject.transform.position,getPoint(nextwaypoint)) < distanceToMove)
        {
            // am I at a control point as well?
            for (int i = 0; i < spline.points.Length; i++)
            {
                if (Vector3.Distance(spline.newPoints[nextwaypoint], spline.points[i].position) < 0.05f)
                {
                    //Debug.Log("Changing lanes");
                    needToPickRandomLane = true;
                    foreach (Catmul s in splines)
                    {
                        if (s == spline)
                        {
                            continue;
                        }
                        // it's one of the other two splines. 
                        for (int j = 0; j < s.points.Length; j++)
                        {
                            if (Vector3.Distance(spline.points[i].position, s.points[j].position) < 0.05f)
                            {
                                Debug.Log("At an intersection");
                                needToPickRandomLane = false; // not at intersection

                                if (lane == 1) // middle
                                {
                                    break; // can stop checking
                                }
                                // swap the spline
                                // first though, we need to find our new direction.
                                AV = spline.points[(i + 1) % spline.points.Length].position - spline.points[i].position;
                                BV = s.points[(j + 1) % s.points.Length].position - s.points[j].position;
                                AB = Vector3.SignedAngle(AV, BV, Vector3.up);
                                nd = driveDir; // new direction
                                // 8 if statements
                                if (lane == 0) // left
                                {
                                    if (driveDir == 1)
                                    {
                                        if (AB < 0)
                                        {
                                            nd = 1;
                                        }
                                        else
                                        {
                                            nd = -1;
                                        }
                                    }
                                    else
                                    {
                                        if (AB < 0)
                                        {
                                            nd = 1;
                                        }
                                        else
                                        {
                                            nd = -1;
                                        }
                                    }
                                }
                                else
                                {
                                    if (driveDir == 1)
                                    {
                                        if (AB < 0)
                                        {
                                            nd = -1;
                                        }
                                        else
                                        {
                                            nd = 11;
                                        }
                                    }
                                    else
                                    {
                                        if (AB < 0)
                                        {
                                            nd = -1;
                                        }
                                        else
                                        {
                                            nd = 1;
                                        }
                                    }
                                }
                                driveDir = nd;
                                spline = s;
                                swappedSplines = true;
                                // reset nextwaypoint properly factoring in direction
                                nextwaypoint = (j - 1 + s.points.Length) % s.points.Length * (int)s.amountOfPoints+driveDir;
                                if (nextwaypoint >= spline.newPoints.Count)
                                {
                                    nextwaypoint = 0;
                                }
                                else if (nextwaypoint < 0)
                                {
                                    nextwaypoint = spline.newPoints.Count - 1;
                                }
                                break;
                            }
                        }
                        if (swappedSplines == true)
                        {
                            break;
                        }
                    }
                }
            }
            if (needToPickRandomLane == true)
            {
                lane = Random.Range(0, 3);
            }
            nextwaypoint += driveDir;
            if (nextwaypoint >= spline.newPoints.Count)
            {
                nextwaypoint = 0;
                AdjustSpeed();
            }
            else if (nextwaypoint < 0)
            {
                nextwaypoint = spline.newPoints.Count-1;
                AdjustSpeed();
            }
        }
        
    }

    Vector3 getPoint(int index)
    {
        if (lane == 0)
        {
            return spline.leftPoints[index];
        }
        else if (lane == 1)
        {
            return spline.newPoints[index];
        }
        else
        {
            return spline.rightPoints[index];
        }
    }

    void AdjustSpeed()
    {
        speed *= 1.1f;
        if (speed > 20.0f)
        {
            speed = 20.0f;
        }

    }
}
