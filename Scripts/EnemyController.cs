using UnityEngine;

/// <summary>
/// Controlador básico de inimigos - implementa a interface IInimigo
/// </summary>
public class EnemyController : MonoBehaviour, IInimigo
{
    [Header("Configurações do Inimigo")]
    [SerializeField] private float vidaMaxima = 100f;
    [SerializeField] private float velocidadeMovimento = 2f;
    [SerializeField] private float danoAtaque = 10f;
    [SerializeField] private float alcanceAtaque = 1.5f;
    [SerializeField] private float tempoEntreAtaques = 2f;
    
    [Header("Configurações de IA")]
    [SerializeField] private float distanciaDeteccao = 5f;
    [SerializeField] private LayerMask layerJogador = 1;
    [SerializeField] private Transform pontoAtaque;
    
    [Header("Configurações Visuais")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animatorInimigo;
    [SerializeField] private GameObject efeitoMorte;
    
    // Estado do inimigo
    private float vidaAtual;
    private bool estaVivo = true;
    private bool estaAtacando = false;
    private float tempoUltimoAtaque = 0f;
    private Transform jogadorAlvo;
    
    // Componentes
    private Rigidbody2D rb2D;
    private Collider2D colisor;
    
    // Referencias para outros sistemas
    private ScoreManager scoreManager;
    private AudioManager audioManager;
    
    // Eventos
    public static System.Action<EnemyController> OnInimigoMorreu;
    public static System.Action<EnemyController> OnInimigoAtacou;
    
    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        colisor = GetComponent<Collider2D>();
        
        scoreManager = FindObjectOfType<ScoreManager>();
        audioManager = FindObjectOfType<AudioManager>();
        
        // Inicializa vida
        vidaAtual = vidaMaxima;
    }
    
    private void Start()
    {
        // Busca o jogador na cena
        GameObject jogador = GameObject.FindGameObjectWithTag("Player");
        if (jogador != null)
        {
            jogadorAlvo = jogador.transform;
        }
        
        // Configura física
        if (rb2D != null)
        {
            rb2D.freezeRotation = true;
        }
    }
    
    private void Update()
    {
        if (!estaVivo) return;
        
        // Verifica se o jogador está no alcance
        if (jogadorAlvo != null)
        {
            float distanciaJogador = Vector2.Distance(transform.position, jogadorAlvo.position);
            
            if (distanciaJogador <= distanciaDeteccao)
            {
                // Persegue o jogador
                PerseguirJogador();
                
                // Verifica se pode atacar
                if (distanciaJogador <= alcanceAtaque && PodeAtacar())
                {
                    AtacarJogador();
                }
            }
        }
        
        // Atualiza animações
        AtualizarAnimacoes();
    }
    
