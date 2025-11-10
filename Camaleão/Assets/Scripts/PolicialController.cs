using UnityEngine;

// 1. Definição dos Estados da Máquina de Estados Finitos (FSM)
public enum GuardState { Patrol, Alert, Chase }

public class PolicialController : MonoBehaviour
{
    private Rigidbody2D _npcRigidbody2D;
    private Animator _npcAnimator;

    [Header("Configurações Gerais")]
    public GuardState currentState = GuardState.Patrol;
    public float _npcSpeed = 2f; // Velocidade de Patrulha
    public float chaseSpeed = 4f; // Velocidade em estado de Alerta/Perseguição
    
    // --- Patrulha (Grafo/Waypoints) ---
    [Header("Patrulha (Grafo/Waypoints)")]
    [Tooltip("Pontos que o guarda deve seguir na patrulha.")]
    public Transform[] patrolPoints; 
    public float waitTimeAtPoint = 1.5f; // Tempo de espera em cada ponto
    
    // Variáveis internas da Patrulha
    private int currentPointIndex = 0;
    private bool isWaiting = false;
    // Posição alvo do guarda (Waypoint ou Ponto de Alerta)
    public Vector3 targetPosition; 

    // --- Visão e Detecção ---
    [Header("Visão e Detecção")]
    public float visionDistance = 7f; // Alcance máximo da lanterna
    public LayerMask obstacleLayer; // Camada das paredes (Bloqueia o Raycast)
    public LayerMask playerLayer;   // Camada do player (Aciona a Detecção)
    
    // Onde a câmera/barulho detectou (Ajustado externamente pela Câmera)
    [HideInInspector] public Vector3 alertPosition; 

    // --- Configurações de Animação e Lanterna ---
    [Header("Animações e Lanterna")]
    public Transform _lanternLightTransform;
    public float _lanternRotationOffset = 90f; 
    
    // Inicializado para garantir uma direção inicial
    private float _lastMoveX = 0f;
    private float _lastMoveY = 1f;

    void Start()
    {
        _npcRigidbody2D = GetComponent<Rigidbody2D>();
        _npcAnimator = GetComponent<Animator>();

        if (_npcRigidbody2D != null)
        {
            _npcRigidbody2D.interpolation = RigidbodyInterpolation2D.Extrapolate;
            _npcRigidbody2D.gravityScale = 0f;
            _npcRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation; 
        }

        if (patrolPoints.Length > 0)
        {
            // O guarda inicia na posição do primeiro ponto de patrulha
            transform.position = patrolPoints[currentPointIndex].position;
            
            // Aqui, antes de chamar GoToNextPoint, forçamos o olhar inicial para o primeiro alvo.
            Vector3 initialDirection = patrolPoints[0].position - transform.position;
            if (initialDirection.sqrMagnitude > 0.01f)
            {
                Vector2 dir = initialDirection.normalized;
                _lastMoveX = dir.x;
                _lastMoveY = dir.y;
            }
            
            GoToNextPoint(); 
        }
        else
        {
            Debug.LogWarning("O Guarda não tem pontos de patrulha definidos!");
        }
    }

    void Update()
    {
        CheckVision();

        switch (currentState)
        {
            case GuardState.Patrol:
                HandlePatrol();
                break;
            case GuardState.Alert:
                HandleAlert();
                break;
            case GuardState.Chase:
                HandleChase();
                break;
        }
    }

    void FixedUpdate()
{
    // Aplica a física somente nos estados de movimento
    if (currentState == GuardState.Patrol || currentState == GuardState.Alert)
    {
        // 1. Calcula a direção
        Vector3 direction3D = targetPosition - transform.position;
        float distanceToTarget = direction3D.magnitude;

        // Se a distância for menor que o limite de parada (0.1f), consideramos parado
        if (distanceToTarget < 0.1f)
        {
            // CORREÇÃO DO AVISO: Define a velocidade linear como zero.
            _npcRigidbody2D.linearVelocity = Vector2.zero;
            UpdateAnimation(Vector2.zero); // Chama Idle, que usa _lastMoveX/Y fixos
        }
        else // O policial ainda está se movendo para o alvo
        {
            // 1b. Normaliza a direção APENAS para o movimento
            Vector2 moveDirection2D = new Vector2(direction3D.x, direction3D.y).normalized;

            // 2. Define a velocidade atual
            float currentSpeed = (currentState == GuardState.Alert) ? chaseSpeed : _npcSpeed;
            
            // 3. Move o policial 
            _npcRigidbody2D.MovePosition(_npcRigidbody2D.position + moveDirection2D * currentSpeed * Time.fixedDeltaTime);
            
            // 4. Atualiza a animação e lanterna
            UpdateAnimation(moveDirection2D);
        }
    }
    else 
    {
         // Garante que a animação de Idle toque (se estiver parado em outros estados)
         UpdateAnimation(Vector2.zero); 
    }
}

