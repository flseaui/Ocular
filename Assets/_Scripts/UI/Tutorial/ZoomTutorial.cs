using UnityEngine;

public class ZoomTutorial : MonoBehaviour
{
    void Update()
    {
        var scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta > 0 || scrollDelta < 0)
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
