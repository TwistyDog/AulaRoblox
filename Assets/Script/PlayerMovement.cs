using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 5f;
    public float velocidadeRotacao = 10f;
    public float alturaPulo = 1.5f;
    public float gravidade = -20f;

    private CharacterController controller;
    public Vector3 velocidadeVertical;
    private Vector3 _moveInput;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
    }

    public void MovePerson(InputAction.CallbackContext value)
    {
        _moveInput = value.ReadValue<Vector3>();
    }

    public void JumpPerson(InputAction.CallbackContext value)
    {
        velocidadeVertical.y = Mathf.Sqrt(alturaPulo * -2f * gravidade);
    }

    void Move()
    {
        // Frente / Trás
        float frenteTras = _moveInput.y;

        // Esquerda / Direita = Rotação
        float esquerdaDireita = _moveInput.x;

        // =========================
        // ROTAÇÃO
        // =========================

        if (Mathf.Abs(esquerdaDireita) > 0.01f)
        {
            float rotacao = esquerdaDireita * velocidadeRotacao * Time.deltaTime;

            transform.Rotate(0f, rotacao, 0f);
        }

        // =========================
        // MOVIMENTO
        // =========================

        Vector3 movimento = transform.forward * frenteTras;

        controller.Move(
            movimento * velocidade * Time.deltaTime
        );

        // =========================
        // CHÃO
        // =========================

        if (controller.isGrounded)
        {
            if (velocidadeVertical.y < 0)
            {
                velocidadeVertical.y = -2f;
            }
        }

        // =========================
        // GRAVIDADE
        // =========================

        velocidadeVertical.y += gravidade * Time.deltaTime;

        controller.Move(
            velocidadeVertical * Time.deltaTime
        );
    }

    }