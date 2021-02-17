using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowBubble : MonoBehaviour
{
   //  Animator myAnimator = null;
    SuperPower superPower = null;
    List<KinematicBubble> kinematicBubbles = new List<KinematicBubble>();


    private void Awake()
    {
        // myAnimator = GetComponent<Animator>();
        superPower = GetComponent<SuperPower>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        KinematicBubble kinematicBubble = other.collider.GetComponent<KinematicBubble>();
        if (kinematicBubble)
        {
            if (!kinematicBubbles.Contains(kinematicBubble)) 
            {
                kinematicBubbles.Add(kinematicBubble);
                kinematicBubble.CountNumbersOfMyColliderAndDestroyIfNeeded();
                // Debug.Log(kinematicBubble.name + "called from superBubble");

                //myAnimator.SetBool(fireBubbleBlowBoolName, true);

                 Destroy(gameObject, 0.1f);
                 gameObject.layer = 11; // dont do physical action
            }
        }
    }

    public void DestroyItSelf()
    {
        Destroy(gameObject);
    }


    private void FixedUpdate()
    {
        if (superPower.GetObjIsMoving() && GetComponent<Rigidbody2D>().velocity == Vector2.zero)
        {
           //  DestroyItSelf();
            // myAnimator.SetBool(fireBubbleBlowBoolName, true);
        }
    }
}
