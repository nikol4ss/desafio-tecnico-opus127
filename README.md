# Dengue BH

Aplicação para importar, persistir, consultar e visualizar dados de dengue de Belo Horizonte. A API consulta os últimos seis meses disponíveis na fonte AlertaDengue/InfoDengue, atualiza o SQL Server ao iniciar e fornece consultas semanais para o dashboard React.

## O que foi entregue

- importação de seis meses de dados de Belo Horizonte (`geocode=3106200`);
- cálculo das semanas epidemiológicas correspondentes ao período;
- persistência idempotente no SQL Server com Entity Framework Core;
- endpoint para consultar uma semana por `ew` e `ey`;
- dashboard com as três semanas mais recentes, cards, gráfico e tabela;
- testes unitários do backend e do contrato HTTP do frontend;
- OpenAPI, tratamento padronizado de erros e health check.

## Tecnologias

### Backend

- .NET 10 e C#;
- ASP.NET Core Web API;
- Entity Framework Core 10;
- SQL Server;
- xUnit.

### Frontend

- React 19 e JavaScript;
- Vite;
- Tailwind CSS;
- Recharts;
- Vitest e ESLint.

## Início rápido com Docker

Este é o caminho recomendado em máquinas x86-64 porque mantém o SQL Server isolado e reproduzível. A aplicação continua sendo executada diretamente pelo .NET e pelo Vite.

