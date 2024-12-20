using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Networking;

public class FPSPlayerAnimations : MonoBehaviour
{



    private Animator anim;

    private string MOVE = "Move";
    private string VELOCITY_Y = "VelocityY";
    private string CROUCH = "Crouch";
    private string CROUCH_WALK = "CrouchWalk";


    private string STAND_SHOOT = "StandShoot";
    private string CROUCH_SHOOT = "CrouchShoot";
    private string RELOAD = "Reload";

    public RuntimeAnimatorController animController_Pistol, animatorController_MachineGun;

  


    private void Awake()
    {
        anim = GetComponent<Animator>();
       
    }


    public void Movement(float magnitude)
    {
        anim.SetFloat(MOVE, magnitude);
    }


    public void PlayerJump(float velocity)
    {
        anim.SetFloat(VELOCITY_Y, velocity);
    }

    public void PlayerCrouch(bool isCrouching)
    {
        anim.SetBool(CROUCH, isCrouching);
    }

    public void PlayerCrouchWalk(float magnitude)
    {
        anim.SetFloat(CROUCH_WALK, magnitude);
    }


    public void Shoot(bool isStanding)
    {
        if(isStanding)
        {
            anim.SetTrigger(STAND_SHOOT);
        }
        else
        {
            anim.SetTrigger(CROUCH_SHOOT);
        }
    }

    public void ReloadGun()
    {
        anim.SetTrigger(RELOAD);
    }


    public void ChangeController(bool isPistol)
    {
        if(isPistol)
        {
            anim.runtimeAnimatorController = animController_Pistol;
        }
        else
        {
            anim.runtimeAnimatorController = animatorController_MachineGun;
        }
    }



} // class
