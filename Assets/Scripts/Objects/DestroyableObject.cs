﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour
{
    [SerializeField]
    private bool _sinkable = true;

    [SerializeField]
    private float
        _health,
        _explosionSize,
        _partForceMulti = 3;

    [SerializeField]
    private GameObject
        _destroyedPrefab;

    [SerializeField]
    private MonoBehaviour[] _removedBehaviours;

    private bool _exploded = false;
    private bool _destroyableByImpact = false;

    private Rigidbody _rigidBody;


    void Update ()
    {
        if (_rigidBody != null && !_rigidBody.isKinematic && Globe.SceneToGlobePosition(transform.position).y < 0)
            Sink();

        if (!_exploded && _health < 0)
            Explode();

        _rigidBody = GetComponent<Rigidbody>();

        if (_rigidBody != null)
            _destroyableByImpact = true;
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (!_destroyableByImpact)
            return;

        float damage = collision.impulse.magnitude;
        Damage(damage);
    }

    public void Damage(float damage)
    {
        _health -= damage;
    }

    private void Sink()
    {
        Instantiate(ServiceLocator.Locate<Effects>().Splash, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public void Explode()
    {
        GameObject destroyedObject;

        if (_destroyedPrefab != null)
        {
            destroyedObject = Instantiate(_destroyedPrefab, transform.position, transform.rotation);
            destroyedObject.transform.localScale = transform.lossyScale;
        }
        else
            destroyedObject = gameObject;

        List<GameObject> parts = GetAllVisualParts(destroyedObject);

        foreach (MonoBehaviour behaviour in _removedBehaviours)
            Destroy(behaviour);

        foreach (GameObject part in parts)
        {
            Part PartComp = part.AddComponent<Part>();

            Vector3 force = (part.transform.position - transform.position).normalized * _partForceMulti;

            if (_rigidBody != null)
                force += _rigidBody.velocity;

            PartComp.ExplodeForce = force;
        }

        ExplodeAnimation();
        DamageSurroundings();

        if (_destroyedPrefab != null)
            Destroy(gameObject);

        _exploded = true;
    }

    private void DamageSurroundings()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionSize * 2);

        foreach (Collider collider in colliders)
        {
            DestroyableObject dObject = collider.GetComponent<DestroyableObject>();

            if (dObject == null)
                continue;

            float damage = Mathf.Clamp(_explosionSize - Vector3.Distance(transform.position, dObject.transform.position), 0, Mathf.Infinity);
            dObject.Damage(damage * 10);
        }
    }

    private void ExplodeAnimation()
    {
        GameObject explosion = Instantiate(ServiceLocator.Locate<Effects>().Explosion, transform.position, transform.rotation);
        explosion.GetComponent<ParticleSystem>().Play();
    }

    private List<GameObject> GetAllVisualParts(GameObject gameObject, bool onlyVisualObjects = true)
    {
        List<GameObject> objects = new List<GameObject>();
        objects.Add(gameObject);
        gameObject.transform.parent = null;

        for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
            objects.AddRange(GetAllVisualParts(gameObject.transform.GetChild(i).gameObject, false));

        if (!onlyVisualObjects)
            return objects;

        for (int i = objects.Count - 1; i >= 0; i--)
            if (objects[i].GetComponent<MeshRenderer>() == null)
            {
                GameObject obj = objects[i];
                objects.RemoveAt(i);

                Destroy(obj);
            }

        return objects;
    }
}
