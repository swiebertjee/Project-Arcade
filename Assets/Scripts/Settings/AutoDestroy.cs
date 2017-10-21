﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField]
    private float _destroyTime;
    private float _timer = 0;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > _destroyTime)
            Destroy(gameObject);
    }
}
