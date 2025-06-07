# Tutorial Completo - Implementação dos Scripts Unity C#

## Introdução
Este tutorial guiará você na implementação completa do sistema Game Over baseado na interface fornecida. Você aprenderá onde colocar cada script, como configurá-los e conectá-los para criar um jogo funcional.

---

## Passo 1: Preparação do Projeto Unity

### 1.1 Criando o Projeto
1. Abra o Unity Hub
2. Clique em "New Project" 
3. Selecione template "2D (Built-in Render Pipeline)"
4. Nome do projeto: "GameOverSystem"
5. Clique em "Create project"

### 1.2 Estrutura de Pastas
Crie a seguinte estrutura no Project window:
```
Assets/
├── Scripts/           # Todos os scripts C#
├── Sprites/           # Imagens e sprites SVG
├── Audio/            # Sons e músicas
├── Prefabs/          # Prefabs dos objetos
├── Scenes/           # Cenas do jogo
└── Materials/        # Materiais visuais
```

---

## Passo 2: Hierarchy e Configuração Inicial

### 2.1 Configuração da Hierarquia Principal
Na janela Hierarchy, crie a seguinte estrutura:

```
Main Camera
├── GameManager (Empty GameObject)
├── Canvas (UI)
│   ├── MenuScreen (Panel)
│   ├── GameScreen (Panel)
│   ├── PauseScreen (Panel)
│   └── GameOverScreen (Panel)
├── Player (Empty GameObject)
├── AudioSystem (Empty GameObject)
└── Environment (Empty GameObject)
```

### 2.2 Configurando o Canvas
1. Selecione o Canvas
2. No Canvas Scaler:
   - UI Scale Mode: "Scale With Screen Size"
   - Reference Resolution: 1920x1080
   - Match: 0.5

---

## Passo 3: Implementação dos Scripts Principais

### 3.1 GameManager - O Coração do Sistema

**Onde arrastar:** GameObject "GameManager" na Hierarchy

**Configuração passo a passo:**

1. **Criar o GameObject:**
   ```
   Hierarchy → Botão direito → Create Empty
   Nome: "GameManager"
   ```

2. **Adicionar o Script:**
   ```
   - Arraste GameManager.cs para o GameObject GameManager
   - Arraste GameState.cs para o GameObject GameManager
   ```

3. **Configurar as Referências no Inspector:**
   ```
   GameManager (Script):
   ├── Tempo Limite: 60
   ├── Vidas Iniciais: 3
   ├── Pontuacao Objetivo: 1000
   ├── UI Manager: [Arraste o Canvas aqui]
   ├── Score Manager: [Arraste o ScoreSystem aqui]
   ├── Timer Manager: [Arraste o TimerSystem aqui]
   ├── Audio Manager: [Arraste o AudioSystem aqui]
   └── Player Controller: [Arraste o Player aqui]
   ```

### 3.2 UIManager - Interface do Usuário

**Onde arrastar:** Canvas na Hierarchy

**Configuração:**

1. **Adicionar ao Canvas:**
   ```
   Selecione Canvas → Add Component → UIManager
   ```

2. **Criar os Painéis:**
   ```
   Canvas → Botão direito → UI → Panel
   Nomes: MenuScreen, GameScreen, PauseScreen, GameOverScreen
   ```

3. **Configurar Referências:**
   ```
   UIManager (Script):
   ├── Tela Menu: [Arraste MenuScreen]
   ├── Tela Jogo: [Arraste GameScreen]
   ├── Tela Pausa: [Arraste PauseScreen]
   ├── Tela Game Over: [Arraste GameOverScreen]
   ├── Texto Pontuacao: [Arraste Text do GameScreen]
   ├── Texto Timer: [Arraste Text do GameScreen]
   ├── Texto Vidas: [Arraste Text do GameScreen]
   └── Barra Vida: [Arraste Slider do GameScreen]
   ```

