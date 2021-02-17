using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
// using UnityEngine

public class KinematicBubble : MonoBehaviour
{
    //config
    [SerializeField] readonly float waitBeforeBlow = 0.07f;
    Vector2[] rayDirections;
    [SerializeField] bool isAnchor = true;
    [SerializeField] Canvas textCanvas = null;



    //cache
    KinematicBubbleManager kinematicBubbleManager;
    Rigidbody2D myRigidbody2D;
    CapsuleCollider2D myCollider2D;
    Animator myAnimator =  null;
    

    //state
    bool objIsAlive = true;
    bool objIsAboutToBlow = false;
    bool destroyWhenMyTypeDestroyed = false;
    // bool isAnchor = true;
    // bool objIsAboutToDrop = false;
    bool droped = false;
    bool alive = true;

    public void SetTextCanvas(Canvas textCanvas)
    {
        this.textCanvas = textCanvas;
    }

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        myAnimator.SetBool("BubbleBlow", true);
        myAnimator.enabled = false;

        myCollider2D = GetComponent<CapsuleCollider2D>();
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myRigidbody2D.useFullKinematicContacts = true;
        InitialRayDirections();
        SetColliderDependOnPosition();

        kinematicBubbleManager = FindObjectOfType<KinematicBubbleManager>();
        kinematicBubbleManager.AddToKinematicList(this);
        kinematicBubbleManager.SetTimerAndCheckForScroll();
    }
    public void SetAnchor( bool isAnchor)
    {
        this.isAnchor = isAnchor;
    }
    public bool GetAnchor()
    {
        return isAnchor;
    }
    public bool CheckForAnchor()
    {
        myCollider2D.enabled = false;
        isAnchor = false;
        for (int i = 2; i < 4; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirections[i], 0.6f);
            Debug.DrawRay(transform.position, rayDirections[i], Color.green, 2);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Roof"))
                {
                    isAnchor = true;
                }
                else if (hit.collider.gameObject.GetComponent<KinematicBubble>() && hit.collider.gameObject.GetComponent<KinematicBubble>().GetAnchor())
                {
                    isAnchor = true;
                }
            }
        }
        myCollider2D.enabled = true;
        return isAnchor;
    }
    public void FindButtomsAndAddToList(List<KinematicBubble> buttomsBubbles)
    {
        myCollider2D.enabled = false;
        for (int i = 4; i < 6; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirections[i], 0.6f);
            if (hit.collider != null && hit.collider.GetComponent<KinematicBubble>() )
            {
                if (!buttomsBubbles.Contains(hit.collider.GetComponent<KinematicBubble>()))
                {
                    buttomsBubbles.Add(hit.collider.GetComponent<KinematicBubble>());
                }
            }
        }
        myCollider2D.enabled = true;
    }
    public void FindNeighborsAndAddToList(List<KinematicBubble> sideBubbles,Vector2 exceptedPosition)
    {
        if (!sideBubbles.Contains(this))
        {
            // Debug.Log(gameObject.name);
            sideBubbles.Add(this);
            myCollider2D.enabled = false;
            for (int i = 0; i < 2; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirections[i], 0.6f);
                Debug.DrawRay(transform.position , rayDirections[i],Color.red , 3);
                if (hit.collider != null && (Vector2)hit.collider.transform.position != exceptedPosition && !hit.collider.CompareTag("Roof")
                    && hit.collider.GetComponent<KinematicBubble>() && hit.collider.GetComponent<KinematicBubble>().ObjIsAboutToBlow() == false
                    )
                {
                    hit.collider.GetComponent<KinematicBubble>().FindNeighborsAndAddToList(sideBubbles, transform.position);
                }
            }
            myCollider2D.enabled = true;
        }
    }
    public bool ObjIsAboutToBlow()
    {
        return objIsAboutToBlow;
    }
    public void CountNumbersOfMyColliderAndDestroyIfNeeded()
    {
        int myColliderTypeCount = 0;

        // Debug.Log("Called" + gameObject.name);      
        
        myCollider2D.enabled = false;
        List<KinematicBubble> bubblesNearMe = new List<KinematicBubble>();
        for (int i = 0; i < 6; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirections[i] , 0.6f);
            if (hit.collider != null && (hit.collider.CompareTag(gameObject.tag) || hit.collider.CompareTag("Rainbow Bubble")))
            {
                if (!bubblesNearMe.Contains(hit.collider.GetComponent<KinematicBubble>()))
                {
                    bubblesNearMe.Add(hit.collider.gameObject.GetComponent<KinematicBubble>());
                    myColliderTypeCount++;
                }
            }
        }
        myCollider2D.enabled = true;
        if (myColliderTypeCount >= 2)
        {
             StartCoroutine(DestroyBubbleWhenMyTypeDestroyed());
        }
        
    }

    public void CountNumbersOfMyColliderAndShowPreDestroy()
    {
        int myColliderTypeCount = 0;
        myCollider2D.enabled = false;
        List<KinematicBubble> bubblesNearMe = new List<KinematicBubble>();
        for (int i = 0; i < 6; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirections[i], 0.6f);
            if (hit.collider != null && hit.collider.CompareTag(gameObject.tag))
            {
                if (!bubblesNearMe.Contains(hit.collider.GetComponent<KinematicBubble>()))
                {
                    bubblesNearMe.Add(hit.collider.gameObject.GetComponent<KinematicBubble>());
                    myColliderTypeCount++;
                }
            }
        }
        myCollider2D.enabled = true;
        if (myColliderTypeCount >= 2)
        {
            // show preDestroy
            Debug.Log("show preDestroy");

            foreach(KinematicBubble bubble in bubblesNearMe)
            {
                bubble.CountNumbersOfMyColliderAndShowPreDestroy();
            }
        }

    }






    protected void InitialRayDirections()
    {
        rayDirections = new Vector2[]
        {
            new Vector2(1, 0),
            new Vector2(-1f, 0),
            new Vector2(0.5f, 1),
            new Vector2(-0.5f, 1),
            new Vector2(-0.5f, -1),
            new Vector2(0.5f, -1),
        };
    }
    protected void SetColliderDependOnPosition()
    {
        if (transform.position.x == -4.5f)
        {
            myCollider2D.offset = new Vector2(-0.43f, 0);
            myCollider2D.size = new Vector2(2.16f, 1.3f);
        }
        else if (transform.position.x == 4.5f)
        {
            myCollider2D.offset = new Vector2(0.43f, 0);
            myCollider2D.size = new Vector2(2.16f, 1.3f);
        }
    }
    public void Drop(Vector2 exceptedPosition)
    {
        if (!droped)
        {
            kinematicBubbleManager.RemoveFromKinematicList(this);
            droped = true;
            myCollider2D.enabled = false;
            for (int i = 0; i < 6; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirections[i], 0.6f);
                if (hit.collider != null && (Vector2)hit.collider.transform.position != exceptedPosition && hit.collider.GetComponent<KinematicBubble>())
                {
                    hit.collider.GetComponent<KinematicBubble>().Drop(transform.position);
                }
            }
            myCollider2D.enabled = true;
            gameObject.layer = 11; // none Collision
            myRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            GiveRandomVelocityToBubble();
            myCollider2D.isTrigger = true;            
        }
    }



    public bool IsAnchor()
    {
        return isAnchor;
    }
    private void GiveRandomVelocityToBubble()
    {
        float xVelocity = Random.Range(-0.8f , 0.8f);
        float yVelocity = Random.Range(0.5f , 2f);
        Vector2 randomVelocity = new Vector2(xVelocity, yVelocity);
        myRigidbody2D.velocity = randomVelocity;
    }
    public IEnumerator DestroyBubble()
    {
        if (alive)
        {
            alive = false;
            // play vfx and sfx
            myAnimator.enabled = true;

            yield return new WaitForSeconds(waitBeforeBlow);

            objIsAlive = false;
            myCollider2D.enabled = false;
            AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);

            SpawnTextAndUpdateSlider();
            Destroy(gameObject, info.length);
        }
    }
    public IEnumerator DestroyBubbleWhenMyTypeDestroyed()
    {
        if (alive)
        {
            alive = false;
            // play vfx and sfx
            myAnimator.enabled = true;
            destroyWhenMyTypeDestroyed = true;

            yield return new WaitForSeconds(waitBeforeBlow);

            objIsAlive = false;
            myCollider2D.enabled = false;
            AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);

            SpawnTextAndUpdateSlider();
            Destroy(gameObject , info.length);
        }
    }

    public bool GetDestroyWhenMyTypeDestroyed()
    {
        return destroyWhenMyTypeDestroyed;
    }
    public bool IsObjAlive()
    {
        return objIsAlive;
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        KinematicBubble kinematicBubble = other.collider.GetComponent<KinematicBubble>();
        if (other.collider != null && kinematicBubble)
        {
            if (myCollider2D.enabled == true && other.collider.CompareTag(gameObject.tag) && kinematicBubble.IsObjAlive() == false && 
                kinematicBubble.GetDestroyWhenMyTypeDestroyed())
            {
                StartCoroutine(DestroyBubbleWhenMyTypeDestroyed());
                kinematicBubbleManager.SetTimerAndCheckForDropAndRemoveFromList(this);
            }
            else if (myCollider2D.enabled == true && kinematicBubble.IsObjAlive() == false && alive)
            {
                if (objIsAboutToBlow == false)
                {
                    kinematicBubbleManager.SetTimerAndCheckForDropAndAddToList(this);
                }
            }
        }
    }

    private void OnDestroy()
    {
        kinematicBubbleManager.RemoveFromKinematicList(this);
        if (!droped)
        {
            kinematicBubbleManager.SetTimerAndCheckForScroll();
        }
        kinematicBubbleManager.SetTimerForDropToZero();
    }

    private void SpawnTextAndUpdateSlider()
    {
        SliderController sliderController = FindObjectOfType<SliderController>();
        sliderController.SetAmount(kinematicBubbleManager.GetAmount());

        var canvas = Instantiate(textCanvas, transform.position, Quaternion.identity);
        TextMeshPro text = canvas.GetComponentInChildren<TextMeshPro>();
        //kinematicBubbleManager.GetAmount().ToString()
        TextCanvasController textCanvasController = canvas.GetComponent<TextCanvasController>();
        textCanvasController.SetTextValue(kinematicBubbleManager.GetAmount());
    }
}
