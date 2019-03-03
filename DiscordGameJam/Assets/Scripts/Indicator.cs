using System.Linq;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public GameObject Player;
    
    string[] tags = {"Stairs", "Floor", "Goal"};

    private void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            if (hit.transform.parent != null)
                if (tags.Contains(hit.transform.parent.tag))
                {
                    transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y + .5f,
                        hit.transform.position.z);
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (Player != null)
                        {
                            Player.GetComponent<PathManager>()
                                .NavigateTo(hit.transform.parent.Find("Waypoint").GetComponent<Waypoint>());
                        }
                    }
                }
        }
    }
}
