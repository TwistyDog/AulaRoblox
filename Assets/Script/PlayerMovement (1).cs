using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // =========================================================
    // CONFIGURAÇÕES DO MOVIMENTO
    // =========================================================

    [Header("Movimento")]

    // Velocidade que o personagem vai andar
    public float velocidade = 5f;


    // =========================================================
    // CONFIGURAÇÕES DO PULO
    // =========================================================

    [Header("Pulo")]

    // Altura máxima aproximada do pulo
    public float alturaPulo = 1.5f;

    // Força da gravidade
    // Quanto mais negativo, mais rápido o personagem cai
    public float gravidade = -20f;


    // =========================================================
    // CONFIGURAÇÕES DA ROTAÇÃO
    // =========================================================

    [Header("Rotação")]

    // Sensibilidade do mouse para girar o personagem
    public float sensibilidadeMouse = 2f;


    // =========================================================
    // COMPONENTES
    // =========================================================

    // Componente CharacterController usado para movimentar
    // o personagem sem precisar utilizar Rigidbody
    private CharacterController controller;


    // =========================================================
    // VARIÁVEIS DE CONTROLE
    // =========================================================

    // Guarda a velocidade vertical do personagem.
    // É utilizada principalmente para controlar o pulo e a gravidade.
    [SerializeField]
    private Vector3 velocidadeVertical;


    // Guarda o comando recebido do teclado/controle.
    //
    // X = esquerda / direita
    // Y = não utilizado
    // Z = frente / trás
    [SerializeField]
    private Vector3 _moveInput;


    // Guarda o movimento recebido pelo mouse.
    //
    // X = movimento horizontal do mouse
    // Y = movimento vertical do mouse
    [SerializeField]
    private Vector2 mouseInput;

    [SerializeField]
    private bool _checkAttack;

    Animator _anim;


    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        _anim = GetComponent<Animator>();
        // Pega o CharacterController que está no mesmo GameObject
        controller = GetComponent<CharacterController>();


        // Prende o mouse no centro da tela
        Cursor.lockState = CursorLockMode.Locked;

        // Esconde o cursor
        Cursor.visible = false;

    }


    // =========================================================
    // UPDATE
    // =========================================================

    void Update()
    {
        // Verifica o movimento do mouse
        // e gira o personagem
        RotateMouse();


        // Executa o movimento do personagem
        // e também a gravidade
        Move();
    }


    // =========================================================
    // INPUT DO TECLADO / CONTROLE
    // =========================================================

    public void MovePerson(InputAction.CallbackContext value)
    {
        // Lê o valor do Input Action.
        //
        // Como nosso Movimento é Vector2:
        //
        // X = esquerda / direita
        // Y = frente / trás
        Vector3 input = value.ReadValue<Vector3>();


        // Converte o Vector2 para Vector3.
        //
        // X = esquerda / direita
        // Y = 0, porque o teclado não controla a altura
        // Z = frente / trás
        _moveInput = new Vector3(
            input.x,
            0f,
            input.y
        );
    }


    // =========================================================
    // INPUT DO MOUSE
    // =========================================================

    public void LookPerson(InputAction.CallbackContext value)
    {
        // Recebe o movimento do mouse.
        //
        // X = mouse para esquerda/direita
        // Y = mouse para cima/baixo
        mouseInput = value.ReadValue<Vector2>();
    }

    public void AtaquePerson(InputAction.CallbackContext value)
    {
        if (!_checkAttack)
        {
            _checkAttack = true;
          _anim.SetBool("Attack", true);

        }
    }

    public void CheckAtaqueFase()
    {
        _checkAttack = false;
        _anim.SetBool("Attack", false);
    }


    // =========================================================
    // ROTAÇÃO DO PERSONAGEM
    // =========================================================

    void RotateMouse()
    {
        // Pegamos somente o movimento horizontal do mouse.
        //
        // Mouse para esquerda/direita
        float mouseX = mouseInput.x * sensibilidadeMouse;


        // Rotaciona o personagem no eixo Y.
        //
        // X = 0
        // Y = rotação horizontal
        // Z = 0
        transform.Rotate(
            0f,
            mouseX,
            0f
        );
    }


    // =========================================================
    // MOVIMENTO DO PERSONAGEM
    // =========================================================

    void Move()
    {
        // =====================================================
        // PEGA O INPUT
        // =====================================================

        // X representa:
        // A / D
        // esquerda / direita
        float horizontal = _moveInput.x;


        // Z representa:
        // W / S
        // frente / trás
        float vertical = _moveInput.z;


        // =====================================================
        // CRIA A DIREÇÃO DO MOVIMENTO
        // =====================================================

        // transform.right representa o lado direito
        // do personagem.
        //
        // Por isso:
        // A = esquerda
        // D = direita
        //
        // transform.forward representa a frente
        // do personagem.
        //
        // Por isso:
        // W = frente
        // S = trás
        Vector3 movimento =
            transform.right * horizontal +
            transform.forward * vertical;


        // =====================================================
        // NORMALIZAÇÃO
        // =====================================================

        // Quando o jogador aperta duas teclas ao mesmo tempo,
        // por exemplo W + D, o personagem poderia andar
        // mais rápido na diagonal.
        //
        // Normalize impede isso.
        if (movimento.magnitude > 1f)
        {
            movimento.Normalize();
        }


        // =====================================================
        // MOVIMENTA O PERSONAGEM
        // =====================================================

        // Move o CharacterController.
        //
        // velocidade = velocidade do personagem
        // Time.deltaTime = deixa o movimento independente
        // da quantidade de FPS.
        controller.Move(
            movimento * velocidade * Time.deltaTime
        );


        // =====================================================
        // VERIFICA SE ESTÁ NO CHÃO
        // =====================================================

        if (controller.isGrounded)
        {
            // Se estiver caindo e tocar no chão,
            // colocamos uma pequena velocidade para baixo.
            //
            // Isso ajuda o CharacterController a permanecer
            // corretamente encostado no chão.
            if (velocidadeVertical.y < 0)
            {
                velocidadeVertical.y = -2f;
            }
        }


        // =====================================================
        // GRAVIDADE
        // =====================================================

        // A cada frame aumentamos a velocidade de queda.
        //
        // Como gravidade é negativa:
        //
        // -20
        // -40
        // -60...
        //
        // O personagem vai caindo cada vez mais rápido.
        velocidadeVertical.y += gravidade * Time.deltaTime;


        // =====================================================
        // APLICA A GRAVIDADE
        // =====================================================

        // Movimenta o personagem verticalmente.
        //
        // Aqui acontece:
        // - queda
        // - pulo
        // - gravidade
        controller.Move(
            velocidadeVertical * Time.deltaTime
        );
    }


    // =========================================================
    // PULO
    // =========================================================

    public void JumpPerson(InputAction.CallbackContext value)
    {
        // value.performed verifica se o botão de pulo
        // foi realmente pressionado.
        //
        // controller.isGrounded verifica se o personagem
        // está no chão.
        //
        // Assim, o jogador não consegue pular no ar.
        if (value.performed && controller.isGrounded)
        {
            // Calcula a força necessária para atingir
            // a altura definida em alturaPulo.
            //
            // Mathf.Sqrt = raiz quadrada
            //
            // O resultado é colocado no eixo Y,
            // fazendo o personagem subir.
            velocidadeVertical.y =
                Mathf.Sqrt(
                    alturaPulo * -2f * gravidade
                );
        }
    }
}