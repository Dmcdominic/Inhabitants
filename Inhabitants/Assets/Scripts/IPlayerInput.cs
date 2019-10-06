using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerInput
{
    float MoveX();
    float MoveY();
    bool SelectionButtonDown();
    bool OwnerSetButtonDown();
    bool OwnerRemoveButtonDown();
}
