using UnityEngine;

public class QAndE : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
        {
            FadeOut();
        }
    }

    public void DisableMe()
    {
        gameObject.SetActive(false);
    }

    public void FadeOut()
    {
        GetComponent<Animator>().SetTrigger("FadeOut");
    }
}
