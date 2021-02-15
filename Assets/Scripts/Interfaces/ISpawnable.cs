using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpawnable 
{
    IMoveableAndHideable SpawnAtPoint(Vector2 position);
}
