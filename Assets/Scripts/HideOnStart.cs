using UnityEngine;

public class HideOnStart : MonoBehaviour
{
    public void Start()
    {
        gameObject.SetActive(false);
    }
}
