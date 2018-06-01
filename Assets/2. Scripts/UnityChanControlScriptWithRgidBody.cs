using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// 필요한 구성요소의 나열
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class UnityChanControlScriptWithRgidBody : MonoBehaviour
{
    public float animSpeed = 1.5f;              // 애니메이션 재생 속도 설정
    public float lookSmoother = 3.0f;           // 카메라 움직임을 부드럽게하는 설정
    public bool useCurves = true;               // Mecanim 커브 조정을 사용하거나 설정하기
                                                // 이 스위치가 켜져 있지 않으면 곡선은 사용되지 않는다.
    public float useCurvesHeight = 0.5f;        // 커브 보정의 유효 높이 (땅을 빠져나가기 쉬울때는 확대)
                                                // 다음 캐릭터 컨트롤러 매개 변수            
    public float forwardSpeed = 7.0f;           // 전진속도
    public float backwardSpeed = 4.0f;          // 후진속도
    public float rotateSpeed = 2.0f;            // 회전속도
    public float jumpPower = 1.0f;              // 점프위력
    private CapsuleCollider col;                // 캐릭터 충돌 컨트롤러
    private Rigidbody rb;
    private Vector3 velocity;                   // 캐릭터 충돌 컨트롤러의 이동량

    // CapsuleCollider로 설정 되어 있는 코라이다의 Height, Center의 초깃값을 담을 변수
    private float orgColHight;
    private Vector3 orgVectColCenter;
    private Animator anim;                          // 캐릭터에 연결된 애니메이터에 대한 참조
    private AnimatorStateInfo currentBaseState;         // base layer에서 사용되는 애니메이터 현재 상태를 참조
    private GameObject cameraObject;    // 메인 카메라에 대한 참조

    // 애니메이터 각 상태에 대한 참조
    static int idleState = Animator.StringToHash("Base Layer.Idle");
    static int locoState = Animator.StringToHash("Base Layer.Locomotion");
    static int jumpState = Animator.StringToHash("Base Layer.Jump");
    static int restState = Animator.StringToHash("Base Layer.Rest");

    //Player의 생명 변수
    public int hp = 100;
    //Player의 생명 초깃값
    private int initHp;
    //Player의 Health bar 이미지
    public Image imgHpbar;
    //델리게이트 및 이벤트 선언
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;
    //게임 매니저에 접근하기 위한 변수
    //private GameMgr gameMgr;
    //마우스 위아래로 움직일때 카메라 rotate 위아래로 하기위한 CamPos
    private Transform standardPos;
    public Transform magicCircle;
    public Transform throwPos;

    void Start()
    {
        // Animator 구성 요소를 얻음
        anim = GetComponent<Animator>();
        // CapsuleCollider 구성 요소를 얻음
        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        // 메인 카메라 게임 오브젝트를 얻음
        cameraObject = GameObject.FindWithTag("MainCamera");
        //standardPos Trasnform을 얻음
        standardPos = GameObject.Find("CamPos").transform;
        // CapsuleCollider 구성 요소의 Height, Center의 초깃값을 저장한다.
        orgColHight = col.height;
        orgVectColCenter = col.center;

        //생명 초깃값 설정
        initHp = hp;

        //마우스커서 숨기기, 커서 움직이지 않게함
        //Cursor.lockState = CursorLockMode.Locked;//마우스 커서 고정
        //Cursor.visible = false; //마우스 커서 보이기
    }
    
    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");              // 수평입력 값을 h라고 정의
        float v = Input.GetAxis("Vertical");                // 수직입력 값을 v라고 정의
        anim.SetFloat("Speed", v);                          // Animator 측에서 설정하고있는 Speed매개 변수에 v를 전달
        anim.SetFloat("Direction", h);                      // Animator 측에서 설정한 Direction 매개 변수에 h를 전달
        anim.speed = animSpeed;                             // Animator의 모션 재생 속도에 animSpeed를 설정
        currentBaseState = anim.GetCurrentAnimatorStateInfo(0); // 참조용 상태 변수 Base Layer(0)의 현재 상태를 설정
        rb.useGravity = true;//점프 중에 일정 시간 중력의 영향을 받지 않게 한다.
        // --- 캐릭터 이동 처리 ---
        velocity = new Vector3(0, 0, v);        // 상하 키 입력에서 Z축 방향의 이동량을 취득
                                                // 캐릭터의 로컬 공간에서 방향으로 변환
        velocity = transform.TransformDirection(velocity);
        //다음 v 임계 값은 Mecanim 측의 전환과 함께 조정
        if (v > 0.1)
        {
            velocity *= forwardSpeed;       // 전진 이동속도를 곱한다.
        }
        else if (v < -0.1)
        {
            velocity *= backwardSpeed;  // 후퇴 이동속도를 곱한다.
        }
        // 스페이스바를 입력할 경우
        if (Input.GetButtonDown("Jump"))
        {   
            //애니메이션 상태가 Locomotion 중일 경우
            if (currentBaseState.fullPathHash == locoState)
            {
                //애니메이션이 종료가 되면
                if (!anim.IsInTransition(0))
                {
                    //위쪽으로 물리적 힘을 가한다.
                    rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                    anim.SetBool("Jump", true);     // Animator을 점프로 전환
                }
            }
        }
        // 상하 키 입력으로 캐릭터를 이동시킨다.
        transform.localPosition += velocity * Time.fixedDeltaTime;
        // 좌우의 키 입력으로 문자 Y축으로 회전시킨다.
        transform.Rotate(0, h * rotateSpeed, 0);

        // --- Animator의 각 상태에서의 처리 ---
        // Locomotion 중일때 처리
        if (currentBaseState.fullPathHash == locoState)
        {
            //곡선 collider 조정을 하고 있을때는 안전을 위해 재설정
            if (useCurves)
            {
                resetCollider();
            }
        }
        // JUMP 중일때 처리
        else if (currentBaseState.fullPathHash == jumpState)
        {
            cameraObject.SendMessage("setCameraPositionJumpView");  // 메인 카메라의 JumpView 함수 호출
            // 상태가 전환되고 있지 않는 경우
            if (!anim.IsInTransition(0))
            {
                // 커브 조정을 하는 경우의 처리
                if (useCurves)
                {
                    // JUMP00 애니메이션에 붙어 있는 JumpHeight와 GravityControl
                    // JumpHeight:JUMP00에서 점프의 높이（0〜1）
                    // GravityControl:1⇒점프 중(중력 비활성), 0⇒중력 활성화
                    float jumpHeight = anim.GetFloat("JumpHeight");
                    float gravityControl = anim.GetFloat("GravityControl");
                    if (gravityControl > 0)
                        rb.useGravity = false;  //점프 중 중력의 영향을 없앤다.
                    // Ray Casting 캐릭터의 센터에서 떨어뜨린다.
                    Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
                    RaycastHit hitInfo = new RaycastHit();
                    // 높이가 useCurvesHeight 이상인 경우에만 Collider의 
                    //높이와 중심을 JUMP00 애니메이션에 붙어있는 곡선조정
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        if (hitInfo.distance > useCurvesHeight)
                        {
                            col.height = orgColHight - jumpHeight;          // 조정된 collider의 높이
                            float adjCenterY = orgVectColCenter.y + jumpHeight;
                            col.center = new Vector3(0, adjCenterY, 0); // 조정된 collider의 센터
                        }
                        else
                        {
                            // 임계 값보다 낮은 경우에는 초기값으로 복원(만약을 위해)
                            resetCollider();
                        }
                    }
                }
                // Jump bool값을 재설정(루프하지 않도록 한다)
                anim.SetBool("Jump", false);
            }
        }
        // IDLE 중일때 처리
        else if (currentBaseState.fullPathHash == idleState)
        {
            //곡선 Collider 조정을 하고 있을때는 안전을 위해 재설정
            if (useCurves)
            {
                resetCollider();
            }
            // 스페이스 키를 입력하면 Rest 상태로
            if (Input.GetButtonDown("Jump"))
            {
                anim.SetBool("Rest", true);
            }
        }
        // REST 중일때 처리
        else if (currentBaseState.fullPathHash == restState)
        {
            cameraObject.SendMessage("setCameraPositionFrontView");		// 카메라를 정면으로 전환
            // 상태가 전환되고 있지 않은 경우, Rest bool 값을 재설정(루프하지 않도록한다.)
            if (!anim.IsInTransition(0))
            {
                anim.SetBool("Rest", false);
            }
        }
        // 이외 모션중일때(Shift누를 때) 카메라를 정면으로 바꾼다.
        else
        {
            cameraObject.SendMessage("setCameraPositionFrontView");		// 카메라를 정면으로 전환
        }
        // --- 랜덤 모션에 실행하기 ---
        //움직이면 모션이 캔슬됨
        if (h != 0 || v != 0)
            anim.SetTrigger("MotionCancel");

        //랜덤으로 모션 실행
        if (Input.GetKeyDown(KeyCode.K))  // K
        {
            
            anim.SetInteger("RandomMotion", Random.Range(1, 17));
            anim.SetTrigger("Motion");
        }
        
        // --- 캐릭터 children의 rotaion 처리 ---
        //마우스 위 아래로 움직일때 CamPos Rotation 처리
        float MouseRot = Input.GetAxis("Mouse Y");
        if (MouseRot>0 && standardPos.rotation.x >= -0.3f)
        {
            standardPos.Rotate(Vector3.left * Time.deltaTime * 30.0f * MouseRot);
            magicCircle.Rotate(Vector3.left * Time.deltaTime * 30.0f * MouseRot);
            throwPos.Rotate(Vector3.left * Time.deltaTime * 30.0f * MouseRot);

        }
        else if(MouseRot<0 && standardPos.rotation.x <= 0.3f)
        {
            standardPos.Rotate(Vector3.left * Time.deltaTime * 30.0f * MouseRot);
            magicCircle.Rotate(Vector3.left * Time.deltaTime * 30.0f * MouseRot);
            throwPos.Rotate(Vector3.left * Time.deltaTime * 30.0f * MouseRot);
        }
    }

    

    // 캐릭터 collider 크기 재설정 함수
    void resetCollider()
    {
        // 구성 요소의 Height, Center의 초기값을 리턴
        col.height = orgColHight;
        col.center = orgVectColCenter;
    }
}
