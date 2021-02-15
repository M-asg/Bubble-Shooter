using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBubble : MonoBehaviour
{
    Animator myAnimator = null;
    string fireBubbleBlowBoolName = "FireBubbleBlow";
    SuperPower superPower = null;



    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        superPower = GetComponent<SuperPower>();

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        KinematicBubble kinematicBubble = other.collider.GetComponent<KinematicBubble>();
        if (kinematicBubble)
        {
            StartCoroutine(kinematicBubble.DestroyBubble());
            myAnimator.SetBool(fireBubbleBlowBoolName,true);
            gameObject.layer = 11;
        }
        // should chagne to corotine
        // maybe handle in animation event
    }

    public void DestroyItSelf()
    {
        Destroy(gameObject);
    }


    private void FixedUpdate()
    {
        if (superPower.GetObjIsMoving() && GetComponent<Rigidbody2D>().velocity == Vector2.zero)
        {
            myAnimator.SetBool(fireBubbleBlowBoolName, true);
        }
    }
}
