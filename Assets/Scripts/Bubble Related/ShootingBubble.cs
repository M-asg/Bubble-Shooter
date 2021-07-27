using UnityEngine;

public class ShootingBubble : MonoBehaviour, IMoveableAndHideable, ISpawnable
{
    //config
    [SerializeField] ShootingBubbleConfigs ShootingBubbleConfigs = null;
    [SerializeField] Canvas textCanvas = null;
    float rotationSpeed = 300f;
    Vector2 finalPosition;
    Animator myAnimator = null;

    //Cached
    Rigidbody2D rigidbody2d;

    TrailRenderer myTrailRenderer = null;

    // Animator myAnimator = null;
    Vector2 centerPoint = Vector2.zero;
    Vector2 shootingPosition = Vector2.zero;


    //state
    bool rotateAround = false;
    bool isSelected = false;


    void Awake()
    {
        AwakeInitialize();
    }

    protected void AwakeInitialize()
    {
        myAnimator = GetComponent<Animator>();
        finalPosition = Vector2.zero;
        rigidbody2d = GetComponent<Rigidbody2D>();
        myTrailRenderer = GetComponent<TrailRenderer>();
        rigidbody2d.useFullKinematicContacts = true;
        myTrailRenderer.enabled = false;
        Physics2D.IgnoreLayerCollision(10, 9, true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        KinematicBubble kinematicBubble = collision.gameObject.GetComponent<KinematicBubble>();
        if (kinematicBubble != null && collision.gameObject.CompareTag(gameObject.tag))
        {
            kinematicBubble.CountNumbersOfMyColliderAndDestroyIfNeeded();
        }
    }

    private void FixedUpdate()
    {
        if (finalPosition != Vector2.zero)
        {
            if (DistanceBetween(finalPosition, transform.position) <= 4)
            {
                rigidbody2d.velocity = Vector2.zero;
                transform.position = finalPosition;
                Physics2D.IgnoreLayerCollision(10, 9, false);
                ChangeToKinematicBubble();
            }
        }

        if (rotateAround)
        {
            if (Vector2.Distance((Vector2) transform.position, shootingPosition) >= 0.3f)
            {
                transform.RotateAround(centerPoint, new Vector3(0, 0, 1), rotationSpeed * Time.deltaTime);
                transform.Rotate(new Vector3(0, 0, 1), -rotationSpeed * Time.deltaTime);
            }
            else
            {
                rotateAround = false;
                transform.position = shootingPosition;
            }
        }
    }

    public float GetBubbleSpeed()
    {
        return ShootingBubbleConfigs.GetBubbleSpeed();
    }

    public Rigidbody2D GetBubbleRigidBody()
    {
        return GetComponent<Rigidbody2D>();
    }

    public Collider2D GetBubbleCollider()
    {
        return GetComponent<Collider2D>();
    }

    public void SetFinalPosition(Vector2 finalPosition)
    {
        this.finalPosition = finalPosition;
    }

    private void ChangeToKinematicBubble()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        KinematicBubble kinematicBubble = gameObject.AddComponent(typeof(KinematicBubble)) as KinematicBubble;
        kinematicBubble.SetTextCanvas(textCanvas);


        Destroy(myTrailRenderer);
        // myAnimator.SetBool("BubbleBlow", true);


        rigidbody2d.gravityScale = 1;
        gameObject.layer = 9; // bubble
        kinematicBubble.CountNumbersOfMyColliderAndDestroyIfNeeded();
        Destroy(this);
    }

    public void Move(Vector2 direction, Vector2 finalPosition)
    {
        transform.parent = FindObjectOfType<KinematicBubbleManager>().transform;
        this.finalPosition = finalPosition;
        gameObject.AddComponent(typeof(CapsuleCollider2D));
        myTrailRenderer.enabled = true;
        GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Horizontal;
        GetComponent<CapsuleCollider2D>().size = new Vector2(1.45f, 1.45f);
        rigidbody2d.velocity = direction * GetBubbleSpeed();
    }

    protected float DistanceBetween(Vector2 fristDir, Vector2 SecoundDir)
    {
        return Mathf.Sqrt(Mathf.Pow((fristDir.x - SecoundDir.x), 2) + Mathf.Pow((fristDir.y - SecoundDir.y), 2));
    }

    public IMoveableAndHideable SpawnAtPoint(Vector2 position)
    {
        ShootingBubble moveableObj =
            Instantiate(this.gameObject, position, Quaternion.identity).GetComponent<ShootingBubble>();
        return moveableObj;
    }

    public void ChangePosition(Vector2 shootingPos, Vector2 centerPoint, bool isSelected)
    {
        rotateAround = true;
        this.isSelected = isSelected;
        if (isSelected)
        {
            rotationSpeed += 200f;
        }
        else
        {
            rotationSpeed = 300f;
        }

        shootingPosition = shootingPos;
        this.centerPoint = centerPoint;
    }

    public void Hide(bool shouldHide)
    {
        if (this != null)
        {
            this.gameObject.SetActive(!shouldHide);
        }
    }

    public Material GetAimMaterial()
    {
        return ShootingBubbleConfigs.AimMaterial;
    }

    public void DestroyMoveableObj()
    {
        Destroy(gameObject);
    }

    public bool RdyToShoot()
    {
        return !rotateAround;
    }

    public string GetTag()
    {
        return gameObject.tag;
    }
}