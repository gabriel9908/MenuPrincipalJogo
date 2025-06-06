using UnityEngine;

/// <summary>
/// Gerenciador de tempo - controla cronômetros, countdowns e eventos relacionados ao tempo
/// </summary>
public class TimerManager : MonoBehaviour
{
    [Header("Configurações do Timer")]
    [SerializeField] private float tempoLimitePartida = 180f; // 3 minutos
    [SerializeField] private bool contagemRegressiva = true;
    [SerializeField] private bool pausaAutomaticamente = false;
    
    [Header("Avisos de Tempo")]
    [SerializeField] private float tempoAvisoUrgente = 30f; // Aviso quando restam 30s
    [SerializeField] private float tempoAvisoCritico = 10f;  // Aviso quando restam 10s
    
    // Estado do timer
    private float tempoRestante;
    private float tempoDecorrido;
    private bool timerAtivo = false;
    private bool timerPausado = false;
    private bool avisoUrgenteExibido = false;
    private bool avisoCriticoExibido = false;
    
    // Eventos
    public static System.Action<float> OnTempoAtualizado;
    public static System.Action<float> OnTempoDecorridoAtualizado;
    public static System.Action OnTempoEsgotado;
    public static System.Action OnAvisoUrgente;
    public static System.Action OnAvisoCritico;
    public static System.Action OnTimerIniciado;
    public static System.Action OnTimerPausado;
    public static System.Action OnTimerRetomado;
    public static System.Action OnTimerParado;
    
    // Propriedades públicas
    public float TempoRestante => tempoRestante;
    public float TempoDecorrido => tempoDecorrido;
    public float TempoLimite => tempoLimitePartida;
    public bool TimerAtivo => timerAtivo;
    public bool TimerPausado => timerPausado;
    public float ProgressoTempo => contagemRegressiva ? 
        (tempoLimitePartida - tempoRestante) / tempoLimitePartida : 
        tempoDecorrido / tempoLimitePartida;
    
