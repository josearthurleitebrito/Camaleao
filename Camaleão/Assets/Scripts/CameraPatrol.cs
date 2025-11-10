using UnityEngine;
using UnityEngine.Rendering.Universal; // Necessário para acessar o Light 2D

public class CameraPatrol : MonoBehaviour
{
    [Header("Configuração de Movimento")]
    // Eixo Z é a rotação em 2D
    public float anguloInicial = 45f; 
    public float anguloFinal = -45f;
    public float velocidadeRotacao = 1.5f;

    [Header("Configuração de Detecção")]
    public float tempoDetecao = 0.5f; // Tempo para o jogador ser detectado
    private float tempoPassado = 0f;
    
    private bool jogadorDetectado = false;

    // Componentes
    private Transform coneVisao; // O Transform do objeto filho Cone_Visao
    private Light2D luzCone;     // O componente Light2D

    void Start()
    {
        // Encontra o objeto filho que tem o nome "Cone_Visao"
        coneVisao = transform.Find("Cone_Visao"); 
        if (coneVisao == null)
        {
            Debug.LogError("Objeto filho 'Cone_Visao' não encontrado. Verifique a hierarquia!");
            return;
        }

        // Obtém o Light2D para mudar a cor
        luzCone = coneVisao.GetComponent<Light2D>();
        
        // Define a rotação inicial para começar o ciclo
        coneVisao.localRotation = Quaternion.Euler(0, 0, anguloInicial);
    }

    void Update()
    {
        // 1. Lógica de Animação (Rotação Contínua)
        RotacionarCone();

        // 2. Lógica de Detecção
        if (jogadorDetectado)
        {
            tempoPassado += Time.deltaTime;
            
            // Aqui você chamaria o método global para aumentar a Suspeita
            // Exemplo: SuspectMeter.instance.AumentarSuspeita(tempoPassado);

            // Feedback Visual: Piscar/Mudar a Cor da Luz para Vermelho
            luzCone.color = Color.Lerp(Color.red, Color.yellow, Mathf.PingPong(Time.time * 4, 1));

            if (tempoPassado >= tempoDetecao)
            {
                // Após o tempo limite, se o jogador não sair, Game Over ou Fuga
                Debug.Log("JOGADOR DETECTADO PELA CÂMERA! INÍCIO DA PERSEGUIÇÃO/FALHA NA MISSÃO.");
                // Adicione a transição para a tela Game Over aqui.
            }
        }
    }

    // Método para fazer o cone rotacionar continuamente entre dois ângulos
    private void RotacionarCone()
    {
        // Usa Mathf.PingPong para criar um movimento suave de ida e volta
        float t = Mathf.PingPong(Time.time * velocidadeRotacao, 1f);
        
        // Interpola a rotação entre os ângulos definidos
        float zRotation = Mathf.Lerp(anguloInicial, anguloFinal, t);
        
        coneVisao.localRotation = Quaternion.Euler(0, 0, zRotation);
    }
    
    // Método chamado quando algo entra no Collider de Detecção (Trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorDetectado = true;
            tempoPassado = 0f; // Reinicia o contador para dar um tempo de reação
        }
    }

    // Método chamado quando algo sai do Collider de Detecção
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorDetectado = false;
            luzCone.color = Color.yellow; // Retorna à cor normal (Amarela/Âmbar)
        }
    }
}