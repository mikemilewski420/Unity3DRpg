﻿using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    public void IdleAnimation()
    {
        animator.SetFloat("Speed", 0);
        animator.SetBool("Attacking", false);
    }

    public void RunAnimation()
    {
        animator.SetFloat("Speed", 1);
    }

    public void AttackAnimation()
    {
        animator.SetBool("Attacking", true);
    }

    public void EndAttackAnimation()
    {
        this.GetComponent<BasicAttack>().GetAutoAttackTime = 0;

        animator.SetBool("Attacking", false);
    }

    public void DamagedAnimation()
    {
        animator.SetBool("Damaged", true);
    }

    //Animation event placed at the end of the Damaged animation.
    public void EndDamagedAnimation()
    {
        animator.SetBool("Damaged", false);
    }

    public void DeathAnimation()
    {
        animator.SetBool("Attacking", false);
        animator.SetBool("Damaged", false);

        animator.SetBool("Dead", true);
    }

    public void PlayResurrectAnimation()
    {
        animator.SetBool("Dead", false);
        animator.SetBool("Ressurecting", true);
    }

    //Animation event placed at the end of the Resurrection animation.
    public void EndRessurectAnimation()
    {
        animator.SetBool("Ressurecting", false);
        this.GetComponent<PlayerController>().enabled = true;
    }
}