    // Referências
    private AudioManager audioManager;
    private GameManager gameManager;
    
    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        gameManager = FindObjectOfType<GameManager>();
    }
    
    private void Start()
    {
        InicializarTimer();
    }
    
    private void Update()
    {
        if (timerAtivo && !timerPausado)
        {
            AtualizarTimer();
        }
    }
    
    /// <summary>
    /// Inicializa o timer com valores padrão
    /// </summary>
    private void InicializarTimer()
    {
        if (contagemRegressiva)
        {
            tempoRestante = tempoLimitePartida;
            tempoDecorrido = 0f;
        }
        else
        {
            tempoRestante = 0f;
            tempoDecorrido = 0f;
        }
        
        timerAtivo = false;
        timerPausado = false;
        avisoUrgenteExibido = false;
        avisoCriticoExibido = false;
        
        // Notifica estado inicial
        OnTempoAtualizado?.Invoke(tempoRestante);
        OnTempoDecorridoAtualizado?.Invoke(tempoDecorrido);
        
        Debug.Log($"Timer inicializado. Modo: {(contagemRegressiva ? "Regressiva" : "Progressiva")}");
    }
    
    /// <summary>
    /// Configura o timer com tempo específico
    /// </summary>
    /// <param name="tempoLimite">Tempo limite em segundos</param>
    public void ConfigurarTimer(float tempoLimite)
    {
        tempoLimitePartida = tempoLimite;
        InicializarTimer();
        Debug.Log($"Timer configurado para {tempoLimite} segundos");
    }
    
    /// <summary>
    /// Inicia o timer
    /// </summary>
    public void IniciarTimer()
    {
        if (!timerAtivo)
        {
            timerAtivo = true;
            timerPausado = false;
            OnTimerIniciado?.Invoke();
            Debug.Log("Timer iniciado");
        }
        else
        {
            Debug.LogWarning("Timer já está ativo");
        }
    }
    
    /// <summary>
    /// Pausa o timer
    /// </summary>
    public void PausarTimer()
    {
        if (timerAtivo && !timerPausado)
        {
            timerPausado = true;
            OnTimerPausado?.Invoke();
            Debug.Log("Timer pausado");
        }
    }
    
    /// <summary>
    /// Retoma o timer pausado
    /// </summary>
    public void RetomarTimer()
    {
        if (timerAtivo && timerPausado)
        {
            timerPausado = false;
            OnTimerRetomado?.Invoke();
            Debug.Log("Timer retomado");
        }
    }
    
    /// <summary>
    /// Para o timer completamente
    /// </summary>
    public void PararTimer()
    {
        if (timerAtivo)
        {
            timerAtivo = false;
            timerPausado = false;
            OnTimerParado?.Invoke();
            Debug.Log("Timer parado");
        }
    }
    
    /// <summary>
    /// Reseta o timer para o estado inicial
    /// </summary>
    public void ResetarTimer()
    {
        PararTimer();
        InicializarTimer();
        Debug.Log("Timer resetado");
    }
    
    /// <summary>
    /// Atualiza o timer a cada frame
    /// </summary>
    private void AtualizarTimer()
    {
        float deltaTime = Time.deltaTime;
        
        if (contagemRegressiva)
        {
            // Contagem regressiva
            tempoRestante -= deltaTime;
            tempoDecorrido += deltaTime;
            
            // Verifica se o tempo acabou
            if (tempoRestante <= 0f)
            {
                tempoRestante = 0f;
                TempoEsgotado();
                return;
            }
            
            // Verifica avisos de tempo
            VerificarAvisosTempo();
        }
        else
        {
            // Contagem progressiva
            tempoDecorrido += deltaTime;
            tempoRestante = tempoDecorrido;
            
            // Verifica se atingiu o limite
            if (tempoDecorrido >= tempoLimitePartida)
            {
                TempoEsgotado();
                return;
            }
        }
        
        // Notifica atualizações
        OnTempoAtualizado?.Invoke(tempoRestante);
        OnTempoDecorridoAtualizado?.Invoke(tempoDecorrido);
    }
    
    /// <summary>
    /// Verifica se deve exibir avisos de tempo
    /// </summary>
    private void VerificarAvisosTempo()
    {
        // Aviso crítico (10 segundos)
        if (!avisoCriticoExibido && tempoRestante <= tempoAvisoCritico)
        {
            avisoCriticoExibido = true;
            OnAvisoCritico?.Invoke();
            
            if (audioManager != null)
            {
                audioManager.TocarSomAvisoCritico();
            }
            
            Debug.Log("AVISO CRÍTICO: Tempo quase esgotado!");
        }
        // Aviso urgente (30 segundos)
        else if (!avisoUrgenteExibido && tempoRestante <= tempoAvisoUrgente)
        {
            avisoUrgenteExibido = true;
            OnAvisoUrgente?.Invoke();
            
            if (audioManager != null)
            {
                audioManager.TocarSomAvisoUrgente();
            }
            
            Debug.Log("AVISO URGENTE: Tempo se esgotando!");
        }
    }
    
    /// <summary>
    /// Chamado quando o tempo se esgota
    /// </summary>
    private void TempoEsgotado()
    {
        Debug.Log("TEMPO ESGOTADO!");
        
        // Para o timer
        timerAtivo = false;
        
        // Toca som de tempo esgotado
        if (audioManager != null)
        {
            audioManager.TocarSomTempoEsgotado();
        }
        
        // Notifica evento
        OnTempoEsgotado?.Invoke();
        
        // Pausa automaticamente se configurado
        if (pausaAutomaticamente)
        {
            Time.timeScale = 0f;
        }
    }
    
    /// <summary>
    /// Adiciona tempo ao timer (power-up)
    /// </summary>
    /// <param name="tempoAdicional">Tempo em segundos para adicionar</param>
    public void AdicionarTempo(float tempoAdicional)
    {
        if (contagemRegressiva)
        {
            tempoRestante += tempoAdicional;
            // Garante que não ultrapasse o limite
            tempoRestante = Mathf.Min(tempoRestante, tempoLimitePartida);
        }
        else
        {
            // Em contagem progressiva, pode estender o limite
            tempoLimitePartida += tempoAdicional;
        }
        
        // Reseta avisos se ganhou tempo suficiente
        if (tempoRestante > tempoAvisoUrgente)
        {
            avisoUrgenteExibido = false;
        }
        if (tempoRestante > tempoAvisoCritico)
        {
            avisoCriticoExibido = false;
        }
        
        Debug.Log($"Tempo adicionado: +{tempoAdicional}s. Tempo restante: {tempoRestante}s");
    }
    
    /// <summary>
    /// Remove tempo do timer (penalidade)
    /// </summary>
    /// <param name="tempoPerdido">Tempo em segundos para remover</param>
    public void RemoverTempo(float tempoPerdido)
    {
        if (contagemRegressiva)
        {
            tempoRestante -= tempoPerdido;
            tempoRestante = Mathf.Max(0f, tempoRestante);
            
            // Verifica se o tempo acabou
            if (tempoRestante <= 0f)
            {
                TempoEsgotado();
                return;
            }
        }
        
        Debug.Log($"Tempo removido: -{tempoPerdido}s. Tempo restante: {tempoRestante}s");
    }
    
    /// <summary>
    /// Formata o tempo para exibição
    /// </summary>
    /// <param name="tempo">Tempo em segundos</param>
    /// <returns>String formatada (MM:SS)</returns>
    public static string FormatarTempo(float tempo)
    {
        int minutos = Mathf.FloorToInt(tempo / 60f);
        int segundos = Mathf.FloorToInt(tempo % 60f);
        return $"{minutos:00}:{segundos:00}";
    }
    
    /// <summary>
    /// Formata o tempo com milissegundos
    /// </summary>
    /// <param name="tempo">Tempo em segundos</param>
    /// <returns>String formatada (MM:SS:MS)</returns>
    public static string FormatarTempoDetalhado(float tempo)
    {
        int minutos = Mathf.FloorToInt(tempo / 60f);
        int segundos = Mathf.FloorToInt(tempo % 60f);
        int milissegundos = Mathf.FloorToInt((tempo % 1f) * 100f);
        return $"{minutos:00}:{segundos:00}:{milissegundos:00}";
    }
    
    /// <summary>
    /// Obtém informações detalhadas do timer
    /// </summary>
    /// <returns>String com informações do timer</returns>
    public string ObterInfoTimer()
    {
        return $"Timer Status:\n" +
               $"Ativo: {timerAtivo}\n" +
               $"Pausado: {timerPausado}\n" +
               $"Tempo Restante: {FormatarTempo(tempoRestante)}\n" +
               $"Tempo Decorrido: {FormatarTempo(tempoDecorrido)}\n" +
               $"Progresso: {(ProgressoTempo * 100f):F1}%";
    }
}
