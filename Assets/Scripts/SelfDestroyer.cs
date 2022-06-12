using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SelfDestroyer : MonoBehaviour
{
    [SerializeField] private float _delay;
    [SerializeField] private bool _needScaling;

    private void Start()
    {
        if (_needScaling)
        {
            StartCoroutine(Scaling());
        }

        Destroy(gameObject, _delay);
    }

    private void OnDestroy()
    {
        DOTween.CompleteAll();
    }

    private IEnumerator Scaling()
    {
        yield return new WaitForSeconds(_delay - 0.5f);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(1.3f, 0.2f));
        sequence.Append(transform.DOScale(0f, 0.3f));
    }
}
