using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gerenciador principal do jogo - controla o fluxo geral e coordena outros sistemas
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Configurações do Jogo")]
    [SerializeField] private float tempoLimite = 60f; // Tempo limite em segundos
    [SerializeField] private int vidasIniciais = 3;
    [SerializeField] private int pontuacaoObjetivo = 1000;
    
    [Header("Referências dos Sistemas")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TimerManager timerManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private PlayerController playerController;
    
    // Propriedades públicas
    public float TempoRestante => timerManager != null ? timerManager.TempoRestante : 0f;
    public int PontuacaoAtual => scoreManager != null ? scoreManager.PontuacaoAtual : 0;
    public int VidasRestantes { get; private set; }
    
    // Singleton
    public static GameManager Instancia { get; private set; }
    
    // Eventos
    public static System.Action OnJogoIniciado;
    public static System.Action OnJogoTerminado;
    public static System.Action<int> OnVidasAlteradas;
    
    private void Awake()
    {
        // Implementa Singleton
        if (Instancia != null && Instancia != this)
        {
            Destroy(this.gameObject);
            return;
        }
        
        Instancia = this;
        DontDestroyOnLoad(this.gameObject);
        
        // Inicializa vidas
        VidasRestantes = vidasIniciais;
    }
    
    private void Start()
    {
        InicializarSistemas();
        RegistrarEventos();
    }
    
    private void OnDestroy()
    {
        DesregistrarEventos();
    }
    
    /// <summary>
    /// Inicializa todos os sistemas do jogo
    /// </summary>
    private void InicializarSistemas()
    {
        // Busca referências se não foram atribuídas no inspector
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>();
        if (scoreManager == null) scoreManager = FindObjectOfType<ScoreManager>();
        if (timerManager == null) timerManager = FindObjectOfType<TimerManager>();
        if (audioManager == null) audioManager = FindObjectOfType<AudioManager>();
        if (playerController == null) playerController = FindObjectOfType<PlayerController>();
        
        // Configura o timer
        if (timerManager != null)
        {
            timerManager.ConfigurarTimer(tempoLimite);
        }
        
        // Inicia no menu
        GameState.Instancia?.AlterarEstado(EstadoJogo.Menu);
        
        Debug.Log("Sistemas do jogo inicializados");
    }
    
    /// <summary>
    /// Registra eventos dos outros sistemas
    /// </summary>
    private void RegistrarEventos()
    {
        // Eventos do timer
        if (timerManager != null)
        {
            TimerManager.OnTempoEsgotado += TerminarJogoPorTempo;
        }
        
        // Eventos do estado do jogo
        GameState.OnGameOver += HandleGameOver;
        GameState.OnJogoIniciado += HandleJogoIniciado;
    }
    
    /// <summary>
    /// Remove registro dos eventos
    /// </summary>
    private void DesregistrarEventos()
    {
        if (timerManager != null)
        {
            TimerManager.OnTempoEsgotado -= TerminarJogoPorTempo;
        }
        
        GameState.OnGameOver -= HandleGameOver;
        GameState.OnJogoIniciado -= HandleJogoIniciado;
    }
    
    /// <summary>
    /// Inicia uma nova partida
    /// </summary>
    public void IniciarNovaPartida()
    {
        Debug.Log("Iniciando nova partida");
        
        // Reseta variáveis
        VidasRestantes = vidasIniciais;
        
        // Reseta sistemas
        scoreManager?.ResetarPontuacao();
        timerManager?.IniciarTimer();
        
        // Habilita o jogador
        if (playerController != null)
        {
            playerController.HabilitarControles();
        }
        
        // Altera estado para jogando
        GameState.Instancia?.AlterarEstado(EstadoJogo.Jogando);
        
        // Toca música de fundo
        audioManager?.TocarMusicaFundo();
        
        // Notifica evento
        OnJogoIniciado?.Invoke();
    }
    
    /// <summary>
    /// Pausa o jogo
    /// </summary>
    public void PausarJogo()
    {
        if (GameState.Instancia.EstaNoEstado(EstadoJogo.Jogando))
        {
            GameState.Instancia.AlterarEstado(EstadoJogo.Pausado);
            timerManager?.PausarTimer();
            audioManager?.PausarMusicaFundo();
        }
    }
    
    /// <summary>
    /// Retoma o jogo pausado
    /// </summary>
    public void RetomarJogo()
    {
        if (GameState.Instancia.EstaNoEstado(EstadoJogo.Pausado))
        {
            GameState.Instancia.AlterarEstado(EstadoJogo.Jogando);
            timerManager?.RetomarTimer();
            audioManager?.RetomarMusicaFundo();
        }
    }
    
    /// <summary>
    /// Perde uma vida
    /// </summary>
    public void PerderVida()
    {
        VidasRestantes = Mathf.Max(0, VidasRestantes - 1);
        OnVidasAlteradas?.Invoke(VidasRestantes);
        
        audioManager?.TocarSomHit();
        
        if (VidasRestantes <= 0)
        {
            TerminarJogoPorVidas();
        }
        
        Debug.Log($"Vida perdida. Vidas restantes: {VidasRestantes}");
    }
    
    /// <summary>
    /// Termina o jogo por falta de vidas
    /// </summary>
    private void TerminarJogoPorVidas()
    {
        Debug.Log("Game Over - Sem vidas");
        TerminarJogo("Suas vidas acabaram!");
    }
    
    /// <summary>
    /// Termina o jogo por tempo esgotado
    /// </summary>
    private void TerminarJogoPorTempo()
    {
        Debug.Log("Game Over - Tempo esgotado");
        TerminarJogo("Tempo esgotado!");
    }
    
    /// <summary>
    /// Termina o jogo com uma mensagem
    /// </summary>
    /// <param name="motivo">Motivo do game over</param>
    private void TerminarJogo(string motivo)
    {
        // Desabilita controles do jogador
        if (playerController != null)
        {
            playerController.DesabilitarControles();
        }
        
        // Para o timer
        timerManager?.PararTimer();
        
        // Para a música
        audioManager?.PararMusicaFundo();
        
        // Altera estado para game over
        GameState.Instancia?.AlterarEstado(EstadoJogo.GameOver);
        
        // Notifica evento
        OnJogoTerminado?.Invoke();
        
        Debug.Log($"Jogo terminado: {motivo}");
    }
    
    /// <summary>
    /// Reinicia o jogo atual
    /// </summary>
    public void ReiniciarJogo()
    {
        Debug.Log("Reiniciando jogo");
        
        // Recarrega a cena atual
        string cenaAtual = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(cenaAtual);
    }
    
    /// <summary>
    /// Volta ao menu principal
    /// </summary>
    public void VoltarAoMenu()
    {
        Debug.Log("Voltando ao menu");
        
        // Carrega cena do menu (assumindo que existe uma cena chamada "Menu")
        SceneManager.LoadScene("Menu");
    }
    
    /// <summary>
    /// Handle para quando o jogo inicia
    /// </summary>
    private void HandleJogoIniciado()
    {
        Debug.Log("Jogo iniciado - GameManager notificado");
    }
    
    /// <summary>
    /// Handle para quando entra em game over
    /// </summary>
    private void HandleGameOver()
    {
        Debug.Log("Game Over - GameManager notificado");
        uiManager?.MostrarGameOver();
    }
    
    /// <summary>
    /// Verifica condições de vitória
    /// </summary>
    public void VerificarVitoria()
    {
        if (scoreManager != null && scoreManager.PontuacaoAtual >= pontuacaoObjetivo)
        {
            Debug.Log("Vitória alcançada!");
            // Implementar lógica de vitória se necessário
        }
    }
}