### 3.3 GameOverUI - Tela Específica de Game Over

**Onde arrastar:** GameObject "GameOverScreen"

**Configuração detalhada:**

1. **Preparar o GameOverScreen:**
   ```
   GameOverScreen → Add Component → GameOverUI
   ```

2. **Criar elementos filhos do GameOverScreen:**
   ```
   GameOverScreen/
   ├── Background (Image) - Cor vermelha semi-transparente
   ├── EmojiChorando (Image) - Use o SVG criado
   ├── TextoGameOver (Text) - "GAME OVER"
   ├── BotaoRestart (Button) - Texto "RESTART"
   ├── TimerArea (Panel)
   │   ├── FundoTimer (Image) - Use o SVG do timer
   │   └── TextoTimer (Text) - "05"
   ├── AreaCartas (Panel)
   │   ├── BotaoNextPiece (Button)
   │   ├── BotaoCards (Button)
   │   └── CartasInventario (GridLayoutGroup)
   ├── AreaJogadores (Panel)
   │   ├── Avatar1 (Image) - Use o SVG do avatar
   │   ├── Avatar2 (Image)
   │   ├── TextoNivel1 (Text) - "2/3"
   │   └── TextoNivel2 (Text) - "1/3"
   └── AreaPropaganda (Panel)
       └── TextoPropaganda (Text) - "PROPAGANDA"
   ```

3. **Configurar as referências no GameOverUI:**
   ```
   GameOverUI (Script):
   ├── Fundo Game Over: [Background]
   ├── Emoji Chorando: [EmojiChorando]
   ├── Texto Game Over: [TextoGameOver]
   ├── Botao Restart: [BotaoRestart]
   ├── Fundo Timer: [FundoTimer]
   ├── Texto Timer: [TextoTimer]
   ├── Area Cartas: [AreaCartas]
   ├── Botao Next Piece: [BotaoNextPiece]
   ├── Botao Cards: [BotaoCards]
   ├── Cartas Inventario: [Array de Images das cartas]
   ├── Avatars Jogadores: [Array de Images dos avatares]
   ├── Textos Nivel Jogadores: [Array de Texts dos níveis]
   ├── Area Propaganda: [AreaPropaganda]
   └── Texto Propaganda: [TextoPropaganda]
   ```

### 3.4 Player - Personagem Jogável

**Onde arrastar:** GameObject "Player"

**Configuração:**

1. **Preparar o Player:**
   ```
   Player → Add Component → PlayerController
   Player → Add Component → Rigidbody2D
   Player → Add Component → BoxCollider2D
   Player → Add Component → SpriteRenderer
   ```

2. **Configurar Rigidbody2D:**
   ```
   ├── Mass: 1
   ├── Linear Drag: 5
   ├── Angular Drag: 5
   ├── Gravity Scale: 3
   └── Freeze Rotation Z: ✓ (marcado)
   ```

3. **Configurar PlayerController:**
   ```
   PlayerController (Script):
   ├── Velocidade Movimento: 5
   ├── Forca Pulo: 10
   ├── Layer Chao: [Selecione layer do chão]
   ├── Dano Ataque: 25
   ├── Alcance Ataque: 1.5
   ├── Layer Inimigos: [Selecione layer dos inimigos]
   ├── Vida Maxima: 100
   ├── Tempo Invencibilidade: 1
   ├── Ponto Ataque: [Crie Empty child e arraste]
   ├── Colidir Chao: [Arraste o BoxCollider2D]
   ├── Animator Player: [Arraste o Animator se tiver]
   └── Sprite Renderer: [Arraste o SpriteRenderer]
   ```

### 3.5 Audio System - Gerenciamento de Som

**Onde arrastar:** GameObject "AudioSystem"

**Configuração:**

1. **Preparar AudioSystem:**
   ```
   AudioSystem → Add Component → AudioManager
   AudioSystem → Add 3x AudioSource components
   ```

