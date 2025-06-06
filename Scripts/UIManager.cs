using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gerenciador da interface do usuário - controla todos os elementos visuais da UI
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Telas Principais")]
    [SerializeField] private GameObject telaMenu;
    [SerializeField] private GameObject telaJogo;
    [SerializeField] private GameObject telaPausa;
    [SerializeField] private GameObject telaGameOver;
    
    [Header("Elementos do HUD")]
    [SerializeField] private TextMeshProUGUI textoPontuacao;
    [SerializeField] private TextMeshProUGUI textoTimer;
    [SerializeField] private TextMeshProUGUI textoVidas;
    [SerializeField] private Slider barraVida;
    
    [Header("Botões")]
    [SerializeField] private Button botaoIniciar;
    [SerializeField] private Button botaoPausar;
    [SerializeField] private Button botaoReiniciar;
    [SerializeField] private Button botaoMenu;
    
    [Header("Animações")]
    [SerializeField] private Animator animadorUI;
    
    // Referências para outros sistemas
    private GameManager gameManager;
    private ScoreManager scoreManager;
    private TimerManager timerManager;
    private GameOverUI gameOverUI;
    
    private void Awake()
    {
        // Busca referências
        gameManager = FindObjectOfType<GameManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        timerManager = FindObjectOfType<TimerManager>();
        gameOverUI = FindObjectOfType<GameOverUI>();
    }
    
    private void Start()
    {
        ConfigurarBotoes();
        RegistrarEventos();
        MostrarTela(telaMenu);
    }
    
    private void OnDestroy()
    {
        DesregistrarEventos();
    }
    
    /// <summary>
    /// Configura os eventos dos botões
    /// </summary>
    private void ConfigurarBotoes()
    {
        if (botaoIniciar != null)
            botaoIniciar.onClick.AddListener(() => gameManager?.IniciarNovaPartida());
        
        if (botaoPausar != null)
            botaoPausar.onClick.AddListener(() => gameManager?.PausarJogo());
        
        if (botaoReiniciar != null)
            botaoReiniciar.onClick.AddListener(() => gameManager?.ReiniciarJogo());
        
        if (botaoMenu != null)
            botaoMenu.onClick.AddListener(() => gameManager?.VoltarAoMenu());
    }
    
    /// <summary>
    /// Registra eventos dos outros sistemas
    /// </summary>
    private void RegistrarEventos()
    {
        // Eventos do estado do jogo
        GameState.OnEstadoAlterado += HandleMudancaEstado;
        
        // Eventos do score
        if (scoreManager != null)
        {
            ScoreManager.OnPontuacaoAlterada += AtualizarPontuacao;
        }
        
        // Eventos do timer
        if (timerManager != null)
        {
            TimerManager.OnTempoAtualizado += AtualizarTimer;
        }
        
        // Eventos do game manager
        GameManager.OnVidasAlteradas += AtualizarVidas;
    }
    
    /// <summary>
    /// Remove registro dos eventos
    /// </summary>
    private void DesregistrarEventos()
    {
        GameState.OnEstadoAlterado -= HandleMudancaEstado;
        
        if (scoreManager != null)
        {
            ScoreManager.OnPontuacaoAlterada -= AtualizarPontuacao;
        }
        
        if (timerManager != null)
        {
            TimerManager.OnTempoAtualizado -= AtualizarTimer;
        }
        
        GameManager.OnVidasAlteradas -= AtualizarVidas;
    }
    
    /// <summary>
    /// Handle para mudanças de estado do jogo
    /// </summary>
    /// <param name="novoEstado">Novo estado do jogo</param>
    private void HandleMudancaEstado(EstadoJogo novoEstado)
    {
        switch (novoEstado)
        {
            case EstadoJogo.Menu:
                MostrarTela(telaMenu);
                break;
                
            case EstadoJogo.Jogando:
                MostrarTela(telaJogo);
                break;
                
            case EstadoJogo.Pausado:
                MostrarTela(telaPausa);
                break;
                
            case EstadoJogo.GameOver:
                MostrarGameOver();
                break;
        }
    }
    
    /// <summary>
    /// Mostra uma tela específica e esconde as outras
    /// </summary>
    /// <param name="telaParaMostrar">Tela que deve ficar visível</param>
    private void MostrarTela(GameObject telaParaMostrar)
    {
        // Esconde todas as telas
        if (telaMenu != null) telaMenu.SetActive(false);
        if (telaJogo != null) telaJogo.SetActive(false);
        if (telaPausa != null) telaPausa.SetActive(false);
        if (telaGameOver != null) telaGameOver.SetActive(false);
        
        // Mostra a tela solicitada
        if (telaParaMostrar != null)
        {
            telaParaMostrar.SetActive(true);
        }
    }
    
    /// <summary>
    /// Exibe a tela de game over
    /// </summary>
    public void MostrarGameOver()
    {
        MostrarTela(telaGameOver);
        
        // Ativa a UI específica de game over
        if (gameOverUI != null)
        {
            gameOverUI.ExibirGameOver();
        }
        
        // Animação de entrada se disponível
        if (animadorUI != null)
        {
            animadorUI.SetTrigger("GameOver");
        }
    }
    
    /// <summary>
    /// Atualiza o display da pontuação
    /// </summary>
    /// <param name="novaPontuacao">Nova pontuação para exibir</param>
    private void AtualizarPontuacao(int novaPontuacao)
    {
        if (textoPontuacao != null)
        {
            textoPontuacao.text = $"Pontos: {novaPontuacao:N0}";
        }
    }
    
    /// <summary>
    /// Atualiza o display do timer
    /// </summary>
    /// <param name="tempoRestante">Tempo restante em segundos</param>
    private void AtualizarTimer(float tempoRestante)
    {
        if (textoTimer != null)
        {
            int minutos = Mathf.FloorToInt(tempoRestante / 60);
            int segundos = Mathf.FloorToInt(tempoRestante % 60);
            textoTimer.text = $"{minutos:00}:{segundos:00}";
        }
    }
    
    /// <summary>
    /// Atualiza o display das vidas
    /// </summary>
    /// <param name="vidasRestantes">Número de vidas restantes</param>
    private void AtualizarVidas(int vidasRestantes)
    {
        if (textoVidas != null)
        {
            textoVidas.text = $"Vidas: {vidasRestantes}";
        }
        
        if (barraVida != null)
        {
            // Assume máximo de 3 vidas para a barra
            barraVida.value = vidasRestantes / 3f;
        }
    }
    
    /// <summary>
    /// Mostra uma mensagem temporária na tela
    /// </summary>
    /// <param name="mensagem">Mensagem para exibir</param>
    /// <param name="duracao">Duração em segundos</param>
    public void MostrarMensagem(string mensagem, float duracao = 2f)
    {
        // Implementar sistema de mensagens temporárias se necessário
        Debug.Log($"Mensagem UI: {mensagem}");
    }
    
    /// <summary>
    /// Ativa/desativa o cursor do mouse
    /// </summary>
    /// <param name="mostrar">True para mostrar, false para esconder</param>
    public void ConfigurarCursor(bool mostrar)
    {
        Cursor.visible = mostrar;
        Cursor.lockState = mostrar ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
