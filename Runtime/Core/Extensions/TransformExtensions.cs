using UnityEngine;

namespace BaseTool
{
    public static class TransformExtensions
    {
        public static void Clear(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}