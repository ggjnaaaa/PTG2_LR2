using UnityEngine;

public class HumanScript : MonoBehaviour
{
    Animator anim;
    GameObject vector;                      // ���������� ���� ��������� �������� (GameObject ������ HumanMale_Character_FREE)
    GameObject pivot;                       // ����� �������� � �������� ��������� (�� ������ ������ ����� �� ������� Mesh ������ HumanMale_Character_FREE)
    Rigidbody rb;
    bool isWalkNow;                         // ��� �� �������� ������
    bool isRunNow;                          // ����� �� �������� ������
    bool isGroundingNow;                    // ������������ �� �������� ������
    bool isJumpNow;                         // ������� �� �������� ������ ���
    bool isStandNow;                        // ����� �� �������� ������
    bool lastW;                             // ���� �� ������� W �������
    float initialLengthFromGroundToPivot;   // ����������� ���������� �� ������ �� ����� ���� �������� �����

    void Start()
    {
        anim = GetComponent<Animator>();
        vector = GameObject.FindGameObjectWithTag("vector");
        pivot = GameObject.FindGameObjectWithTag("pivot");
        rb = GetComponent<Rigidbody>();
        isWalkNow = false;
        isRunNow = false;
        isGroundingNow = false;
        isJumpNow = false;
        isStandNow = true;
        lastW = false;
    }

    void Update()
    {
        //~~~~~~    �������    ~~~~~~//
        if (rb.velocity.y < -0.1)
            Falling();
        //~~~~~~    �����    ~~~~~~//
        else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && !anim.GetCurrentAnimatorStateInfo(0).IsName("PunchLeft"))
            anim.Play("PunchLeft");
        //~~~~~~    ����� �� �����, �� ������ �������� �����������, ������ �� ������� � �� �������    ~~~~~~//
        else if (!lastW && !anim.GetCurrentAnimatorStateInfo(0).IsName("Jump_Down") && !isJumpNow && !anim.GetCurrentAnimatorStateInfo(0).IsName("PunchLeft"))
            Stand();


        //~~~~~~    ������ � ���    ~~~~~~//
        if (Input.GetAxis("Vertical") > 0)
            Walk();
        else if (Input.GetAxis("Vertical") <= 0)  // ������� W �� ����������
            lastW = false;
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))  // ��� ����������
            anim.SetBool("isRun", false);


        //~~~~~~    �������    ~~~~~~//
        if (Input.GetKey(KeyCode.D))
            transform.Rotate(Vector3.up, 90 * Time.deltaTime, Space.Self);
        if (Input.GetKey(KeyCode.A))
            transform.Rotate(Vector3.up, -90 * Time.deltaTime, Space.Self);


        //~~~~~~    ������    ~~~~~~//
        if (Input.GetKeyDown(KeyCode.Space) && !isJumpNow && isStandNow)
        {
            anim.Play("Jump_Up");
            rb.AddForce(Vector3.up * 40, ForceMode.Impulse);  // ����� �������� ����� ������� ����� ������
            isJumpNow = true;  // ������ �������
        }
    }

    private void Falling()
    {
        //~~~~~~    ������������ ���������� �� ������ �� �����    ~~~~~~//
        Ray ray = new(pivot.transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //~~~~~~    ���� ���������� ������, ��� ����������� + 3,17, �� ������������� �������� �������    ~~~~~~//
            if (Vector3.Distance(pivot.transform.position, hit.point) <= initialLengthFromGroundToPivot + 3.17  && !isGroundingNow)
            {
                isGroundingNow = true;  // ������ ������������
                anim.Play("Jump_Down");
            }
            else if (!isGroundingNow)  // ����� ��������������� ��������� �������
            {
                anim.Play("FallingLoop");
            }
        }
        else anim.Play("FallingLoop");  // ���� ��� �� ������� �� �������� ����-���� - ��������������� �������� �������

        isJumpNow = false;  // ������ ������ �� �������
        isStandNow = false;  // ������ �� �����
    }

    private void Stand()
    {
        //~~~~~~    ������ �� ����� � �� ���    ~~~~~~//
        anim.SetBool("isWalk", false);
        anim.SetBool("isRun", false);

        anim.Play("Idle");
        isGroundingNow = false;  // ������ �� ������������

        isWalkNow = false;  // ������ �� ���
        isRunNow = false;  // ������ �� �����
        isStandNow = true;  // ������ �����

        //~~~~~~    ���������� ���������� �� ������ �� �����    ~~~~~~//
        Ray ray = new(pivot.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit))
            initialLengthFromGroundToPivot = Vector3.Distance(pivot.transform.position, hit.point);
    }

    private void Walk()
    {
        //~~~~~~    ���� ������ ����� (�� ���, �� �����, �� �������)    ~~~~~~//
        if (!isWalkNow && !isRunNow && !isJumpNow)
        {
            anim.SetBool("isWalk", true);  // ������ ��������
            isWalkNow = true;  // ������ ���
        }
        //~~~~~~    �������� �������    ~~~~~~//
        transform.position += vector.transform.position - transform.position;  // ����� ����� ������������ ������ ����� ������ ��� �������
        lastW = true;  // ������� W ���� ������

        //~~~~~~    ���    ~~~~~~//
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            isWalkNow = false;
            if (!isRunNow && !isJumpNow)
            {
                anim.SetBool("isRun", true);
                isRunNow = true;
            }
            transform.position += (vector.transform.position - transform.position) * 1.0008f;  // ����� ����� ������������ ������ ����� ������ ��� �������
        }
        else if (isRunNow)  // ���� shift �� ����� � ������ �����
        {
            isRunNow = false;
            anim.SetBool("isRun", false);;
        }
    }
}
