using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinionView : MonoBehaviour
{
    [Header("Components")]
    public Transform avatar;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Outline highlightEffect;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;

    public Animator Animator => animator;

    private void Awake()
    {
        if (avatar == null)
            Debug.LogError("[MinionView] Avatar가 설정되지 않았습니다.");

        animator = avatar != null ? avatar.GetComponent<Animator>() : GetComponentInChildren<Animator>();
    }

    public void PlayMinionAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            //hitEffect?.Play();
            //audioSource?.PlayOneShot(attackSound);
        }
    }

    public void PlayMinionDeathAnimation()
    {
        if (animator != null)
            animator.SetTrigger("Dead");
    }

    public void SetHighlight(bool active)
    {
        if (highlightEffect != null)
            highlightEffect.enabled = active;
    }
}