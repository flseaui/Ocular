using UnityEngine;

public class LeftAndRight : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
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