2. **Configurar AudioSources:**
   ```
   AudioSource 1 - Música de Fundo:
   ├── Loop: ✓
   ├── Play On Awake: ✗
   └── Volume: 0.7

   AudioSource 2 - Efeitos Sonoros:
   ├── Loop: ✗
   ├── Play On Awake: ✗
   └── Volume: 0.8

   AudioSource 3 - Vozes:
   ├── Loop: ✗
   ├── Play On Awake: ✗
   └── Volume: 0.9
   ```

3. **Configurar AudioManager:**
   ```
   AudioManager (Script):
   ├── Musica Fundo: [AudioSource 1]
   ├── Efeitos Sonoros: [AudioSource 2]
   ├── Vozes Personagens: [AudioSource 3]
   ├── Musica Menu: [Arraste clip de áudio]
   ├── Musica Jogo: [Arraste clip de áudio]
   ├── Musica Game Over: [Arraste clip de áudio]
   ├── Som Pulo: [Arraste clip de áudio]
   ├── Som Ataque: [Arraste clip de áudio]
   ├── Som Hit: [Arraste clip de áudio]
   ├── Som Botao: [Arraste clip de áudio]
   └── [Configure todos os outros sons...]
   ```

### 3.6 Score System - Sistema de Pontuação

**Onde arrastar:** Novo GameObject "ScoreSystem"

**Configuração:**

1. **Criar ScoreSystem:**
   ```
   Hierarchy → Create Empty → Nome: "ScoreSystem"
   ScoreSystem → Add Component → ScoreManager
   ```

2. **Configurar ScoreManager:**
   ```
   ScoreManager (Script):
   ├── Pontuacao Inicial: 0
   ├── Multiplicador Base: 1
   ├── Multiplicador Maximo: 10
   ├── Tempo Para Perder Multiplicador: 5
   ├── Pontos Inimigo Basico: 100
   ├── Pontos Inimigo Especial: 250
   ├── Pontos Power Up: 50
   └── Pontos Combo: 25
   ```

### 3.7 Timer System - Sistema de Tempo

**Onde arrastar:** Novo GameObject "TimerSystem"

**Configuração:**

1. **Criar TimerSystem:**
   ```
   Hierarchy → Create Empty → Nome: "TimerSystem"
   TimerSystem → Add Component → TimerManager
   ```

2. **Configurar TimerManager:**
   ```
   TimerManager (Script):
   ├── Tempo Limite Partida: 180 (3 minutos)
   ├── Contagem Regressiva: ✓
   ├── Pausa Automaticamente: ✗
   ├── Tempo Aviso Urgente: 30
   └── Tempo Aviso Critico: 10
   ```

### 3.8 Card System - Sistema de Cartas

**Onde arrastar:** Novo GameObject "CardSystem"

**Configuração:**

1. **Criar CardSystem:**
   ```
   Hierarchy → Create Empty → Nome: "CardSystem"
   CardSystem → Add Component → CardSystem
   ```

2. **Configurar CardSystem:**
   ```
   CardSystem (Script):
   ├── Capacidade Maxima Inventario: 6
   ├── Container Cartas: [Arraste CartasInventario do GameOverScreen]
   ├── Prefab Carta UI: [Crie um prefab com Image + Button]
   ├── Botao Cards: [Arraste BotaoCards do GameOverScreen]
   └── Botao Next Piece: [Arraste BotaoNextPiece]
   ```

---

## Passo 4: Configuração de Inimigos e Power-ups

### 4.1 Inimigo Básico

**Criar Prefab do Inimigo:**

1. **Criar GameObject:**
   ```
   Hierarchy → Create Empty → Nome: "Enemy_Basic"
   ```

2. **Adicionar Componentes:**
   ```
   Enemy_Basic → Add Component → EnemyController
   Enemy_Basic → Add Component → Rigidbody2D
   Enemy_Basic → Add Component → BoxCollider2D
   Enemy_Basic → Add Component → SpriteRenderer
   Enemy_Basic → Tag: "Enemy" (crie se não existir)
   Enemy_Basic → Layer: "Enemies" (crie se não existir)
   ```

