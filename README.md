# CompiladorAula

Um compilador simples desenvolvido para fins educacionais como parte de uma disciplina de Compiladores. O projeto visa demonstrar os conceitos fundamentais do processo de compilação através de uma implementação prática.

## Sobre o Projeto

Este compilador foi criado para entender e aplicar os conceitos teóricos de compiladores, com foco nas três principais fases de análise:

1. **Análise Léxica**
2. **Análise Sintática**
3. **Análise Semântica**

## A Linguagem

A linguagem implementada é uma linguagem de programação simples e imperativa com as seguintes características:

- Tipagem estática (tipos inteiros e strings)
- Variáveis com escopo de bloco
- Estruturas de controle básicas (if-else, while)
- Expressões aritméticas e de comparação
- Comando de saída simples (print)

### Exemplo de código na linguagem:

```
int x = 10; 
string message = "Hello, World!";
if (x > 5) { 
print(message); 
} else { 
print("x is too small"); 
}
while (x > 0) { 
x = x - 1; print(x); 
}
```


## Componentes do Compilador

### 1. Analisador Léxico (Lexer)

O analisador léxico é responsável por transformar o código-fonte em uma sequência de tokens. Ele identifica:

- Palavras-chave (if, else, while, print, int, string)
- Identificadores (nomes de variáveis)
- Literais (números e strings)
- Operadores (aritméticos, comparação e atribuição)
- Delimitadores (parênteses, chaves, ponto e vírgula)

### 2. Analisador Sintático (Parser)

O analisador sintático utiliza os tokens gerados pelo lexer para construir uma Árvore Sintática Abstrata (AST). Ele implementa:

- Análise descendente recursiva (recursive descent parsing)
- Verificação da estrutura gramatical do programa
- Construção hierárquica das expressões e declarações
- Tratamento de erros sintáticos básicos

### 3. Analisador Semântico

O analisador semântico verifica a correção semântica do programa, incluindo:

- Verificação de tipos em expressões e atribuições
- Validação do uso de variáveis (declaração antes do uso)
- Verificação de inicialização de variáveis
- Análise de compatibilidade de tipos em condicionais e loops

## Objetivos Pedagógicos

Este projeto foi desenvolvido para:

- Entender o pipeline de compilação na prática
- Aplicar conceitos teóricos de linguagens formais e autômatos
- Implementar estruturas de dados para representação da AST
- Compreender o processo de análise estática de programas
- Desenvolver habilidades em processamento de linguagens

## Limitações

Como um projeto educacional, este compilador tem algumas limitações:

- Conjunto reduzido de tipos e operações
- Ausência de funções ou procedimentos
- Sem suporte para módulos ou bibliotecas
- Verificação semântica básica
- Sem otimizações de código

## Implementação

O compilador foi implementado em C# utilizando .NET 8, aproveitando características da linguagem como expressões lambda, LINQ e o sistema de tipos para modelar os componentes do compilador.

## Como Usar

### Compilando o Projeto

1. **Requisitos Prévios**
   - Visual Studio 2022 com suporte para .NET 8
   - SDK do .NET 8 instalado

2. **Abrir o Projeto**
   - Abra o Visual Studio 2022
   - Selecione `Arquivo > Abrir > Projeto/Solução`
   - Navegue até a pasta do projeto e selecione o arquivo de solução `CompiladorAula.sln`

3. **Compilar o Projeto**
   - Pressione `Ctrl+Shift+B` ou selecione `Compilação > Compilar Solução` no menu principal
   - Alternativamente, clique com o botão direito no projeto `CompiladorAula` no Gerenciador de Soluções e selecione `Compilar`

### Executando o Compilador

1. **Executando a partir do Visual Studio**
   - Clique com o botão direito no projeto `CompiladorAula` no Gerenciador de Soluções
   - Selecione `Propriedades`
   - Na guia `Depurar`, expanda `Propriedades de inicialização`
   - Em `Argumentos da linha de comando`, insira o caminho para o arquivo de código-fonte que você deseja compilar, por exemplo: `C:\Exemplos\exemplo.sl`
   - Pressione `F5` ou clique no botão `Iniciar` para executar o compilador

2. **Executando a partir da Linha de Comando**
   - Abra um terminal (Prompt de Comando ou PowerShell)
   - Navegue até a pasta de saída do projeto (geralmente `bin\Debug\net8.0`)
   - Execute o compilador passando o arquivo de código-fonte como argumento:
   
   

## Conclusão

Este projeto demonstra os princípios fundamentais da construção de compiladores, servindo como uma introdução prática aos conceitos teóricos apresentados na disciplina de Compiladores.
