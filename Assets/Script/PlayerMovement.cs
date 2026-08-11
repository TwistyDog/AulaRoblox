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
        // Entrada
        float horizontal = _moveInput.x;
        float vertical = _moveInput.y;

        Vector3 movimento = new Vector3(horizontal, 0, vertical);

        // Evita velocidade maior na diagonal
        movimento = Vector3.ClampMagnitude(movimento, 1f);

        // Movimento relativo à direção do personagem
        if (movimento.magnitude > 0.1f)
        {
            Vector3 direcao = transform.TransformDirection(movimento);

            controller.Move(direcao * velocidade * Time.deltaTime);
        }

        // Verifica se está no chão
        if (controller.isGrounded)
        {
            if (velocidadeVertical.y < 0)
                velocidadeVertical.y = -2f;

           
        }

        // Gravidade
        velocidadeVertical.y += gravidade * Time.deltaTime;

        controller.Move(velocidadeVertical * Time.deltaTime);
    }
}