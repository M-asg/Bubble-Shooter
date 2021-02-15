using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BubbleSpawner : MonoBehaviour
{
    //configs
    Vector2 finalPosition;
    [SerializeField] ShootingBubble[] shootingBubbles = null;
    Vector2 unSelectedMoveableObjPos;
    Vector2 selectedMoveableObjPos;
    IMoveableAndHideable selectedMoveableObj;
    IMoveableAndHideable unSelectedMoveableObj;
    IMoveableAndHideable superBubble;

    //cache
    [SerializeField] KinematicBubbleManager kinematicBubbleManager = null;
    string lastSelectedTag;


    //state
    bool shootingSuperBubble = false;

    private void Start()
    {
        selectedMoveableObjPos = new Vector2(transform.position.x , transform.position.y + 1.3f);
        unSelectedMoveableObjPos = new Vector2(transform.position.x + 1.3f , transform.position.y);


        SetMoveableObjAfterShoot(1);
        SetMoveableObjAfterShoot(2);
    }

    public Vector2 GetselectedMoveableObjPos()
    {
        return selectedMoveableObjPos;
    }
    private void SetAndInstantietMoveableObj(ISpawnable spawnable, int ID)
    {
        switch (ID) 
        {
            case 1: selectedMoveableObj = spawnable.SpawnAtPoint(selectedMoveableObjPos);
                break;
            case 2: unSelectedMoveableObj = spawnable.SpawnAtPoint(unSelectedMoveableObjPos);
                break;
        }
    }
    public void SetFinalPositionSpawner(Vector2 finalPosition)
    {
        this.finalPosition = finalPosition;
    }
    public void ShootBubble(Vector2 direction)
    {
        Vector2 shootDirection = GetTouchDistanceFromGivenObject(direction , transform.position);
        shootDirection *= StandardCoefficient(shootDirection);

        if (shootingSuperBubble)
        {
            //Debug.Log("shoot super power");
            superBubble.Move(shootDirection, finalPosition);
            shootingSuperBubble = false;
            selectedMoveableObj.DestroyMoveableObj();
            unSelectedMoveableObj.DestroyMoveableObj();
            StartCoroutine(SetMoveableObj(1));
            StartCoroutine(SetMoveableObj(2));
            superBubble = null;
        }
        else
        {
            selectedMoveableObj.Move(shootDirection, finalPosition);
            // swap
            selectedMoveableObj = unSelectedMoveableObj;
            selectedMoveableObj.ChangePosition(selectedMoveableObjPos, transform.position , false);
            StartCoroutine(SetMoveableObj(2));
        }

    }

    public bool GetShootingSuperBubble()
    {
        return shootingSuperBubble;
    }

    public void SelectSuperBubble(ISpawnable superBubble)
    {
        if (shootingSuperBubble == false)
        {
            if (this.superBubble != null)
            {
                this.superBubble.Hide(false);
            }
           // Debug.Log("called");
            selectedMoveableObj.Hide(true);
            unSelectedMoveableObj.Hide(true);
            shootingSuperBubble = true;
            this.superBubble = superBubble.SpawnAtPoint(selectedMoveableObjPos);
        }
    }


    public void SwapBubble()
    {
        selectedMoveableObj.ChangePosition(unSelectedMoveableObjPos, transform.position , true);
        unSelectedMoveableObj.ChangePosition(selectedMoveableObjPos, transform.position , false);

        IMoveableAndHideable temp = null;
        temp = selectedMoveableObj;
        selectedMoveableObj = unSelectedMoveableObj;
        unSelectedMoveableObj = temp;
        lastSelectedTag = unSelectedMoveableObj.GetTag();

    }

    public void UnSelecctSuperBubble()
    {
        if (this.superBubble != null)
        {
            this.superBubble.Hide(true);
        }
        selectedMoveableObj.Hide(false);
        unSelectedMoveableObj.Hide(false);
        shootingSuperBubble = false;
        superBubble.DestroyMoveableObj();
        superBubble = null;
    }

    IEnumerator SetMoveableObj(int ID)
    {
        yield return new WaitForSeconds(0.5f);
        SetMoveableObjAfterShoot(ID);
    }

    public bool RdyToAction()
    {
        return selectedMoveableObj.RdyToShoot() && unSelectedMoveableObj.RdyToShoot();
    }
    private void SetMoveableObjAfterShoot(int ID)
    {
        List<string> bubbleTags = kinematicBubbleManager.GetBubbleTypesThatLeft();
        List<string> uniqTags = bubbleTags.Distinct().ToList();
        int random = Mathf.RoundToInt(Random.Range(0, uniqTags.Count));
        bool continueWhile = true;


        // prevent to give 2 bubble in row
        if (uniqTags.Count > 1)
        {
            do
            {
                if (lastSelectedTag != uniqTags[random])
                {
                    continueWhile = false;
                }
                else
                {
                    random = Mathf.RoundToInt(Random.Range(0, uniqTags.Count));
                }
            } while (continueWhile);
        }

        string selectedBubbleTag = uniqTags[random];
        lastSelectedTag = selectedBubbleTag;

        foreach(ShootingBubble shootingBubble in shootingBubbles)
        {
            if (shootingBubble.CompareTag(selectedBubbleTag))
            {
                SetAndInstantietMoveableObj(shootingBubble, ID);
            }
        }
    }
    private float StandardCoefficient(Vector2 shootDirection)
    {
        float radiance = Mathf.Sqrt(Mathf.Pow(shootDirection.x, 2) + Mathf.Pow(shootDirection.y, 2));
        return 1 / radiance;
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
