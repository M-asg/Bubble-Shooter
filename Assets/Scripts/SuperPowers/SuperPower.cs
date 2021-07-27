using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperPower : MonoBehaviour, IMoveableAndHideable, ISpawnable
{
    //config
    [SerializeField] float speed = 0;
    [SerializeField] Vector2 collisionSize = Vector2.zero;
    [SerializeField] private Material aimMaterial;

    //cache
    Rigidbody2D rigidbody2d;
    TrailRenderer myTrailRenderer = null;
    Animator myAnimator = null;
    Vector2 centerPoint = Vector2.zero;
    Vector2 shootingPosition = Vector2.zero;

    //variable 
    Vector2 finalPosition;


    //state
    bool objisMoving = false;

    private void Awake()
    {
        InitialAwake();
    }

    private void InitialAwake()
    {
        myAnimator = GetComponent<Animator>();
        //myAnimator.enabled = false;
        finalPosition = Vector2.zero;
        rigidbody2d = GetComponent<Rigidbody2D>();
        myTrailRenderer = GetComponent<TrailRenderer>();
        myTrailRenderer.enabled = false;
        Physics2D.IgnoreLayerCollision(10, 9, true);
    }

    private void FixedUpdate()
    {
        if (finalPosition != Vector2.zero)
        {
            if (Vector2.Distance(finalPosition, transform.position) <= 4)
            {
                GetComponent<CapsuleCollider2D>().size = collisionSize;
                rigidbody2d.velocity = Vector2.zero;
                rigidbody2d.bodyType = RigidbodyType2D.Kinematic;
                rigidbody2d.useFullKinematicContacts = true;
                transform.position = finalPosition;
                Physics2D.IgnoreLayerCollision(10, 9, false);
                Destroy(this);
            }
        }
    }

    public void Move(Vector2 direction, Vector2 finalPosition)
    {
        this.finalPosition = finalPosition;
        gameObject.AddComponent(typeof(CapsuleCollider2D));
        //  GetComponent<CapsuleCollider2D>().size = new Vector2(0.1f , 0.1f);
        GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Horizontal;
        myTrailRenderer.enabled = true;
        rigidbody2d.velocity = direction * speed;
        objisMoving = true;
    }

    public IMoveableAndHideable SpawnAtPoint(Vector2 position)
    {
        SuperPower moveableObj = Instantiate(this.gameObject, position, Quaternion.identity).GetComponent<SuperPower>();
        return moveableObj;
    }

    public void ChangePosition(Vector2 finalPosition, Vector2 centerPoint, bool isSelected)
    {
    }

    public void Hide(bool shouldHide)
    {
        if (this != null)
        {
            this.gameObject.SetActive(shouldHide);
        }
    }

    public Material GetAimMaterial()
    {
        return aimMaterial;
    }

    // ReSharper restore Unity.ExpensiveCode

    public bool GetObjIsMoving()
    {
        return objisMoving;
    }

    public void DestroyMoveableObj()
    {
        Destroy(gameObject);
    }

    public bool RdyToShoot()
    {
        return true;
    }

    public string GetTag()
    {
        throw new System.NotImplementedException();
    }
}