using UnityEngine;

/// <summary>
/// Enumeração dos estados possíveis do jogo
/// </summary>
public enum EstadoJogo
{
    Menu,           // Estado do menu principal
    Jogando,        // Estado de jogo ativo
    Pausado,        // Estado pausado
    GameOver,       // Estado de fim de jogo
    Carregando      // Estado de carregamento
}

/// <summary>
/// Classe responsável por gerenciar os estados do jogo
/// </summary>
public class GameState : MonoBehaviour
{
    [Header("Estado Atual do Jogo")]
    [SerializeField] private EstadoJogo estadoAtual = EstadoJogo.Menu;
    
    // Eventos para notificar mudanças de estado
    public static System.Action<EstadoJogo> OnEstadoAlterado;
    public static System.Action OnGameOver;
    public static System.Action OnJogoIniciado;
    public static System.Action OnJogoPausado;
    public static System.Action OnJogoRetomado;
    
    // Propriedade pública para acessar o estado atual
    public EstadoJogo EstadoAtual => estadoAtual;
    
    // Singleton para acesso global
    public static GameState Instancia { get; private set; }
    
    private void Awake()
    {
        // Implementa padrão Singleton
        if (Instancia != null && Instancia != this)
        {
            Destroy(this.gameObject);
            return;
        }
        
        Instancia = this;
        DontDestroyOnLoad(this.gameObject);
    }
    
    /// <summary>
    /// Altera o estado do jogo
    /// </summary>
    /// <param name="novoEstado">Novo estado para o jogo</param>
    public void AlterarEstado(EstadoJogo novoEstado)
    {
        if (estadoAtual == novoEstado) return;
        
        EstadoJogo estadoAnterior = estadoAtual;
        estadoAtual = novoEstado;
        
        // Loga a mudança de estado para debug
        Debug.Log($"Estado alterado de {estadoAnterior} para {novoEstado}");
        
        // Executa ações específicas baseadas no novo estado
        ExecutarAcoesDoEstado(novoEstado);
        
        // Notifica outros sistemas sobre a mudança
        OnEstadoAlterado?.Invoke(novoEstado);
    }
    
    /// <summary>
    /// Executa ações específicas baseadas no estado atual
    /// </summary>
    /// <param name="estado">Estado atual do jogo</param>
    private void ExecutarAcoesDoEstado(EstadoJogo estado)
    {
        switch (estado)
        {
            case EstadoJogo.Menu:
                Time.timeScale = 1f;
                break;
                
            case EstadoJogo.Jogando:
                Time.timeScale = 1f;
                OnJogoIniciado?.Invoke();
                break;
                
            case EstadoJogo.Pausado:
                Time.timeScale = 0f;
                OnJogoPausado?.Invoke();
                break;
                
            case EstadoJogo.GameOver:
                Time.timeScale = 0f;
                OnGameOver?.Invoke();
                break;
                
            case EstadoJogo.Carregando:
                Time.timeScale = 0f;
                break;
        }
    }
    
    /// <summary>
    /// Métodos de conveniência para mudanças de estado comuns
    /// </summary>
    public void IniciarJogo() => AlterarEstado(EstadoJogo.Jogando);
    public void PausarJogo() => AlterarEstado(EstadoJogo.Pausado);
    public void RetomarJogo() => AlterarEstado(EstadoJogo.Jogando);
    public void TerminarJogo() => AlterarEstado(EstadoJogo.GameOver);
    public void VoltarAoMenu() => AlterarEstado(EstadoJogo.Menu);
    
    /// <summary>
    /// Verifica se o jogo está em um estado específico
    /// </summary>
    /// <param name="estado">Estado a ser verificado</param>
    /// <returns>True se estiver no estado especificado</returns>
    public bool EstaNoEstado(EstadoJogo estado)
    {
        return estadoAtual == estado;
    }
    
    /// <summary>
    /// Verifica se o jogo está pausado ou parado
    /// </summary>
    /// <returns>True se o jogo estiver pausado ou em game over</returns>
    public bool JogoParado()
    {
        return estadoAtual == EstadoJogo.Pausado || 
               estadoAtual == EstadoJogo.GameOver || 
               estadoAtual == EstadoJogo.Menu;
    }
}
