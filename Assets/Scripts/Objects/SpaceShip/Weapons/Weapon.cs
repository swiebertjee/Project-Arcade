﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Weapon : MonoBehaviour
{
    private bool _firing = false;

    public void Fire()
    {
        if (!_firing)
            OnFireEnabled();

        _firing = true;
        OnFire();
    }

    public void Release()
    {
        if (_firing)
            OnFireDisabled();

        _firing = false;
    }

    private void OnDisable()
    {
        Release();
    }

    protected virtual void OnFireEnabled() { }
    protected virtual void OnFire() { }
    protected virtual void OnFireDisabled() { }

    public virtual void Aim(Vector2 movement) { }

    protected bool Firing
    {
        get { return _firing; }
    }
}
