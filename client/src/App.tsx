import React, { useState, useEffect } from "react";
import "@fontsource/inter";

interface ScriptInfo {
  name: string;
  description: string;
  features: string[];
  path: string;
}

const scripts: ScriptInfo[] = [
  {
    name: "GameState.cs",
    description: "Gerencia os estados do jogo (Menu, Jogando, Pausado, GameOver, Carregando)",
    features: [
      "Padrão Singleton",
      "Sistema de eventos para notificação de mudanças",
      "Controle automático do timeScale",
      "Métodos de conveniência para mudanças de estado"
    ],
    path: "Scripts/GameState.cs"
  },
  {
    name: "GameManager.cs", 
    description: "Controlador principal que coordena todos os sistemas do jogo",
    features: [
      "Gerencia vidas do jogador",
      "Controla início/fim de partidas",
      "Coordena interação entre sistemas",
      "Sistema de pause/resume",
      "Detecção de condições de vitória/derrota"
    ],
    path: "Scripts/GameManager.cs"
  },
  {
    name: "GameOverUI.cs",
    description: "Interface específica da tela de Game Over baseada na imagem fornecida",
    features: [
      "Animações de entrada suaves com DOTween",
      "Timer de 5 segundos como mostrado na imagem",
      "Sistema de cartas e avatares dos jogadores",
      "Botões funcionais (Restart, Next Piece, Cards)",
      "Área de propaganda personalizável"
    ],
    path: "Scripts/GameOverUI.cs"
  },
  {
    name: "PlayerController.cs",
    description: "Controla o jogador e suas ações no jogo 2D",
    features: [
      "Movimento 2D com física",
      "Sistema de combate com detecção de inimigos",
      "Gerenciamento de vida e invencibilidade",
      "Animações baseadas em estados",
      "Sistema de input responsivo"
    ],
    path: "Scripts/PlayerController.cs"
  },
  {
    name: "ScoreManager.cs",
    description: "Sistema completo de pontuação e recordes",
    features: [
      "Multiplicadores dinâmicos",
      "Sistema de combo",
      "Salvamento de recordes",
      "Diferentes tipos de pontuação",
      "Sistema de ranking"
    ],
    path: "Scripts/ScoreManager.cs"
  },
  {
    name: "CardSystem.cs",
    description: "Sistema de cartas como mostrado na interface Game Over",
    features: [
      "Inventário de cartas com capacidade limitada",
      "Diferentes tipos e raridades de cartas",
      "Sistema de efeitos (cura, ataque, buffs)",
      "Interface visual das cartas",
      "Seleção aleatória ponderada por raridade"
    ],
    path: "Scripts/CardSystem.cs"
  },
  {
    name: "AudioManager.cs",
    description: "Gerenciamento completo de áudio do jogo",
    features: [
      "Música de fundo com fade",
      "Efeitos sonoros categorizados",
      "Controles de volume individuais",
      "Sistema de mute/unmute",
      "Salvamento de configurações"
    ],
    path: "Scripts/AudioManager.cs"
  },
  {
    name: "TimerManager.cs",
    description: "Gerenciamento de tempo e cronômetros",
    features: [
      "Contagem regressiva/progressiva",
      "Avisos de tempo (urgente/crítico)",
      "Pausar/retomar funcionalidade",
      "Adicionar/remover tempo (power-ups/penalidades)",
      "Formatação de tempo para display"
    ],
    path: "Scripts/TimerManager.cs"
  }
];

const svgAssets = [
  { name: "game_over_emoji.svg", description: "Emoji chorando para tela de Game Over" },
  { name: "restart_button.svg", description: "Botão RESTART com gradiente laranja" },
  { name: "timer_background.svg", description: "Fundo do timer com ícone de relógio" },
  { name: "card_background.svg", description: "Template para cartas do inventário" },
  { name: "player_avatar.svg", description: "Avatar de jogador para interface" }
];

