using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour {

    private void Awake()
    {
        //각 레이어 물리작용 무시
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("OurArmy"), LayerMask.NameToLayer("Weapon"), true);
    }
}
