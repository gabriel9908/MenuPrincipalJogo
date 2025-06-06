# Scripts Unity 2D - Sistema Game Over

## Descrição
Este conjunto de scripts C# foi desenvolvido para Unity 2D baseado na interface Game Over fornecida. Os scripts implementam um sistema completo de jogo com gerenciamento de estados, pontuação, tempo, cartas e áudio.

## Estrutura dos Scripts

### 1. GameState.cs
- **Função**: Gerencia os estados do jogo (Menu, Jogando, Pausado, GameOver, Carregando)
- **Características**:
  - Padrão Singleton
  - Sistema de eventos para notificação de mudanças
  - Controle automático do timeScale
  - Métodos de conveniência para mudanças de estado

### 2. GameManager.cs
- **Função**: Controlador principal que coordena todos os sistemas
- **Características**:
  - Gerencia vidas do jogador
  - Controla início/fim de partidas
  - Coordena interação entre sistemas
  - Sistema de pause/resume
  - Detecção de condições de vitória/derrota

### 3. UIManager.cs
- **Função**: Gerencia toda a interface do usuário
- **Características**:
  - Controle de visibilidade das telas
  - Atualização do HUD (pontuação, timer, vidas)
  - Configuração de botões
  - Sistema de animações de UI

### 4. GameOverUI.cs
- **Função**: Interface específica da tela de Game Over
- **Características**:
  - Animações de entrada suaves com DOTween
  - Timer de 5 segundos (como mostrado na imagem)
  - Sistema de cartas e avatares dos jogadores
  - Botões funcionais (Restart, Next Piece, Cards)
  - Área de propaganda personalizável

### 5. PlayerController.cs
- **Função**: Controla o jogador e suas ações
- **Características**:
  - Movimento 2D com física
  - Sistema de combate com detecção de inimigos
  - Gerenciamento de vida e invencibilidade
  - Animações baseadas em estados
  - Sistema de input responsivo

### 6. ScoreManager.cs
- **Função**: Sistema completo de pontuação
- **Características**:
  - Multiplicadores dinâmicos
  - Sistema de combo
  - Salvamento de recordes
  - Diferentes tipos de pontuação
  - Sistema de ranking

### 7. TimerManager.cs
- **Função**: Gerenciamento de tempo e cronômetros
- **Características**:
  - Contagem regressiva/progressiva
  - Avisos de tempo (urgente/crítico)
  - Pausar/retomar funcionalidade
  - Adicionar/remover tempo (power-ups/penalidades)
  - Formatação de tempo para display

### 8. CardSystem.cs
- **Função**: Sistema de cartas como mostrado na interface
- **Características**:
  - Inventário de cartas com capacidade limitada
  - Diferentes tipos e raridades de cartas
  - Sistema de efeitos (cura, ataque, buffs)
  - Interface visual das cartas
  - Seleção aleatória ponderada por raridade

### 9. AudioManager.cs
- **Função**: Gerenciamento completo de áudio
- **Características**:
  - Música de fundo com fade
  - Efeitos sonoros categorizados
  - Controles de volume individuais
  - Sistema de mute/unmute
  - Salvamento de configurações

## Assets SVG Incluídos

### Sprites Criados:
1. **game_over_emoji.svg** - Emoji chorando para a tela de Game Over
2. **restart_button.svg** - Botão RESTART com gradiente laranja
3. **timer_background.svg** - Fundo do timer com ícone de relógio
4. **card_background.svg** - Template para cartas do inventário
5. **player_avatar.svg** - Avatar de jogador para a interface

## Como Usar

### Configuração Inicial:
1. Adicione todos os scripts aos GameObjects apropriados
2. Configure as referências no Inspector
3. Atribua os sprites SVG aos componentes UI correspondentes
4. Configure os AudioClips no AudioManager

### Estrutura Recomendada na Hierarquia:
```
GameManager (GameManager.cs, GameState.cs)
├── UI Canvas
│   ├── MenuScreen (UIManager.cs)
│   ├── GameScreen (HUD elements)
│   ├── PauseScreen
│   └── GameOverScreen (GameOverUI.cs)
├── Player (PlayerController.cs)
├── ScoreSystem (ScoreManager.cs)
├── TimerSystem (TimerManager.cs)
├── CardSystem (CardSystem.cs)
└── AudioSystem (AudioManager.cs)
```

### Eventos Principais:
- `GameState.OnEstadoAlterado` - Mudança de estado do jogo
- `GameManager.OnJogoTerminado` - Fim de partida
- `ScoreManager.OnPontuacaoAlterada` - Mudança na pontuação
- `TimerManager.OnTempoEsgotado` - Tempo esgotado
- `PlayerController.OnMorreu` - Morte do jogador

## Funcionalidades da Tela Game Over

### Elementos Implementados:
- ✅ Emoji chorando animado
- ✅ Texto "GAME OVER" com animação
- ✅ Botão RESTART funcional
- ✅ Timer de 5 segundos
- ✅ Sistema de cartas (Next Piece, Cards)
- ✅ Avatares de jogadores com níveis
- ✅ Área de propaganda personalizável
- ✅ Animações suaves de entrada

### Customizações Possíveis:
- Cores e gradientes dos elementos
- Duração das animações
- Mensagens de propaganda
- Tipos de cartas disponíveis
- Efeitos sonoros específicos

## Dependências
- **DOTween** (para animações - pode ser substituído por Coroutines)
- **TextMeshPro** (para textos da UI)
- **Unity Input System** (opcional - atualmente usa Input clássico)

## Notas de Implementação
- Todos os scripts seguem boas práticas do Unity
- Sistema de eventos desacoplado
- Padrão Singleton onde apropriado
- Comentários em português brasileiro
- Logging para debug
- Tratamento de referências nulas
- Salvamento automático de configurações

## Compatibilidade
- Unity 2020.3 LTS ou superior
- Funciona em 2D e 2.5D
- Compatível com builds mobile e desktop