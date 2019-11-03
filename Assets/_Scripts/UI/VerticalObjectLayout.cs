using Misc;
using UnityEngine;

namespace UI
{
    [ExecuteInEditMode]
    public class VerticalObjectLayout : MonoBehaviour
    {
        public void Update()
        {
            if (Application.isPlaying) return;

            var count = 0;
            transform.ForEachChild(child =>
            {
                child.localPosition = new Vector3(0, count * -18, 0);
                count++;
            });
        }
    }
}
