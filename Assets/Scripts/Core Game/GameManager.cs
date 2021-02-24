using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //config
    [SerializeField] BubbleSpawner bubbleSpawner = null;
    [SerializeField] LineRenderer line = null;
    [SerializeField] bool _mouseActive=true;
    float timePassed=0.5f;

    //cache
    DrawAimAndPrePlacement drawAim = null;
    KinematicBubbleManager kinematicBubbleManager = null;

    private void Start()
    {
        drawAim = line.GetComponent<DrawAimAndPrePlacement>();
        kinematicBubbleManager = FindObjectOfType<KinematicBubbleManager>();
    }


    private void Update()
    {
        timePassed += Time.deltaTime;
        if (!_mouseActive)
        {
            if (kinematicBubbleManager.RdyToShoot() && bubbleSpawner.RdyToAction() && timePassed >= 0.55f)
            {
                if (Input.touchCount > 0)
                {
                    Touch fristTouch = Input.GetTouch(0);
                    Vector2 fristTouchPos = fristTouch.position;

                    if (fristTouch.phase == TouchPhase.Began || fristTouch.phase == TouchPhase.Moved || fristTouch.phase == TouchPhase.Stationary)
                    {
                        AttempToDrawAndPlacePrePlacement();
                    }
                    else
                    {
                        AttempToShoot();
                    }
                }
            }
        }
        else
        {
            if (bubbleSpawner.RdyToAction() && kinematicBubbleManager.RdyToShoot() && timePassed >= 0.55f)
            {
                AttempToDrawAndPlacePrePlacement();
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    AttempToShoot();
                    timePassed = 0;
                }
            }
        }

    }

    private void AttempToDrawAndPlacePrePlacement()
    {
        drawAim.DrawAimAndPlacePrePlacement(bubbleSpawner.GetselectedMoveableObjPos(), Input.mousePosition);
        
    }
    private void AttempToShoot()
    {
        if (!drawAim.PrePlacementExist()) { return; }
        bubbleSpawner.SetFinalPositionSpawner(drawAim.GetFinalPostion());
        drawAim.DestroyPrePlacement();
        drawAim.ClearAim();
        bubbleSpawner.ShootBubble(Input.mousePosition);
        //shooted = true;
    }
}
