using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public float smooth = 5.0f;		// 카메라 모션 부드럽게하기용 변수
    private Transform mainCamera;
    private Transform standardPos;          // 카메라의 일반적인 위치
    private Transform frontPos;         // 카메라 전면 탐지기
    private Transform jumpPos;          // 카메라 점프 표시용
    private Quaternion prevRot;          // 이전 각도 저장용

    // 원활하게 연결되지 않을 때(빠른전환)에 대한 bool 플래그
    private bool bQuickSwitch = false;  //카메라 위치를 빠르게 변경하라(정면 카메라, 노말 카메라 변경할때 사용)

    public bool mouseLock = true; //마우스 움직일때 화면과 캐릭터 회전 할것인가

    //UpdateMouseLock 함수 전용
    private enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    private RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 3.0f;
    public float sensitivityY = 3.0f;
    private float minimumX = -30.0f;
    private float maximumX = 30.0f;
    private float minimumY = -30.0f;
    private float maximumY = 30.0f;
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
    private Quaternion originalRotation;

    //카메라 줌 인/아웃
    public float zoomSpeed = 0.5f; // 줌 스피드
    private float distance; // 카메라와의 거리
    private Vector3 AxisVec; //축의 벡터

    private void Start()
    {
        mainCamera = GetComponent<Transform>();
        // 각 참조의 초기 설정
        standardPos = GameObject.Find("CamPos").transform;

        if (GameObject.Find("FrontPos"))
            frontPos = GameObject.Find("FrontPos").transform;

        if (GameObject.Find("JumpPos"))
            jumpPos = GameObject.Find("JumpPos").transform;

        originalRotation = transform.localRotation;

        // 카메라 시작 포지션
        transform.position = standardPos.position;
        transform.forward = standardPos.forward;
    }

    private void FixedUpdate()
    {
        //if (Input.GetButton("Fire1"))   // left Ctlr
        //{
        //    // 전면 카메라로 변경
        //    setCameraPositionFrontView();
        //}
        //else if (Input.GetButton("Jump"))    //spacebar
        //{
        //    // 점프 카메라로 변경
        //    setCameraPositionJumpView();
        //}
        //else
        //{
        //    // 카메라를 표준 위치 및 방향으로 되돌린다.
        //    setCameraPositionNormalView();
        //}

        // 카메라를 표준 위치 및 방향으로 되돌린다.
        setCameraPositionNormalView();
        // 마우스 움직임 설정
        UpdateMouseLook();
        // 마우스 휠 줌/아웃
        zoomInZoomOut();

        // === 단축키 처리 ===
        //마우스 움직일때 카메라 회전 켜기/닫기
        if (Input.GetKeyDown(KeyCode.L))
        {
            mouseLock = !mouseLock;
        }
    }

    private void setCameraPositionNormalView()
    {
        if (bQuickSwitch == false)
        {
            // 표준 위치와 방향으로 카메라를 부드럽게 움직인다.
            transform.position = Vector3.Lerp(transform.position, standardPos.position, Time.fixedDeltaTime * smooth);
            transform.forward = Vector3.Lerp(transform.forward, standardPos.forward, Time.fixedDeltaTime * smooth);
        }
        else
        {
            // 표준 위치와 방향으로 카메라를 빠르게 움직인다.
            transform.position = standardPos.position;
            transform.forward = standardPos.forward;
            bQuickSwitch = false;
        }
        
    }

    private void setCameraPositionFrontView()
    {
        // 정면 카메라로 전환
        bQuickSwitch = true;
        transform.position = frontPos.position;
        transform.forward = frontPos.forward;
    }

    private void setCameraPositionJumpView()
    {
        // 점프 카메라로 전환
        bQuickSwitch = false;
        transform.position = Vector3.Lerp(transform.position, jumpPos.position, Time.fixedDeltaTime * smooth);
        transform.forward = Vector3.Lerp(transform.forward, jumpPos.forward, Time.fixedDeltaTime * smooth);
    }

    private void UpdateMouseLook()
    {
        //마우스 고정시킬때
        if (mouseLock)
        {
            mainCamera.rotation = Quaternion.Slerp(prevRot, standardPos.rotation, Time.deltaTime * smooth);
            prevRot = standardPos.rotation;
        }
        //마우스 고정해제일때
        else
        {
            if (axes == RotationAxes.MouseXAndY)
            {
                // Read the mouse input axis
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

                rotationX = ClampAngle(rotationX, minimumX, maximumX);
                rotationY = ClampAngle(rotationY, minimumY, maximumY);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);

                transform.localRotation = originalRotation * xQuaternion * yQuaternion;
            }
            else if (axes == RotationAxes.MouseX)
            {
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationX = ClampAngle(rotationX, minimumX, maximumX);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                transform.localRotation = originalRotation * xQuaternion;
            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = ClampAngle(rotationY, minimumY, maximumY);

                Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
                transform.localRotation = originalRotation * yQuaternion;
            }

            //움직일 경우 화면 회전 초기화
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                transform.rotation = Quaternion.Slerp(prevRot, standardPos.rotation, Time.deltaTime * smooth);
                prevRot = standardPos.rotation;
                originalRotation = transform.localRotation;
                rotationX = rotationY = 0;
            }
        }
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
        {
            angle += 360F;
        }
        if (angle > 360F)
        {
            angle -= 360F;
        }

        return Mathf.Clamp(angle, min, max);
    }

    private void zoomInZoomOut()
    {
        distance += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * -1;
        distance = Mathf.Clamp(distance, -0.17f, 0.5f);

        AxisVec = standardPos.forward * -1;
        AxisVec *= distance;
        mainCamera.position = transform.position + AxisVec;
        
    }
}