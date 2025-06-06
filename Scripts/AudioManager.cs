using UnityEngine;
using System.Collections;

/// <summary>
/// Gerenciador de áudio - controla música de fundo, efeitos sonoros e configurações de som
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Componentes de Áudio")]
    [SerializeField] private AudioSource musicaFundo;
    [SerializeField] private AudioSource efeitosSonoros;
    [SerializeField] private AudioSource vozesPersonagens;
    
    [Header("Músicas")]
    [SerializeField] private AudioClip musicaMenu;
    [SerializeField] private AudioClip musicaJogo;
    [SerializeField] private AudioClip musicaGameOver;
    [SerializeField] private AudioClip musicaVitoria;
    
    [Header("Efeitos Sonoros - Jogador")]
    [SerializeField] private AudioClip somPulo;
    [SerializeField] private AudioClip somAtaque;
    [SerializeField] private AudioClip somHit;
    [SerializeField] private AudioClip somMorte;
    
    [Header("Efeitos Sonoros - UI")]
    [SerializeField] private AudioClip somBotao;
    [SerializeField] private AudioClip somPontos;
    [SerializeField] private AudioClip somRecorde;
    [SerializeField] private AudioClip somMultiplicador;
    
    [Header("Efeitos Sonoros - Cartas")]
    [SerializeField] private AudioClip somCartaObtida;
    [SerializeField] private AudioClip somCartaUsada;
    
    [Header("Efeitos Sonoros - Avisos")]
    [SerializeField] private AudioClip somAvisoUrgente;
    [SerializeField] private AudioClip somAvisoCritico;
    [SerializeField] private AudioClip somTempoEsgotado;
    
    [Header("Configurações")]
    [SerializeField, Range(0f, 1f)] private float volumeMaster = 1f;
    [SerializeField, Range(0f, 1f)] private float volumeMusica = 0.7f;
    [SerializeField, Range(0f, 1f)] private float volumeEfeitos = 0.8f;
    [SerializeField, Range(0f, 1f)] private float volumeVozes = 0.9f;
    [SerializeField] private bool musicaAtiva = true;
    [SerializeField] private bool efeitosAtivos = true;
    
    // Estado atual
    private AudioClip musicaAtualTocando;
    private Corrotine fadeMusica;
    private bool sistemaMutado = false;
    
    // Singleton
    public static AudioManager Instancia { get; private set; }
    
    // Eventos
    public static System.Action<bool> OnMusicaAlterada;
    public static System.Action<bool> OnEfeitosAlterados;
    public static System.Action<float> OnVolumeAlterado;
    
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
        
        InicializarAudio();
    }
    
    private void Start()
    {
        CarregarConfiguracoes();
        RegistrarEventos();
    }
    
    private void OnDestroy()
    {
        DesregistrarEventos();
    }
    
    /// <summary>
    /// Inicializa os componentes de áudio
    /// </summary>
    private void InicializarAudio()
    {
        // Cria AudioSources se não existirem
        if (musicaFundo == null)
        {
            GameObject musicaObj = new GameObject("MusicaFundo");
            musicaObj.transform.SetParent(this.transform);
            musicaFundo = musicaObj.AddComponent<AudioSource>();
            musicaFundo.loop = true;
            musicaFundo.playOnAwake = false;
        }
        
        if (efeitosSonoros == null)
        {
            GameObject efeitosObj = new GameObject("EfeitosSonoros");
            efeitosObj.transform.SetParent(this.transform);
            efeitosSonoros = efeitosObj.AddComponent<AudioSource>();
            efeitosSonoros.loop = false;
            efeitosSonoros.playOnAwake = false;
        }
        
        if (vozesPersonagens == null)
        {
            GameObject vozesObj = new GameObject("VozesPersonagens");
            vozesObj.transform.SetParent(this.transform);
            vozesPersonagens = vozesObj.AddComponent<AudioSource>();
            vozesPersonagens.loop = false;
            vozesPersonagens.playOnAwake = false;
        }
        
        AtualizarVolumes();
        
        Debug.Log("Sistema de áudio inicializado");
    }
    
    /// <summary>
    /// Registra eventos de outros sistemas
    /// </summary>
    private void RegistrarEventos()
    {
        GameState.OnEstadoAlterado += HandleMudancaEstado;
    }
    
    /// <summary>
    /// Remove registro de eventos
    /// </summary>
    private void DesregistrarEventos()
    {
        GameState.OnEstadoAlterado -= HandleMudancaEstado;
    }
    
    /// <summary>
    /// Handle para mudanças de estado do jogo
    /// </summary>
    private void HandleMudancaEstado(EstadoJogo novoEstado)
    {
        switch (novoEstado)
        {
            case EstadoJogo.Menu:
                TocarMusicaFundo(musicaMenu);
                break;
                
            case EstadoJogo.Jogando:
                TocarMusicaFundo(musicaJogo);
                break;
                
            case EstadoJogo.GameOver:
                TocarMusicaFundo(musicaGameOver);
                break;
        }
    }
    
    /// <summary>
    /// Toca música de fundo
    /// </summary>
    /// <param name="novaMusica">Clip de áudio para tocar</param>
    /// <param name="fadeIn">Se deve fazer fade in</param>
    public void TocarMusicaFundo(AudioClip novaMusica, bool fadeIn = true)
    {
        if (!musicaAtiva || novaMusica == null || novaMusica == musicaAtualTocando)
            return;
        
        if (fadeMusica != null)
        {
            StopCoroutine(fadeMusica);
        }
        
        if (fadeIn && musicaFundo.isPlaying)
        {
            fadeMusica = StartCoroutine(FadeMusica(novaMusica));
        }
        else
        {
            musicaFundo.clip = novaMusica;
            musicaFundo.Play();
            musicaAtualTocando = novaMusica;
        }
        
        Debug.Log($"Tocando música: {novaMusica.name}");
    }
    
    /// <summary>
    /// Para a música de fundo
    /// </summary>
    public void PararMusicaFundo()
    {
        if (musicaFundo.isPlaying)
        {
            musicaFundo.Stop();
            musicaAtualTocando = null;
            Debug.Log("Música de fundo parada");
        }
    }
    
    /// <summary>
    /// Pausa a música de fundo
    /// </summary>
    public void PausarMusicaFundo()
    {
        if (musicaFundo.isPlaying)
        {
            musicaFundo.Pause();
            Debug.Log("Música de fundo pausada");
        }
    }
    
    /// <summary>
    /// Retoma a música de fundo pausada
    /// </summary>
    public void RetomarMusicaFundo()
    {
        if (!musicaFundo.isPlaying && musicaFundo.clip != null)
        {
            musicaFundo.UnPause();
            Debug.Log("Música de fundo retomada");
        }
    }
    
    /// <summary>
    /// Corrotina para fazer fade entre músicas
    /// </summary>
    private IEnumerator FadeMusica(AudioClip novaMusica)
    {
        float duracaoFade = 1f;
        float volumeOriginal = musicaFundo.volume;
        
        // Fade out da música atual
        while (musicaFundo.volume > 0)
        {
            musicaFundo.volume -= volumeOriginal * Time.deltaTime / duracaoFade;
            yield return null;
        }
        
        // Troca a música
        musicaFundo.clip = novaMusica;
        musicaFundo.Play();
        musicaAtualTocando = novaMusica;
        
        // Fade in da nova música
        while (musicaFundo.volume < volumeOriginal)
        {
            musicaFundo.volume += volumeOriginal * Time.deltaTime / duracaoFade;
            yield return null;
        }
        
        musicaFundo.volume = volumeOriginal;
        fadeMusica = null;
    }
    
    /// <summary>
    /// Toca um efeito sonoro
    /// </summary>
    /// <param name="clip">Clip de áudio para tocar</param>
    /// <param name="volume">Volume específico (opcional)</param>
    public void TocarSomEfeito(AudioClip clip, float volume = 1f)
    {
        if (!efeitosAtivos || clip == null) return;
        
        efeitosSonoros.PlayOneShot(clip, volume);
    }
    
    // Métodos específicos para cada tipo de som
    public void TocarSomPulo() => TocarSomEfeito(somPulo);
    public void TocarSomAtaque() => TocarSomEfeito(somAtaque);
    public void TocarSomHit() => TocarSomEfeito(somHit);
    public void TocarSomMorte() => TocarSomEfeito(somMorte);
    public void TocarSomBotao() => TocarSomEfeito(somBotao);
    public void TocarSomPontos() => TocarSomEfeito(somPontos);
    public void TocarSomRecorde() => TocarSomEfeito(somRecorde);
    public void TocarSomMultiplicador() => TocarSomEfeito(somMultiplicador);
    public void TocarSomCartaObtida() => TocarSomEfeito(somCartaObtida);
    public void TocarSomCartaUsada() => TocarSomEfeito(somCartaUsada);
    public void TocarSomAvisoUrgente() => TocarSomEfeito(somAvisoUrgente);
    public void TocarSomAvisoCritico() => TocarSomEfeito(somAvisoCritico);
    public void TocarSomTempoEsgotado() => TocarSomEfeito(somTempoEsgotado);
    
    /// <summary>
    /// Configura volume master
    /// </summary>
    /// <param name="novoVolume">Volume de 0 a 1</param>
    public void ConfigurarVolumeMaster(float novoVolume)
    {
        volumeMaster = Mathf.Clamp01(novoVolume);
        AtualizarVolumes();
        OnVolumeAlterado?.Invoke(volumeMaster);
        SalvarConfiguracoes();
    }
    
    /// <summary>
    /// Configura volume da música
    /// </summary>
    public void ConfigurarVolumeMusica(float novoVolume)
    {
        volumeMusica = Mathf.Clamp01(novoVolume);
        AtualizarVolumes();
        SalvarConfiguracoes();
    }
    
    /// <summary>
    /// Configura volume dos efeitos
    /// </summary>
    public void ConfigurarVolumeEfeitos(float novoVolume)
    {
        volumeEfeitos = Mathf.Clamp01(novoVolume);
        AtualizarVolumes();
        SalvarConfiguracoes();
    }
    
    /// <summary>
    /// Liga/desliga música
    /// </summary>
    public void AlternarMusica()
    {
        musicaAtiva = !musicaAtiva;
        
        if (!musicaAtiva)
        {
            PausarMusicaFundo();
        }
        else if (musicaAtualTocando != null)
        {
            RetomarMusicaFundo();
        }
        
        OnMusicaAlterada?.Invoke(musicaAtiva);
        SalvarConfiguracoes();
    }
    
    /// <summary>
    /// Liga/desliga efeitos sonoros
    /// </summary>
    public void AlternarEfeitos()
    {
        efeitosAtivos = !efeitosAtivos;
        OnEfeitosAlterados?.Invoke(efeitosAtivos);
        SalvarConfiguracoes();
    }
    
    /// <summary>
    /// Muta/desmuta todo o sistema
    /// </summary>
    public void AlternarMute()
    {
        sistemaMutado = !sistemaMutado;
        AtualizarVolumes();
        
        Debug.Log($"Sistema de áudio {(sistemaMutado ? "mutado" : "desmutado")}");
    }
    
    /// <summary>
    /// Atualiza volumes de todos os AudioSources
    /// </summary>
    private void AtualizarVolumes()
    {
        float multiplicadorMute = sistemaMutado ? 0f : 1f;
        
        if (musicaFundo != null)
        {
            musicaFundo.volume = volumeMaster * volumeMusica * multiplicadorMute;
        }
        
        if (efeitosSonoros != null)
        {
            efeitosSonoros.volume = volumeMaster * volumeEfeitos * multiplicadorMute;
        }
        
        if (vozesPersonagens != null)
        {
            vozesPersonagens.volume = volumeMaster * volumeVozes * multiplicadorMute;
        }
    }
    
    /// <summary>
    /// Salva configurações de áudio
    /// </summary>
    private void SalvarConfiguracoes()
    {
        PlayerPrefs.SetFloat("VolumeMaster", volumeMaster);
        PlayerPrefs.SetFloat("VolumeMusica", volumeMusica);
        PlayerPrefs.SetFloat("VolumeEfeitos", volumeEfeitos);
        PlayerPrefs.SetInt("MusicaAtiva", musicaAtiva ? 1 : 0);
        PlayerPrefs.SetInt("EfeitosAtivos", efeitosAtivos ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Carrega configurações salvas
    /// </summary>
    private void CarregarConfiguracoes()
    {
        volumeMaster = PlayerPrefs.GetFloat("VolumeMaster", 1f);
        volumeMusica = PlayerPrefs.GetFloat("VolumeMusica", 0.7f);
        volumeEfeitos = PlayerPrefs.GetFloat("VolumeEfeitos", 0.8f);
        musicaAtiva = PlayerPrefs.GetInt("MusicaAtiva", 1) == 1;
        efeitosAtivos = PlayerPrefs.GetInt("EfeitosAtivos", 1) == 1;
        
        AtualizarVolumes();
        
        Debug.Log("Configurações de áudio carregadas");
    }
}