    // --- LÓGICA DE ESTADOS ---

    void HandlePatrol()
    {
        if (isWaiting) return;

        // Se chegou ao ponto alvo (a mesma verificação de distância usada no FixedUpdate)
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isWaiting = true;
            Invoke("GoToNextPoint", waitTimeAtPoint);
        }
    }

    void HandleAlert()
    {
        // Define o alvo de movimento como o local do alerta
        targetPosition = alertPosition;
        
        // Se a posição de alerta for atingida, volta a patrulhar
        if (Vector3.Distance(transform.position, alertPosition) < 0.1f)
        {
            SetState(GuardState.Patrol);
        }
    }

    void HandleChase()
    {
        Debug.Log("GAME OVER: Guarda te viu!");
        Time.timeScale = 0; 
    }

    // --- LÓGICA DE VISÃO (Raycasting) ---

    void CheckVision()
    {
        if (currentState == GuardState.Chase) return;

        // A direção para o Raycast é para onde a lanterna está apontando (última direção de movimento)
        Vector2 sightDirection = new Vector2(_lastMoveX, _lastMoveY).normalized;
        
        // --- A) Visão Direta do Player (GAME OVER) ---
        RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, sightDirection, visionDistance, playerLayer);

        if (hitPlayer.collider != null)
        {
            // Confirma que não há uma parede entre o guarda e o player
            RaycastHit2D hitBlocker = Physics2D.Raycast(transform.position, sightDirection, visionDistance, obstacleLayer);
            
            // Se detectou o player e a parede está mais longe que o player, ou não detectou a parede
            if (hitBlocker.collider == null || hitPlayer.distance < hitBlocker.distance)
            {
                SetState(GuardState.Chase);
            }
        }
    }

    // --- MÉTODOS DE CONTROLE ---

    public void SetState(GuardState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        
        if (newState == GuardState.Alert)
        {
            CancelInvoke("GoToNextPoint"); 
            isWaiting = false;
        }
        else if (newState == GuardState.Patrol)
        {
            GoToNextPoint(); 
        }
    }

    void GoToNextPoint()
    {
        isWaiting = false;
        
        if (patrolPoints.Length == 0) return;

        int newPointIndex;

        // LÓGICA DE SELEÇÃO ALEATÓRIA: Garante que o novo ponto seja diferente do ponto atual
        do
        {
            newPointIndex = Random.Range(0, patrolPoints.Length);
        } while (newPointIndex == currentPointIndex);

        currentPointIndex = newPointIndex;
        SetTargetPosition(patrolPoints[currentPointIndex].position);
    }
    
    void SetTargetPosition(Vector3 newTarget)
    {
        targetPosition = newTarget;
    }

    // --- LÓGICA DE ANIMAÇÃO E LANTERNA ---
    
    void UpdateAnimation(Vector2 moveDirection)
    {
        // Se estiver parado (moveDirection é Vector2.zero)
        if (moveDirection.sqrMagnitude < 0.01f) 
        {
            _npcAnimator.SetInteger("Movimento", 0); 
            // Usa as últimas direções válidas armazenadas para o Idle
            _npcAnimator.SetFloat("LastMoveX", _lastMoveX);
            _npcAnimator.SetFloat("LastMoveY", _lastMoveY);
            
            // Rotaciona a lanterna para a última direção
            RotateLanternToDirection(new Vector2(_lastMoveX, _lastMoveY));
            return;
        }
        
        // Se estiver movendo
        _npcAnimator.SetInteger("Movimento", 1); 
        _npcAnimator.SetFloat("AxisX", moveDirection.x);
        _npcAnimator.SetFloat("AxisY", moveDirection.y);
        
        // Armazena a última direção válida (só se estiver movendo ativamente)
        _lastMoveX = moveDirection.x;
        _lastMoveY = moveDirection.y;
        
        RotateLanternToDirection(moveDirection);
    }

    void RotateLanternToDirection(Vector2 direction)
    {
        if (_lanternLightTransform != null && direction.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            _lanternLightTransform.rotation = Quaternion.Euler(0, 0, angle + _lanternRotationOffset);
        }
    }
}