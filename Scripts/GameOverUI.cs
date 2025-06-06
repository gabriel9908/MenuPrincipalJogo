using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // Para animações suaves (DOTween)

/// <summary>
/// Controla especificamente a interface de Game Over baseada na imagem fornecida
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("Elementos Visuais")]
    [SerializeField] private Image fundoGameOver;
    [SerializeField] private Image emojiChorando;
    [SerializeField] private TextMeshProUGUI textoGameOver;
    [SerializeField] private Button botaoRestart;
    [SerializeField] private Image fundoTimer;
    [SerializeField] private TextMeshProUGUI textoTimer;
    
    [Header("Área de Cartas")]
    [SerializeField] private GameObject areaCartas;
    [SerializeField] private Button botaoNextPiece;
    [SerializeField] private Button botaoCards;
    [SerializeField] private Image[] cartasInventario;
    
    [Header("Jogadores")]
    [SerializeField] private Image[] avatarsJogadores;
    [SerializeField] private TextMeshProUGUI[] textosNivelJogadores;
    
    [Header("Propaganda")]
    [SerializeField] private GameObject areaPropaganda;
    [SerializeField] private TextMeshProUGUI textoPropaganda;
    
    [Header("Configurações Visuais")]
    [SerializeField] private Color corFundoGameOver = new Color(0.8f, 0.2f, 0.2f, 0.9f);
    [SerializeField] private Color corTextoGameOver = Color.white;
    [SerializeField] private float duracaoAnimacaoEntrada = 1f;
    [SerializeField] private float delayEntradaElementos = 0.2f;
    
    [Header("Sons")]
    [SerializeField] private AudioClip somGameOver;
    [SerializeField] private AudioClip somBotao;
    
    // Referencias para outros sistemas
    private GameManager gameManager;
    private AudioManager audioManager;
    private ScoreManager scoreManager;
    
    // Controle de animação
    private Sequence sequenciaAnimacao;
    
    private void Awake()
    {
        // Busca referências
        gameManager = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        
        // Inicializa elementos invisíveis
        ConfigurarEstadoInicial();
    }
    
    private void Start()
    {
        ConfigurarBotoes();
    }
    
    /// <summary>
    /// Configura o estado inicial dos elementos (invisíveis)
    /// </summary>
    private void ConfigurarEstadoInicial()
    {
        // Torna todos os elementos invisíveis inicialmente
        if (fundoGameOver != null)
        {
            fundoGameOver.color = new Color(corFundoGameOver.r, corFundoGameOver.g, corFundoGameOver.b, 0f);
        }
        
        if (emojiChorando != null)
        {
            emojiChorando.transform.localScale = Vector3.zero;
        }
        
        if (textoGameOver != null)
        {
            textoGameOver.color = new Color(corTextoGameOver.r, corTextoGameOver.g, corTextoGameOver.b, 0f);
            textoGameOver.transform.localScale = Vector3.zero;
        }
        
        if (botaoRestart != null)
        {
            botaoRestart.transform.localScale = Vector3.zero;
        }
        
        // Esconde o painel inteiro inicialmente
        this.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Configura os eventos dos botões
    /// </summary>
    private void ConfigurarBotoes()
    {
        if (botaoRestart != null)
        {
            botaoRestart.onClick.AddListener(ReiniciarJogo);
        }
        
        if (botaoNextPiece != null)
        {
            botaoNextPiece.onClick.AddListener(ProximaPeca);
        }
        
        if (botaoCards != null)
        {
            botaoCards.onClick.AddListener(AbrirCartas);
        }
    }
    
    /// <summary>
    /// Exibe a tela de Game Over com animações
    /// </summary>
    public void ExibirGameOver()
    {
        Debug.Log("Exibindo tela de Game Over");
        
        // Ativa o GameObject
        this.gameObject.SetActive(true);
        
        // Toca som de game over
        if (audioManager != null && somGameOver != null)
        {
            audioManager.TocarSomEfeito(somGameOver);
        }
        
        // Inicia sequência de animação
        IniciarAnimacaoEntrada();
        
        // Atualiza informações dos jogadores
        AtualizarInformacoesJogadores();
        
        // Configura timer de game over (5 segundos como na imagem)
        ConfigurarTimerGameOver();
    }
    
    /// <summary>
    /// Inicia a animação de entrada dos elementos
    /// </summary>
    private void IniciarAnimacaoEntrada()
    {
        // Para qualquer animação anterior
        if (sequenciaAnimacao != null)
        {
            sequenciaAnimacao.Kill();
        }
        
        sequenciaAnimacao = DOTween.Sequence();
        
        // Animação do fundo
        if (fundoGameOver != null)
        {
            sequenciaAnimacao.Append(
                fundoGameOver.DOColor(corFundoGameOver, duracaoAnimacaoEntrada * 0.5f)
            );
        }
        
        // Animação do emoji (aparecer e balançar)
        if (emojiChorando != null)
        {
            sequenciaAnimacao.Join(
                emojiChorando.transform.DOScale(Vector3.one, duracaoAnimacaoEntrada)
                    .SetEase(Ease.OutBounce)
            );
        }
        
        // Animação do texto GAME OVER
        if (textoGameOver != null)
        {
            sequenciaAnimacao.Join(
                textoGameOver.DOColor(corTextoGameOver, duracaoAnimacaoEntrada * 0.7f)
                    .SetDelay(delayEntradaElementos)
            );
            
            sequenciaAnimacao.Join(
                textoGameOver.transform.DOScale(Vector3.one, duracaoAnimacaoEntrada)
                    .SetEase(Ease.OutBack)
                    .SetDelay(delayEntradaElementos)
            );
        }
        
        // Animação do botão restart
        if (botaoRestart != null)
        {
            sequenciaAnimacao.Join(
                botaoRestart.transform.DOScale(Vector3.one, duracaoAnimacaoEntrada * 0.8f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(delayEntradaElementos * 2)
            );
        }
        
        Debug.Log("Animação de Game Over iniciada");
    }
    
    /// <summary>
    /// Configura o timer de game over (5 segundos)
    /// </summary>
    private void ConfigurarTimerGameOver()
    {
        if (textoTimer != null)
        {
            // Inicia contagem regressiva de 5 segundos
            StartCoroutine(ContadorGameOver(5f));
        }
    }
    
    /// <summary>
    /// Corrotina para contagem regressiva no game over
    /// </summary>
    /// <param name="tempoInicial">Tempo inicial em segundos</param>
    private System.Collections.IEnumerator ContadorGameOver(float tempoInicial)
    {
        float tempoRestante = tempoInicial;
        
        while (tempoRestante > 0)
        {
            // Atualiza o display do timer
            if (textoTimer != null)
            {
                textoTimer.text = Mathf.Ceil(tempoRestante).ToString("00");
            }
            
            tempoRestante -= Time.unscaledDeltaTime; // Usa unscaledDeltaTime pois o jogo está pausado
            yield return null;
        }
        
        // Tempo esgotado - volta ao menu automaticamente
        if (textoTimer != null)
        {
            textoTimer.text = "00";
        }
        
        // Pode implementar ação automática aqui se necessário
        Debug.Log("Timer de Game Over esgotado");
    }
    
    /// <summary>
    /// Atualiza as informações dos jogadores (níveis e avatares)
    /// </summary>
    private void AtualizarInformacoesJogadores()
    {
        // Exemplo de configuração baseado na imagem (2/3, 1/3)
        if (textosNivelJogadores != null && textosNivelJogadores.Length >= 2)
        {
            if (textosNivelJogadores[0] != null)
                textosNivelJogadores[0].text = "2/3";
            
            if (textosNivelJogadores[1] != null)
                textosNivelJogadores[1].text = "1/3";
        }
        
        // Configurar cores dos avatares baseado no estado
        ConfigurarAvatarsJogadores();
    }
    
    /// <summary>
    /// Configura a aparência dos avatares dos jogadores
    /// </summary>
    private void ConfigurarAvatarsJogadores()
    {
        if (avatarsJogadores != null)
        {
            for (int i = 0; i < avatarsJogadores.Length; i++)
            {
                if (avatarsJogadores[i] != null)
                {
                    // Exemplo: jogador atual fica destacado
                    bool ehJogadorAtivo = i == 0; // Assume que o primeiro é o jogador ativo
                    
                    avatarsJogadores[i].color = ehJogadorAtivo ? Color.white : Color.gray;
                }
            }
        }
    }
    
    /// <summary>
    /// Função chamada pelo botão Restart
    /// </summary>
    private void ReiniciarJogo()
    {
        Debug.Log("Botão Restart pressionado");
        
        // Toca som do botão
        if (audioManager != null && somBotao != null)
        {
            audioManager.TocarSomEfeito(somBotao);
        }
        
        // Chama o game manager para reiniciar
        if (gameManager != null)
        {
            gameManager.ReiniciarJogo();
        }
    }
    
    /// <summary>
    /// Função chamada pelo botão Next Piece
    /// </summary>
    private void ProximaPeca()
    {
        Debug.Log("Botão Next Piece pressionado");
        
        // Implementar lógica de próxima peça se necessário
        // Por enquanto só toca o som
        if (audioManager != null && somBotao != null)
        {
            audioManager.TocarSomEfeito(somBotao);
        }
    }
    
    /// <summary>
    /// Função chamada pelo botão Cards
    /// </summary>
    private void AbrirCartas()
    {
        Debug.Log("Botão Cards pressionado");
        
        // Implementar abertura do sistema de cartas
        if (audioManager != null && somBotao != null)
        {
            audioManager.TocarSomEfeito(somBotao);
        }
    }
    
    /// <summary>
    /// Esconde a tela de Game Over
    /// </summary>
    public void EsconderGameOver()
    {
        // Para animações
        if (sequenciaAnimacao != null)
        {
            sequenciaAnimacao.Kill();
        }
        
        // Esconde o GameObject
        this.gameObject.SetActive(false);
        
        Debug.Log("Tela de Game Over escondida");
    }
    
    /// <summary>
    /// Configura a mensagem de propaganda
    /// </summary>
    /// <param name="mensagem">Mensagem da propaganda</param>
    public void ConfigurarPropaganda(string mensagem)
    {
        if (textoPropaganda != null)
        {
            textoPropaganda.text = mensagem;
        }
        
        if (areaPropaganda != null)
        {
            areaPropaganda.SetActive(!string.IsNullOrEmpty(mensagem));
        }
    }
    
    private void OnDestroy()
    {
        // Limpa animações ao destruir
        if (sequenciaAnimacao != null)
        {
            sequenciaAnimacao.Kill();
        }
    }
}
