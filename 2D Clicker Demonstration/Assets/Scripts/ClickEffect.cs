using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickEffect : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();        
    }

    public void Initialize()
    {
        ps.time = 0f; // 이펙트 처음으로 초기화
        ps.Play();
        StartCoroutine(AutoReturn());        
    }

    private IEnumerator AutoReturn()
    {
        yield return new WaitForSeconds(ps.main.duration);        
        EffectManager.Instance.ReturnObject(this);
    }


}
