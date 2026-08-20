using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Alvo da Câmera")]
    public Transform alvo; // Arraste o "Modelo" do seu Player aqui

    [Header("Posicionamento")]
    public float distancia = 4f; // Distância nas costas
    public float altura = 1.5f;  // Altura acima do ombro/cabeça

    [Header("Controle de Delay")]
    [Tooltip("Tempo (em segundos) que a câmera demora para alinhar nas costas")]
    [Range(0f, 2f)] // Cria uma barrinha (slider) no Inspector de 0 a 2 segundos
    public float tempoDeDelay = 0.3f;

    // Variáveis internas para a matemática funcionar
    private float velocidadeGiroAtual;
    private float anguloAtualY;

    void Start()
    {
        // Ao começar o jogo, a câmera já pega o ângulo inicial do jogador
        if (alvo != null)
        {
            anguloAtualY = alvo.eulerAngles.y;
        }
    }

    void LateUpdate()
    {
        if (alvo == null) return;

        // 1. O SEGREDO DO DELAY: Suaviza o ângulo exato baseado no tempo configurado
        anguloAtualY = Mathf.SmoothDampAngle(
            anguloAtualY,
            alvo.eulerAngles.y,
            ref velocidadeGiroAtual,
            tempoDeDelay
        );

        // 2. Converte esse ângulo calculado em uma rotação (Orbit)
        Quaternion rotacaoOrbita = Quaternion.Euler(0f, anguloAtualY, 0f);

        // 3. SEGUE SEM ATRASO: Posiciona no jogador, mas usando o ângulo suavizado
        Vector3 posicaoCentro = alvo.position + (Vector3.up * altura);
        transform.position = posicaoCentro - (rotacaoOrbita * Vector3.forward * distancia);

        // 4. Garante que a lente da câmera sempre aponte para o centro do jogador
        transform.LookAt(posicaoCentro);
    }
}