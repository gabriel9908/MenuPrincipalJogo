import React, { useState } from "react";
import "@fontsource/inter";

const scripts = [
  {
    name: "🎮 Tutorial Completo",
    description: "Guia passo a passo de implementação dos scripts Unity",
    path: "Scripts/TUTORIAL_IMPLEMENTACAO.md",
    category: "tutorial"
  },
  {
    name: "GameState.cs",
    description: "Gerencia os estados do jogo",
    path: "Scripts/GameState.cs",
    category: "core"
  },
  {
    name: "GameManager.cs", 
    description: "Controlador principal do jogo",
    path: "Scripts/GameManager.cs",
    category: "core"
  },
  {
    name: "GameOverUI.cs",
    description: "Interface de Game Over",
    path: "Scripts/GameOverUI.cs",
    category: "ui"
  },
  {
    name: "PlayerController.cs",
    description: "Controle do jogador",
    path: "Scripts/PlayerController.cs",
    category: "player"
  },
  {
    name: "ScoreManager.cs",
    description: "Sistema de pontuação",
    path: "Scripts/ScoreManager.cs",
    category: "systems"
  },
  {
    name: "CardSystem.cs",
    description: "Sistema de cartas",
    path: "Scripts/CardSystem.cs",
    category: "systems"
  },
  {
    name: "AudioManager.cs",
    description: "Gerenciamento de áudio",
    path: "Scripts/AudioManager.cs",
    category: "systems"
  },
  {
    name: "TimerManager.cs",
    description: "Gerenciamento de tempo",
    path: "Scripts/TimerManager.cs",
    category: "systems"
  },
  {
    name: "UIManager.cs",
    description: "Gerenciamento da interface",
    path: "Scripts/UIManager.cs",
    category: "ui"
  },
  {
    name: "EnemyController.cs",
    description: "IA dos inimigos",
    path: "Scripts/EnemyController.cs",
    category: "enemies"
  },
  {
    name: "PowerUpSystem.cs",
    description: "Sistema de power-ups",
    path: "Scripts/PowerUpSystem.cs",
    category: "systems"
  },
  {
    name: "MenuController.cs",
    description: "Controle do menu",
    path: "Scripts/MenuController.cs",
    category: "ui"
  },
  {
    name: "InputManager.cs",
    description: "Gerenciamento de input",
    path: "Scripts/InputManager.cs",
    category: "core"
  }
];

const projectStructure = {
  name: "GameOverSystem",
  folders: [
    {
      name: "Assets",
      items: [
        {
          name: "Scenes",
          items: [
            { name: "MainMenu.unity", type: "scene", icon: "🎬" },
            { name: "GameLevel.unity", type: "scene", icon: "🎬" },
            { name: "GameOver.unity", type: "scene", icon: "🎬" }
          ]
        },
        {
          name: "Scripts",
          items: [
            {
              name: "Core",
              items: [
                { name: "GameState.cs", type: "script", icon: "📜" },
                { name: "GameManager.cs", type: "script", icon: "📜" },
                { name: "InputManager.cs", type: "script", icon: "📜" }
              ]
            },
            {
              name: "UI",
              items: [
                { name: "UIManager.cs", type: "script", icon: "📜" },
                { name: "GameOverUI.cs", type: "script", icon: "📜" },
                { name: "MenuController.cs", type: "script", icon: "📜" }
              ]
            },
            {
              name: "Player",
              items: [
                { name: "PlayerController.cs", type: "script", icon: "📜" }
              ]
            },
            {
              name: "Systems",
              items: [
                { name: "ScoreManager.cs", type: "script", icon: "📜" },
                { name: "AudioManager.cs", type: "script", icon: "📜" },
                { name: "TimerManager.cs", type: "script", icon: "📜" },
                { name: "CardSystem.cs", type: "script", icon: "📜" },
                { name: "PowerUpSystem.cs", type: "script", icon: "📜" }
              ]
            }
          ]
        },
        {
          name: "Prefabs",
          items: [
            { name: "Player.prefab", type: "prefab", icon: "🎭" },
            { name: "Enemy_Basic.prefab", type: "prefab", icon: "🎭" },
            { name: "PowerUp_Health.prefab", type: "prefab", icon: "🎭" },
            { name: "UI_GameOver.prefab", type: "prefab", icon: "🎭" }
          ]
        },
        {
          name: "Sprites",
          items: [
            { name: "game_over_emoji.svg", type: "sprite", icon: "🖼️" },
            { name: "restart_button.svg", type: "sprite", icon: "🖼️" },
            { name: "timer_background.svg", type: "sprite", icon: "🖼️" },
            { name: "card_background.svg", type: "sprite", icon: "🖼️" },
            { name: "player_avatar.svg", type: "sprite", icon: "🖼️" }
          ]
        }
      ]
    }
  ]
};

