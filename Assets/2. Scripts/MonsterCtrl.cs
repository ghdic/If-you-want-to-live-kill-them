using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCtrl : MonoBehaviour {

    private Rigidbody rigid;
    private int hp;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        hp = 100;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Grenade는 GrenadeCtrl에서 함수 호출하여 처리함
    }

    private void onDamage(object[] _params)
    {
        //혈흔효과 함수 호출
        //TO DO:혈흔 만들기
        //CreateBloofEffect((Vector3)_params[0]);

        //맞은 총알의 Dmage를 추출해 몬스터 hp 차감
        hp -= (int)_params[1];
        if (hp <= 0)
        {
            //TO DO: 몬스터죽음 만들기
            //onDeath();
        }
        //TO DO: 맞는 애니메이션
        //animator.SetTrigger("IsHit");
        Vector3 knockback = new Vector3(0.0f, 1.0f, 1.0f);
        rigid.AddForce(knockback * 350.0f, ForceMode.Impulse);
    }

    private void onDeath()
    {
        GameUI.score += 10;
        Destroy(gameObject, 0.5f);
    }
}
