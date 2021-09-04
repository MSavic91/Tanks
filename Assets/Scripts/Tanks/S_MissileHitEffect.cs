using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_MissileHitEffect : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(DeactivateIE());
    }

    IEnumerator DeactivateIE() 
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