const hierarchyStructure = [
  { name: "Main Camera", icon: "📹", level: 0 },
  { name: "GameManager", icon: "⚙️", level: 0, scripts: ["GameState", "InputManager"] },
  { name: "Canvas", icon: "🖼️", level: 0, children: [
    { name: "MenuScreen", icon: "🎯", level: 1 },
    { name: "GameScreen", icon: "🎯", level: 1 },
    { name: "PauseScreen", icon: "🎯", level: 1 },
    { name: "GameOverScreen", icon: "🎯", level: 1 }
  ]},
  { name: "Player", icon: "🏃", level: 0, scripts: ["PlayerController"] },
  { name: "AudioSystem", icon: "🔊", level: 0, scripts: ["AudioManager"] },
  { name: "Environment", icon: "🌍", level: 0, children: [
    { name: "Ground", icon: "🟫", level: 1 },
    { name: "EnemySpawner", icon: "🚪", level: 1 }
  ]}
];

function App() {
  const [selectedScript, setSelectedScript] = useState(null);
  const [scriptContent, setScriptContent] = useState("");
  const [loading, setLoading] = useState(false);
  const [activeTab, setActiveTab] = useState('project');
  const [expandedFolders, setExpandedFolders] = useState(new Set(['Assets', 'Scripts']));

  const loadScript = async (script) => {
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

  const toggleFolder = (folderName) => {
    const newExpanded = new Set(expandedFolders);
    if (newExpanded.has(folderName)) {
      newExpanded.delete(folderName);
    } else {
      newExpanded.add(folderName);
    }
    setExpandedFolders(newExpanded);
  };

  const renderTreeItem = (item, level = 0) => {
    const isExpanded = expandedFolders.has(item.name);
    const indent = level * 20;
    
    return (
      <div key={item.name}>
        <div 
          style={{
            paddingLeft: `${indent}px`,
            padding: '4px 8px',
            display: 'flex',
            alignItems: 'center',
            cursor: item.items ? 'pointer' : 'default',
            backgroundColor: 'rgba(255,255,255,0.1)',
            borderRadius: '4px',
            margin: '1px 0',
            fontSize: '12px'
          }}
          onClick={() => {
            if (item.items) {
              toggleFolder(item.name);
            } else if (item.type === 'script') {
              const script = scripts.find(s => s.name === item.name);
              if (script) loadScript(script);
            }
          }}
        >
          {item.items && (
            <span style={{ marginRight: '4px', fontSize: '10px' }}>
              {isExpanded ? '▼' : '▶'}
            </span>
          )}
          <span style={{ marginRight: '4px' }}>{item.icon || '📁'}</span>
          <span style={{ color: item.type === 'script' ? '#4a9eff' : '#ffffff' }}>
            {item.name}
          </span>
        </div>
        
        {item.items && isExpanded && (
          <div style={{ marginLeft: '8px' }}>
            {item.items.map(child => renderTreeItem(child, level + 1))}
          </div>
        )}
      </div>
    );
  };

  const renderHierarchyItem = (item, level = 0) => {
    const indent = level * 20;
    
    return (
      <div key={item.name}>
        <div 
          style={{
            paddingLeft: `${indent}px`,
            padding: '4px 8px',
            display: 'flex',
            alignItems: 'center',
            backgroundColor: 'rgba(255,255,255,0.1)',
            borderRadius: '4px',
            margin: '1px 0',
            fontSize: '12px'
          }}
        >
          <span style={{ marginRight: '4px' }}>{item.icon}</span>
          <span style={{ color: '#ffffff' }}>{item.name}</span>
          {item.scripts && (
            <span style={{ marginLeft: '8px', fontSize: '10px', color: '#888' }}>
              {item.scripts.join(', ')}
            </span>
          )}
        </div>
        
        {item.children && item.children.map(child => renderHierarchyItem(child, level + 1))}
      </div>
    );
  };

  return (
    <div style={{ 
      fontFamily: 'Inter, sans-serif',
      width: '100vw', 
      height: '100vh', 
      background: '#2b2b2b',
      color: '#ffffff',
      overflow: 'hidden',
      display: 'flex'
    }}>
      {/* Unity-style sidebar */}
      <div style={{
        width: '300px',
        background: '#393939',
        borderRight: '1px solid #555',
        display: 'flex',
        flexDirection: 'column'
      }}>
        {/* Tabs */}
        <div style={{
          display: 'flex',
          borderBottom: '1px solid #555'
        }}>
          <button
            onClick={() => setActiveTab('project')}
            style={{
              flex: 1,
              padding: '8px',
              background: activeTab === 'project' ? '#4a4a4a' : '#393939',
              border: 'none',
              color: '#ffffff',
              fontSize: '12px',
              cursor: 'pointer'
            }}
          >
            Project
          </button>
          <button
            onClick={() => setActiveTab('hierarchy')}
            style={{
              flex: 1,
              padding: '8px',
              background: activeTab === 'hierarchy' ? '#4a4a4a' : '#393939',
              border: 'none',
              color: '#ffffff',
              fontSize: '12px',
              cursor: 'pointer'
            }}
          >
            Hierarchy
          </button>
        </div>

        {/* Content */}
        <div style={{
          flex: 1,
          padding: '8px',
          overflowY: 'auto'
        }}>
          {activeTab === 'project' ? (
            <div>
              <div style={{ 
                fontSize: '14px', 
                fontWeight: 'bold', 
                marginBottom: '8px',
                color: '#ffffff'
              }}>
                Unity Project - GameOverSystem
              </div>
              {projectStructure.folders.map(folder => renderTreeItem(folder))}
            </div>
          ) : (
            <div>
              <div style={{ 
                fontSize: '14px', 
                fontWeight: 'bold', 
                marginBottom: '8px',
                color: '#ffffff'
              }}>
                Scene Hierarchy
              </div>
              {hierarchyStructure.map(item => renderHierarchyItem(item))}
            </div>
          )}
        </div>
      </div>

      {/* Main content area */}
      <div style={{
        flex: 1,
        background: '#1e1e1e',
        display: 'flex',
        flexDirection: 'column'
      }}>
        {selectedScript ? (
          <>
            {/* Script header */}
            <div style={{
              padding: '12px 16px',
              borderBottom: '1px solid #555',
              background: '#2d2d2d'
            }}>
              <h2 style={{ 
                margin: '0 0 4px 0', 
                color: '#ffffff',
                fontSize: '16px'
              }}>
                {selectedScript.name}
              </h2>
              <p style={{ 
                margin: 0, 
                color: '#cccccc',
                fontSize: '12px'
              }}>
                {selectedScript.description}
              </p>
            </div>
            
            {/* Script content */}
            <div style={{
              flex: 1,
              padding: '16px',
              overflow: 'auto'
            }}>
              {loading ? (
                <div style={{ 
                  textAlign: 'center', 
                  color: '#888',
                  fontSize: '14px'
                }}>
                  Carregando script...
                </div>
              ) : (
                <pre style={{
                  background: '#1e1e1e',
                  color: '#d4d4d4',
                  padding: '16px',
                  borderRadius: '4px',
                  fontSize: '12px',
                  lineHeight: '1.4',
                  overflow: 'auto',
                  margin: 0,
                  fontFamily: 'Consolas, Monaco, monospace',
                  border: '1px solid #555'
                }}>
                  {scriptContent}
                </pre>
              )}
            </div>
          </>
        ) : (
          /* Welcome screen */
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
              width: '80px',
              height: '80px',
              background: '#555',
              borderRadius: '8px',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              marginBottom: '16px',
              fontSize: '32px'
            }}>
              🎮
            </div>
            <h2 style={{ 
              color: '#ffffff', 
              marginBottom: '8px',
              fontSize: '20px'
            }}>
              Unity Game Over System
            </h2>
            <p style={{ 
              color: '#cccccc', 
              maxWidth: '400px', 
              lineHeight: '1.5',
              fontSize: '14px'
            }}>
              Sistema completo de scripts C# para Unity 2D baseado na interface Game Over.
              Navegue pelos scripts no Project window à esquerda para visualizar o código.
            </p>
            <div style={{
              marginTop: '20px',
              padding: '8px 16px',
              background: '#333',
              borderRadius: '4px',
              fontSize: '12px',
              color: '#888'
            }}>
              📁 Selecione um script no Project para começar
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

export default App;