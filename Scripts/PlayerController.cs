using UnityEngine;

/// <summary>
/// Controlador do jogador - gerencia movimento, ações e input
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float velocidadeMovimento = 5f;
    [SerializeField] private float forcaPulo = 10f;
    [SerializeField] private LayerMask layerChao = 1;
    
    [Header("Configurações de Combate")]
    [SerializeField] private float danoAtaque = 25f;
    [SerializeField] private float alcanceAtaque = 1.5f;
    [SerializeField] private LayerMask layerInimigos = 1 << 8;
    
    [Header("Configurações de Vida")]
    [SerializeField] private float vidaMaxima = 100f;
    [SerializeField] private float vidaAtual;
    [SerializeField] private float tempoInvencibilidade = 1f;
    
    [Header("Referências")]
    [SerializeField] private Transform pontoAtaque;
    [SerializeField] private Collider2D colidirChao;
    [SerializeField] private Animator animatorPlayer;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    // Componentes
    private Rigidbody2D rb2D;
    private GameManager gameManager;
    private AudioManager audioManager;
    
    // Estados
    private bool controlesHabilitados = true;
    private bool estaNoChao = false;
    private bool estaInvencivel = false;
    private bool olhandoDireita = true;
    
    // Input
    private float inputHorizontal;
    private bool inputPulo;
    private bool inputAtaque;
    
    // Eventos
    public static System.Action<float> OnVidaAlterada;
    public static System.Action OnMorreu;
    
    private void Awake()
    {
        // Busca componentes
        rb2D = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();
        
        // Inicializa vida
        vidaAtual = vidaMaxima;
    }
    
    private void Start()
    {
        // Configura física
        rb2D.freezeRotation = true;
        
        // Notifica vida inicial
        OnVidaAlterada?.Invoke(vidaAtual / vidaMaxima);
    }
    
    private void Update()
    {
        if (!controlesHabilitados) return;
        
        // Captura input
        CapturarInput();
        
        // Verifica se está no chão
        VerificarChao();
    }
    
    private void FixedUpdate()
    {
        if (!controlesHabilitados) return;
        
        // Executa movimentos
        ExecutarMovimento();
        ExecutarPulo();
    }
    
    /// <summary>
    /// Captura entrada do jogador
    /// </summary>
    private void CapturarInput()
    {
        // Movimento horizontal
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        
        // Pulo
        inputPulo = Input.GetButtonDown("Jump");
        
        // Ataque
        inputAtaque = Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.J);
        
        // Executa ações baseadas no input
        if (inputAtaque)
        {
            ExecutarAtaque();
        }
    }
    
    /// <summary>
    /// Executa movimento horizontal
    /// </summary>
    private void ExecutarMovimento()
    {
        // Aplica velocidade horizontal
        Vector2 velocidade = rb2D.velocity;
        velocidade.x = inputHorizontal * velocidadeMovimento;
        rb2D.velocity = velocidade;
        
        // Vira o sprite baseado na direção
        if (inputHorizontal > 0 && !olhandoDireita)
        {
            VirarSprite();
        }
        else if (inputHorizontal < 0 && olhandoDireita)
        {
            VirarSprite();
        }
        
        // Atualiza animações
        AtualizarAnimacoes();
    }
    
    /// <summary>
    /// Executa pulo se possível
    /// </summary>
    private void ExecutarPulo()
    {
        if (inputPulo && estaNoChao)
        {
            rb2D.AddForce(Vector2.up * forcaPulo, ForceMode2D.Impulse);
            
            // Toca som de pulo
            if (audioManager != null)
            {
                audioManager.TocarSomPulo();
            }
            
            Debug.Log("Jogador pulou");
        }
    }
    
    /// <summary>
    /// Executa ataque
    /// </summary>
    private void ExecutarAtaque()
    {
        Debug.Log("Jogador atacou");
        
        // Toca animação de ataque
        if (animatorPlayer != null)
        {
            animatorPlayer.SetTrigger("Ataque");
        }
        
        // Detecta inimigos no alcance
        Vector2 posicaoAtaque = pontoAtaque != null ? pontoAtaque.position : transform.position;
        Collider2D[] inimigosAtingidos = Physics2D.OverlapCircleAll(posicaoAtaque, alcanceAtaque, layerInimigos);
        
        foreach (Collider2D inimigo in inimigosAtingidos)
        {
            // Aplica dano ao inimigo
            var inimigoScript = inimigo.GetComponent<IInimigo>();
            if (inimigoScript != null)
            {
                inimigoScript.ReceberDano(danoAtaque);
            }
        }
        
        // Toca som de ataque
        if (audioManager != null)
        {
            audioManager.TocarSomAtaque();
        }
    }
    
    /// <summary>
    /// Verifica se o jogador está no chão
    /// </summary>
    private void VerificarChao()
    {
        if (colidirChao != null)
        {
            estaNoChao = Physics2D.IsTouchingLayers(colidirChao, layerChao);
        }
        else
        {
            // Método alternativo usando raycast
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, layerChao);
            estaNoChao = hit.collider != null;
        }
    }
    
    /// <summary>
    /// Vira o sprite do jogador
    /// </summary>
    private void VirarSprite()
    {
        olhandoDireita = !olhandoDireita;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !olhandoDireita;
        }
        else
        {
            // Método alternativo virando o transform
            Vector3 escala = transform.localScale;
            escala.x *= -1;
            transform.localScale = escala;
        }
    }
    
    /// <summary>
    /// Atualiza as animações baseadas no estado atual
    /// </summary>
    private void AtualizarAnimacoes()
    {
        if (animatorPlayer == null) return;
        
        // Parâmetros de animação
        animatorPlayer.SetFloat("VelocidadeX", Mathf.Abs(inputHorizontal));
        animatorPlayer.SetFloat("VelocidadeY", rb2D.velocity.y);
        animatorPlayer.SetBool("NoChao", estaNoChao);
    }
    
    /// <summary>
    /// Jogador recebe dano
    /// </summary>
    /// <param name="quantidade">Quantidade de dano</param>
    public void ReceberDano(float quantidade)
    {
        if (estaInvencivel) return;
        
        vidaAtual = Mathf.Max(0, vidaAtual - quantidade);
        
        Debug.Log($"Jogador recebeu {quantidade} de dano. Vida atual: {vidaAtual}");
        
        // Notifica mudança de vida
        OnVidaAlterada?.Invoke(vidaAtual / vidaMaxima);
        
        // Toca som de dano
        if (audioManager != null)
        {
            audioManager.TocarSomHit();
        }
        
        // Inicia invencibilidade temporária
        IniciarInvencibilidade();
        
        // Verifica se morreu
        if (vidaAtual <= 0)
        {
            Morrer();
        }
        else
        {
            // Perde uma vida no game manager
            if (gameManager != null)
            {
                gameManager.PerderVida();
            }
        }
    }
    
    /// <summary>
    /// Cura o jogador
    /// </summary>
    /// <param name="quantidade">Quantidade de cura</param>
    public void Curar(float quantidade)
    {
        vidaAtual = Mathf.Min(vidaMaxima, vidaAtual + quantidade);
        OnVidaAlterada?.Invoke(vidaAtual / vidaMaxima);
        
        Debug.Log($"Jogador curado em {quantidade}. Vida atual: {vidaAtual}");
    }
    
    /// <summary>
    /// Inicia período de invencibilidade
    /// </summary>
    private void IniciarInvencibilidade()
    {
        if (estaInvencivel) return;
        
        StartCoroutine(CorotinaInvencibilidade());
    }
    
    /// <summary>
    /// Corrotina de invencibilidade temporária
    /// </summary>
    private System.Collections.IEnumerator CorotinaInvencibilidade()
    {
        estaInvencivel = true;
        
        // Efeito visual de piscada
        float tempoDecorrido = 0f;
        float intervalosPiscada = 0.1f;
        
        while (tempoDecorrido < tempoInvencibilidade)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            }
            
            yield return new WaitForSeconds(intervalosPiscada);
            
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
            }
            
            yield return new WaitForSeconds(intervalosPiscada);
            
            tempoDecorrido += intervalosPiscada * 2;
        }
        
        // Garante que o sprite volta ao normal
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
        
        estaInvencivel = false;
    }
    
    /// <summary>
    /// Jogador morre
    /// </summary>
    private void Morrer()
    {
        Debug.Log("Jogador morreu");
        
        // Desabilita controles
        DesabilitarControles();
        
        // Toca animação de morte
        if (animatorPlayer != null)
        {
            animatorPlayer.SetTrigger("Morte");
        }
        
        // Notifica evento
        OnMorreu?.Invoke();
        
        // Termina o jogo
        if (gameManager != null)
        {
            gameManager.PerderVida(); // Isso pode levar ao game over
        }
    }
    
    /// <summary>
    /// Habilita os controles do jogador
    /// </summary>
    public void HabilitarControles()
    {
        controlesHabilitados = true;
        Debug.Log("Controles do jogador habilitados");
    }
    
    /// <summary>
    /// Desabilita os controles do jogador
    /// </summary>
    public void DesabilitarControles()
    {
        controlesHabilitados = false;
        
        // Para o movimento
        if (rb2D != null)
        {
            rb2D.velocity = Vector2.zero;
        }
        
        Debug.Log("Controles do jogador desabilitados");
    }
    
    /// <summary>
    /// Reseta o jogador para o estado inicial
    /// </summary>
    public void ResetarJogador()
    {
        vidaAtual = vidaMaxima;
        estaInvencivel = false;
        controlesHabilitados = true;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
        
        OnVidaAlterada?.Invoke(vidaAtual / vidaMaxima);
        
        Debug.Log("Jogador resetado");
    }
    
    /// <summary>
    /// Desenha gizmos para debug
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Desenha alcance do ataque
        if (pontoAtaque != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pontoAtaque.position, alcanceAtaque);
        }
        
        // Desenha verificação de chão
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 0.6f);
    }
}

/// <summary>
/// Interface para objetos que podem receber dano
/// </summary>
public interface IInimigo
{
    void ReceberDano(float quantidade);
}
