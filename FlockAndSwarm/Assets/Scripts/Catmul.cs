using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Catmul : MonoBehaviour
{
    public GameObject controlPointPrefab;
    public int splineNumber;
    //Use the transforms of GameObjects in 3d space as your points or define array with desired points
    public Transform[] points;

    //Store points on the Catmull curve so we can visualize them
    public List<Vector3> newPoints = new List<Vector3>();
    // space for the two parallel paths
    public List<Vector3> leftPoints = new List<Vector3>();
    public List<Vector3> rightPoints = new List<Vector3>();
    //How many points you want on the curve
    public float amountOfPoints = 20.0f;

    //set from 0-1
    public float alpha = 0.5f;

    /////////////////////////////
    private void Awake()
    {
        if (splineNumber == 1)
        {
            transform.position = new Vector3(0, 0, 0);
        }
        if (splineNumber == 2)
        {
            transform.position = new Vector3(20.0f, 0, 0);
        }
        if (splineNumber == 3)
        {
            transform.position = new Vector3(20.0f*Mathf.Cos(60*Mathf.Deg2Rad), 0, -20.0f * Mathf.Sin(60 * Mathf.Deg2Rad));
        }
        points = new Transform[12];
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 pos = transform.position;
            pos.x += Mathf.Cos(Mathf.Deg2Rad * 360/ points.Length * i) * 20.0f;
            pos.z += Mathf.Sin(Mathf.Deg2Rad * 360 / points.Length * i) * 20.0f;
            GameObject cp = GameObject.Instantiate(controlPointPrefab, pos, Quaternion.identity);
            points[i] = cp.transform;
        }
        newPoints.Clear();
        for (int i = 0; i < points.Length; i++)
        {
            CatmulRom(i);
        }
        // create the parallel paths
        for (int i = 0; i < newPoints.Count; i++)
        {
            Vector3 p1 = newPoints[i];
            Vector3 p2 = newPoints[(i + 1) % newPoints.Count];
            Vector3 dir = p2 - p1;
            Vector3 left = Vector3.Cross(dir, Vector3.up).normalized * 1.1f;
            Vector3 leftPos = p1 + left;
            Vector3 rightPos = p1 - left;
            leftPoints.Add(leftPos);
            rightPoints.Add(rightPos);
        }
    }
    int getIndex(int index)
    {
        return index % points.Length;
    }

    void CatmulRom(int index)
    {


        Vector3 p0 = points[getIndex(index+0)].position; // Vector3 has an implicit conversion to Vector2
        Vector3 p1 = points[getIndex(index + 1)].position;
        Vector3 p2 = points[getIndex(index + 2)].position;
        Vector3 p3 = points[getIndex(index + 3)].position;

        float t0 = 0.0f;
        float t1 = GetT(t0, p0, p1);
        float t2 = GetT(t1, p1, p2);
        float t3 = GetT(t2, p2, p3);

        for (float t = t1; t < t2; t += ((t2 - t1) / amountOfPoints))
        {
            Vector3 A1 = (t1 - t) / (t1 - t0) * p0 + (t - t0) / (t1 - t0) * p1;
            Vector3 A2 = (t2 - t) / (t2 - t1) * p1 + (t - t1) / (t2 - t1) * p2;
            Vector3 A3 = (t3 - t) / (t3 - t2) * p2 + (t - t2) / (t3 - t2) * p3;

            Vector3 B1 = (t2 - t) / (t2 - t0) * A1 + (t - t0) / (t2 - t0) * A2;
            Vector3 B2 = (t3 - t) / (t3 - t1) * A2 + (t - t1) / (t3 - t1) * A3;

            Vector3 C = (t2 - t) / (t2 - t1) * B1 + (t - t1) / (t2 - t1) * B2;

            if (newPoints.Count == 0 || newPoints[newPoints.Count - 1] != C)
            {
                newPoints.Add(C);
            }
        }
        if (newPoints.Count == 121)
        {
            newPoints.RemoveAt(120);
        }
    }

    float GetT(float t, Vector3 p0, Vector3 p1)
    {
        float a = Mathf.Pow((p1.x - p0.x), 2.0f) + Mathf.Pow((p1.y - p0.y), 2.0f)+ Mathf.Pow((p1.z-p0.z),2.0f);
        float b = Mathf.Pow(a, 0.5f);
        float c = Mathf.Pow(b, alpha);

        return (c + t);
    }

    //Visualize the points
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //foreach (Vector3 temp in newPoints)
        for (int i = 0; i < newPoints.Count;i++)
        {
            Vector3 pos = newPoints[i];
            Vector3 pos2 = newPoints[(i + 1) % newPoints.Count];
            Gizmos.DrawLine(pos,pos2);
        }
        Gizmos.color = Color.green;
        //foreach (Vector3 temp in newPoints)
        for (int i = 0; i < leftPoints.Count; i++)
        {
            Vector3 pos = leftPoints[i];
            Vector3 pos2 = leftPoints[(i + 1) % leftPoints.Count];
            Gizmos.DrawLine(pos, pos2);
        }
        Gizmos.color = Color.blue;
        //foreach (Vector3 temp in newPoints)
        for (int i = 0; i < rightPoints.Count; i++)
        {
            Vector3 pos = rightPoints[i];
            Vector3 pos2 = rightPoints[(i + 1) % rightPoints.Count];
            Gizmos.DrawLine(pos, pos2);
        }
    }
}