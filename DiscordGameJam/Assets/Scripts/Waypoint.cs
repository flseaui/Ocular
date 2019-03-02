using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public List<Waypoint> Neighbors;
    
    public Waypoint Previous { get; set; }
    
    public float Distance { get; set; }

    private void Start()
    {
        RaycastHit hit;
        
        for (int x = -1; x <= 1; x += 2)
        {
            for (int z = -1; z <= 1; z += 2)
            {
                if (Physics.Raycast(transform.position, new Vector3(x, 0, z), out hit, 10))
                {
                    Debug.Log("hit " + hit.transform.name);
                    if (hit.transform.childCount > 0)
                    {
                        var waypoint = hit.transform.GetChild(0);
                        if (waypoint.CompareTag("Waypoint"))
                            Neighbors.Add(waypoint.GetComponent<Waypoint>());
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Neighbors == null)
            return;
        Gizmos.color = new Color(0f, 0f, 0f);
        foreach (var neighbor in Neighbors)
        {
            if (neighbor != null)
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
        }
    }
}
