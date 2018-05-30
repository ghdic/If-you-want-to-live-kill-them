using UnityEngine;
using System.Collections;


public class ThirdPersonCamera : MonoBehaviour
{
	public float smooth = 3f;		// 카메라 모션 부드럽게하기용 변수
    Transform mainCamera;
	Transform standardPos;			// 카메라의 일반적인 위치
	Transform frontPos;			// 카메라 전면 탐지기
	Transform jumpPos;          // 카메라 점프 표시용
    Quaternion prevRot;          // 이전 각도 저장용
	// 원활하게 연결되지 않을 때(빠른전환)에 대한 bool 플래그
	bool bQuickSwitch = false;	//카메라 위치를 빠르게 변경하라(정면 카메라, 노말 카메라 변경할때 사용)
	
	
	void Start()
	{
        mainCamera = GetComponent<Transform>();
		// 각 참조의 초기 설정
		standardPos = GameObject.Find ("CamPos").transform;
		
		if(GameObject.Find ("FrontPos"))
			frontPos = GameObject.Find ("FrontPos").transform;

		if(GameObject.Find ("JumpPos"))
			jumpPos = GameObject.Find ("JumpPos").transform;

		// 카메라 시작 포지션
			transform.position = standardPos.position;	
			transform.forward = standardPos.forward;	
	}

	
	void FixedUpdate ()
	{
		
		if(Input.GetButton("Fire1"))	// left Ctlr
		{	
			// 전면 카메라로 변경
			setCameraPositionFrontView();
		}
		
		else if(Input.GetButton("Fire2"))	//Alt
		{	
			// 점프 카메라로 변경
			setCameraPositionJumpView();
		}
		
		else
		{	
			// 카메라를 표준 위치 및 방향으로 되돌린다.
			setCameraPositionNormalView();
		}
	}

	void setCameraPositionNormalView()
	{
		if(bQuickSwitch == false){
		// 표준 위치와 방향으로 카메라를 부드럽게 움직인다.
			transform.position = Vector3.Lerp(transform.position, standardPos.position, Time.fixedDeltaTime * smooth);	
			transform.forward = Vector3.Lerp(transform.forward, standardPos.forward, Time.fixedDeltaTime * smooth);
            mainCamera.rotation = Quaternion.Slerp(prevRot, standardPos.rotation, Time.deltaTime * 0.5f);
            prevRot = standardPos.rotation;
        }
		else{
			// 표준 위치와 방향으로 카메라를 빠르게 움직인다.
			transform.position = standardPos.position;	
			transform.forward = standardPos.forward;
            mainCamera.rotation = Quaternion.Slerp(prevRot, standardPos.rotation, Time.deltaTime * 0.5f);
            prevRot = standardPos.rotation;
			bQuickSwitch = false;
		}
	}

	
	void setCameraPositionFrontView()
	{
		// 정면 카메라로 전환
		bQuickSwitch = true;
		transform.position = frontPos.position;	
		transform.forward = frontPos.forward;
	}

	void setCameraPositionJumpView()
	{
		// 점프 카메라로 전환
		bQuickSwitch = false;
				transform.position = Vector3.Lerp(transform.position, jumpPos.position, Time.fixedDeltaTime * smooth);	
				transform.forward = Vector3.Lerp(transform.forward, jumpPos.forward, Time.fixedDeltaTime * smooth);		
	}
}
