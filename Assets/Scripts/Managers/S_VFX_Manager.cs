using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_VFX_Manager : MonoBehaviour
{
    public static S_VFX_Manager instance;

#pragma warning disable 0649
    [SerializeField] private GameObject missile;
    [SerializeField] private GameObject missileHitEffect;
#pragma warning restore 0649

    private List<GameObject> missileList;
    private List<GameObject> missileHitEffectList;

    public void Init()
    {
        if (instance != null)
        {
            if (instance == this)
            {
                return;
            }
            Destroy(gameObject);
            return;
        }
        
        instance = this;

        InstantiateInstances(ref missileList, missile, 30);
        InstantiateInstances(ref missileHitEffectList, missileHitEffect, 30);
    }

    private void InstantiateInstances(ref List<GameObject> list, GameObject prefab, int count) 
    {
        list = new List<GameObject>(count);
        for (int i = 0; i < count; i++)
        {
            var newInstance = Instantiate(prefab, transform);
            newInstance.SetActive(false);
            list.Add(newInstance);
        }
    }

    public static S_Missile SpawnMissile(Vector3 postion, Direction direction, S_Tank owner) 
    {
        float rotationY = 0;
        switch (direction)
        {
            case Direction.Up:
                break;
            case Direction.Down:
                rotationY = 180;
                break;
            case Direction.Left:
                rotationY = 270;
                break;
            case Direction.Right:
                rotationY = 90;
                break;
            default:
                break;
        }
        var inst = instance.GetFirstAvailableInstance(instance.missileList, instance.missile);
        inst.transform.position = postion;
        inst.transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        var effect = inst.GetComponent<S_Missile>();
        effect.Init(owner);

        return effect;
    }

    public static void SpawnMissileHitEffect(Vector3 postion)
    {
        var inst = instance.GetFirstAvailableInstance(instance.missileHitEffectList, instance.missileHitEffect);
        inst.transform.position = postion;
        inst.gameObject.SetActive(true);
    }

    private GameObject GetFirstAvailableInstance(List<GameObject> fromList, GameObject fallbackPrefab)
    {
        if (instance == null)
        {
            return null;
        }

        int count = fromList.Count;
        for (int i = 0; i < count; i++)
        {
            if (fromList[i] == null)
            {
                var newInstance = Instantiate(fallbackPrefab, transform);
                newInstance.SetActive(false);
                fromList[i] = newInstance;
                return fromList[i];
            }
            if (!fromList[i].activeSelf)
            {
                return fromList[i];
            }
        }

        var newInd = AddNewInstances(ref fromList, fallbackPrefab);
        return fromList[newInd];
    }

    private int AddNewInstances(ref List<GameObject> list, GameObject prefab)
    {
        int count = list.Count;
        int first = 0;
        for (int i = count; i < count + count / 5 + 2; i++)
        {
            if (first == 0)
            {
                first = i;
            }
            var newInstance = Instantiate(prefab, transform);
            newInstance.SetActive(false);
            list.Add(newInstance);
        }
        return first;
    }
}
