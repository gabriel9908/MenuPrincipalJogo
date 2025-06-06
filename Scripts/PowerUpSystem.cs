using UnityEngine;
using System.Collections;

/// <summary>
/// Sistema de power-ups para o jogo - coleta e efeitos especiais
/// </summary>
public class PowerUpSystem : MonoBehaviour
{
    [Header("Configurações de Power-Up")]
    [SerializeField] private TipoPowerUp tipo = TipoPowerUp.Vida;
    [SerializeField] private float valorEfeito = 50f;
    [SerializeField] private float duracaoEfeito = 10f;
    [SerializeField] private bool efeitoTemporario = false;
    
    [Header("Configurações Visuais")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animatorPowerUp;
    [SerializeField] private ParticleSystem efeitoColeta;
    [SerializeField] private GameObject efeitoAura;
    
    [Header("Som")]
    [SerializeField] private AudioClip somColeta;
    
    // Estado do power-up
    private bool foiColetado = false;
    private Collider2D colisor;
    
    // Referências
    private AudioManager audioManager;
    private ScoreManager scoreManager;
    
    // Eventos
    public static System.Action<TipoPowerUp, float> OnPowerUpColetado;
    
    private void Awake()
    {
        colisor = GetComponent<Collider2D>();
        audioManager = FindObjectOfType<AudioManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
    }
    
    private void Start()
    {
        InicializarPowerUp();
    }
    
    /// <summary>
    /// Inicializa o power-up baseado no tipo
    /// </summary>
    private void InicializarPowerUp()
    {
        // Configura aparência baseada no tipo
        ConfigurarAparencia();
        
        // Inicia animação idle
        if (animatorPowerUp != null)
        {
            animatorPowerUp.SetTrigger("Idle");
        }
        
        // Ativa efeito de aura se disponível
        if (efeitoAura != null)
        {
            efeitoAura.SetActive(true);
        }
    }
    
    /// <summary>
    /// Configura aparência baseada no tipo de power-up
    /// </summary>
    private void ConfigurarAparencia()
    {
        if (spriteRenderer == null) return;
        
        Color corPowerUp = Color.white;
        
        switch (tipo)
        {
            case TipoPowerUp.Vida:
                corPowerUp = Color.red;
                break;
            case TipoPowerUp.Velocidade:
                corPowerUp = Color.cyan;
                break;
            case TipoPowerUp.Força:
                corPowerUp = new Color(1f, 0.5f, 0f); // Laranja
                break;
            case TipoPowerUp.Tempo:
                corPowerUp = Color.yellow;
                break;
            case TipoPowerUp.Pontos:
                corPowerUp = Color.green;
                break;
            case TipoPowerUp.Escudo:
                corPowerUp = Color.blue;
                break;
        }
        
        spriteRenderer.color = corPowerUp;
    }
    
    /// <summary>
    /// Detecta colisão com o jogador
    /// </summary>
    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (foiColetado) return;
        
        // Verifica se é o jogador
        PlayerController jogador = outro.GetComponent<PlayerController>();
        if (jogador != null)
        {
            ColetarPowerUp(jogador);
        }
    }
    
    /// <summary>
    /// Executa a coleta do power-up
    /// </summary>
    /// <param name="jogador">Referência do jogador</param>
    private void ColetarPowerUp(PlayerController jogador)
    {
        foiColetado = true;
        
        Debug.Log($"Power-up {tipo} coletado! Valor: {valorEfeito}");
        
        // Desabilita colisor
        if (colisor != null)
        {
            colisor.enabled = false;
        }
        
        // Toca som de coleta
        if (audioManager != null)
        {
            if (somColeta != null)
            {
                audioManager.TocarSomEfeito(somColeta);
            }
            else
            {
                audioManager.TocarSomPontos();
            }
        }
        
        // Cria efeito visual
        if (efeitoColeta != null)
        {
            efeitoColeta.Play();
        }
        
        // Aplica efeito baseado no tipo
        AplicarEfeito(jogador);
        
        // Adiciona pontos ao score
        if (scoreManager != null)
        {
            scoreManager.AdicionarPontosPowerUp();
        }
        
        // Notifica evento
        OnPowerUpColetado?.Invoke(tipo, valorEfeito);
        
        // Animação de coleta
        if (animatorPowerUp != null)
        {
            animatorPowerUp.SetTrigger("Coletado");
        }
        
        // Destroi o objeto após um tempo
        StartCoroutine(DestruirAposColeta());
    }
    
    /// <summary>
    /// Aplica o efeito específico do power-up
    /// </summary>
    /// <param name="jogador">Referência do jogador</param>
    private void AplicarEfeito(PlayerController jogador)
    {
        switch (tipo)
        {
            case TipoPowerUp.Vida:
                AplicarEfeitoVida(jogador);
                break;
                
            case TipoPowerUp.Velocidade:
                if (efeitoTemporario)
                {
                    StartCoroutine(AplicarEfeitoVelocidadeTemporario(jogador));
                }
                else
                {
                    AplicarEfeitoVelocidade(jogador);
                }
                break;
                
            case TipoPowerUp.Força:
                if (efeitoTemporario)
                {
                    StartCoroutine(AplicarEfeitoForcaTemporario(jogador));
                }
                break;
                
            case TipoPowerUp.Tempo:
                AplicarEfeitoTempo();
                break;
                
            case TipoPowerUp.Pontos:
                AplicarEfeitoPontos();
                break;
                
            case TipoPowerUp.Escudo:
                if (efeitoTemporario)
                {
                    StartCoroutine(AplicarEfeitoEscudoTemporario(jogador));
                }
                break;
        }
    }
    
