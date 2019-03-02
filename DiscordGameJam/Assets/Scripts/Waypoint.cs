using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public List<Waypoint> Neighbors;
    
    public Waypoint Previous { get; set; }
    
    public float Distance { get; set; }

    public bool Enabled { get; set; }
    
    private void Start()
    {

        string parentTag = transform.parent.tag;
        Debug.Log("YEAH " + parentTag);
        
        
        Enabled = true;
        if (Physics.Raycast(transform.position, new Vector3(0, 1, 0), out var hit, 10))
        {
            string[] tags = {"Stairs", "Floor", "Goal"};
            if (tags.Contains(hit.transform.tag))
            {
                Enabled = false;
            }
        }

        if (parentTag == "Stairs")
        {
            for (int i = 0; i < 2; i++)
            {
                if (Physics.Raycast(transform.position, new Vector3(i == 0 ? 1 : -1, i == 0 ? -1 : 0, 0), out hit, 1))
                {
                    if (hit.transform.parent.childCount > 0)
                    {
                        var waypoint = hit.transform.parent.Find("Waypoint");
                        if (waypoint != null)
                            Neighbors.Add(waypoint.GetComponent<Waypoint>());
                    }
                }
            }
        }
        else
        {
            for (var x = -1; x <= 1; x++)
            {
                for (var z = -1; z <= 1; z++)
                {
                    if (Mathf.Abs(x) != Mathf.Abs(z))
                    {
                        if (Physics.Raycast(transform.position, new Vector3(x, 0, z), out hit, 1))
                        {
                            if (hit.transform.parent.childCount > 0)
                            {
                                var waypoint = hit.transform.parent.Find("Waypoint");
                                if (waypoint != null)
                                    Neighbors.Add(waypoint.GetComponent<Waypoint>());
                            }
                        }
                    }
                }
            }
        }

        transform.Translate(0, 1.5f, 0);
    }

    public void CheckBelow()
    {
        if (Physics.Raycast(transform.position, new Vector3(0, -1, 0), out var hit, 10))
        {
            string[] tags = {"Stairs", "Floor", "Goal"};
            if (tags.Contains(hit.transform.tag))
            {
                hit.transform.parent.Find("Waypoint").GetComponent<Waypoint>().Enabled = true;
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!Enabled)
            return;
        if (Neighbors == null)
            return;
        Gizmos.color = new Color(0f, 0f, 0f);
        foreach (var neighbor in Neighbors)
        {
            if (!neighbor.Enabled) continue;
            if (neighbor != null)
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
        }
    }
}
