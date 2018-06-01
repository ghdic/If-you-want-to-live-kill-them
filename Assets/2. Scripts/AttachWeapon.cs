using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachWeapon : MonoBehaviour {

    public Transform attachPoint;   //무기 장착할 곳
    public GameObject Weapon;   //무기 게임 오브젝트
    public static bool changeItem = false; //아이템 바뀔때
    public int itemNum = 3; //장착할 아이템 넘버
    private enum WeaponKinds {MagicStick=1, Gun=2, Grenade=3 };

    private void Start()
    {
        itemNum = 3;
        //default로 설정되있는 무기 장착
        attachWeapon();

        Weapon.GetComponent<Rigidbody>().isKinematic = true;
        //무기 튕겨나가지 않게 물리작용 끔
        //SetEquip(Weapon, true);
    }

    private void Update()
    {
        //아이템 바꾸는 키가 눌러졌을 경우(여기서 입력받는건 어때?
        if (changeItem)
        {
            if(itemNum == (int)WeaponKinds.MagicStick)
            {
                Destroy(Weapon, 0.0f);
                Weapon = (GameObject)Instantiate(Resources.Load("MagicStick"));
                Weapon.GetComponent<Rigidbody>().isKinematic = true;
                attachWeapon();
                
            }
            else if(itemNum == (int)WeaponKinds.Grenade)
            {
                Weapon = (GameObject)Instantiate(Resources.Load("Grenade"));
                Weapon.GetComponent<Rigidbody>().isKinematic = true;
                attachWeapon();
            }

            // TODO: 나머지 아이템도 업데이트 하기

            //아이템 다바꿨으니 다시 false로 전환
            changeItem = false;

        }
    }

    private void attachWeapon()
    {
        Weapon.transform.parent = attachPoint;
        Weapon.transform.position = attachPoint.position;
        Weapon.transform.rotation = attachPoint.rotation;
    }


    //무기와 충돌해 튕겨져 나가는 것을 방지한다.
    public void SetEquip(GameObject item, bool isEquip)
    {
        Collider[] itemColliders = item.GetComponents<Collider>();
        Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();

        foreach (Collider itemCollider in itemColliders)
        {
            itemCollider.enabled = !isEquip;
        }
        itemRigidbody.isKinematic = isEquip;
    }
}
