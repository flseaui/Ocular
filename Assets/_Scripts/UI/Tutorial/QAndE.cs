using UnityEngine;

public class QAndE : MonoBehaviour
{
    private bool _q, _e;
    private bool _fading;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            _q = true;
        if (Input.GetKeyDown(KeyCode.E))
            _e = true;
        
        if (_q && _e && !_fading)
        {
            _fading = true;
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
