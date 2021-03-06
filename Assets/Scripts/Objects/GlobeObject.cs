﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeObject : MonoBehaviour
{
    [SerializeField]
    protected Vector3 _globePosition = new Vector3();

    private Globe _globe;

    protected virtual void Awake()
    {
        Globe.onGlobeChange += OnGlobeChanged;
        ObjectSafe.onGameStart += OnGameStart;
    }

    private void OnGameStart(ObjectSafe objectSafe)
    {
        objectSafe.Safe(gameObject);
    }

    private void OnGlobeChanged()
    {
        if (!Application.isPlaying)
            GlobePosition = GlobePosition;
    }

    public void SetPosition(Vector3 ScenePosition)
    {
        Vector3 tempPosition = Globe.SceneToGlobePosition(ScenePosition);
        tempPosition.y = GlobePosition.y;
        GlobePosition = tempPosition;
    }

    protected virtual void OnValidate()
    {
        GlobePosition = GlobePosition;
    }

    public virtual Vector3 GlobePosition
    {
        get { return _globePosition; }
        set
        {
            _globePosition = value;

            Vector3 fwd = transform.forward;
            Vector3 normal;

            transform.position = Globe.GlobeToScenePosition(value, out normal);
            transform.rotation = Quaternion.LookRotation(fwd, normal);
        }
    }

    public Vector3 ScenePosition
    {
        get
        {
            try { return transform.position; }
            catch { return Globe.GlobeToScenePosition(_globePosition + new Vector3(0, GlobeRadius, 0)); }
        }
    }

    public Globe Globe
    {
        get
        {
            if (_globe == null)
                _globe = ServiceLocator.Locate<Globe>();

            return _globe;
        }
    }

    public float GlobeRadius
    {
        get { return Globe.Radius; }
    }

    public Vector3 GlobeUp
    {
        get { return transform.position.normalized; }
    }
}
