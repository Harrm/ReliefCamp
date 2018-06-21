using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Refugee.Misc
{
    public class TextFade : MonoBehaviour
    {
        [SerializeField]
        private float timeBeforeFading;
        [SerializeField]
        private float fadeTime;

        private void Start()
        {
            StartCoroutine(Fade());
        }

        private IEnumerator Fade()
        {
            Text text = GetComponent<Text>();
            yield return new WaitForSeconds(timeBeforeFading);
            float fadeStep = fadeTime / 100;
            for(int i = 0; i < 100; i++)
            {
                yield return new WaitForSeconds(fadeStep);
                Color currentColor = text.color;
                currentColor.a -= 1f / 100f;
                text.color = currentColor;
            }
        }
    }
}