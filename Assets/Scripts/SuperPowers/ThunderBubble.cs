using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderBubble : MonoBehaviour
{
    SuperPower superPower = null;
    Animator myAnimator = null;
    string thunderBubbleBlowBoolName = "ThunderBubbleBlow";

    private void Awake()
    {
        superPower = GetComponent<SuperPower>();
        myAnimator = GetComponent<Animator>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        KinematicBubble kinematicBubble = other.collider.GetComponent<KinematicBubble>();
        if (kinematicBubble)
        {
            StartCoroutine(kinematicBubble.DestroyBubble());
            myAnimator.SetBool(thunderBubbleBlowBoolName, true);
            gameObject.layer = 11;
        }
        // should chagne to corotine
        // maybe handle in animation event
    }


    private void FixedUpdate()
    {
        if(superPower.GetObjIsMoving() && GetComponent<Rigidbody2D>().velocity == Vector2.zero)
        {
            myAnimator.SetBool(thunderBubbleBlowBoolName, true);
            Destroy(gameObject, 0.2f);
        }
    }

}