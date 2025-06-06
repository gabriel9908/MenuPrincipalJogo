using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gerenciador de entrada - centraliza controles de teclado, mouse e gamepad
/// </summary>
public class InputManager : MonoBehaviour
{
    [Header("Configurações de Input")]
    [SerializeField] private bool habilitarGamepad = true;
    [SerializeField] private bool habilitarTeclado = true;
    [SerializeField] private bool habilitarMouse = true;
    
    [Header("Sensibilidade")]
    [SerializeField] private float sensibilidadeMouse = 1f;
    [SerializeField] private float deadZoneGamepad = 0.2f;
    
    // Mapeamento de teclas
    [System.Serializable]
    public class MapaTeclas
    {
        public KeyCode teclaEsquerda = KeyCode.A;
        public KeyCode teclaDireita = KeyCode.D;
        public KeyCode teclaCima = KeyCode.W;
        public KeyCode teclaBaixo = KeyCode.S;
        public KeyCode teclaPulo = KeyCode.Space;
        public KeyCode teclaAtaque = KeyCode.J;
        public KeyCode teclaEspecial = KeyCode.K;
        public KeyCode teclaPausa = KeyCode.Escape;
        public KeyCode teclaInventario = KeyCode.I;
    }
    
    [Header("Mapeamento de Teclas")]
    [SerializeField] private MapaTeclas mapa = new MapaTeclas();
    
    // Estados dos inputs
    private Dictionary<string, bool> inputsAtivos = new Dictionary<string, bool>();
    private Dictionary<string, bool> inputsPressed = new Dictionary<string, bool>();
    private Dictionary<string, bool> inputsReleased = new Dictionary<string, bool>();
    
    // Valores analógicos
    private Vector2 movimentoAnalogico = Vector2.zero;
    private Vector2 posicaoMouse = Vector2.zero;
    
    // Singleton
    public static InputManager Instancia { get; private set; }
    
    // Eventos
    public static System.Action<string> OnInputPressed;
    public static System.Action<string> OnInputReleased;
    public static System.Action<Vector2> OnMovimentoAnalogico;
    
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
        