3. **Configurar EnemyController:**
   ```
   EnemyController (Script):
   ├── Vida Maxima: 50
   ├── Velocidade Movimento: 2
   ├── Dano Ataque: 10
   ├── Alcance Ataque: 1.5
   ├── Tempo Entre Ataques: 2
   ├── Distancia Deteccao: 5
   ├── Layer Jogador: "Default" (layer do player)
   ├── Ponto Ataque: [Crie Empty child]
   ├── Sprite Renderer: [Arraste SpriteRenderer]
   └── Animator Inimigo: [Se tiver animações]
   ```

4. **Transformar em Prefab:**
   ```
   Arraste Enemy_Basic da Hierarchy para pasta Prefabs
   Delete o Enemy_Basic da Hierarchy
   ```

### 4.2 Power-up de Vida

**Criar Prefab de Power-up:**

1. **Criar GameObject:**
   ```
   Hierarchy → Create Empty → Nome: "PowerUp_Vida"
   ```

2. **Adicionar Componentes:**
   ```
   PowerUp_Vida → Add Component → PowerUpSystem
   PowerUp_Vida → Add Component → BoxCollider2D
   PowerUp_Vida → Add Component → SpriteRenderer
   PowerUp_Vida → Tag: "PowerUp"
   ```

3. **Configurar Collider como Trigger:**
   ```
   BoxCollider2D → Is Trigger: ✓
   ```

4. **Configurar PowerUpSystem:**
   ```
   PowerUpSystem (Script):
   ├── Tipo: Vida
   ├── Valor Efeito: 50
   ├── Duracao Efeito: 0 (instantâneo)
   ├── Efeito Temporario: ✗
   ├── Sprite Renderer: [Arraste SpriteRenderer]
   └── Som Coleta: [Arraste clip de áudio]
   ```

---

## Passo 5: Configuração da UI Detalhada

### 5.1 Tela de Menu (MenuScreen)

**Elementos necessários:**

1. **Criar estrutura:**
   ```
   MenuScreen/
   ├── Background (Image) - Imagem de fundo
   ├── Title (Text) - Título do jogo
   ├── ButtonsPanel (Panel)
   │   ├── BotaoJogar (Button) - "JOGAR"
   │   ├── BotaoOpcoes (Button) - "OPÇÕES"
   │   ├── BotaoCreditos (Button) - "CRÉDITOS"
   │   └── BotaoSair (Button) - "SAIR"
   ├── RecordeText (Text) - "Recorde: 0"
   └── VersaoText (Text) - "v1.0"
   ```

2. **Adicionar MenuController:**
   ```
   MenuScreen → Add Component → MenuController
   ```

3. **Configurar MenuController:**
   ```
   MenuController (Script):
   ├── Painel Menu Principal: [MenuScreen]
   ├── Painel Opcoes: [Crie OptionsScreen]
   ├── Painel Creditos: [Crie CreditsScreen]
   ├── Botao Jogar: [BotaoJogar]
   ├── Botao Opcoes: [BotaoOpcoes]
   ├── Botao Creditos: [BotaoCreditos]
   ├── Botao Sair: [BotaoSair]
   ├── Texto Recorde: [RecordeText]
   └── Texto Versao: [VersaoText]
   ```

### 5.2 Tela de Jogo (GameScreen)

**HUD do jogo:**

1. **Criar elementos:**
   ```
   GameScreen/
   ├── TopBar (Panel)
   │   ├── PontuacaoText (Text) - "Pontos: 0"
   │   ├── TimerText (Text) - "03:00"
   │   └── VidasText (Text) - "Vidas: 3"
   ├── HealthBar (Panel)
   │   ├── BarraVidaBackground (Image)
   │   └── BarraVidaFill (Slider)
   └── PauseButton (Button) - "⏸"
   ```

