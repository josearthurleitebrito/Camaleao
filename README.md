O Legado do Camaleão: A Verdade por Trás da Arte

Gênero: Ação e Aventura, Stealth e Puzzle (Foco em Exploração Cultural)

Visão Geral do Projeto:
"O Legado do Camaleão: A Verdade por Trás da Arte" é um jogo de stealth e investigação top-down desenvolvido na Unity Engine. O jogador assume o papel de um(a) historiador(a) astuto(a) infiltrado(a) no Museu de Arte e Cultura da Universidade de Fortaleza.

  - A missão é desvendar uma conspiração, corrigindo a catalogação de obras falsificadas ou mal representadas e restaurando a verdade cultural. O core do jogo reside na combinação de furtividade tática e resolução de puzzles baseados em conhecimento de arte e história.

Mecânicas e Diferenciais
O gameplay se baseia em uma fusão única de gêneros:
  - Stealth de Observação: O jogador deve se misturar à multidão, evitando o medidor de suspeita e os guardas.
  - Controle Dinâmico de Velocidade: Implementamos três modos de movimento (Correr, Normal e Stealth/Lento) para gerenciar o risco e a discrição.
  - Puzzles de Conhecimento Cultural: A progressão depende da resolução de enigmas que exigem a aplicação de conhecimento histórico e artístico (Ex: Detecção de Falsificações, Análise de Obras e Cifras Artísticas).
  - Sistema de Iluminação 2D URP: O ambiente utiliza o Universal Render Pipeline para criar um mood noturno/suspense, com focos de luz dinâmicos (como a Lanterna do policial) que são essenciais para a detecção e jogabilidade.
  - Animação Top-Down com Blend Trees: O movimento dos personagens (Jogador e NPCs) utiliza Blend Trees 2D para garantir transições suaves e direcionais (8 direções), mantendo a direção de sprite correta mesmo ao parar.

Arquitetura Técnica:
Em linha com os objetivos do projeto, o código utiliza estruturas de dados e algoritmos avançados:
  - Inteligência Artificial (A ser implementada): Utilização de Grafos e Pathfinding (A*) para modelar o comportamento de patrulha dos NPCs e criar Árvores de Conhecimento interconectadas no museu.
  - Estrutura de Dados: Uso de Dicionários/Hash Maps para indexar e gerenciar o vasto Codex Cultural do jogo.
  - Física Otimizada: Controle de personagem utilizando Rigidbody2D.MovePosition em FixedUpdate com Extrapolação para garantir movimento suave e livre de input lag.
