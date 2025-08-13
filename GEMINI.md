# Contexto do Projeto: ConsultasPsicologiaMVC

## Resumo Geral do Projeto
- **Aplicação:** Web ASP.NET Core MVC.
- **Objetivo:** Sistema para agendamento e gerenciamento de consultas de psicologia.
- **Arquitetura:** Model-View-Controller (MVC).

## Funcionalidades Implementadas

1.  **Página Inicial (`Home`)**
    - Apresentação profissional da psicóloga Dra. Caroline Guimarães, com foto (`https://s2-g1.glbimg.com/WfXg9j4PrruL9Pw85zLiTUv6STc=/0x0:1000x750/984x0/smart/filters:strip_icc()/i.s3.glbimg.com/v1/AUTH_59edd422c0c84a879bd37670ae4f538a/internal_photos/bs/2019/1/d/ufohYYTeKTSmCZbb5SDw/foto-cerebro-quiz1.jpg`).
    - Layout dinâmico com animações ao rolar a tela (AOS - Animate On Scroll).
    - Animações refinadas nos itens de especialidade (Ansiedade, Depressão, Estresse, Relacionamentos) ativadas ao passar o mouse.
    - Botão "Agende sua Consulta" que aciona um modal.

2.  **Modal de Agendamento de Consulta**
    - Formulário para agendamento de consulta.
    - **Calendário Dinâmico (Flatpickr):**
        - Localizado em português.
        - Abre ao clicar no campo de data e fecha após a seleção.
    - **Seleção de Horário:**
        - Após selecionar o dia, exibe horários disponíveis (ex: 09:00 às 17:00, de seg a sex).
        - A seleção de horário preenche o campo do formulário e esconde a lista de horários.
    - **Lógica de Salvamento (Backend):**
        - Controller: `AgendamentoController` com a ação `Salvar`.
        - Model: `Agendamento.cs` (`Id`, `DataHora`).
        - Envio dos dados via AJAX sem recarregar a página.

3.  **Modal de Cadastro de Usuário**
    - Formulário de registro em um modal flutuante e responsivo.
    - Acionado pelo link "Cadastre-se" na barra de navegação.
    - **Validação de Senha:**
        - Validação de correspondência de senhas em tempo real (ao sair do campo da segunda senha).
        - Botão "Cadastrar" desabilitado se as senhas não coincidirem.
        - Botões de "visualizar senha" nos campos de senha.
    - **Processamento de Dados:**
        - `Nome` e `Sobrenome` são concatenados no frontend antes do envio.
        - `Sobrenome` e `SenhaDigitadaNovamente` são propriedades `[NotMapped]` no modelo `Cadastrar.cs` (não persistidas no banco).
    - **Lógica de Salvamento (Backend):**
        - Controller: `CadastroController` com a ação `Salvar`.
        - **Persistência:** Utiliza comandos SQL diretos para a tabela `usuariopaciente` (sem gerenciamento de esquema pelo Entity Framework Core para esta tabela).
        - **Criptografia de Senha:** Senhas são hasheadas usando PBKDF2 com salt, e o resultado combinado é salvo na coluna `Senha`.
        - **Validação de E-mail:** Verifica se o e-mail já existe no banco de dados antes de salvar.
        - Envio dos dados via AJAX sem recarregar a página.
    - **Experiência do Usuário:** A modal de cadastro fecha instantaneamente antes da exibição da mensagem de sucesso.

4.  **Modal de Login**
    - Formulário de login em um modal flutuante.
    - Acionado pelo botão "Entrar" na barra de navegação.
    - Transição direta do modal de Login para o modal de Cadastro (clicando em "Não tem uma conta? Cadastre-se").

## Backend e Tecnologia
- **Banco de Dados:** PostgreSQL.
  - A string de conexão é `DefaultConnection`.
- **ORM:** Entity Framework Core.
  - `AppDbContext.cs` é o `DbContext`.
  - `DbSet<Agendamento> Agendamentos` (o `DbSet` para `Cadastrar` foi removido para permitir gerenciamento manual da tabela `usuariopaciente`).
  - Migrações aplicadas para sincronizar o modelo com o banco de dados (exceto para `usuariopaciente`).

## Status e Próximos Passos (O que Ainda Falta)

- **Problemas de Layout/Estilização:**
    - **Rodapé:** O rodapé ainda não está fixo corretamente no final da tela, sobrepondo o conteúdo em algumas situações. (Temporariamente removido para depuração).
    - **Alinhamento de Botões de Senha:** O alinhamento dos botões de visualizar senha no formulário de cadastro ainda precisa de ajuste fino.

- **Lógica de Negócio Incompleta:**
    - A função de **salvar** os dados do formulário de **cadastro** no banco de dados agora está funcional, incluindo validação de e-mail e criptografia de senha.

- **Funcionalidades Ausentes:**
    - Sistema de **Autenticação e Login** (lógica de backend).
    - **Área Administrativa:** Para a psicóloga gerenciar seus horários disponíveis.
    - **Validação de Horários:** Impedir o agendamento de horários já ocupados (lógica de backend).
    - **Integração com Google Agenda:** Envio de convites após o agendamento.

## Persona de Desenvolvimento
- **Perfil:** Desenvolvedor Frontend Sênior e Líder Técnico.
- **Habilidades:** Frontend (HTML, CSS, JS, CSHTML, UI/UX), Orientação e Direcionamento Técnico.
- **Foco:** Garantir o frontend 100% funcional e visualmente perfeito, entregando dados formatados para o backend.
- **Colaboração:** Atuar como líder técnico para o desenvolvedor backend júnior, oferecendo direcionamento e suporte no código de backend apenas quando solicitado.
- **Idioma:** Português do Brasil.

---

## Análise e Resolução de Loop (05/08/2025)

**Problema:** O agente entrou em um loop ao tentar modificar o arquivo `Views/Home/Index.cshtml` usando a ferramenta `replace`. A operação falhava repetidamente com a mensagem "Failed to edit, 0 occurrences found for old_string".

**Causa Raiz:** A ferramenta `replace` exige uma correspondência exata para o `old_string`, incluindo todos os caracteres de espaço em branco, quebras de linha e indentação. Ao tentar substituir um bloco grande de HTML/Razor, é extremamente difícil garantir essa correspondência exata, pois pequenas variações no arquivo podem ocorrer (por exemplo, devido a formatação automática do IDE, diferentes sistemas operacionais, etc.). Isso levava a falhas consecutivas e à repetição da tentativa de substituição.

**Estratégia de Resolução:** Em vez de tentar substituir um bloco grande e propenso a erros, a estratégia foi alterada para usar um "ponto de ancoragem" menor e mais estável dentro do HTML. Este ponto de ancoragem é uma string que é menos provável de mudar e que pode ser usada para localizar o local exato onde o novo conteúdo deve ser inserido.

**Ação Tomada:**
1.  Identificado o elemento `<input type="hidden" id="hiddenAppointmentDate">` como um ponto de ancoragem estável dentro do `<form id="appointmentForm">` em `Views/Home/Index.cshtml`.
2.  A nova lógica de inserção será construída para adicionar os elementos de tipo de consulta e valor *antes* deste ponto de ancoragem.

**Próximos Passos:** Continuar a implementação da funcionalidade de tipo de consulta e valor no modal de agendamento, utilizando a nova estratégia de `replace` para modificar `Views/Home/Index.cshtml` de forma mais robusta.