### 5.3 Configuração de Input

**Adicionar InputManager:**

1. **Criar InputSystem:**
   ```
   Hierarchy → Create Empty → Nome: "InputSystem"
   InputSystem → Add Component → InputManager
   ```

2. **Configurar teclas no Inspector:**
   ```
   InputManager (Script):
   ├── Habilitar Gamepad: ✓
   ├── Habilitar Teclado: ✓
   ├── Habilitar Mouse: ✓
   ├── Mapa Teclas:
   │   ├── Tecla Esquerda: A
   │   ├── Tecla Direita: D
   │   ├── Tecla Cima: W
   │   ├── Tecla Baixo: S
   │   ├── Tecla Pulo: Space
   │   ├── Tecla Ataque: J
   │   ├── Tecla Especial: K
   │   ├── Tecla Pausa: Escape
   │   └── Tecla Inventario: I
   ```

---

## Passo 6: Layers e Physics

### 6.1 Configurar Layers

**Tags & Layers (Window → Layers):**

1. **Layers necessários:**
   ```
   ├── Default (0) - Player
   ├── Ground (8) - Chão e plataformas
   ├── Enemies (9) - Inimigos
   ├── PowerUps (10) - Power-ups
   ├── UI (5) - Interface (já existe)
   └── Projectiles (11) - Projéteis
   ```

2. **Tags necessários:**
   ```
   ├── Player
   ├── Enemy
   ├── PowerUp
   ├── Ground
   └── GameController
   ```

### 6.2 Physics 2D Settings

**Edit → Project Settings → Physics 2D:**

1. **Configurar Layer Collision Matrix:**
   ```
   - Player não colide com PowerUps (só trigger)
   - Enemies não colidem entre si
   - Projectiles colidem com Ground e Enemies
   - PowerUps não colidem com nada (só trigger)
   ```

---

## Passo 7: Criação do Cenário

### 7.1 Chão e Plataformas

1. **Criar chão:**
   ```
   Environment → Create Empty → Nome: "Ground"
   Ground → Add Component → BoxCollider2D
   Ground → Add Component → SpriteRenderer
   Ground → Tag: "Ground"
   Ground → Layer: "Ground"
   ```

2. **Configurar tamanho:**
   ```
   Transform → Scale: (10, 1, 1)
   Transform → Position: (0, -4, 0)
   ```

### 7.2 Spawners de Inimigos

1. **Criar EnemySpawner:**
   ```
   Environment → Create Empty → Nome: "EnemySpawner"
   ```

2. **Script de Spawn (opcional):**
   ```csharp
   public class EnemySpawner : MonoBehaviour
   {
       [SerializeField] private GameObject enemyPrefab;
       [SerializeField] private float spawnRate = 2f;
       
       private void Start()
       {
           InvokeRepeating(nameof(SpawnEnemy), 1f, spawnRate);
       }
       
       private void SpawnEnemy()
       {
           if (GameState.Instancia.EstaNoEstado(EstadoJogo.Jogando))
           {
               Instantiate(enemyPrefab, transform.position, Quaternion.identity);
           }
       }
   }
   ```

---

## Passo 8: Configuração Final e Testes

### 8.1 Ordem de Execução dos Scripts

**Edit → Project Settings → Script Execution Order:**

1. **Ordem recomendada:**
   ```
   1. GameState (-100)
   2. InputManager (-50)
   3. AudioManager (-25)
   4. GameManager (0) - Default
   5. UIManager (50)
   6. PlayerController (100)
   7. EnemyController (100)
   ```

### 8.2 Conectar Todas as Referências

**Checklist final:**

