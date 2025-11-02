using UnityEngine;

public class PolicialController : MonoBehaviour
{
    private Rigidbody2D _npcRigidbody2D;
    private Animator _npcAnimator;

    [Header("Configurações do NPC")]
    public float _npcSpeed; // Velocidade de Patrulha

    [Header("Configurações da Lanterna")]
    [Tooltip("Arraste o objeto Freeform Light 2D da lanterna aqui.")]
    public Transform _lanternLightTransform; // Referência para o Transform da lanterna
    [Tooltip("Offset (graus) para alinhar a rotação da lanterna com a direção do NPC.")]
    public float _lanternRotationOffset; // Ajuste para alinhar a lanterna (Ex: 90 para Freeform padrão)

    private Vector2 _targetDirection; // Direção que o NPC deve seguir
    private float _lastMoveX;
    private float _lastMoveY;

    // Lógica de teste simples: o NPC muda de direção a cada X segundos
    private float _currentPatrolTime; 
    public float _directionChangeTime; // Tempo para mudar de direção

    void Start()
    {
        _npcRigidbody2D = GetComponent<Rigidbody2D>();
        _npcAnimator = GetComponent<Animator>();

        // Começa movendo-se para a direita (apenas para inicializar)
        _targetDirection = Vector2.right;
        _currentPatrolTime = _directionChangeTime; 

        // Configura o Rigidbody para interpolação para suavizar o movimento visual.
        if (_npcRigidbody2D != null)
        {
            _npcRigidbody2D.interpolation = RigidbodyInterpolation2D.Extrapolate; 
        }

        // Garante que o objeto da lanterna foi atribuído
        if (_lanternLightTransform == null)
        {
            Debug.LogWarning("O _lanternLightTransform não foi atribuído no NPCController para " + gameObject.name + ". A lanterna não girará.", this);
        }
    }

    void Update()
    {
        // Lógica de teste simples: muda de direção a cada 'directionChangeTime'
        _currentPatrolTime -= Time.deltaTime;
        if (_currentPatrolTime <= 0)
        {
            ChangeDirection();
            _currentPatrolTime = _directionChangeTime;
        }
    }

    void FixedUpdate()
    {
        // Movimento da Patrulha
        _npcRigidbody2D.MovePosition(_npcRigidbody2D.position + _targetDirection * _npcSpeed * Time.fixedDeltaTime);

        // Lógica de Animação
        if (_targetDirection.sqrMagnitude > 0.01f) 
        {
            _npcAnimator.SetFloat("AxisX", _targetDirection.x);
            _npcAnimator.SetFloat("AxisY", _targetDirection.y);
            _npcAnimator.SetInteger("Movimento", 1); // Walk

            // Armazena a última direção válida
            _lastMoveX = _targetDirection.x;
            _lastMoveY = _targetDirection.y;
            
            // Rotação da Lanterna 
            RotateLanternToDirection(_targetDirection);
        }
        else
        {
            _npcAnimator.SetInteger("Movimento", 0); 
            _npcAnimator.SetFloat("LastMoveX", _lastMoveX);
            _npcAnimator.SetFloat("LastMoveY", _lastMoveY);

            // A lanterna deve manter a última direção para onde o NPC olhou antes de parar
            RotateLanternToDirection(new Vector2(_lastMoveX, _lastMoveY));
        }
    }

    // Muda a direção do NPC aleatoriamente (direita, esquerda, cima, baixo)
    void ChangeDirection()
    {
        int randomDir = Random.Range(0, 4); 

        switch (randomDir)
        {
            case 0: _targetDirection = Vector2.right; break; // Direita
            case 1: _targetDirection = Vector2.left; break; // Esquerda
            case 2: _targetDirection = Vector2.up; break; // Cima
            case 3: _targetDirection = Vector2.down; break; // Baixo
        }
    }

    // Rotaciona a lanterna para apontar na direção do movimento
    void RotateLanternToDirection(Vector2 direction)
    {
        if (_lanternLightTransform != null && direction.sqrMagnitude > 0.01f)
        {
            // Calcula o ângulo em graus a partir do vetor de direção
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            // Aplica a rotação ao Transform da lanterna, adicionando o offset
            _lanternLightTransform.rotation = Quaternion.Euler(0, 0, angle + _lanternRotationOffset);
        }
    }
}