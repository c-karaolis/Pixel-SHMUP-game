using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowAnimation : MonoBehaviour
{
    public bool horizontal;
    Sequence mySequence;
    // Start is called before the first frame update
    void Start()
    {
    }

   public void StartRowAnimation()
    {
        if (horizontal)
        {
            TweenHorizontal();
        }
        else
        {
            TweenVertical();
        }
    }

    void TweenHorizontal()
    {
        mySequence = DOTween.Sequence();

        mySequence.Append(transform.DOMoveX(-0.2f, 1, true))
          .Append(transform.DOMoveX(0.4f, 1, true));
        mySequence.SetLoops(-1);
    }

    void TweenVertical()
    {
        mySequence = DOTween.Sequence();
        float transformY = transform.position.y;
        mySequence.Append(transform.DOMoveY(transformY - 0.3f,1f, false))
          .Append(transform.DOMoveY(transformY, 1f, false));
        mySequence.SetLoops(-1,LoopType.Restart);
    }

}
