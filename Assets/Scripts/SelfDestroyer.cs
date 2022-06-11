using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyer : MonoBehaviour
{
    [SerializeField] private float _delay;

    private void Start()
    {
        Destroy(gameObject, _delay);
    }
}
