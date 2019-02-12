using System.Collections;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    private Animator anim;
    private bool isFading = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void AnimationComplete()
    {
        isFading = false;
    }

    public IEnumerator FadeToClear()
    {
        isFading = true;
        anim.SetTrigger("FadeIn");
        while (isFading)
        {
            yield return null;
        }
    }

    public IEnumerator FadeToBlack()
    {
        isFading = true;
        anim.SetTrigger("FadeOut");
        while (isFading)
        {
            yield return null;
        }
    }
}
