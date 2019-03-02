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
            for (int y = -1; y <= 1; y += 2)
            {
                if (Physics.Raycast(transform.position, new Vector3(x, y, 0), out hit, 10))
                {
                    if(hit.transform.tag == "Waypoint")
                        Neighbors.Add(hit.transform.gameObject.GetComponent<Waypoint>());
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
