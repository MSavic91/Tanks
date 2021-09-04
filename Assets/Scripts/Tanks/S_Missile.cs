using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Missile : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] protected TrailRenderer trail;
    [SerializeField] protected float speed = 1;
#pragma warning restore 0649

    private S_Tank owner;
    string tag;

    public void Init(S_Tank owner)
    {
        gameObject.SetActive(true);
        this.owner = owner;
        tag = owner.tag;

    }

    private void OnEnable()
    {
        trail.emitting = true;
    }

    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        var collisionObject = other.gameObject;
        if (collisionObject == owner.gameObject)
        {
            return;
        }

        if(collisionObject.CompareTag("Destructible"))
        {
            collisionObject.GetComponent<S_BoardTile>().HitDestructibleTile();
        }
        else if (collisionObject.CompareTag("Base"))
        {
            collisionObject.GetComponent<S_BoardTile>().HitBase();
        }
        else if (collisionObject.CompareTag("Tank"))
        {
            var tank = collisionObject.GetComponent<S_Tank>();
            if (tank.IsPlayer && !owner.IsPlayer)
            {
                tank.Destroy();
            }
            else if(!tank.IsPlayer && owner.IsPlayer)
            {
                tank.Destroy();
            }
        }
        
        S_VFX_Manager.SpawnMissileHitEffect(transform.position);
        trail.emitting = false;
        this.gameObject.SetActive(false);
    }

    
    
    
}