        InicializarInputs();
    }
    
    private void Update()
    {
        if (Time.timeScale == 0) return; // Não processa input quando pausado
        
        ProcessarInputTeclado();
        ProcessarInputMouse();
        ProcessarInputGamepad();
        
        AtualizarEventos();
    }
    
    /// <summary>
    /// Inicializa o sistema de inputs
    /// </summary>
    private void InicializarInputs()
    {
        // Inicializa dicionários
        string[] nomeInputs = {
            "Esquerda", "Direita", "Cima", "Baixo",
            "Pulo", "Ataque", "Especial", "Pausa", "Inventario",
            "MouseEsquerdo", "MouseDireito", "MouseMeio"
        };
        
        foreach (string nome in nomeInputs)
        {
            inputsAtivos[nome] = false;
            inputsPressed[nome] = false;
            inputsReleased[nome] = false;
        }
        
        Debug.Log("Sistema de input inicializado");
    }
    
    /// <summary>
    /// Processa entrada do teclado
    /// </summary>
    private void ProcessarInputTeclado()
    {
        if (!habilitarTeclado) return;
        
        // Movimento
        AtualizarInput("Esquerda", Input.GetKey(mapa.teclaEsquerda) || Input.GetKey(KeyCode.LeftArrow));
        AtualizarInput("Direita", Input.GetKey(mapa.teclaDireita) || Input.GetKey(KeyCode.RightArrow));
        AtualizarInput("Cima", Input.GetKey(mapa.teclaCima) || Input.GetKey(KeyCode.UpArrow));
        AtualizarInput("Baixo", Input.GetKey(mapa.teclaBaixo) || Input.GetKey(KeyCode.DownArrow));
        
        // Ações
        AtualizarInput("Pulo", Input.GetKey(mapa.teclaPulo));
        AtualizarInput("Ataque", Input.GetKey(mapa.teclaAtaque));
        AtualizarInput("Especial", Input.GetKey(mapa.teclaEspecial));
        AtualizarInput("Pausa", Input.GetKey(mapa.teclaPausa));
        AtualizarInput("Inventario", Input.GetKey(mapa.teclaInventario));
        
        // Calcula movimento analógico baseado no teclado
        Vector2 movimentoTeclado = Vector2.zero;
        if (inputsAtivos["Esquerda"]) movimentoTeclado.x -= 1f;
        if (inputsAtivos["Direita"]) movimentoTeclado.x += 1f;
        if (inputsAtivos["Cima"]) movimentoTeclado.y += 1f;
        if (inputsAtivos["Baixo"]) movimentoTeclado.y -= 1f;
        
        movimentoAnalogico = movimentoTeclado.normalized;
    }
    
    /// <summary>
    /// Processa entrada do mouse
    /// </summary>
    private void ProcessarInputMouse()
    {
        if (!habilitarMouse) return;
        
        // Botões do mouse
        AtualizarInput("MouseEsquerdo", Input.GetMouseButton(0));
        AtualizarInput("MouseDireito", Input.GetMouseButton(1));
        AtualizarInput("MouseMeio", Input.GetMouseButton(2));
        
        // Posição do mouse
        posicaoMouse = Input.mousePosition;
        
        // Scroll do mouse (pode ser usado para zoom ou outras funcionalidades)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.1f)
        {
            // Implementar lógica de scroll se necessário
        }
    }
    
    /// <summary>
    /// Processa entrada do gamepad
    /// </summary>
    private void ProcessarInputGamepad()
    {
        if (!habilitarGamepad) return;
        
        // Stick analógico esquerdo (movimento)
        Vector2 stickEsquerdo = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );
        
        // Aplica dead zone
        if (stickEsquerdo.magnitude < deadZoneGamepad)
        {
            stickEsquerdo = Vector2.zero;
        }
        
        // Usa movimento do gamepad se for maior que o do teclado
        if (stickEsquerdo.magnitude > movimentoAnalogico.magnitude)
        {
            movimentoAnalogico = stickEsquerdo;
        }
        
        // Botões do gamepad
        AtualizarInput("Pulo", inputsAtivos["Pulo"] || Input.GetButton("Jump"));
        AtualizarInput("Ataque", inputsAtivos["Ataque"] || Input.GetButton("Fire1"));
        AtualizarInput("Especial", inputsAtivos["Especial"] || Input.GetButton("Fire2"));
        
        // D-Pad
        float dPadX = Input.GetAxis("D-Pad X");
        float dPadY = Input.GetAxis("D-Pad Y");
        
        AtualizarInput("Esquerda", inputsAtivos["Esquerda"] || dPadX < -0.5f);
        AtualizarInput("Direita", inputsAtivos["Direita"] || dPadX > 0.5f);
        AtualizarInput("Cima", inputsAtivos["Cima"] || dPadY > 0.5f);
        AtualizarInput("Baixo", inputsAtivos["Baixo"] || dPadY < -0.5f);
    }
    
    /// <summary>
    /// Atualiza o estado de um input específico
    /// </summary>
    /// <param name="nomeInput">Nome do input</param>
    /// <param name="ativo">Se o input está ativo</param>
    private void AtualizarInput(string nomeInput, bool ativo)
    {
        bool estavaPressionado = inputsAtivos.ContainsKey(nomeInput) ? inputsAtivos[nomeInput] : false;
        
        inputsAtivos[nomeInput] = ativo;
        inputsPressed[nomeInput] = ativo && !estavaPressionado;
        inputsReleased[nomeInput] = !ativo && estavaPressionado;
    }
    
    /// <summary>
    /// Dispara eventos baseados nos inputs
    /// </summary>
    private void AtualizarEventos()
    {
        // Eventos de input pressionado
        foreach (var input in inputsPressed)
        {
            if (input.Value)
            {
                OnInputPressed?.Invoke(input.Key);
            }
        }
        
        // Eventos de input solto
        foreach (var input in inputsReleased)
        {
            if (input.Value)
            {
                OnInputReleased?.Invoke(input.Key);
            }
        }
        
        // Evento de movimento analógico
        OnMovimentoAnalogico?.Invoke(movimentoAnalogico);
    }
    
    // Métodos públicos para consulta de inputs
    public bool GetInput(string nomeInput)
    {
        return inputsAtivos.ContainsKey(nomeInput) ? inputsAtivos[nomeInput] : false;
    }
    
    public bool GetInputDown(string nomeInput)
    {
        return inputsPressed.ContainsKey(nomeInput) ? inputsPressed[nomeInput] : false;
    }
    
    public bool GetInputUp(string nomeInput)
    {
        return inputsReleased.ContainsKey(nomeInput) ? inputsReleased[nomeInput] : false;
    }
    
    public Vector2 GetMovimentoAnalogico()
    {
        return movimentoAnalogico;
    }
    
    public Vector2 GetPosicaoMouse()
    {
        return posicaoMouse;
    }
    
    /// <summary>
    /// Habilita/desabilita tipos de input
    /// </summary>
    public void ConfigurarTiposInput(bool teclado, bool mouse, bool gamepad)
    {
        habilitarTeclado = teclado;
        habilitarMouse = mouse;
        habilitarGamepad = gamepad;
        
        Debug.Log($"Input configurado - Teclado: {teclado}, Mouse: {mouse}, Gamepad: {gamepad}");
    }
    
    /// <summary>
    /// Remapeia uma tecla
    /// </summary>
    /// <param name="acao">Ação a ser remapeada</param>
    /// <param name="novaTecla">Nova tecla</param>
    public void RemapearTecla(string acao, KeyCode novaTecla)
    {
        switch (acao.ToLower())
        {
            case "esquerda":
                mapa.teclaEsquerda = novaTecla;
                break;
            case "direita":
                mapa.teclaDireita = novaTecla;
                break;
            case "cima":
                mapa.teclaCima = novaTecla;
                break;
            case "baixo":
                mapa.teclaBaixo = novaTecla;
                break;
            case "pulo":
                mapa.teclaPulo = novaTecla;
                break;
            case "ataque":
                mapa.teclaAtaque = novaTecla;
                break;
            case "especial":
                mapa.teclaEspecial = novaTecla;
                break;
            case "pausa":
                mapa.teclaPausa = novaTecla;
                break;
            case "inventario":
                mapa.teclaInventario = novaTecla;
                break;
        }
        
        SalvarMapeamento();
        Debug.Log($"Tecla {acao} remapeada para {novaTecla}");
    }
    
    /// <summary>
    /// Salva o mapeamento atual
    /// </summary>
    private void SalvarMapeamento()
    {
        PlayerPrefs.SetInt("TeclaEsquerda", (int)mapa.teclaEsquerda);
        PlayerPrefs.SetInt("TeclaDireita", (int)mapa.teclaDireita);
        PlayerPrefs.SetInt("TeclaCima", (int)mapa.teclaCima);
        PlayerPrefs.SetInt("TeclaBaixo", (int)mapa.teclaBaixo);
        PlayerPrefs.SetInt("TeclaPulo", (int)mapa.teclaPulo);
        PlayerPrefs.SetInt("TeclaAtaque", (int)mapa.teclaAtaque);
        PlayerPrefs.SetInt("TeclaEspecial", (int)mapa.teclaEspecial);
        PlayerPrefs.SetInt("TeclaPausa", (int)mapa.teclaPausa);
        PlayerPrefs.SetInt("TeclaInventario", (int)mapa.teclaInventario);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Carrega o mapeamento salvo
    /// </summary>
    private void CarregarMapeamento()
    {
        mapa.teclaEsquerda = (KeyCode)PlayerPrefs.GetInt("TeclaEsquerda", (int)KeyCode.A);
        mapa.teclaDireita = (KeyCode)PlayerPrefs.GetInt("TeclaDireita", (int)KeyCode.D);
        mapa.teclaCima = (KeyCode)PlayerPrefs.GetInt("TeclaCima", (int)KeyCode.W);
        mapa.teclaBaixo = (KeyCode)PlayerPrefs.GetInt("TeclaBaixo", (int)KeyCode.S);
        mapa.teclaPulo = (KeyCode)PlayerPrefs.GetInt("TeclaPulo", (int)KeyCode.Space);
        mapa.teclaAtaque = (KeyCode)PlayerPrefs.GetInt("TeclaAtaque", (int)KeyCode.J);
        mapa.teclaEspecial = (KeyCode)PlayerPrefs.GetInt("TeclaEspecial", (int)KeyCode.K);
        mapa.teclaPausa = (KeyCode)PlayerPrefs.GetInt("TeclaPausa", (int)KeyCode.Escape);
        mapa.teclaInventario = (KeyCode)PlayerPrefs.GetInt("TeclaInventario", (int)KeyCode.I);
    }
    
    /// <summary>
    /// Reseta mapeamento para padrão
    /// </summary>
    public void ResetarMapeamento()
    {
        mapa = new MapaTeclas();
        SalvarMapeamento();
        Debug.Log("Mapeamento de teclas resetado para padrão");
    }
    
    private void Start()
    {
        CarregarMapeamento();
    }
}