    /// <summary>
    /// Persegue o jogador
    /// </summary>
    private void PerseguirJogador()
    {
        if (estaAtacando) return;
        
        Vector2 direcao = (jogadorAlvo.position - transform.position).normalized;
        
        // Move em direção ao jogador
        if (rb2D != null)
        {
            rb2D.velocity = new Vector2(direcao.x * velocidadeMovimento, rb2D.velocity.y);
        }
        else
        {
            transform.Translate(direcao * velocidadeMovimento * Time.deltaTime);
        }
        
        // Vira o sprite baseado na direção
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direcao.x < 0;
        }
    }
    
    /// <summary>
    /// Verifica se pode atacar
    /// </summary>
    private bool PodeAtacar()
    {
        return Time.time - tempoUltimoAtaque >= tempoEntreAtaques;
    }
    
    /// <summary>
    /// Ataca o jogador
    /// </summary>
    private void AtacarJogador()
    {
        estaAtacando = true;
        tempoUltimoAtaque = Time.time;
        
        // Toca animação de ataque
        if (animatorInimigo != null)
        {
            animatorInimigo.SetTrigger("Ataque");
        }
        
        // Detecta jogador no alcance de ataque
        Vector2 posicaoAtaque = pontoAtaque != null ? pontoAtaque.position : transform.position;
        Collider2D jogadorAtingido = Physics2D.OverlapCircle(posicaoAtaque, alcanceAtaque, layerJogador);
        
        if (jogadorAtingido != null)
        {
            PlayerController player = jogadorAtingido.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ReceberDano(danoAtaque);
            }
        }
        
        // Toca som de ataque
        if (audioManager != null)
        {
            audioManager.TocarSomAtaque();
        }
        
        // Notifica evento
        OnInimigoAtacou?.Invoke(this);
        
        // Para o movimento temporariamente
        if (rb2D != null)
        {
            rb2D.velocity = Vector2.zero;
        }
        
        // Reseta ataque após um tempo
        Invoke(nameof(FinalizarAtaque), 0.5f);
        
        Debug.Log($"{gameObject.name} atacou o jogador causando {danoAtaque} de dano");
    }
    
    /// <summary>
    /// Finaliza o estado de ataque
    /// </summary>
    private void FinalizarAtaque()
    {
        estaAtacando = false;
    }
    
    /// <summary>
    /// Implementação da interface IInimigo - recebe dano
    /// </summary>
    /// <param name="quantidade">Quantidade de dano recebido</param>
    public void ReceberDano(float quantidade)
    {
        if (!estaVivo) return;
        
        vidaAtual -= quantidade;
        
        Debug.Log($"{gameObject.name} recebeu {quantidade} de dano. Vida: {vidaAtual}/{vidaMaxima}");
        
        // Efeito visual de dano (piscar)
        if (spriteRenderer != null)
        {
            StartCoroutine(EfeitoDano());
        }
        
        // Toca som de hit
        if (audioManager != null)
        {
            audioManager.TocarSomHit();
        }
        
        // Verifica se morreu
        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }
    
    /// <summary>
    /// Efeito visual de dano (piscar vermelho)
    /// </summary>
    private System.Collections.IEnumerator EfeitoDano()
    {
        Color corOriginal = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        
        yield return new WaitForSeconds(0.1f);
        
        spriteRenderer.color = corOriginal;
    }
    
    /// <summary>
    /// Inimigo morre
    /// </summary>
    private void Morrer()
    {
        estaVivo = false;
        
        Debug.Log($"{gameObject.name} foi eliminado");
        
        // Para movimento
        if (rb2D != null)
        {
            rb2D.velocity = Vector2.zero;
        }
        
        // Desabilita colisor
        if (colisor != null)
        {
            colisor.enabled = false;
        }
        
        // Toca animação de morte
        if (animatorInimigo != null)
        {
            animatorInimigo.SetTrigger("Morte");
        }
        
        // Cria efeito de morte
        if (efeitoMorte != null)
        {
            Instantiate(efeitoMorte, transform.position, Quaternion.identity);
        }
        
        // Adiciona pontos ao score
        if (scoreManager != null)
        {
            scoreManager.AdicionarPontosInimigoBasico();
        }
        
        // Notifica evento
        OnInimigoMorreu?.Invoke(this);
        
        // Destroi o objeto após um tempo
        Destroy(gameObject, 1f);
    }
    
    /// <summary>
    /// Atualiza animações baseadas no estado
    /// </summary>
    private void AtualizarAnimacoes()
    {
        if (animatorInimigo == null) return;
        
        // Parâmetros de animação
        bool estaMovendo = rb2D != null ? Mathf.Abs(rb2D.velocity.x) > 0.1f : false;
        
        animatorInimigo.SetBool("EstaMovendo", estaMovendo);
        animatorInimigo.SetBool("EstaVivo", estaVivo);
        animatorInimigo.SetBool("EstaAtacando", estaAtacando);
    }
    
    /// <summary>
    /// Desenha gizmos para debug
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Desenha alcance de detecção
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccao);
        
        // Desenha alcance de ataque
        if (pontoAtaque != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pontoAtaque.position, alcanceAtaque);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, alcanceAtaque);
        }
    }
    
    /// <summary>
    /// Configura vida máxima (para inimigos especiais)
    /// </summary>
    /// <param name="novaVidaMaxima">Nova vida máxima</param>
    public void ConfigurarVida(float novaVidaMaxima)
    {
        vidaMaxima = novaVidaMaxima;
        vidaAtual = vidaMaxima;
    }
    
    /// <summary>
    /// Configura velocidade (para diferentes tipos de inimigos)
    /// </summary>
    /// <param name="novaVelocidade">Nova velocidade</param>
    public void ConfigurarVelocidade(float novaVelocidade)
    {
        velocidadeMovimento = novaVelocidade;
    }
    
    /// <summary>
    /// Força o inimigo a atacar (para eventos especiais)
    /// </summary>
    public void ForcarAtaque()
    {
        if (estaVivo && jogadorAlvo != null)
        {
            AtacarJogador();
        }
    }
    
    /// <summary>
    /// Para o inimigo temporariamente
    /// </summary>
    /// <param name="duracao">Duração em segundos</param>
    public void PararTemporariamente(float duracao)
    {
        if (rb2D != null)
        {
            rb2D.velocity = Vector2.zero;
        }
        
        enabled = false;
        Invoke(nameof(ReativarInimigo), duracao);
    }
    
    /// <summary>
    /// Reativa o inimigo após parada temporária
    /// </summary>
    private void ReativarInimigo()
    {
        enabled = true;
    }
}