- [ ] GameManager tem todas as referências preenchidas
- [ ] UIManager conectado aos painéis corretos
- [ ] GameOverUI tem todos os elementos configurados
- [ ] AudioManager tem todos os clips atribuídos
- [ ] Player configurado com layers corretos
- [ ] Inimigos como prefabs funcionais
- [ ] Power-ups como prefabs funcionais
- [ ] Botões da UI conectados aos métodos corretos

### 8.3 Teste do Sistema

1. **Pressione Play**
2. **Teste sequencial:**
   - Menu aparece corretamente ✓
   - Botão "Jogar" inicia o jogo ✓
   - Player se move com WASD ✓
   - Player pula com Space ✓
   - Player ataca com J ✓
   - Inimigos perseguem o player ✓
   - Coleta de power-ups funciona ✓
   - HUD atualiza pontuação ✓
   - Timer conta regressivamente ✓
   - Game Over aparece ao morrer/tempo acabar ✓
   - Botão Restart funciona ✓

---

## Passo 9: Customização e Polimento

### 9.1 Sprites e Visuais

**Implementar os SVGs criados:**

1. **Importar SVGs:**
   ```
   - Copie os arquivos .svg para Assets/Sprites/
   - Configure Texture Type: "Sprite (2D and UI)"
   - Configure Pixels Per Unit: 100
   ```

2. **Aplicar aos elementos:**
   ```
   - game_over_emoji.svg → GameOverScreen/EmojiChorando
   - restart_button.svg → GameOverScreen/BotaoRestart
   - timer_background.svg → GameScreen/TimerArea
   - card_background.svg → Prefab de carta
   - player_avatar.svg → GameOverScreen/Avatars
   ```

### 9.2 Animações (Opcional)

**Para animações suaves:**

1. **Instale DOTween (Asset Store)**
2. **Ou use Coroutines nativas do Unity**
3. **Animações recomendadas:**
   - Entrada da tela Game Over
   - Pulsação do timer quando crítico
   - Rotação dos power-ups
   - Bounce dos botões ao clicar

### 9.3 Sons e Música

**Adicionar áudios:**

1. **Formatos recomendados:**
   - Música: .ogg (compressão menor)
   - Efeitos: .wav (sem delay)

2. **Configurações de Import:**
   ```
   Música:
   ├── Load Type: Streaming
   ├── Compression Format: Vorbis
   └── Quality: 70%

   Efeitos:
   ├── Load Type: Decompress On Load
   ├── Compression Format: PCM
   └── Quality: 100%
   ```

---

## Resolução de Problemas Comuns

### ❌ Erro: "NullReferenceException"
**Solução:** Verifique se todas as referências estão conectadas no Inspector

### ❌ Player não se move
**Solução:** 
- Verifique se Rigidbody2D está presente
- Confirme se InputManager está ativo
- Teste se as teclas estão configuradas corretamente

### ❌ Game Over não aparece
**Solução:**
- Verifique se GameState está conectado ao GameManager
- Confirme se os eventos estão registrados
- Teste se UIManager tem a referência da tela

### ❌ Sons não tocam
**Solução:**
- Verifique se AudioManager tem os clips atribuídos
- Confirme se os AudioSources estão configurados
- Teste o volume master do Unity

### ❌ Inimigos não perseguem
**Solução:**
- Confirme se Player tem tag "Player"
- Verifique se layers estão configurados
- Teste se EnemyController está ativo

---

## Próximos Passos

Após implementar todo o sistema, você pode:

1. **Expandir o jogo:**
   - Adicionar mais tipos de inimigos
   - Criar diferentes power-ups
   - Implementar sistema de fases

2. **Melhorar visuais:**
   - Adicionar partículas
   - Criar animações mais elaboradas
   - Implementar shader effects

3. **Adicionar recursos:**
   - Sistema de achievements
   - Leaderboard online
   - Múltiplos jogadores

4. **Polir o jogo:**
   - Balance de dificuldade
   - Feedback tátil
   - Testes extensivos

---

**Parabéns! Você implementou com sucesso um sistema completo de Game Over para Unity 2D! 🎮**