function App() {
  const [selectedScript, setSelectedScript] = useState<ScriptInfo | null>(null);
  const [scriptContent, setScriptContent] = useState<string>("");
  const [loading, setLoading] = useState(false);

  const loadScript = async (script: ScriptInfo) => {
    setLoading(true);
    setSelectedScript(script);
    
    try {
      const response = await fetch(`/${script.path}`);
      const content = await response.text();
      setScriptContent(content);
    } catch (error) {
      setScriptContent("// Erro ao carregar o script");
    }
    
    setLoading(false);
  };

  return (
    <div style={{ 
      fontFamily: 'Inter, sans-serif',
      width: '100vw', 
      height: '100vh', 
      background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
      overflow: 'hidden'
    }}>
      <div style={{
        display: 'flex',
        height: '100%'
      }}>
        {/* Sidebar */}
        <div style={{
          width: '350px',
          background: 'rgba(255, 255, 255, 0.95)',
          backdropFilter: 'blur(10px)',
          borderRight: '1px solid rgba(255, 255, 255, 0.2)',
          overflowY: 'auto',
          padding: '20px'
        }}>
          <div style={{
            marginBottom: '20px',
            textAlign: 'center'
          }}>
            <h1 style={{
              margin: '0 0 10px 0',
              color: '#2d3748',
              fontSize: '24px',
              fontWeight: 'bold'
            }}>
              Scripts Unity C# 
            </h1>
            <p style={{
              margin: 0,
              color: '#666',
              fontSize: '14px'
            }}>
              Sistema Game Over Completo
            </p>
          </div>

          <div style={{ marginBottom: '20px' }}>
            <h3 style={{ margin: '0 0 10px 0', color: '#2d3748', fontSize: '16px' }}>
              Scripts Principais
            </h3>
            {scripts.map((script, index) => (
              <div
                key={index}
                onClick={() => loadScript(script)}
                style={{
                  padding: '12px',
                  margin: '8px 0',
                  background: selectedScript?.name === script.name 
                    ? 'rgba(102, 126, 234, 0.1)' 
                    : 'rgba(255, 255, 255, 0.5)',
                  border: selectedScript?.name === script.name 
                    ? '2px solid #667eea' 
                    : '1px solid rgba(255, 255, 255, 0.3)',
                  borderRadius: '8px',
                  cursor: 'pointer',
                  transition: 'all 0.2s ease'
                }}
              >
                <div style={{ fontWeight: 'bold', color: '#2d3748', fontSize: '14px' }}>
                  {script.name}
                </div>
                <div style={{ color: '#666', fontSize: '12px', marginTop: '4px' }}>
                  {script.description}
                </div>
              </div>
            ))}
          </div>

          <div>
            <h3 style={{ margin: '0 0 10px 0', color: '#2d3748', fontSize: '16px' }}>
              Assets SVG Criados
            </h3>
            {svgAssets.map((asset, index) => (
              <div
                key={index}
                style={{
                  padding: '8px',
                  margin: '4px 0',
                  background: 'rgba(255, 255, 255, 0.3)',
                  borderRadius: '6px',
                  fontSize: '12px'
                }}
              >
                <div style={{ fontWeight: 'bold', color: '#2d3748' }}>
                  {asset.name}
                </div>
                <div style={{ color: '#666' }}>
                  {asset.description}
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Main Content */}
        <div style={{
          flex: 1,
          background: 'rgba(255, 255, 255, 0.9)',
          backdropFilter: 'blur(10px)',
          display: 'flex',
          flexDirection: 'column'
        }}>
          {selectedScript ? (
            <>
              <div style={{
                padding: '20px',
                borderBottom: '1px solid rgba(255, 255, 255, 0.2)',
                background: 'rgba(255, 255, 255, 0.5)'
              }}>
                <h2 style={{ margin: '0 0 10px 0', color: '#2d3748' }}>
                  {selectedScript.name}
                </h2>
                <p style={{ margin: '0 0 15px 0', color: '#666' }}>
                  {selectedScript.description}
                </p>
                <div>
                  <strong style={{ color: '#2d3748' }}>Características:</strong>
                  <ul style={{ margin: '5px 0 0 0', paddingLeft: '20px' }}>
                    {selectedScript.features.map((feature, index) => (
                      <li key={index} style={{ color: '#666', fontSize: '14px' }}>
                        {feature}
                      </li>
                    ))}
                  </ul>
                </div>
              </div>
              
              <div style={{
                flex: 1,
                padding: '20px',
                overflow: 'auto'
              }}>
                {loading ? (
                  <div style={{ textAlign: 'center', color: '#666' }}>
                    Carregando script...
                  </div>
                ) : (
                  <pre style={{
                    background: '#2d3748',
                    color: '#e2e8f0',
                    padding: '20px',
                    borderRadius: '8px',
                    fontSize: '12px',
                    lineHeight: '1.5',
                    overflow: 'auto',
                    margin: 0,
                    fontFamily: 'Monaco, Consolas, "Lucida Console", monospace'
                  }}>
                    {scriptContent}
                  </pre>
                )}
              </div>
            </>
          ) : (
            <div style={{
              flex: 1,
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              flexDirection: 'column',
              textAlign: 'center',
              padding: '40px'
            }}>
              <div style={{
                width: '120px',
                height: '120px',
                background: 'linear-gradient(45deg, #667eea, #764ba2)',
                borderRadius: '50%',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                marginBottom: '20px',
                fontSize: '48px'
              }}>
                🎮
              </div>
              <h2 style={{ color: '#2d3748', marginBottom: '10px' }}>
                Scripts Unity C# - Game Over System
              </h2>
              <p style={{ color: '#666', maxWidth: '500px', lineHeight: '1.6' }}>
                Sistema completo de scripts C# para Unity 2D baseado na interface Game Over fornecida. 
                Inclui 13 scripts principais com funcionalidades completas de gerenciamento de jogo, 
                interface, áudio, pontuação e muito mais.
              </p>
              <p style={{ color: '#667eea', fontWeight: 'bold', marginTop: '20px' }}>
                ← Selecione um script na barra lateral para visualizar o código
              </p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

export default App;
