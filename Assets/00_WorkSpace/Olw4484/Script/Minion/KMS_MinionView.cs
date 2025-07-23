using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionView : MonoBehaviour
{
    [Header("Components")]
    public Transform avatar;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private AudioSource audioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;

    public Animator Animator => animator;

    private void Awake()
    {
        animator = avatar.GetComponent<Animator>();
    }

    public void PlayMinionAttackAnimation( )
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            hitEffect?.Play();
            audioSource?.PlayOneShot(attackSound);
        }
    }

    public void PlayMinionDeathAnimation()
    {
        if (animator != null)
            animator.SetTrigger("Dead");
    }
}