using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Box : MonoBehaviour
{
    private Vector3 destination;
    private float moveTimeCurrent;
    private float moveTimeTotal = 1;

    private void Start()
    {
        destination = transform.position;
    }
    
    private void Update()
    {
        if (transform.position != destination)
        {
            moveTimeCurrent += Time.deltaTime;
            if (moveTimeCurrent > moveTimeTotal)
                moveTimeCurrent = moveTimeTotal;
            transform.position = Vector3.Lerp(transform.position, destination, moveTimeCurrent / moveTimeTotal);
        }
        else
            moveTimeCurrent = 0;
        
        if(!Physics.Raycast(transform.position, new Vector3(0, -1, 0), 1))
            destination = new Vector3(transform.position.x, transform.position.y - 1,
                transform.position.z);
        
    }

    private void OnCollisionEnter(Collision col)
    {       
        if (col.transform.tag == "Player")
        {
            if (transform.position.z != col.transform.position.z)
            {       
                int direction = transform.position.z > col.transform.position.z ? 1 : -1;

                if (!Physics.Raycast(transform.position, new Vector3(0, 0, direction), 1))
                {
                    destination = new Vector3(transform.position.x, transform.position.y,
                        transform.position.z + direction);
                }
            }
            else if (transform.position.x != col.transform.position.x)
            {                             
                int direction = transform.position.x > col.transform.position.x ? 1 : -1;

                if (!Physics.Raycast(transform.position, new Vector3(direction, 0, 0), 1))
                {
                    destination = new Vector3(transform.position.x + direction, transform.position.y,
                        transform.position.z);
                }
            }           
        }  
    }
}
