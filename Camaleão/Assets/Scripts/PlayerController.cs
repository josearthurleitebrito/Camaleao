using UnityEngine;

// Removidas referências não utilizadas (mantido apenas UnityEngine).

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _playerRigidbody2D;
    private Animator _playerAnimator;

    // --- VARIÁVEIS DE VELOCIDADE PÚBLICAS ---
    [Header("Configurações de Velocidade")]
    public float _playerRunSpeed = 8f;      // Velocidade de Corrida (Ex: Shift)
    public float _playerNormalSpeed = 5f;   // Velocidade Padrão
    public float _playerSlowSpeed = 2f;     // Velocidade Lenta (Stealth)

    // --- CONTROLE DE ESTADO ---
    private float _currentSpeed;          // Velocidade sendo usada no momento
    
    // Variável para armazenar o Input LIDO NO UPDATE()
    private Vector2 _rawInput;
    
    // Variáveis para persistência da direção
    private float _lastMoveX;
    private float _lastMoveY;

    // A variável _playerInitialSpeed foi removida, pois _playerNormalSpeed atende à mesma função.

    // --- CONFIGURAÇÃO DE TECLAS ---
    [Header("Configuração de Teclas")]
    [Tooltip("Tecla ou botão para andar mais devagar (modo Stealth)")]
    public KeyCode _slowMoveKey = KeyCode.LeftControl; 
    [Tooltip("Tecla ou botão para Correr (Run)")]
    public KeyCode _runKey = KeyCode.LeftShift;

    void Start()
    {
        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();

        // Define a velocidade atual como a normal.
        _currentSpeed = _playerNormalSpeed;
        
        // Configura o Rigidbody para interpolação para ajudar a suavizar o movimento visual (se não tiver feito no Editor).
        // if (_playerRigidbody2D != null)
        // {
        //     _playerRigidbody2D.interpolation = RigidbodyInterpolation2D.Extrapolate; 
        // }
    }

    void Update()
    {
        // 1. LER INPUT NO UPDATE (frequência alta)
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        _rawInput = new Vector2(horizontalInput, verticalInput).normalized;

        // 2. CONTROLAR VELOCIDADE E ANIMAÇÕES NO UPDATE
        ControlPlayerSpeed();
    }

    void FixedUpdate()
    {
        // VARIÁVEL LOCAL PARA MOVIMENTO BASEADA NO INPUT LIDO NO UPDATE
        Vector2 movementVector = _rawInput;

        // 1. Lógica de Movimento
        if (movementVector.sqrMagnitude > 0.01f) // Usa 0.01f para maior precisão
        {
            // Movimenta o jogador
            MovePlayer(movementVector);

            // Seta os parâmetros do Animator
            _playerAnimator.SetFloat("AxisX", movementVector.x);
            _playerAnimator.SetFloat("AxisY", movementVector.y);
            
            // Armazenar a última direção válida quando o jogador está se movendo
            _lastMoveX = movementVector.x;
            _lastMoveY = movementVector.y;
            
            _playerAnimator.SetInteger("Movimento", 1); // Animação de Movimento
        }
        else
        {
            // QUANDO PARAR:
            _playerAnimator.SetInteger("Movimento", 0); // Animação de Idle
            
            // Usa a última direção para a Blend Tree de Idle
            _playerAnimator.SetFloat("LastMoveX", _lastMoveX);
            _playerAnimator.SetFloat("LastMoveY", _lastMoveY);
        }
    }

    /// <summary>
    /// Altera a velocidade do jogador entre Lenta, Normal e Correr, com base nas teclas.
    /// A ordem de prioridade é: Lenta > Correr > Normal.
    /// </summary>
    void ControlPlayerSpeed()
    {
        // 1. Prioridade: Movimento Lento (Stealth)
        if (Input.GetKey(_slowMoveKey))
        {
            _currentSpeed = _playerSlowSpeed;
        }
        // 2. Próxima Prioridade: Corrida
        else if (Input.GetKey(_runKey))
        {
            _currentSpeed = _playerRunSpeed;
        }
        // 3. Padrão: Movimento Normal
        else
        {
            _currentSpeed = _playerNormalSpeed;
        }
    }

    /// <summary>
    /// Aplica a velocidade de movimento ao Rigidbody2D.
    /// </summary>
    void MovePlayer(Vector2 direction)
    {
        // Usa a velocidade atual (_currentSpeed)
        _playerRigidbody2D.MovePosition(_playerRigidbody2D.position + direction * _currentSpeed * Time.fixedDeltaTime);
    }
}