using System.Linq;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;
    string[] tags = {"Stairs", "Floor", "Goal"};
    void Update()
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
                        _player.GetComponent<PathManager>()
                            .NavigateTo(hit.transform.parent.Find("Waypoint").GetComponent<Waypoint>());
                    }
                }
        }
    }
}
