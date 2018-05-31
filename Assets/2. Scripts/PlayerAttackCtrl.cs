using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAttackCtrl : MonoBehaviour {
    //파이어볼 프리팹
    public GameObject fireball;
    //메테오 프리팹
    public GameObject metao;
    //발사 좌표
    public Transform firePos;
    //공격시 script 사운드
    public AudioClip sfx;
    //AudioSource 컴포넌트를 저장할 변수
    private AudioSource source = null;
    //마법진
    public ParticleSystem flameRing;
    public ParticleSystem fireWhirl;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        flameRing.Stop();
        fireWhirl.Stop();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(fireBallAttack(3.0f));
        }
    }

    IEnumerator fireBallAttack(float time)
    {
        flameRing.Play();
        
        flameRing.startSize = 1;
        yield return new WaitForSeconds(5.0f);
        flameRing.startSize = 20;
        yield return new WaitForSeconds(time);
        flameRing.Stop();
    }
}