    /// <summary>
    /// Aplica efeito de cura de vida
    /// </summary>
    private void AplicarEfeitoVida(PlayerController jogador)
    {
        jogador.Curar(valorEfeito);
        Debug.Log($"Vida restaurada: {valorEfeito} pontos");
    }
    
    /// <summary>
    /// Aplica efeito de velocidade permanente
    /// </summary>
    private void AplicarEfeitoVelocidade(PlayerController jogador)
    {
        // Implementação dependeria da estrutura do PlayerController
        Debug.Log($"Velocidade aumentada permanentemente em {valorEfeito}%");
    }
    
    /// <summary>
    /// Aplica efeito de velocidade temporário
    /// </summary>
    private IEnumerator AplicarEfeitoVelocidadeTemporario(PlayerController jogador)
    {
        Debug.Log($"Velocidade aumentada temporariamente em {valorEfeito}% por {duracaoEfeito} segundos");
        
        // Aqui aumentaria a velocidade do jogador
        // jogador.MultiplicarVelocidade(valorEfeito / 100f + 1f);
        
        yield return new WaitForSeconds(duracaoEfeito);
        
        // Restaura velocidade normal
        // jogador.RestaurarVelocidadeNormal();
        
        Debug.Log("Efeito de velocidade terminou");
    }
    
    /// <summary>
    /// Aplica efeito de força temporário
    /// </summary>
    private IEnumerator AplicarEfeitoForcaTemporario(PlayerController jogador)
    {
        Debug.Log($"Força de ataque aumentada em {valorEfeito}% por {duracaoEfeito} segundos");
        
        // Aqui aumentaria o dano do jogador
        // jogador.MultiplicarDano(valorEfeito / 100f + 1f);
        
        yield return new WaitForSeconds(duracaoEfeito);
        
        // Restaura dano normal
        // jogador.RestaurarDanoNormal();
        
        Debug.Log("Efeito de força terminou");
    }
    
    /// <summary>
    /// Aplica efeito de escudo temporário
    /// </summary>
    private IEnumerator AplicarEfeitoEscudoTemporario(PlayerController jogador)
    {
        Debug.Log($"Escudo ativado por {duracaoEfeito} segundos");
        
        // Aqui tornaria o jogador invencível temporariamente
        // jogador.AtivarInvencibilidade(duracaoEfeito);
        
        yield return new WaitForSeconds(duracaoEfeito);
        
        Debug.Log("Efeito de escudo terminou");
    }
    
    /// <summary>
    /// Aplica efeito de tempo extra
    /// </summary>
    private void AplicarEfeitoTempo()
    {
        TimerManager timerManager = FindObjectOfType<TimerManager>();
        if (timerManager != null)
        {
            timerManager.AdicionarTempo(valorEfeito);
            Debug.Log($"Tempo adicionado: {valorEfeito} segundos");
        }
    }
    
    /// <summary>
    /// Aplica efeito de pontos bonus
    /// </summary>
    private void AplicarEfeitoPontos()
    {
        if (scoreManager != null)
        {
            scoreManager.AdicionarPontos((int)valorEfeito, false);
            Debug.Log($"Pontos bonus adicionados: {valorEfeito}");
        }
    }
    
    /// <summary>
    /// Corrotina para destruir o power-up após coleta
    /// </summary>
    private IEnumerator DestruirAposColeta()
    {
        // Esconde o sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        
        // Desativa aura
        if (efeitoAura != null)
        {
            efeitoAura.SetActive(false);
        }
        
        // Aguarda efeito de partículas terminar
        yield return new WaitForSeconds(1f);
        
        // Destroi o objeto
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Configura o power-up programaticamente
    /// </summary>
    /// <param name="novoTipo">Tipo do power-up</param>
    /// <param name="novoValor">Valor do efeito</param>
    /// <param name="temporario">Se o efeito é temporário</param>
    /// <param name="duracao">Duração do efeito (se temporário)</param>
    public void ConfigurarPowerUp(TipoPowerUp novoTipo, float novoValor, bool temporario = false, float duracao = 10f)
    {
        tipo = novoTipo;
        valorEfeito = novoValor;
        efeitoTemporario = temporario;
        duracaoEfeito = duracao;
        
        ConfigurarAparencia();
    }
    
    /// <summary>
    /// Força a coleta do power-up (para testes)
    /// </summary>
    public void ForcarColeta()
    {
        PlayerController jogador = FindObjectOfType<PlayerController>();
        if (jogador != null)
        {
            ColetarPowerUp(jogador);
        }
    }
}

/// <summary>
/// Tipos de power-ups disponíveis
/// </summary>
public enum TipoPowerUp
{
    Vida,           // Restaura vida
    Velocidade,     // Aumenta velocidade
    Força,          // Aumenta dano de ataque
    Tempo,          // Adiciona tempo ao timer
    Pontos,         // Pontos bonus
    Escudo          // Invencibilidade temporária
}