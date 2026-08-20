using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 5f;

    [Header("Rotação do Filho")]
    public float velocidadeRotacao = 120f;

    [Header("Pulo")]
    public float alturaPulo = 1.5f;
    public float gravidade = -20f;

    [Header("Referência")]
    public Transform modelo; // Filho que vai girar

    private CharacterController controller;
    private Animator _animator;

    private Vector3 velocidadeVertical;

    // X = A/D
    // Y = W/S
    public Vector3 moveInput;

    private bool _checkAtaque;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        RotacaoFilho();
        Movimento();
    }


    // =====================================================
    // INPUT
    // =====================================================

    public void MovePerson(InputAction.CallbackContext value)
    {
        moveInput = value.ReadValue<Vector3>();
    }


    // =====================================================
    // ROTAÇÃO DO FILHO
    // =====================================================

    void RotacaoFilho()
    {
        float rotacao = moveInput.x * velocidadeRotacao;

        modelo.Rotate(
            0f,
            rotacao * Time.deltaTime,
            0f
        );
    }


    // =====================================================
    // MOVIMENTO DO PAI
    // =====================================================

    void Movimento()
    {
        // W/S
        float frenteTras = moveInput.y;

        // Anda na direção para onde o FILHO está olhando
        Vector3 movimento = modelo.forward * frenteTras;

        if (movimento.magnitude > 1f)
        {
            movimento.Normalize();
        }

        controller.Move(
            movimento * velocidade * Time.deltaTime
        );

        // Gravidade
        if (controller.isGrounded)
        {
            if (velocidadeVertical.y < 0)
            {
                velocidadeVertical.y = -2f;
            }
        }

        velocidadeVertical.y += gravidade * Time.deltaTime;

        controller.Move(
            velocidadeVertical * Time.deltaTime
        );
    }

    public void JumpPerson(InputAction.CallbackContext value)
    {
        if (value.performed && controller.isGrounded)
        {
            velocidadeVertical.y =
                Mathf.Sqrt(
                    alturaPulo * -2f * gravidade
                );
        }
    }


    // =====================================================
    // ATAQUE
    // =====================================================

    public void AtaquePerson(InputAction.CallbackContext value)
    {
        if (!_checkAtaque)
        {
            _checkAtaque = true;

            _animator.SetBool("Ataque", true);
        }
    }


    public void CheckAtaqueFase()
    {
        _checkAtaque = false;

        _animator.SetBool("Ataque", false);
    }
}