using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Sistema de cartas baseado na interface mostrada na imagem
/// </summary>
public class CardSystem : MonoBehaviour
{
    [Header("Configurações das Cartas")]
    [SerializeField] private int capacidadeMaximaInventario = 6;
    [SerializeField] private List<CartaData> cartasDisponiveis = new List<CartaData>();
    [SerializeField] private List<CartaData> cartasInventario = new List<CartaData>();
    
    [Header("Interface das Cartas")]
    [SerializeField] private Transform containerCartas;
    [SerializeField] private GameObject prefabCartaUI;
    [SerializeField] private UnityEngine.UI.Button botaoCards;
    [SerializeField] private UnityEngine.UI.Button botaoNextPiece;
    
    // Eventos
    public static System.Action<CartaData> OnCartaAdicionada;
    public static System.Action<CartaData> OnCartaUsada;
    public static System.Action<CartaData> OnCartaRemovida;
    public static System.Action OnInventarioAtualizado;
    
    // Referências
    private AudioManager audioManager;
    private PlayerController playerController;
    private GameManager gameManager;
    
    // Estado do sistema
    private bool interfaceCartasAberta = false;
    
    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        playerController = FindObjectOfType<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
    }
    
    private void Start()
    {
        InicializarSistema();
        ConfigurarBotoes();
    }
    
    /// <summary>
    /// Inicializa o sistema de cartas
    /// </summary>
    private void InicializarSistema()
    {
        // Cria cartas básicas se a lista estiver vazia
        if (cartasDisponiveis.Count == 0)
        {
            CriarCartasBasicas();
        }
        
        // Atualiza interface
        AtualizarInterfaceCartas();
        
        Debug.Log("Sistema de cartas inicializado");
    }
    
    /// <summary>
    /// Cria cartas básicas para o jogo
    /// </summary>
    private void CriarCartasBasicas()
    {
        // Carta de Cura
        CartaData cartaCura = new CartaData
        {
            id = "cura_basica",
            nome = "Poção de Cura",
            descricao = "Restaura 50 pontos de vida",
            raridade = RaridadeCarta.Comum,
            tipo = TipoCarta.Consumivel,
            custoEnergia = 1,
            icone = "💊"
        };
        cartasDisponiveis.Add(cartaCura);
        
        // Carta de Ataque
        CartaData cartaAtaque = new CartaData
        {
            id = "ataque_especial",
            nome = "Golpe Poderoso",
            descricao = "Próximo ataque causa 2x mais dano",
            raridade = RaridadeCarta.Incomum,
            tipo = TipoCarta.Habilidade,
            custoEnergia = 2,
            icone = "⚔️"
        };
        cartasDisponiveis.Add(cartaAtaque);
        
        // Carta de Velocidade
        CartaData cartaVelocidade = new CartaData
        {
            id = "velocidade",
            nome = "Rajada de Vento",
            descricao = "Aumenta velocidade por 10 segundos",
            raridade = RaridadeCarta.Comum,
            tipo = TipoCarta.Buff,
            custoEnergia = 1,
            icone = "💨"
        };
        cartasDisponiveis.Add(cartaVelocidade);
        
        // Carta Rara
        CartaData cartaRara = new CartaData
        {
            id = "invencibilidade",
            nome = "Escudo Divino",
            descricao = "Torna-se invencível por 5 segundos",
            raridade = RaridadeCarta.Rara,
            tipo = TipoCarta.Defesa,
            custoEnergia = 3,
            icone = "🛡️"
        };
        cartasDisponiveis.Add(cartaRara);
    }
    
    /// <summary>
    /// Configura os botões da interface
    /// </summary>
    private void ConfigurarBotoes()
    {
        if (botaoCards != null)
        {
            botaoCards.onClick.AddListener(AlternarInterfaceCartas);
        }
        
        if (botaoNextPiece != null)
        {
            botaoNextPiece.onClick.AddListener(ObterProximaCarta);
        }
    }
    
    /// <summary>
    /// Adiciona uma carta ao inventário
    /// </summary>
    /// <param name="carta">Carta para adicionar</param>
    /// <returns>True se foi adicionada com sucesso</returns>
    public bool AdicionarCarta(CartaData carta)
    {
        if (cartasInventario.Count >= capacidadeMaximaInventario)
        {
            Debug.LogWarning("Inventário de cartas está cheio!");
            return false;
        }
        
        cartasInventario.Add(carta);
        OnCartaAdicionada?.Invoke(carta);
        OnInventarioAtualizado?.Invoke();
        
        // Toca som de carta obtida
        if (audioManager != null)
        {
            audioManager.TocarSomCartaObtida();
        }
        
        AtualizarInterfaceCartas();
        
        Debug.Log($"Carta adicionada: {carta.nome}");
        return true;
    }
    
    /// <summary>
    /// Remove uma carta do inventário
    /// </summary>
    /// <param name="carta">Carta para remover</param>
    /// <returns>True se foi removida com sucesso</returns>
    public bool RemoverCarta(CartaData carta)
    {
        if (cartasInventario.Remove(carta))
        {
            OnCartaRemovida?.Invoke(carta);
            OnInventarioAtualizado?.Invoke();
            AtualizarInterfaceCartas();
            
            Debug.Log($"Carta removida: {carta.nome}");
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Usa uma carta do inventário
    /// </summary>
    /// <param name="carta">Carta para usar</param>
    public void UsarCarta(CartaData carta)
    {
        if (!cartasInventario.Contains(carta))
        {
            Debug.LogWarning("Carta não está no inventário!");
            return;
        }
        
        // Executa efeito da carta
        ExecutarEfeitoCarta(carta);
        
        // Remove carta do inventário se for consumível
        if (carta.tipo == TipoCarta.Consumivel)
        {
            RemoverCarta(carta);
        }
        
        OnCartaUsada?.Invoke(carta);
        
        // Toca som de carta usada
        if (audioManager != null)
        {
            audioManager.TocarSomCartaUsada();
        }
        
        Debug.Log($"Carta usada: {carta.nome}");
    }
    
    /// <summary>
    /// Executa o efeito específico de uma carta
    /// </summary>
    /// <param name="carta">Carta cujo efeito será executado</param>
    private void ExecutarEfeitoCarta(CartaData carta)
    {
        switch (carta.id)
        {
            case "cura_basica":
                if (playerController != null)
                {
                    playerController.Curar(50f);
                }
                break;
                
            case "ataque_especial":
                // Implementar buff de ataque
                StartCoroutine(BuffAtaqueEspecial(10f)); // 10 segundos
                break;
                
            case "velocidade":
                // Implementar buff de velocidade
                StartCoroutine(BuffVelocidade(10f)); // 10 segundos
                break;
                
            case "invencibilidade":
                // Implementar invencibilidade temporária
                StartCoroutine(BuffInvencibilidade(5f)); // 5 segundos
                break;
                
            default:
                Debug.LogWarning($"Efeito não implementado para carta: {carta.id}");
                break;
        }
    }
    
    /// <summary>
    /// Corrotina para buff de ataque especial
    /// </summary>
    private System.Collections.IEnumerator BuffAtaqueEspecial(float duracao)
    {
        Debug.Log("Buff de ataque ativado!");
        // Implementar lógica de dobrar dano
        yield return new WaitForSeconds(duracao);
        Debug.Log("Buff de ataque terminou");
    }
    
    /// <summary>
    /// Corrotina para buff de velocidade
    /// </summary>
    private System.Collections.IEnumerator BuffVelocidade(float duracao)
    {
        Debug.Log("Buff de velocidade ativado!");
        // Implementar aumento de velocidade
        yield return new WaitForSeconds(duracao);
        Debug.Log("Buff de velocidade terminou");
    }
    
    /// <summary>
    /// Corrotina para buff de invencibilidade
    /// </summary>
    private System.Collections.IEnumerator BuffInvencibilidade(float duracao)
    {
        Debug.Log("Invencibilidade ativada!");
        // Implementar invencibilidade temporária
        yield return new WaitForSeconds(duracao);
        Debug.Log("Invencibilidade terminou");
    }
    
    /// <summary>
    /// Obtém uma carta aleatória (função do botão Next Piece)
    /// </summary>
    public void ObterProximaCarta()
    {
        if (cartasDisponiveis.Count == 0)
        {
            Debug.LogWarning("Não há cartas disponíveis!");
            return;
        }
        
        // Seleciona carta aleatória baseada na raridade
        CartaData cartaSelecionada = SelecionarCartaAleatoria();
        
        if (AdicionarCarta(cartaSelecionada))
        {
            Debug.Log($"Nova carta obtida: {cartaSelecionada.nome}");
        }
    }
    
    /// <summary>
    /// Seleciona uma carta aleatória considerando raridade
    /// </summary>
    /// <returns>Carta selecionada</returns>
    private CartaData SelecionarCartaAleatoria()
    {
        // Pesos baseados na raridade
        List<CartaData> cartasPonderadas = new List<CartaData>();
        
        foreach (CartaData carta in cartasDisponiveis)
        {
            int peso = ObterPesoRaridade(carta.raridade);
            for (int i = 0; i < peso; i++)
            {
                cartasPonderadas.Add(carta);
            }
        }
        
        int indiceAleatorio = Random.Range(0, cartasPonderadas.Count);
        return cartasPonderadas[indiceAleatorio];
    }
    
    /// <summary>
    /// Obtém o peso da raridade para seleção aleatória
    /// </summary>
    private int ObterPesoRaridade(RaridadeCarta raridade)
    {
        switch (raridade)
        {
            case RaridadeCarta.Comum: return 10;
            case RaridadeCarta.Incomum: return 5;
            case RaridadeCarta.Rara: return 2;
            case RaridadeCarta.Epica: return 1;
            default: return 1;
        }
    }
    
    /// <summary>
    /// Alterna a visibilidade da interface de cartas
    /// </summary>
    public void AlternarInterfaceCartas()
    {
        interfaceCartasAberta = !interfaceCartasAberta;
        
        // Implementar lógica de mostrar/esconder interface
        if (containerCartas != null)
        {
            containerCartas.gameObject.SetActive(interfaceCartasAberta);
        }
        
        Debug.Log($"Interface de cartas {(interfaceCartasAberta ? "aberta" : "fechada")}");
    }
    
    /// <summary>
    /// Atualiza a interface visual das cartas
    /// </summary>
    private void AtualizarInterfaceCartas()
    {
        if (containerCartas == null || prefabCartaUI == null) return;
        
        // Remove cartas antigas da interface
        foreach (Transform child in containerCartas)
        {
            Destroy(child.gameObject);
        }
        
        // Cria elementos visuais para cada carta
        foreach (CartaData carta in cartasInventario)
        {
            GameObject cartaUI = Instantiate(prefabCartaUI, containerCartas);
            ConfigurarCartaUI(cartaUI, carta);
        }
    }
    
    /// <summary>
    /// Configura a interface visual de uma carta específica
    /// </summary>
    private void ConfigurarCartaUI(GameObject cartaUI, CartaData carta)
    {
        // Implementar configuração da UI da carta
        // Assumindo que o prefab tem componentes Text/Image apropriados
        
        UnityEngine.UI.Text textoNome = cartaUI.GetComponentInChildren<UnityEngine.UI.Text>();
        if (textoNome != null)
        {
            textoNome.text = carta.nome;
        }
        
        // Adicionar evento de clique para usar a carta
        UnityEngine.UI.Button botaoCarta = cartaUI.GetComponent<UnityEngine.UI.Button>();
        if (botaoCarta != null)
        {
            botaoCarta.onClick.AddListener(() => UsarCarta(carta));
        }
    }
    
    /// <summary>
    /// Salva o estado atual das cartas
    /// </summary>
    public void SalvarCartas()
    {
        // Implementar salvamento das cartas se necessário
        Debug.Log("Cartas salvas");
    }
    
    /// <summary>
    /// Carrega o estado das cartas
    /// </summary>
    public void CarregarCartas()
    {
        // Implementar carregamento das cartas se necessário
        Debug.Log("Cartas carregadas");
    }
}

/// <summary>
/// Estrutura de dados para representar uma carta
/// </summary>
[System.Serializable]
public class CartaData
{
    public string id;
    public string nome;
    public string descricao;
    public RaridadeCarta raridade;
    public TipoCarta tipo;
    public int custoEnergia;
    public string icone;
}

/// <summary>
/// Enumeração dos tipos de raridade das cartas
/// </summary>
public enum RaridadeCarta
{
    Comum,
    Incomum,
    Rara,
    Epica,
    Lendaria
}

/// <summary>
/// Enumeração dos tipos de cartas
/// </summary>
public enum TipoCarta
{
    Consumivel,
    Habilidade,
    Buff,
    Defesa,
    Ataque
}
