using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  public Animator animator;
    private CharacterController controller; 
    private Vector3 direction;
    public float forwardSpeed;
    public float maxSpeed;
    private int desiredLane = 1;//0:left 1:middle 2:right
    public float laneDistance = 4;//the distance between two lanes

    public float jumpForce;
    public float Gravity = -20;
    
     public bool isGrounded;
      public LayerMask groundLayer;
    public Transform groundCheck;
    void Start()
    {
      controller = GetComponent<CharacterController>();
        
    }

    // Update is called once per frame
    void Update()
{
    if (!PlayerManager.isGameStarted)
        return;

    if(forwardSpeed < maxSpeed)
    forwardSpeed += 0.1f * Time.deltaTime;

    animator.SetBool("isGameStarted", true);
    direction.z = forwardSpeed;
    direction.y += Gravity * Time.deltaTime;

    // Verificar se o jogador está no chão
    isGrounded = Physics.CheckSphere(groundCheck.position, 0.17f, groundLayer);

    // Condição para animar apenas ao pular
    if (Input.GetKeyDown(KeyCode.UpArrow) && controller.isGrounded)
    {
        Jump();
        animator.SetBool("isGrounded", false); // Atualiza animação apenas ao pular
    }
    else if (controller.isGrounded)
    {
        animator.SetBool("isGrounded", true); // Garante que a animação só ativa ao pousar após pular
    }

    // Inputs para mover entre as pistas
    if (Input.GetKeyDown(KeyCode.DownArrow)){

      StartCoroutine(Slide());
    }






    if (Input.GetKeyDown(KeyCode.RightArrow))
    {
        desiredLane++;
        if (desiredLane == 3)
            desiredLane = 2;
    }
    else if (Input.GetKeyDown(KeyCode.LeftArrow))
    {
        desiredLane--;
        if (desiredLane == -1)
            desiredLane = 0;
    }

    // Calcular a posição futura
    Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;

    if (desiredLane == 0)
    {
        targetPosition += Vector3.left * laneDistance;
    }
    else if (desiredLane == 2)
    {
        targetPosition += Vector3.right * laneDistance;
    }

    if (transform.position == targetPosition)
        return;

    Vector3 diff = targetPosition - transform.position;
    Vector3 moveDir = diff.normalized * 25 * Time.deltaTime;
    if (moveDir.sqrMagnitude < diff.sqrMagnitude)
        controller.Move(moveDir);
    else
        controller.Move(diff);
}



    private void FixedUpdate(){

      if(!PlayerManager.isGameStarted)
    return;
      controller.Move(direction * Time.fixedDeltaTime);  
    }

      private void Jump(){
direction.y =  jumpForce;
      }

   private void OnControllerColliderHit(ControllerColliderHit hit)
{
    if(hit.transform.tag == "Obstacle"){
      PlayerManager.gameOver = true;
      FindObjectOfType<AudioManager>().PlaySound("GameOver");
    }
}

private IEnumerator Slide(){
  animator.SetBool("isSliding", true);
  controller.center = new Vector3(0, -0.5f, 0);
  controller.height = 1;
  yield return new WaitForSeconds(1.3f);
  controller.center = new Vector3(0, 0, 0);
  controller.height = 2;
 animator.SetBool("isSliding", false);

}
}

