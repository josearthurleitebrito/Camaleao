using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _playerRigidbody2D;
    private Animator _playerAnimator;

    // Váriaveis de configuração de velocidade
    [Header("Configurações de Velocidade")]
    public float _playerRunSpeed;      
    public float _playerNormalSpeed;   
    public float _playerSlowSpeed;     
    private float _currentSpeed;          
    private Vector2 _rawInput;
    
    // Variáveis para persistência da direção
    private float _lastMoveX;
    private float _lastMoveY;

    // Configuração de teclas
    [Header("Configuração de Teclas")]
    [Tooltip("Tecla ou botão para andar mais devagar (modo Stealth)")]
    public KeyCode _slowMoveKey = KeyCode.LeftControl; 
    [Tooltip("Tecla ou botão para Correr (Run)")]
    public KeyCode _runKey = KeyCode.LeftShift;

    void Start()
    {
        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();

        _currentSpeed = _playerNormalSpeed; // Define a velocidade atual como a normal.
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        _rawInput = new Vector2(horizontalInput, verticalInput).normalized; // Normaliza o vetor nas diagonais

        ControlPlayerSpeed();
    }

    void FixedUpdate()
    {
        Vector2 movementVector = _rawInput;

        // Lógica de Movimento
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
            _playerAnimator.SetInteger("Movimento", 0); // Animação de Idle 
            
            // Usa a última direção para a Blend Tree de Idle
            _playerAnimator.SetFloat("LastMoveX", _lastMoveX);
            _playerAnimator.SetFloat("LastMoveY", _lastMoveY);
        }
    }

    // Controla a velocidade do jogador com base na entrada do usuário
    void ControlPlayerSpeed()
    {
        // Movimento Lento (Stealth)
        if (Input.GetKey(_slowMoveKey))
        {
            _currentSpeed = _playerSlowSpeed;
        }
        // Corrida
        else if (Input.GetKey(_runKey))
        {
            _currentSpeed = _playerRunSpeed;
        }
        // Movimento Normal
        else
        {
            _currentSpeed = _playerNormalSpeed;
        }
    }

    // Move o jogador com base na direção e velocidade atuais
    void MovePlayer(Vector2 direction)
    {
        _playerRigidbody2D.MovePosition(_playerRigidbody2D.position + direction * _currentSpeed * Time.fixedDeltaTime);
    }
}