using UnityEngine;

public class Shift : MonoBehaviour
{
    public void DisableMe()
    {
        gameObject.SetActive(false);
    }

    public void FadeOut()
    {
        GetComponent<Animator>().SetTrigger("FadeOut");
    }
}
