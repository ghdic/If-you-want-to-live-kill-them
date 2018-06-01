using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeCtrl : MonoBehaviour {

    private Transform tr;
    public Transform firePos;
    private Rigidbody rigibody;
    public bool isAimming = true;
    private RaycastHit rayHit;
    public int throwPower = 1;
    public GameObject expEffect;

    private void Start()
    {
        firePos = GameObject.Find("ThrowPos").GetComponent<Transform>();
        tr = GetComponent<Transform>();
        rigibody = GetComponent<Rigidbody>();
        expEffect = (GameObject)Resources.Load("FireExplosion");
    }

    private void Update()
    {
        Aim();
    }

    void Aim()
    {
        if (!isAimming)
            return;
        if (Input.GetMouseButtonDown(1))
            StartCoroutine(Drop());
        //Layer로 구분하는거도 생각해보자
        //int floatMask = LayerMask.GetMask("Enemy");

        /*
        if(Physics.Raycast(firePos.position, firePos.forward, out rayHit, 100.0f))
        {
            if (rayHit.collider.tag == "Monster")
            {
                Debug.DrawRay(firePos.position, rayHit.point, Color.red);
                Vector3 playerToMouse = rayHit.point - firePos.position;
                playerToMouse.y = 0.0f;
                Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
                rigibody.MoveRotation(newRotation);
                Drop();
                
                object[] _params = new object[2];
                _params[0] = rayHit.point;//Ray에 맞은 정확한 위치값(Vector3)
                _params[1] = 20;    //몬스터에 입힐 데미지값;
                                    //몬스터에 데미지 입히는 함수 호출
                rayHit.collider.gameObject.SendMessage("OnDamage", _params, SendMessageOptions.DontRequireReceiver);
                

            }
        }
        */
    }

    IEnumerator Drop()
    {

        //GetComponent<Transform>().parent.transform.DetachChildren();
        rigibody.isKinematic = false;
        rigibody.AddForce(firePos.forward * throwPower * 10.0F, ForceMode.Impulse);

        //폭발 구현
        StartCoroutine(this.Explosion(2.0f));

        //TO DO : 일단 폭탄 던지면 다시 폭탄 장착으로 구현..
        yield return new WaitForSeconds(0.3f);
        AttachWeapon.changeItem = true;
    }

    void OnCollisionEnter(Collision coll)
    {
        //지면 또는 적에게 충돌한 경우 즉시 폭발하도록
        if (coll.gameObject.tag=="Field" || coll.gameObject.tag == "Monster")        
        StartCoroutine(this.Explosion(0.0f));
    }

    IEnumerator Explosion(float tm)
    {
        yield return new WaitForSeconds(tm);
        //폭발 효과 파티클 생성
        GameObject obj = (GameObject)Instantiate(expEffect, tr.position, Quaternion.identity);
        //지정한 원점을 중심으로 10.0f 반경 내에 들어와 있는 Collider 객체 추출
        Collider[] colls = Physics.OverlapSphere(tr.position, 10.0f, LayerMask.NameToLayer("Enemy"));
        //추출한 Collider 객체에 폭발력 전달
        foreach (Collider coll in colls)
        {
            Rigidbody rbody = coll.GetComponent<Rigidbody>();
            if (rbody != null)
            {
                rbody.mass = 1.0f;
                //AddExplosionForce(폭발력, 원점, 반경, 위로 솟구치는 힘)
                rbody.AddExplosionForce(10.0f, tr.position, 10.0f, 1.0f);
            }
        }
        //3초후 파티클 제거
        Destroy(obj, 3.0f);
        //1초 후 폭탄 제거
        Destroy(this.gameObject, 0.0f);
    }
}
