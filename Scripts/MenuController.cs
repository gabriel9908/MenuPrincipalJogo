using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controlador do menu principal - gerencia navegação e opções
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Painéis do Menu")]
    [SerializeField] private GameObject painelMenuPrincipal;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject painelCreditos;
    [SerializeField] private GameObject painelCarregamento;
    
    [Header("Botões do Menu Principal")]
    [SerializeField] private Button botaoJogar;
    [SerializeField] private Button botaoOpcoes;
    [SerializeField] private Button botaoCreditos;
    [SerializeField] private Button botaoSair;
    
    [Header("Configurações de Áudio")]
    [SerializeField] private Slider sliderVolumeMaster;
    [SerializeField] private Slider sliderVolumeMusica;
    [SerializeField] private Slider sliderVolumeEfeitos;
    [SerializeField] private Toggle toggleMusica;
    [SerializeField] private Toggle toggleEfeitos;
    
    [Header("Informações do Jogo")]
    [SerializeField] private TextMeshProUGUI textoRecorde;
    [SerializeField] private TextMeshProUGUI textoVersao;
    [SerializeField] private Image imagemFundo;
    
    [Header("Efeitos Visuais")]
    [SerializeField] private Animator animadorMenu;
    [SerializeField] private ParticleSystem efeitosParticulas;
    
    // Referências para outros sistemas
    private AudioManager audioManager;
    private ScoreManager scoreManager;
    private GameManager gameManager;
    
    // Estado do menu
    private GameObject painelAtivo;
    
    private void Awake()
    {
        // Busca referências
        audioManager = FindObjectOfType<AudioManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        gameManager = FindObjectOfType<GameManager>();
    }
    
    private void Start()
    {
        InicializarMenu();
        ConfigurarBotoes();
        CarregarConfiguracoes();
    }
    
    /// <summary>
    /// Inicializa o menu principal
    /// </summary>
    private void InicializarMenu()
    {
        // Mostra o painel principal
        MostrarPainel(painelMenuPrincipal);
        
        // Atualiza informações
        AtualizarInformacoes();
        
        // Inicia efeitos visuais
        if (efeitosParticulas != null)
        {
            efeitosParticulas.Play();
        }
        
        // Toca música do menu
        if (audioManager != null)
        {
            audioManager.TocarMusicaFundo(audioManager.GetComponent<AudioSource>().clip);
        }
        
        Debug.Log("Menu principal inicializado");
    }
    
    /// <summary>
    /// Configura os eventos dos botões
    /// </summary>
    private void ConfigurarBotoes()
    {
        // Botões do menu principal
        if (botaoJogar != null)
            botaoJogar.onClick.AddListener(IniciarJogo);
        
        if (botaoOpcoes != null)
            botaoOpcoes.onClick.AddListener(AbrirOpcoes);
        
        if (botaoCreditos != null)
            botaoCreditos.onClick.AddListener(AbrirCreditos);
        
        if (botaoSair != null)
            botaoSair.onClick.AddListener(SairJogo);
        
        // Controles de áudio
        if (sliderVolumeMaster != null)
            sliderVolumeMaster.onValueChanged.AddListener(AlterarVolumeMaster);
        
        if (sliderVolumeMusica != null)
            sliderVolumeMusica.onValueChanged.AddListener(AlterarVolumeMusica);
        
        if (sliderVolumeEfeitos != null)
            sliderVolumeEfeitos.onValueChanged.AddListener(AlterarVolumeEfeitos);
        
        if (toggleMusica != null)
            toggleMusica.onValueChanged.AddListener(AlternarMusica);
        
        if (toggleEfeitos != null)
            toggleEfeitos.onValueChanged.AddListener(AlternarEfeitos);
    }
    
    /// <summary>
    /// Inicia uma nova partida
    /// </summary>
    public void IniciarJogo()
    {
        Debug.Log("Iniciando nova partida");
        
        // Toca som de botão
        if (audioManager != null)
        {
            audioManager.TocarSomBotao();
        }
        
        // Mostra tela de carregamento
        if (painelCarregamento != null)
        {
            MostrarPainel(painelCarregamento);
        }
        
        // Inicia o jogo através do GameManager
        if (gameManager != null)
        {
            gameManager.IniciarNovaPartida();
        }
        
        // Animação de transição
        if (animadorMenu != null)
        {
            animadorMenu.SetTrigger("IniciarJogo");
        }
    }
    
    /// <summary>
    /// Abre o painel de opções
    /// </summary>
    public void AbrirOpcoes()
    {
        Debug.Log("Abrindo opções");
        
        if (audioManager != null)
        {
            audioManager.TocarSomBotao();
        }
        
        MostrarPainel(painelOpcoes);
        AtualizarConfiguracoesAudio();
    }
    
    /// <summary>
    /// Abre o painel de créditos
    /// </summary>
    public void AbrirCreditos()
    {
        Debug.Log("Abrindo créditos");
        
        if (audioManager != null)
        {
            audioManager.TocarSomBotao();
        }
        
        MostrarPainel(painelCreditos);
    }
    
    /// <summary>
    /// Volta ao menu principal
    /// </summary>
    public void VoltarMenuPrincipal()
    {
        Debug.Log("Voltando ao menu principal");
        
        if (audioManager != null)
        {
            audioManager.TocarSomBotao();
        }
        
        MostrarPainel(painelMenuPrincipal);
    }
    
    /// <summary>
    /// Sai do jogo
    /// </summary>
    public void SairJogo()
    {
        Debug.Log("Saindo do jogo");
        
        if (audioManager != null)
        {
            audioManager.TocarSomBotao();
        }
        
        // Salva configurações antes de sair
        SalvarConfiguracoes();
        
        // Sai do jogo
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    /// <summary>
    /// Mostra um painel específico e esconde os outros
    /// </summary>
    /// <param name="painel">Painel para mostrar</param>
    private void MostrarPainel(GameObject painel)
    {
        // Esconde todos os painéis
        if (painelMenuPrincipal != null) painelMenuPrincipal.SetActive(false);
        if (painelOpcoes != null) painelOpcoes.SetActive(false);
        if (painelCreditos != null) painelCreditos.SetActive(false);
        if (painelCarregamento != null) painelCarregamento.SetActive(false);
        
        // Mostra o painel solicitado
        if (painel != null)
        {
            painel.SetActive(true);
            painelAtivo = painel;
        }
    }
    
    /// <summary>
    /// Atualiza informações do menu (recorde, versão, etc.)
    /// </summary>
    private void AtualizarInformacoes()
    {
        // Atualiza recorde
        if (textoRecorde != null && scoreManager != null)
        {
            textoRecorde.text = $"Recorde: {scoreManager.RecordePessoal:N0}";
        }
        
        // Atualiza versão
        if (textoVersao != null)
        {
            textoVersao.text = $"v{Application.version}";
        }
    }
    
    /// <summary>
    /// Atualiza os controles de áudio com os valores atuais
    /// </summary>
    private void AtualizarConfiguracoesAudio()
    {
        if (audioManager == null) return;
        
        // Atualiza sliders sem disparar eventos
        if (sliderVolumeMaster != null)
        {
            sliderVolumeMaster.SetValueWithoutNotify(PlayerPrefs.GetFloat("VolumeMaster", 1f));
        }
        
        if (sliderVolumeMusica != null)
        {
            sliderVolumeMusica.SetValueWithoutNotify(PlayerPrefs.GetFloat("VolumeMusica", 0.7f));
        }
        
        if (sliderVolumeEfeitos != null)
        {
            sliderVolumeEfeitos.SetValueWithoutNotify(PlayerPrefs.GetFloat("VolumeEfeitos", 0.8f));
        }
        
        // Atualiza toggles
        if (toggleMusica != null)
        {
            toggleMusica.SetIsOnWithoutNotify(PlayerPrefs.GetInt("MusicaAtiva", 1) == 1);
        }
        
        if (toggleEfeitos != null)
        {
            toggleEfeitos.SetIsOnWithoutNotify(PlayerPrefs.GetInt("EfeitosAtivos", 1) == 1);
        }
    }
    
    // Métodos para controles de áudio
    private void AlterarVolumeMaster(float valor)
    {
        if (audioManager != null)
        {
            audioManager.ConfigurarVolumeMaster(valor);
        }
    }
    
    private void AlterarVolumeMusica(float valor)
    {
        if (audioManager != null)
        {
            audioManager.ConfigurarVolumeMusica(valor);
        }
    }
    
    private void AlterarVolumeEfeitos(float valor)
    {
        if (audioManager != null)
        {
            audioManager.ConfigurarVolumeEfeitos(valor);
        }
    }
    
    private void AlternarMusica(bool ativo)
    {
        if (audioManager != null)
        {
            if (ativo != (PlayerPrefs.GetInt("MusicaAtiva", 1) == 1))
            {
                audioManager.AlternarMusica();
            }
        }
    }
    
    private void AlternarEfeitos(bool ativo)
    {
        if (audioManager != null)
        {
            if (ativo != (PlayerPrefs.GetInt("EfeitosAtivos", 1) == 1))
            {
                audioManager.AlternarEfeitos();
            }
        }
    }
    
    /// <summary>
    /// Carrega configurações salvas
    /// </summary>
    private void CarregarConfiguracoes()
    {
        // As configurações de áudio são carregadas pelo AudioManager
        // Aqui carregamos outras configurações específicas do menu
        
        Debug.Log("Configurações do menu carregadas");
    }
    
    /// <summary>
    /// Salva configurações atuais
    /// </summary>
    private void SalvarConfiguracoes()
    {
        // As configurações de áudio são salvas pelo AudioManager
        // Aqui salvamos outras configurações específicas do menu
        
        PlayerPrefs.Save();
        Debug.Log("Configurações do menu salvas");
    }
    
    /// <summary>
    /// Reseta todas as configurações para padrão
    /// </summary>
    public void ResetarConfiguracoes()
    {
        Debug.Log("Resetando configurações para padrão");
        
        // Reseta configurações de áudio
        if (audioManager != null)
        {
            audioManager.ConfigurarVolumeMaster(1f);
            audioManager.ConfigurarVolumeMusica(0.7f);
            audioManager.ConfigurarVolumeEfeitos(0.8f);
        }
        
        // Deleta dados salvos do score
        if (scoreManager != null)
        {
            PlayerPrefs.DeleteKey("RecordePontuacao");
        }
        
        // Atualiza interface
        AtualizarConfiguracoesAudio();
        AtualizarInformacoes();
        
        // Toca som de confirmação
        if (audioManager != null)
        {
            audioManager.TocarSomBotao();
        }
    }
    
    /// <summary>
    /// Handle para navegação com teclado
    /// </summary>
    private void Update()
    {
        // ESC volta ao menu principal
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (painelAtivo != painelMenuPrincipal)
            {
                VoltarMenuPrincipal();
            }
        }
        
        // Enter inicia o jogo se estiver no menu principal
        if (Input.GetKeyDown(KeyCode.Return) && painelAtivo == painelMenuPrincipal)
        {
            IniciarJogo();
        }
    }
}