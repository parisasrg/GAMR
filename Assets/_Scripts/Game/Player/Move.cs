using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    PlayerControls controls;
    WeaponAttack wa;
    private Animator animator;
    Vector2 move;

    float rotationSpeed = 720;
    private int speed = 1;

    public float jump;
    bool isGrounded;
    int jumpCount = 0;
    private float distanceToGround;
    public Vector3 dodge = new Vector3(1, 1, 1);

    Rigidbody rb;

    public int gravityDelay =5;

    private bool isRunning = false;
    private bool isAttacking = false;
    [SerializeField] private float animationFinishTime = 0.9f;

    public GameObject cam;
    float heading;

    public GameObject sword;

    private void Awake() {

        isGrounded = true;
        distanceToGround = GetComponent<Collider>().bounds.extents.y;

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        wa = sword.GetComponent<WeaponAttack>();

        controls = new PlayerControls();

        controls.Player.Movement.performed += cntxt => move = cntxt.ReadValue<Vector2>();
        controls.Player.Movement.canceled += cntxt => move = Vector2.zero;
        controls.Player.Attack.performed += cntxt => PlayerAttack();
        controls.Player.Dodge.performed += cntxt => PlayerDodge();
        controls.Player.Interact.performed += cntxt => PickUpObject.instance.PlayerPickUp();

        // StartCoroutine(WaitForGravity());
    }

    void Update()
    {
        // Vector3 m = new Vector3(move.x, 0, move.y);

        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 m = forward * move.y + right * move.x;
        m.Normalize();
        
        
        transform.Translate(m * speed * Time.deltaTime, Space.World);

        if (m != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(m, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        AnimateRun(m);

        if(isAttacking && animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= animationFinishTime)
        {
            isAttacking = false;
        }

        isGrounded = Physics.Raycast(transform.position, -Vector3.up, distanceToGround);
        // isGrounded = Physics.Raycast(transform.position, -Vector3.up, distanceToGround + 0.1f);
        if(isGrounded)
        {
            jumpCount = 0;
        }
    }

    private void OnEnable() {
        controls.Player.Enable();
    }

    private void OnDisable() {
        controls.Player.Disable();
    }

    public void PlayerJump(InputAction.CallbackContext context) {
        if(context.performed)
        {
            if(isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jump);
                isGrounded = false;
                jumpCount += 1;
            }
            else if(!isGrounded && jumpCount < 1)
            {
                rb.velocity = new Vector2(rb.velocity.x, jump);
                isGrounded = false;
                jumpCount += 1;
            }
        }
    }

    public void PlayerDodge() {
        rb.AddForce(-transform.forward * dodge.z + transform.up * dodge.y, ForceMode.Impulse);
    }

    public void PlayerAttack() {
        if(!isAttacking)
        {
            animator.SetTrigger("isAttacking");
            StartCoroutine(InitialiseAttack());
        }
    }

    IEnumerator InitialiseAttack()
    {
        yield return new WaitForSeconds(.1f);
        wa.OnAttack();
        isAttacking = true;
    }

    void AnimateRun(Vector3 m)
    {
        isRunning = (m.x > 0.1f || m.x < -0.1f) || (m.z > 0.1f || m.z < -0.1f) ? true : false;
        animator.SetBool("isRunning", isRunning);
    }

    IEnumerator WaitForGravity()
    {
        rb.useGravity = false;
        yield return new WaitForSeconds(gravityDelay);
        rb.useGravity = true;
    }
}
