using UnityEngine;

public class HumanScript : MonoBehaviour
{
    Animator anim;
    GameObject vector;                      // Определяет куда направлен персонаж (GameObject внутри HumanMale_Character_FREE)
    GameObject pivot;                       // Точка примерно в середине персонажа (за основу берётся точка от объекта Mesh внутри HumanMale_Character_FREE)
    Rigidbody rb;
    bool isWalkNow;                         // Идёт ли персонаж сейчас
    bool isRunNow;                          // Бежит ли персонаж сейчас
    bool isGroundingNow;                    // Приземляется ли персонаж сейчас
    bool isJumpNow;                         // Прыгнул ли персонаж только что
    bool isStandNow;                        // Стоит ли персонаж сейчас
    bool lastW;                             // Было ли нажатие W недавно
    float initialLengthFromGroundToPivot;   // Отслеживает расстояние от пивота до земли пока персонаж стоит

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
        //~~~~~~    Падение    ~~~~~~//
        if (rb.velocity.y < -0.1)
            Falling();
        //~~~~~~    Атака    ~~~~~~//
        else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && !anim.GetCurrentAnimatorStateInfo(0).IsName("PunchLeft"))
            anim.Play("PunchLeft");
        //~~~~~~    Стоит на месте, не играет анимация приземления, объект не прыгнул и не атакует    ~~~~~~//
        else if (!lastW && !anim.GetCurrentAnimatorStateInfo(0).IsName("Jump_Down") && !isJumpNow && !anim.GetCurrentAnimatorStateInfo(0).IsName("PunchLeft"))
            Stand();


        //~~~~~~    Ходьба и бег    ~~~~~~//
        if (Input.GetAxis("Vertical") > 0)
            Walk();
        else if (Input.GetAxis("Vertical") <= 0)  // Клавиша W не нажимается
            lastW = false;
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))  // Бег остановлен
            anim.SetBool("isRun", false);


        //~~~~~~    Поворот    ~~~~~~//
        if (Input.GetKey(KeyCode.D))
            transform.Rotate(Vector3.up, 90 * Time.deltaTime, Space.Self);
        if (Input.GetKey(KeyCode.A))
            transform.Rotate(Vector3.up, -90 * Time.deltaTime, Space.Self);


        //~~~~~~    Прыжок    ~~~~~~//
        if (Input.GetKeyDown(KeyCode.Space) && !isJumpNow && isStandNow)
        {
            anim.Play("Jump_Up");
            rb.AddForce(Vector3.up * 40, ForceMode.Impulse);  // Задаёт движение вверх объекту через физику
            isJumpNow = true;  // Объект прыгает
        }
    }

    private void Falling()
    {
        //~~~~~~    Отслеживание расстояния от пивота до земли    ~~~~~~//
        Ray ray = new(pivot.transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //~~~~~~    Если расстояние меньше, чем изначальное + 3,17, то проигрывается анимация падения    ~~~~~~//
            if (Vector3.Distance(pivot.transform.position, hit.point) <= initialLengthFromGroundToPivot + 3.17  && !isGroundingNow)
            {
                isGroundingNow = true;  // Объект приземляется
                anim.Play("Jump_Down");
            }
            else if (!isGroundingNow)  // Иначе воспроизводится анимацция падения
            {
                anim.Play("FallingLoop");
            }
        }
        else anim.Play("FallingLoop");  // Если луч от объекта не попадает куда-либо - воспроизводится анимация падения

        isJumpNow = false;  // Объект больше не прыгает
        isStandNow = false;  // Объект не стоит
    }

    private void Stand()
    {
        //~~~~~~    Объект не бежит и не идёт    ~~~~~~//
        anim.SetBool("isWalk", false);
        anim.SetBool("isRun", false);

        anim.Play("Idle");
        isGroundingNow = false;  // Объект не приземляется

        isWalkNow = false;  // Объект не идёт
        isRunNow = false;  // Объект не бежит
        isStandNow = true;  // Объект стоит

        //~~~~~~    Обновление расстояния от пивота до земли    ~~~~~~//
        Ray ray = new(pivot.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit))
            initialLengthFromGroundToPivot = Vector3.Distance(pivot.transform.position, hit.point);
    }

    private void Walk()
    {
        //~~~~~~    Если объект стоит (не идёт, не бежит, не прыгает)    ~~~~~~//
        if (!isWalkNow && !isRunNow && !isJumpNow)
        {
            anim.SetBool("isWalk", true);  // Запуск анимации
            isWalkNow = true;  // Объект идёт
        }
        //~~~~~~    Смещение позиции    ~~~~~~//
        transform.position += vector.transform.position - transform.position;  // Обект будет перемещаться дажево время прыжка или падения
        lastW = true;  // Клавиша W была нажата

        //~~~~~~    Бег    ~~~~~~//
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            isWalkNow = false;
            if (!isRunNow && !isJumpNow)
            {
                anim.SetBool("isRun", true);
                isRunNow = true;
            }
            transform.position += (vector.transform.position - transform.position) * 1.0008f;  // Обект будет перемещаться дажево время прыжка или падения
        }
        else if (isRunNow)  // Если shift не нажат и объект бежал
        {
            isRunNow = false;
            anim.SetBool("isRun", false);;
        }
    }
}