> O backend e o frontend são multiplataforma. A imagem oficial local do SQL Server, porém, é suportada apenas em x86-64. Em máquinas ARM, siga a seção [SQL Server remoto](#sql-server-remoto).

### 1. Instale os pré-requisitos

- [Git](https://git-scm.com/downloads);
- [.NET SDK 10](https://learn.microsoft.com/dotnet/core/install/);
- [Node.js 22.13 ou superior](https://nodejs.org/en/download);
- [Docker Desktop ou Docker Engine com Compose](https://docs.docker.com/desktop/).

Instale o pnpm depois de instalar o Node.js:

```shell
npm install --global pnpm@12
```

Confirme as ferramentas:

```shell
git --version
dotnet --version
node --version
pnpm --version
docker compose version
```

### 2. Clone o repositório

```shell
git clone https://github.com/nikol4ss/desafio-tecnico-opus127.git
cd desafio-tecnico-opus127
```

Todos os comandos seguintes partem da raiz do repositório.

### 3. Inicie o SQL Server

O exemplo usa uma senha somente para desenvolvimento local. A porta é publicada apenas em `127.0.0.1` e os dados ficam no volume `sqlserver-data`.

No PowerShell, Windows:

```powershell
$env:MSSQL_SA_PASSWORD="Opus127_Local#2026"
$env:ConnectionStrings__DengueDatabase="Server=localhost,1433;Database=Opus127Dengue;User Id=sa;Password=$env:MSSQL_SA_PASSWORD;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
docker compose up -d --wait
```

No Bash, macOS ou Linux:

```bash
export MSSQL_SA_PASSWORD='Opus127_Local#2026'
export ConnectionStrings__DengueDatabase="Server=localhost,1433;Database=Opus127Dengue;User Id=sa;Password=${MSSQL_SA_PASSWORD};Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
docker compose up -d --wait
```

O `--wait` retorna quando o health check do SQL Server estiver saudável. Caso uma versão antiga do Docker Compose não reconheça essa opção, execute:

```shell
docker compose up -d
docker compose logs -f sqlserver
```

Prossiga quando o log informar que o SQL Server está pronto para conexões. Use `Ctrl+C` para sair da visualização do log; o container continuará executando.

As variáveis de ambiente valem somente para o terminal atual. Execute o backend no mesmo terminal em que definiu `ConnectionStrings__DengueDatabase`.

### 4. Execute o backend

```shell
dotnet restore backend/Opus127.Dengue.slnx
dotnet run --project backend/Opus127.Dengue.Api --launch-profile Opus127.Dengue.Api
```

Na inicialização, a API:

1. cria o banco `Opus127Dengue` quando necessário;
2. aplica as migrations pendentes;
3. calcula as semanas epidemiológicas dos últimos seis meses;
4. consulta a API AlertaDengue/InfoDengue;
5. insere semanas novas e atualiza registros existentes.

Aguarde a mensagem `Application started` antes de iniciar o frontend.

Endereços locais:

- API HTTPS: `https://localhost:54956`;
- API HTTP: `http://localhost:54957`, com redirecionamento para HTTPS;
- OpenAPI: `https://localhost:54956/openapi/v1.json`;
- health check: `https://localhost:54956/health`.

Se o certificado HTTPS local ainda não for confiável, execute uma vez:

```shell
dotnet dev-certs https --trust
```

### 5. Execute o frontend

Abra outro terminal na raiz do repositório:

```shell
cd frontend
pnpm install --frozen-lockfile
pnpm dev
```

Acesse `http://localhost:5173`. Durante o desenvolvimento, o Vite encaminha as chamadas de `/api` para a API HTTPS local.

### 6. Verifique o funcionamento

Com backend e frontend executando:

```text
http://localhost:5173
https://localhost:54956/health
https://localhost:54956/api/dengue/weeks/latest?count=3
https://localhost:54956/api/dengue?ew=35&ey=2026
```

A última URL é apenas um exemplo. Utilize uma semana retornada por `/api/dengue/weeks/latest` para garantir que ela exista no banco.

### 7. Encerre o ambiente

Use `Ctrl+C` nos terminais do backend e do frontend. Depois, na raiz do repositório:

```shell
docker compose stop
```

O comando para o SQL Server e preserva o banco no volume. Para utilizá-lo novamente:

PowerShell:

```powershell
$env:MSSQL_SA_PASSWORD="Opus127_Local#2026"
docker compose start
```

Bash:

```bash
export MSSQL_SA_PASSWORD='Opus127_Local#2026'
docker compose start
```

## Outras formas de configurar o SQL Server

### SQL Server nativo no Windows

Se o SQL Server estiver instalado localmente com autenticação integrada do Windows, inicie o serviço da instância e execute a API sem definir `ConnectionStrings__DengueDatabase`. A configuração padrão é:

```text
Server=localhost;Database=Opus127Dengue;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True
```

Para uma instância nomeada, SQL Server Express ou outro servidor, substitua a conexão no mesmo terminal que executará a API:

```powershell
$env:ConnectionStrings__DengueDatabase="Server=localhost\SQLEXPRESS;Database=Opus127Dengue;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

O usuário da conexão precisa ter permissão para criar o banco na primeira execução e para criar ou alterar seu esquema.

### SQL Server remoto

O backend aceita qualquer SQL Server acessível pela máquina. Defina `ConnectionStrings__DengueDatabase` com servidor, porta, usuário, senha e configuração de criptografia fornecidos pelo responsável pelo banco.

Formato de referência:

```text
Server=SERVIDOR,1433;Database=Opus127Dengue;User Id=USUARIO;Password=SENHA;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True
```

Essa opção permite executar o projeto em máquinas ARM ou em sistemas que não tenham SQL Server local.

> A Microsoft oferece suporte às imagens Linux do SQL Server apenas em hosts Intel/AMD x86-64. Em Apple Silicon, Windows ARM ou Linux ARM, use um SQL Server remoto ou uma máquina virtual x64. Emulação pode funcionar, mas não é uma configuração oficialmente suportada.

## Por que Layered Architecture

O projeto possui duas fronteiras externas diferentes: o SQL Server e a API AlertaDengue/InfoDengue. Colocar acesso HTTP, regras, persistência e respostas da API dentro do controller funcionaria no início, mas tornaria testes, manutenção e substituições mais difíceis.

A Layered Architecture separa essas responsabilidades e direciona as dependências para o núcleo:

```text
Api ──────────────► Application ──────────────► Domain
 │                       ▲                        ▲
 └──────► Infrastructure ┴────────────────────────┘
```

- `Domain` não depende de nenhuma outra camada.
- `Application` depende apenas do `Domain`.
- `Infrastructure` implementa contratos da `Application` e utiliza o `Domain`.
- `Api` monta as dependências e expõe o sistema por HTTP.

### Responsabilidade de cada camada

| Camada | Responsabilidade | Principais elementos | Não deve conhecer |
| --- | --- | --- | --- |
| `Domain` | Estado e regras centrais | `DengueRecord`, `EpidemiologicalWeek` | HTTP, Entity Framework, SQL Server e React |
| `Application` | Casos de uso e contratos | serviços, interfaces e modelos de resultado | controllers, SQL e detalhes da API externa |
| `Infrastructure` | Comunicação com recursos externos | EF Core, repository, migrations e cliente InfoDengue | apresentação do frontend |
| `Api` | Entrada HTTP e composição | controllers, contratos JSON, DI e hosted service | consultas SQL e formato bruto da fonte externa |

### O que essa escolha melhora neste projeto

1. **Controller pequeno:** recebe e valida `ew` e `ey`, chama o caso de uso e transforma o resultado em HTTP.
2. **Testabilidade:** os serviços são testados com repositório e fonte em memória, sem abrir servidor, SQL Server ou internet.
3. **Isolamento da fonte externa:** mudanças no JSON do InfoDengue ficam concentradas em `AlertaDengueResponse` e `AlertaDengueClient`.
4. **Isolamento da persistência:** EF Core e SQL Server ficam na Infrastructure; a Application conhece apenas `IDengueRepository`.
5. **Contratos independentes:** a entidade do banco, a resposta da fonte externa e o JSON público da API são modelos diferentes. Uma mudança externa não precisa quebrar todo o sistema.
6. **Sincronização idempotente:** o repository atualiza semanas existentes e insere apenas as novas, importante porque os dados são revisados retrospectivamente.
7. **Manutenção localizada:** erros de banco, integração, regra ou HTTP podem ser investigados na camada responsável.

### Por que não utilizar uma arquitetura mais complexa

O sistema possui uma entidade principal e dois casos de uso: sincronizar dados e consultar semanas. CQRS, MediatR, Unit of Work adicional, repository genérico ou microsserviços aumentariam a quantidade de código sem resolver um problema real deste escopo.

O custo aceito da arquitetura em camadas é possuir mais projetos, interfaces e arquivos pequenos. Esse custo é justificado pela exigência explícita de separação entre Controller, Service e Repository e pelas duas integrações externas. A estrutura continua simples porque cada responsabilidade possui apenas uma implementação concreta.

## Fluxos do backend

### Sincronização ao iniciar

```text
Program
  → DatabaseInitializer
  → DengueSynchronizationHostedService
  → DengueSynchronizationService
  → EpidemiologicalWeekCalculator
  → AlertaDengueClient
  → DengueRepository.UpsertAsync
  → SQL Server
```

O hosted service executa uma vez em cada inicialização. Se a fonte externa estiver indisponível, a falha é registrada e a API continua atendendo os dados já persistidos.

Para iniciar sem consultar a fonte externa:

PowerShell:

```powershell
$env:DengueData__SynchronizeOnStartup="false"
```

Bash:

```bash
export DengueData__SynchronizeOnStartup=false
```

### Consulta semanal

```text
GET /api/dengue?ew=35&ey=2026
  → DengueController
  → DengueQueryService
  → IDengueRepository
  → DengueRepository
  → SQL Server
  → DengueWeekResponse
```

O endpoint consulta somente o banco preenchido anteriormente. Ele não chama a API externa durante a requisição.

## Arquitetura do frontend

O frontend utiliza arquitetura baseada em componentes e separa acesso HTTP, orquestração, estado e apresentação:

```text
App
  → useDengueData
  → dengueDashboardService
  → dengueApi
  → backend

App
  → SummaryCard
  → WeekCard
  → CasesChart
  → WeeklyTable
```

- `api`: executa `fetch` e trata respostas HTTP;
- `services`: coordena a regra das três semanas;
- `hooks`: controla loading, erro, dados, cancelamento e nova tentativa;
- `utils`: formata datas, números e níveis de alerta;
- `components`: apresenta cada parte da interface;
- `App.jsx`: organiza o dashboard sem conhecer detalhes de rede.

Essa separação evita chamadas HTTP espalhadas nos componentes e permite testar a regra das três requisições sem renderizar a interface.

## Estrutura do repositório

```text
.
├── backend
│   ├── Opus127.Dengue.Domain
│   │   ├── Entities
│   │   └── ValueObjects
│   ├── Opus127.Dengue.Application
│   │   ├── Abstractions
│   │   ├── Configuration
│   │   ├── Models
│   │   └── Services
│   ├── Opus127.Dengue.Infrastructure
│   │   ├── ExternalServices
│   │   └── Persistence
│   │       ├── Configurations
│   │       ├── Migrations
│   │       └── Repositories
│   ├── Opus127.Dengue.Api
│   │   ├── BackgroundServices
│   │   ├── Contracts
│   │   └── Controllers
│   └── tests
│       └── Opus127.Dengue.UnitTests
├── frontend
│   └── src
│       ├── api
│       ├── components
│       ├── hooks
│       ├── services
│       └── utils
├── compose.yaml
└── README.md
```

## Endpoints

### Consultar uma semana

```http
GET /api/dengue?ew=35&ey=2026
```

Resposta:

```json
{
  "semana_epidemiologica": "2026-35",
  "casos_est": 735,
  "casos_notificados": 54,
  "nivel_alerta": 3,
  "data_inicio": "2026-08-30"
}
```

Parâmetros inválidos retornam `400`. Uma semana válida que não esteja no banco retorna `404`. Os erros seguem o formato Problem Details.

### Consultar as semanas mais recentes disponíveis

```http
GET /api/dengue/weeks/latest?count=3
```

Esse endpoint auxiliar retorna os identificadores das semanas mais recentes persistidas. O frontend utiliza o resultado para realizar exatamente três chamadas ao endpoint semanal exigido no desafio. Isso evita presumir que a fonte externa já publicou a semana corrente.

## Dados persistidos

Cada semana armazena:

- município e semana epidemiológica;
- data inicial da semana;
- casos estimados e intervalo de credibilidade;
- casos notificados;
- nível de alerta;
- probabilidade de Rt maior que 1;
- incidência estimada por 100 mil habitantes;
- número reprodutivo;
- identificador e versão do modelo de origem;
- data da última sincronização.

A restrição única `Geocode + EpidemiologicalYear + EpidemiologicalWeek` impede semanas duplicadas. Como os valores da fonte podem mudar retrospectivamente, a sincronização atualiza os registros existentes.

Documentação da fonte: [API InfoDengue](https://info.dengue.mat.br/services/api/doc).

## Migrations

A API aplica migrations pendentes automaticamente ao iniciar. Para aplicar manualmente:

```shell
dotnet tool restore
dotnet ef database update --project backend/Opus127.Dengue.Infrastructure --startup-project backend/Opus127.Dengue.Api
```

Para criar uma migration durante o desenvolvimento:

```shell
dotnet ef migrations add NOME_DA_MIGRATION --project backend/Opus127.Dengue.Infrastructure --startup-project backend/Opus127.Dengue.Api --output-dir Persistence/Migrations
```

Não é necessário executar `database update` antes do primeiro `dotnet run`, porque a inicialização já aplica a migration existente.

## Testes e qualidade

Os testes não dependem do SQL Server nem da API externa.

Backend, na raiz do repositório:

```shell
dotnet format backend/Opus127.Dengue.slnx --verify-no-changes
dotnet build backend/Opus127.Dengue.slnx --configuration Release
dotnet test backend/Opus127.Dengue.slnx --configuration Release --no-build
```

Frontend:

```shell
cd frontend
pnpm install --frozen-lockfile
pnpm lint
pnpm test
pnpm build
```

Os testes cobrem:

- cálculo de semanas epidemiológicas, inclusive mudança de ano;
- regras e atualização da entidade;
- consulta ao repositório por meio do service;
- cálculo e persistência do intervalo de seis meses;
- URLs e parâmetros utilizados pelo frontend;
- execução das três consultas semanais obrigatórias.

## Configurações

| Chave | Padrão | Finalidade |
| --- | --- | --- |
| `ConnectionStrings:DengueDatabase` | SQL Server local com autenticação do Windows | Conexão do EF Core |
| `DengueData:Geocode` | `3106200` | Código IBGE de Belo Horizonte |
| `DengueData:SynchronizeOnStartup` | `true` | Sincronização ao iniciar |
| `AlertaDengue:BaseUrl` | `https://info.dengue.mat.br/` | Fonte externa |
| `AlertaDengue:Disease` | `dengue` | Doença consultada |
| `AlertaDengue:TimeoutSeconds` | `30` | Timeout da consulta externa |
| `AllowedOrigins` | `http://localhost:5173` | Origem permitida pelo CORS |

Em variáveis de ambiente, substitua `:` por `__`. Por exemplo, `AlertaDengue:TimeoutSeconds` se torna `AlertaDengue__TimeoutSeconds`.

Para apontar o frontend para outro backend no build:

PowerShell:

```powershell
$env:VITE_API_BASE_URL="https://api.exemplo.com/api"
pnpm build
```

Bash:

```bash
VITE_API_BASE_URL="https://api.exemplo.com/api" pnpm build
```

## Depuração

### Sincronização

Coloque breakpoints nesta ordem:

1. `DengueSynchronizationHostedService.ExecuteAsync`;
2. `DengueSynchronizationService.SynchronizeLastSixMonthsAsync`;
3. `AlertaDengueClient.FetchAsync`;
4. `DengueRepository.UpsertAsync`.

### Consulta semanal

Coloque breakpoints nesta ordem:

1. `DengueController.GetByWeek`;
2. `DengueQueryService.GetByWeekAsync`;
3. `DengueRepository.GetByWeekAsync`.

No frontend, abra a aba `Network` das ferramentas do navegador. O carregamento normal gera uma chamada para `weeks/latest` e três chamadas para o endpoint semanal.

## Problemas comuns

### O SDK definido no `global.json` não foi encontrado

Execute `dotnet --list-sdks`. O projeto aceita o .NET SDK `10.0.100` ou uma feature band posterior do .NET 10. Instale o SDK pelo link da seção de pré-requisitos se nenhuma versão 10 estiver disponível.

### A API não conecta ao SQL Server

- confirme que `docker compose ps` mostra o serviço como saudável ou que a instância nativa está executando;
- confirme que a variável `ConnectionStrings__DengueDatabase` foi definida no mesmo terminal do `dotnet run`;
- confirme que a porta `1433` não está ocupada por outra instância;
- para instâncias nomeadas do Windows, ajuste o valor de `Server`.

### A porta já está em uso

As portas padrão são `1433`, `5173`, `54956` e `54957`. Encerre o processo conflitante antes de iniciar o projeto.

### O frontend não carrega dados

Confirme primeiro que `https://localhost:54956/health` responde e que `/api/dengue/weeks/latest?count=3` retorna três semanas. Depois verifique a aba `Network` do navegador.

### A fonte externa está indisponível

A API continua funcionando com registros já persistidos. Em um banco novo e vazio, o dashboard só poderá exibir dados depois que uma sincronização com a fonte for concluída com sucesso.
