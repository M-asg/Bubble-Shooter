using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveableAndHideable
{
    void Move(Vector2 direction , Vector2 finalPosition );

    void ChangePosition(Vector2 finalPosition , Vector2 centerPoint , bool isSelected);

    void Hide(bool shouldHide);
    Material GetAimMaterial();    
    bool RdyToShoot();

    string GetTag();

    void DestroyMoveableObj();
}
