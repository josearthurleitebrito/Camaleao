using UnityEngine;

public class ObraDeArte : MonoBehaviour
{
    [Header("Conteúdo da Obra")]
    [TextArea(3, 5)] // Campo de texto maior para a história
    public string historia;
    
    [Header("Configurações")]
    public KeyCode teclaDeInteracao = KeyCode.E;
    public float tempoDeExibicao = 5f; 

    private bool _podeInteragir = false; // Flag para saber se o Player está dentro do Trigger
    private const string PlayerTag = "Player";

    void Update()
    {
        // Só verifica o input se o Player estiver dentro da área e pressionar a tecla correta
        if (_podeInteragir && Input.GetKeyDown(teclaDeInteracao))
        {
            ExibirHistoria();
        }
    }

    // O Player entrou na área de colisão (Trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PlayerTag))
        {
            _podeInteragir = true;
            
            // Dica: Chame o UIManager aqui para mostrar um ícone "Aperte E para Escanear"
            Debug.Log("PRONTO PARA INTERAGIR! Aperte " + teclaDeInteracao);
        }
    }

    // O Player saiu da área de colisão (Trigger)
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(PlayerTag))
        {
            _podeInteragir = false;
            EsconderHistoria();
            
            // Dica: Chame o UIManager aqui para esconder o ícone de interação
            Debug.Log("Saiu da área. Interação desativada.");
        }
    }

    void ExibirHistoria()
    {
        // Implementação de UI necessária aqui! 
        
        // POR ENQUANTO: Exibimos no Console para testar a lógica
        Debug.Log("--- HISTÓRIA DA OBRA (" + gameObject.name + ") ---");
        Debug.Log("História: " + historia);

        // Define o tempo que o texto ficará visível
        CancelInvoke("EsconderHistoria"); 
        Invoke("EsconderHistoria", tempoDeExibicao); 
    }

    void EsconderHistoria()
    {
        // Chamada para esconder o texto da UI.
        Debug.Log("História escondida.");
    }
}