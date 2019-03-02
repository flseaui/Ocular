using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public List<Waypoint> Neighbors;
    
    public Waypoint Previous { get; set; }
    
    public float Distance { get; set; }

    private void Start()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, new Vector3(0, 1, 0), out hit, 10))
        {
            string[] tags = {"Stairs", "Floor", "Goal"};
            Debug.Log(hit.transform.tag);
            if (tags.Contains(hit.transform.tag))
            {
                Destroy(gameObject);
            }
        }
     
        for (var x = -1; x <= 1; x++)
        {
            for (var z = -1; z <= 1; z++)
            {
                if (Mathf.Abs(x) != Mathf.Abs(z))
                {
                    if (Physics.Raycast(transform.position, new Vector3(x, 0, z), out hit, 10))
                    {
                        Debug.Log(hit.transform.name);
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

        transform.Translate(0, 1.5f, 0);
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
