using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroView : MonoBehaviour
{
    public Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // HeroController���� ������ �ִϸ��̼� hash���� ������
    public void PlayAnimation(int hash)
    {
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
            animator.Play(hash, 0, 0f);
        }
    }
}
