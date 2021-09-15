using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopLeftDisplay : MonoBehaviour
{
    private Animator animator;    

    private void Awake()
    {        
        animator = GetComponent<Animator>();
        animator.Play("Close", -1, 1.0f); // 게임 시작 시 보이지 않도록 닫기 애니메이션이 즉시 완료된 상태로 실행
    }

    public void Open()
    {
        animator.Play("Open");
    }

    public void Close()
    {
        StartCoroutine(CloseAfterDelay());
    }

    private IEnumerator CloseAfterDelay()
    {
        animator.SetTrigger("Close");
        yield return new WaitForSeconds(0.2f);
        animator.ResetTrigger("Close");        
    }

}
