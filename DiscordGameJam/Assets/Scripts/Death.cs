using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Death : MonoBehaviour
{
    private void OnTriggerStay(Collider col)
    {
        Debug.Log("FUCK");
        
        string[] tags = {"Stairs", "Floor", "Goal"};

        if (tags.Contains(col.transform.parent.tag))
            LevelManager.Instance.RestartLevel();
    }
}
