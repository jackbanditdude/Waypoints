using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollerScript : MonoBehaviour
{
    public List<WaypointScript> Waypoints = new List<WaypointScript>();
    public float Speed = 1.0f;
    public int DestinationWaypoint = 1;

    private Vector3 Destination;
    private bool Forwards = true;
    private LineRenderer l;
    private float timePassed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //l = this.gameObject.AddComponent<LineRenderer>();
        this.Destination = this.Waypoints[DestinationWaypoint].transform.position;
        l = gameObject.AddComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        StopAllCoroutines();
        StartCoroutine(MoveTo());
        DrawLines();
    }

    void DrawLines()
    {
        List<Vector3> pos = new List<Vector3>();
        pos.Add(transform.position);
        pos.Add(Destination);
        l.startWidth = 0.25f;
        l.endWidth = 0.25f;
        l.SetPositions(pos.ToArray());
        l.useWorldSpace = true;
    }

    IEnumerator MoveTo()
    {
        while ((transform.position - this.Destination).sqrMagnitude > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position,
                this.Destination, this.Speed * Time.deltaTime);
            yield return null;
        }

        if ((transform.position - this.Destination).sqrMagnitude <= 0.01f)
        {
            if (this.Waypoints[DestinationWaypoint].IsSentry)
            {
                while (timePassed < this.Waypoints[DestinationWaypoint].PauseTime)
                {
                    timePassed += Time.deltaTime;
                    yield return null;
                }

                timePassed = 0;
            }

            GetNextWaypoint();
        }
    }

    void GetNextWaypoint()
    {
        if (this.Waypoints[DestinationWaypoint].IsEndpoint)
        {
            if (this.Forwards)
                this.Forwards = false;
            else
                this.Forwards = true;
        }

        if (this.Forwards)
            ++DestinationWaypoint;
        else
            --DestinationWaypoint;

        if (DestinationWaypoint >= this.Waypoints.Count)
            DestinationWaypoint = 0;

        this.Destination = this.Waypoints[DestinationWaypoint].transform.position;
    }
}
