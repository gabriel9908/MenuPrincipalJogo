using UnityEngine;

/// <summary>
/// Gerenciador de pontuação - controla pontos, multiplicadores e recordes
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("Configurações de Pontuação")]
    [SerializeField] private int pontuacaoInicial = 0;
    [SerializeField] private int multiplicadorBase = 1;
    [SerializeField] private int multiplicadorMaximo = 10;
    [SerializeField] private float tempoParaPerderMultiplicador = 5f;
    
    [Header("Valores de Pontos")]
    [SerializeField] private int pontosInimigoBasico = 100;
    [SerializeField] private int pontosInimigoEspecial = 250;
    [SerializeField] private int pontosPowerUp = 50;
    [SerializeField] private int pontosCombo = 25;
    
    // Estado atual
    private int pontuacaoAtual;
    private int multiplicadorAtual;
    private int comboAtual;
    private float tempoUltimaAcao;
    private int recordePessoal;
    
    // Eventos
    public static System.Action<int> OnPontuacaoAlterada;
    public static System.Action<int> OnMultiplicadorAlterado;
    public static System.Action<int> OnComboAlterado;
    public static System.Action<int> OnNovoRecorde;
    
    // Propriedades públicas
    public int PontuacaoAtual => pontuacaoAtual;
    public int MultiplicadorAtual => multiplicadorAtual;
    public int ComboAtual => comboAtual;
    public int RecordePessoal => recordePessoal;
    
    // Referências
    private AudioManager audioManager;
    
    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        
        // Carrega recorde salvo
        CarregarRecorde();
    }
    
    private void Start()
    {
        InicializarPontuacao();
    }
    
    private void Update()
    {
        // Verifica se deve reduzir multiplicador por inatividade
        VerificarMultiplicadorTempo();
    }
    
    /// <summary>
    /// Inicializa o sistema de pontuação
    /// </summary>
    private void InicializarPontuacao()
    {
        pontuacaoAtual = pontuacaoInicial;
        multiplicadorAtual = multiplicadorBase;
        comboAtual = 0;
        tempoUltimaAcao = Time.time;
        
        // Notifica sistemas
        OnPontuacaoAlterada?.Invoke(pontuacaoAtual);
        OnMultiplicadorAlterado?.Invoke(multiplicadorAtual);
        OnComboAlterado?.Invoke(comboAtual);
        
        Debug.Log("Sistema de pontuação inicializado");
    }
    
    /// <summary>
    /// Adiciona pontos por eliminar inimigo básico
    /// </summary>
    public void AdicionarPontosInimigoBasico()
    {
        AdicionarPontos(pontosInimigoBasico, true);
        Debug.Log($"Pontos por inimigo básico: {pontosInimigoBasico}");
    }
    
    /// <summary>
    /// Adiciona pontos por eliminar inimigo especial
    /// </summary>
    public void AdicionarPontosInimigoEspecial()
    {
        AdicionarPontos(pontosInimigoEspecial, true);
        Debug.Log($"Pontos por inimigo especial: {pontosInimigoEspecial}");
    }
    
    /// <summary>
    /// Adiciona pontos por coletar power-up
    /// </summary>
    public void AdicionarPontosPowerUp()
    {
        AdicionarPontos(pontosPowerUp, false);
        Debug.Log($"Pontos por power-up: {pontosPowerUp}");
    }
    
    /// <summary>
    /// Adiciona pontos gerais
    /// </summary>
    /// <param name="pontos">Quantidade de pontos</param>
    /// <param name="contaCombo">Se conta para combo</param>
    public void AdicionarPontos(int pontos, bool contaCombo = false)
    {
        // Calcula pontos com multiplicador
        int pontosComMultiplicador = pontos * multiplicadorAtual;
        
        // Adiciona à pontuação
        pontuacaoAtual += pontosComMultiplicador;
        
        // Atualiza combo se aplicável
        if (contaCombo)
        {
            AtualizarCombo();
        }
        
        // Atualiza tempo da última ação
        tempoUltimaAcao = Time.time;
        
        // Toca som de pontuação
        if (audioManager != null)
        {
            audioManager.TocarSomPontos();
        }
        
        // Notifica mudança
        OnPontuacaoAlterada?.Invoke(pontuacaoAtual);
        
        // Verifica se é novo recorde
        VerificarNovoRecorde();
        
        Debug.Log($"Pontos adicionados: {pontos} x{multiplicadorAtual} = {pontosComMultiplicador}. Total: {pontuacaoAtual}");
    }
    
    /// <summary>
    /// Atualiza o sistema de combo
    /// </summary>
    private void AtualizarCombo()
    {
        comboAtual++;
        
        // Aumenta multiplicador baseado no combo
        if (comboAtual > 0 && comboAtual % 5 == 0) // A cada 5 combos
        {
            AumentarMultiplicador();
        }
        
        // Adiciona pontos bonus por combo
        if (comboAtual > 1)
        {
            int bonusCombo = pontosCombo * comboAtual;
            pontuacaoAtual += bonusCombo;
            Debug.Log($"Bonus de combo: {bonusCombo} pontos");
        }
        
        // Notifica mudança
        OnComboAlterado?.Invoke(comboAtual);
    }
    
    /// <summary>
    /// Aumenta o multiplicador
    /// </summary>
    private void AumentarMultiplicador()
    {
        if (multiplicadorAtual < multiplicadorMaximo)
        {
            multiplicadorAtual++;
            OnMultiplicadorAlterado?.Invoke(multiplicadorAtual);
            
            // Toca som de multiplicador
            if (audioManager != null)
            {
                audioManager.TocarSomMultiplicador();
            }
            
            Debug.Log($"Multiplicador aumentado para: {multiplicadorAtual}x");
        }
    }
    
    /// <summary>
    /// Reduz o multiplicador
    /// </summary>
    private void DiminuirMultiplicador()
    {
        if (multiplicadorAtual > multiplicadorBase)
        {
            multiplicadorAtual--;
            OnMultiplicadorAlterado?.Invoke(multiplicadorAtual);
            Debug.Log($"Multiplicador reduzido para: {multiplicadorAtual}x");
        }
    }
    
    /// <summary>
    /// Quebra o combo atual
    /// </summary>
    public void QuebrarCombo()
    {
        if (comboAtual > 0)
        {
            Debug.Log($"Combo quebrado! Era {comboAtual}");
            comboAtual = 0;
            OnComboAlterado?.Invoke(comboAtual);
            
            // Reduz multiplicador drasticamente
            multiplicadorAtual = Mathf.Max(multiplicadorBase, multiplicadorAtual - 2);
            OnMultiplicadorAlterado?.Invoke(multiplicadorAtual);
        }
    }
    
    /// <summary>
    /// Verifica multiplicador baseado no tempo
    /// </summary>
    private void VerificarMultiplicadorTempo()
    {
        if (Time.time - tempoUltimaAcao > tempoParaPerderMultiplicador)
        {
            if (multiplicadorAtual > multiplicadorBase)
            {
                DiminuirMultiplicador();
                tempoUltimaAcao = Time.time; // Reseta timer
            }
        }
    }
    
    /// <summary>
    /// Verifica se estabeleceu novo recorde
    /// </summary>
    private void VerificarNovoRecorde()
    {
        if (pontuacaoAtual > recordePessoal)
        {
            recordePessoal = pontuacaoAtual;
            OnNovoRecorde?.Invoke(recordePessoal);
            
            // Salva novo recorde
            SalvarRecorde();
            
            // Toca som de recorde
            if (audioManager != null)
            {
                audioManager.TocarSomRecorde();
            }
            
            Debug.Log($"NOVO RECORDE: {recordePessoal} pontos!");
        }
    }
    
    /// <summary>
    /// Reseta a pontuação para começar nova partida
    /// </summary>
    public void ResetarPontuacao()
    {
        pontuacaoAtual = pontuacaoInicial;
        multiplicadorAtual = multiplicadorBase;
        comboAtual = 0;
        tempoUltimaAcao = Time.time;
        
        // Notifica mudanças
        OnPontuacaoAlterada?.Invoke(pontuacaoAtual);
        OnMultiplicadorAlterado?.Invoke(multiplicadorAtual);
        OnComboAlterado?.Invoke(comboAtual);
        
        Debug.Log("Pontuação resetada");
    }
    
    /// <summary>
    /// Carrega o recorde salvo
    /// </summary>
    private void CarregarRecorde()
    {
        recordePessoal = PlayerPrefs.GetInt("RecordePontuacao", 0);
        Debug.Log($"Recorde carregado: {recordePessoal}");
    }
    
    /// <summary>
    /// Salva o recorde atual
    /// </summary>
    private void SalvarRecorde()
    {
        PlayerPrefs.SetInt("RecordePontuacao", recordePessoal);
        PlayerPrefs.Save();
        Debug.Log($"Recorde salvo: {recordePessoal}");
    }
    
    /// <summary>
    /// Obtém informações detalhadas da pontuação
    /// </summary>
    /// <returns>String com informações da pontuação</returns>
    public string ObterInfoPontuacao()
    {
        return $"Pontuação: {pontuacaoAtual:N0}\n" +
               $"Multiplicador: {multiplicadorAtual}x\n" +
               $"Combo: {comboAtual}\n" +
               $"Recorde: {recordePessoal:N0}";
    }
    
    /// <summary>
    /// Calcula ranking baseado na pontuação
    /// </summary>
    /// <returns>Rank do jogador</returns>
    public string CalcularRank()
    {
        if (pontuacaoAtual >= 50000) return "S";
        if (pontuacaoAtual >= 25000) return "A";
        if (pontuacaoAtual >= 15000) return "B";
        if (pontuacaoAtual >= 8000) return "C";
        if (pontuacaoAtual >= 3000) return "D";
        return "E";
    }
}
