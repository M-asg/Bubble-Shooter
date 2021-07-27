using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawAimAndPrePlacement : MonoBehaviour
{
    //variable
    [SerializeField] float linerendererMaxLength = 0;
    [SerializeField] int reflectionTime = 0;
    [SerializeField] GameObject prePlacement = null;
    [SerializeField] LayerMask rayCastWithLayer;
    [SerializeField] Transform TopWallPosition = null;

    //configs
    Ray2D ray;
    float remainingLength;
    RaycastHit2D hit;
    GameObject prePlacementObject;
    Vector2 finalPosition;
    Vector2 targetPosition;
    Vector2 radiance;


    //cache
    LineRenderer lineRenderer;
    private ParticleSystemController _particleSystemController;
    readonly string WallTag = "Wall";
    readonly string RoofTag = "Roof";
    
    private void Awake()
    {
        lineRenderer = this.gameObject.GetComponent<LineRenderer>();
        _particleSystemController = GetComponent<ParticleSystemController>();
    }

    public void SetLineRendererMaterial(Material mat)
    {
        lineRenderer.material = mat;
    }
    
    public void DrawAimAndPlacePrePlacement(Vector2 originPosition, Vector2 direction)
    {
        ray = new Ray2D(originPosition, GetTouchDistanceFromGivenObject(direction, originPosition));
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, originPosition);
        remainingLength = linerendererMaxLength;
        bool prePlacementInstantiated = false;

        for (int i = 0; i < reflectionTime; i++)
        {
            hit = Physics2D.Raycast(ray.origin, ray.direction, remainingLength, rayCastWithLayer);
            if (hit.collider != null)
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
                if (hit.collider.CompareTag(WallTag))
                {
                    ReflectRay();
                }
                else if (hit.collider.CompareTag(RoofTag))
                {
                    if (!CheckBouncesOfPosition(hit.point.x))
                    {
                        return;
                    }

                    InstantietPrePlacementForRoof(hit.point);
                    prePlacementInstantiated = true;
                }
                else
                {
                    if (!CheckBouncesOfPosition(SnapToGrid().x))
                    {
                        return;
                    }

                    finalPosition = SnapToGrid();
                    InstantietPrePlacementForBubble(finalPosition);
                    prePlacementInstantiated = true;
                }
            }

            //else
            //{
            //    lineRenderer.positionCount++;
            //    lineRenderer.SetPosition(lineRenderer.positionCount - 1, ray.origin + ray.direction * remainingLength);
            //}
        }

        if (!prePlacementInstantiated)
        {
            Destroy(prePlacementObject);
        }
    }

    private Vector2 SnapToGrid()
    {
        float xPos, yPos;
        targetPosition = hit.collider.gameObject.transform.position;
        Vector2 direction = Vector2.right;
        Vector2 otherDirection = hit.point - targetPosition;
        float angle = Vector2.Angle(Vector2.right, otherDirection);

        bool hitPointIsAboveBubble = hit.point.y - targetPosition.y >= 0;
        if (hitPointIsAboveBubble)
        {
            if (angle >= 0 && angle < 30)
            {
                xPos = targetPosition.x + 1;
                yPos = targetPosition.y;
            }
            else if (angle >= 30 && angle < 90)
            {
                xPos = targetPosition.x + 0.5f;
                yPos = targetPosition.y + 1;
            }
            else if (angle >= 90 && angle < 150)
            {
                xPos = targetPosition.x - 0.5f;
                yPos = targetPosition.y + 1;
            }
            else
            {
                xPos = targetPosition.x - 1;
                yPos = targetPosition.y;
            }
        }
        else
        {
            if (angle >= 0 && angle < 30)
            {
                xPos = targetPosition.x + 1;
                yPos = targetPosition.y;
            }
            else if (angle >= 30 && angle < 90)
            {
                xPos = targetPosition.x + 0.5f;
                yPos = targetPosition.y - 1;
            }
            else if (angle >= 90 && angle < 150)
            {
                xPos = targetPosition.x - 0.5f;
                yPos = targetPosition.y - 1;
            }
            else
            {
                xPos = targetPosition.x - 1;
                yPos = targetPosition.y;
            }
        }

        return new Vector2(xPos, yPos);
    }

    private void ReflectRay()
    {
        remainingLength -= Vector2.Distance(ray.origin, hit.point);
        Vector2 inDirection = Vector2.Reflect(ray.direction, hit.normal);
        ray = new Ray2D(hit.point + (hit.normal * 0.01f), transform.TransformDirection(inDirection));
    }

    private void InstantietPrePlacementForRoof(Vector2 point)
    {
        float xpos = point.x;
        if (xpos >= 0)
        {
            xpos = (int) xpos;
            xpos += 0.5f;
        }
        else
        {
            xpos = (int) xpos;
            xpos -= 0.5f;
        }

        Vector2 newPosition = new Vector2(xpos, TopWallPosition.transform.position.y - 0.65f);
        if (!prePlacementObject)
        {
            prePlacementObject = Instantiate(prePlacement, newPosition, Quaternion.identity);
        }


        prePlacementObject.transform.position = newPosition;
        finalPosition = newPosition;
    }

    private void InstantietPrePlacementForBubble(Vector2 newPosition)
    {
        if (!prePlacementObject)
        {
            prePlacementObject = Instantiate(prePlacement, newPosition, Quaternion.identity);
        }

        prePlacementObject.transform.position = newPosition;
    }

    public void ClearAim()
    {
        lineRenderer.positionCount = 1;
    }

    public Vector2 GetFinalPostion()
    {
        return finalPosition;
    }

    private bool CheckBouncesOfPosition(float xPos)
    {
        if (xPos < -5 || xPos > 5)
        {
            Destroy(prePlacementObject);
            return false;
        }
        else
        {
            return true;
        }
    }

    public void DestroyPrePlacement()
    {
        Destroy(prePlacementObject);
    }

    public bool PrePlacementExist()
    {
        if (prePlacementObject)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    Vector2 GetTouchDistanceFromGivenObject(Vector2 touchPositionInPixel, Vector2 originPos)
    {
        Vector2 touchDistanceFromThisObject = new Vector2(
            GetTouchPositionInUnity(touchPositionInPixel).x - originPos.x,
            GetTouchPositionInUnity(touchPositionInPixel).y - originPos.y
        );
        return touchDistanceFromThisObject;
    }

    Vector2 GetTouchPositionInUnity(Vector2 touchPositionInPixel)
    {
        Vector2 touchPositionInUnity = Camera.main.ScreenToWorldPoint(touchPositionInPixel);
        return touchPositionInUnity;
    }
}