using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public List<Waypoint> Neighbors;
    
    public Waypoint Previous { get; set; }
    
    public float Distance { get; set; }

    public bool Enabled;
    
    private void Start()
    {
        var parentTag = transform.parent.tag;
           
        Enabled = true;
        if (Physics.Raycast(transform.position, new Vector3(0, 1, 0), out var hit, 10))
        {
            if (hit.transform.parent != null)
            {
                string[] tags = {"Stairs", "Floor", "Goal"};

                if (tags.Contains(hit.transform.parent.tag))
                    Enabled = false;
            }
        }

        if (parentTag == "Stairs")
        {
            RaycastHit[] hits;
            
            for (var i = 0; i < 2; i++)
            {
                hits = Physics.RaycastAll(transform.position, new Vector3(i == 0 ? 1 : -1, i == 0 ? -1 : 0, 0), 1);
                
                for (int b = 0; b < hits.Length; b++)
                {
                    if (hits[b].transform.gameObject != transform.parent.gameObject)
                    {
                        if (hits[b].transform.parent.childCount > 0)
                        {
                            var waypoint = hits[b].transform.parent.Find("Waypoint");
                            if (waypoint != null)
                                Neighbors.Add(waypoint.GetComponent<Waypoint>());

                            waypoint.GetComponent<Waypoint>().Neighbors.Add(this);
                        }
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
            Debug.Log("hit " + hit.transform.tag);
            if (tags.Contains(hit.transform.tag))
            {
                Debug.Log("OH YEAH");
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
                Gizmos.DrawLine(transform.position + Vector3.up, neighbor.transform.position + Vector3.up);
        }
    }
}
