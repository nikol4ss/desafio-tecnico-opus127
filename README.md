# Dengue BH

Aplicação para consultar, persistir e visualizar dados de dengue de Belo Horizonte. O backend importa as semanas epidemiológicas correspondentes aos últimos seis meses da API AlertaDengue/InfoDengue, atualiza os registros ao iniciar e disponibiliza consultas semanais para o dashboard React.

## Tecnologias

### Backend

- .NET 10 e C#
- ASP.NET Core Web API
- Entity Framework Core 10
- SQL Server
- xUnit

### Frontend

- React 19 e JavaScript
- Vite
- Tailwind CSS
- Recharts
- Vitest e ESLint

## Arquitetura

O backend utiliza Layered Architecture com dependências direcionadas para o núcleo da aplicação:

```text
Api ──────────────► Application ──────────────► Domain
 │                       ▲                        ▲
 └──────► Infrastructure ┴────────────────────────┘
```

- `Domain`: entidade e objeto de valor, sem dependência de infraestrutura.
- `Application`: contratos, casos de uso, cálculo de semana epidemiológica e modelos de saída.
- `Infrastructure`: EF Core, SQL Server, repositório e cliente da API InfoDengue.
- `Api`: composição das dependências, endpoints, validação HTTP e sincronização inicial.

O frontend segue arquitetura baseada em componentes. Acesso HTTP, orquestração, estado, formatação e apresentação permanecem separados em `api`, `services`, `hooks`, `utils` e `components`.

### Por que essa arquitetura foi escolhida

O projeto possui quatro responsabilidades diferentes: receber requisições HTTP, executar regras da aplicação, consultar uma fonte externa e persistir dados. Colocar tudo dentro do controller reduziria a quantidade inicial de arquivos, mas misturaria responsabilidades e dificultaria testes e manutenção.

A separação em camadas mantém cada parte focada:

- `Api` trata HTTP, valida parâmetros e monta a resposta;
- `Application` coordena os casos de uso de consulta e sincronização;
- `Domain` protege os dados e as regras que não dependem de tecnologia;
- `Infrastructure` contém EF Core, SQL Server e a integração com o InfoDengue.

Na prática, isso permite testar os services sem iniciar servidor, banco ou internet. Também evita que uma alteração no JSON externo ou no banco seja espalhada pelo controller e pelo frontend.

O custo é possuir mais projetos e interfaces. Para este desafio, o custo é aceitável porque a separação entre Controller, Service e Repository foi solicitada explicitamente e existem duas integrações externas. Não foram adicionados CQRS, MediatR, repository genérico ou microsserviços porque não trariam benefício para este escopo.

### Fluxos principais

Sincronização executada ao iniciar a API:

```text
HostedService
  → DengueSynchronizationService
  → EpidemiologicalWeekCalculator
  → AlertaDengueClient
  → DengueRepository
  → SQL Server
```

Consulta realizada pelo frontend:

```text
React
  → DengueController
  → DengueQueryService
  → DengueRepository
  → SQL Server
```

O endpoint semanal consulta somente o banco. A API externa participa apenas da sincronização.

### Onde encontrar cada parte

```text
backend/
├── Opus127.Dengue.Api/
│   ├── Controllers/
│   ├── Contracts/
│   └── BackgroundServices/
├── Opus127.Dengue.Application/
│   ├── Abstractions/
│   ├── Models/
│   └── Services/
├── Opus127.Dengue.Domain/
│   ├── Entities/
│   └── ValueObjects/
├── Opus127.Dengue.Infrastructure/
│   ├── ExternalServices/
│   └── Persistence/
└── tests/Opus127.Dengue.UnitTests/

frontend/src/
├── api/
├── components/
├── hooks/
├── services/
└── utils/
```

## Como executar do zero

Os comandos funcionam em PowerShell, Bash e terminais equivalentes. O que muda entre máquinas é somente a conexão com a instância do SQL Server disponível.

### 1. Instale os pré-requisitos

- Git.
- .NET SDK `10.0.401` ou versão compatível do .NET 10.
- Node.js 20.19+, 22.12+ ou versão superior compatível com o Vite.
- pnpm 10 ou superior.
- SQL Server acessível localmente ou pela rede.

Confirme as instalações:

```shell
git --version
dotnet --version
node --version
pnpm --version
```

Caso o pnpm não esteja instalado:

```shell
npm install --global pnpm
```

### 2. Clone o repositório

```shell
git clone https://github.com/nikol4ss/desafio-tecnico-opus127.git
cd desafio-tecnico-opus127
```

Os comandos seguintes devem ser executados a partir da raiz do repositório.

### 3. Configure o SQL Server

A configuração padrão funciona com uma instância local do SQL Server no Windows usando autenticação integrada:

```text
Server=localhost;Database=Opus127Dengue;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True
```

Se essa configuração corresponde à sua máquina, apenas confirme que o serviço do SQL Server está executando.

Para qualquer outra instância local ou remota, defina a conexão no mesmo terminal em que executará o backend.

PowerShell:

```powershell
$env:ConnectionStrings__DengueDatabase="Server=SERVIDOR,1433;Database=Opus127Dengue;User Id=USUARIO;Password=SENHA;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

Bash:

```bash
export ConnectionStrings__DengueDatabase='Server=SERVIDOR,1433;Database=Opus127Dengue;User Id=USUARIO;Password=SENHA;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True'
```

Substitua `SERVIDOR`, `USUARIO` e `SENHA` pelos dados da sua instância. A máquina pode utilizar Windows, macOS ou Linux; ela precisa apenas alcançar um SQL Server válido pela conexão informada.

O usuário do banco precisa ter permissão para criar o banco `Opus127Dengue` na primeira execução e aplicar alterações de esquema. A API executa as migrations automaticamente ao iniciar, portanto não é necessário criar tabelas manualmente.

Para aplicar a migration manualmente, se necessário:

```shell
dotnet tool restore
dotnet ef database update --project backend/Opus127.Dengue.Infrastructure --startup-project backend/Opus127.Dengue.Api
```

### 4. Execute o backend

```shell
dotnet restore backend/Opus127.Dengue.slnx
dotnet run --project backend/Opus127.Dengue.Api --launch-profile Opus127.Dengue.Api
```

Aguarde a mensagem `Application started`.

Endereços locais:

- API HTTPS: `https://localhost:54956`
- API HTTP: `http://localhost:54957` (redireciona para HTTPS)
- OpenAPI: `https://localhost:54956/openapi/v1.json`
- Health check: `https://localhost:54956/health`

Na primeira execução, a API:

1. aplica a migration do banco;
2. calcula o intervalo epidemiológico equivalente aos últimos seis meses;
3. consulta Belo Horizonte (`geocode=3106200`) no InfoDengue;
4. insere novos registros e atualiza semanas já existentes.

Se o navegador não confiar no certificado HTTPS de desenvolvimento, execute uma vez:

```shell
dotnet dev-certs https --trust
```

Se a fonte externa estiver indisponível, a API registra o erro e continua atendendo os dados já persistidos. Para iniciar sem sincronizar:

PowerShell:

```powershell
$env:DengueData__SynchronizeOnStartup="false"
```

Bash:

```bash
export DengueData__SynchronizeOnStartup=false
```

### 5. Execute o frontend

Abra outro terminal na raiz do repositório:

```shell
cd frontend
pnpm install --frozen-lockfile
pnpm dev
```

Acesse `http://localhost:5173`. O Vite encaminha `/api` para a API HTTPS local.

O carregamento normal faz uma requisição para descobrir as três semanas disponíveis e depois realiza as três requisições semanais exigidas no desafio.

### 6. Verifique o funcionamento

Com backend e frontend executando, verifique:

```text
http://localhost:5173
https://localhost:54956/health
https://localhost:54956/api/dengue/weeks/latest?count=3
https://localhost:54956/api/dengue?ew=35&ey=2026
```

A última URL é um exemplo. Se a semana não estiver disponível, utilize uma das semanas retornadas por `/api/dengue/weeks/latest?count=3`.

Para encerrar, use `Ctrl+C` nos terminais do backend e do frontend. O banco permanece persistido no SQL Server.

Para apontar um build do frontend para outro backend:

```powershell
$env:VITE_API_BASE_URL="https://api.exemplo.com/api"
pnpm build
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

Parâmetros inválidos retornam `400`; semanas ausentes retornam `404` em formato Problem Details.

### Consultar as semanas mais recentes disponíveis

```http
GET /api/dengue/weeks/latest?count=3
```

Esse endpoint fornece os identificadores das semanas mais recentes persistidas. Em seguida, o frontend realiza exatamente três consultas ao endpoint semanal, uma para cada semana exigida no desafio. Assim, o dashboard não depende de a fonte externa já ter publicado a semana corrente.

## Dados persistidos

Cada semana armazena:

- município e semana epidemiológica;
- data inicial da semana;
- casos estimados e intervalo de credibilidade;
- casos notificados;
- nível de alerta;
- probabilidade de Rt maior que 1;
- incidência estimada por 100 mil habitantes;
- número reprodutivo, identificador e versão do modelo de origem;
- data da última sincronização.

A restrição única `Geocode + EpidemiologicalYear + EpidemiologicalWeek` torna a sincronização idempotente e impede semanas duplicadas.

Documentação da fonte: [API InfoDengue](https://info.dengue.mat.br/services/api/doc).

## Testes e qualidade

Backend:

```powershell
dotnet build backend/Opus127.Dengue.slnx --configuration Release
dotnet test backend/Opus127.Dengue.slnx --configuration Release
```

Frontend:

```powershell
cd frontend
pnpm lint
pnpm test
pnpm build
```

Os testes cobrem o cálculo de semanas epidemiológicas, regras da entidade, consulta ao repositório, sincronização dos seis meses, contrato HTTP do frontend e as três requisições semanais obrigatórias.

## Configurações

| Chave | Padrão | Finalidade |
| --- | --- | --- |
| `ConnectionStrings:DengueDatabase` | SQL Server local | Conexão do EF Core |
| `DengueData:Geocode` | `3106200` | Código IBGE de Belo Horizonte |
| `DengueData:SynchronizeOnStartup` | `true` | Sincronização ao iniciar |
| `AlertaDengue:BaseUrl` | `https://info.dengue.mat.br/` | Fonte externa |
| `AlertaDengue:TimeoutSeconds` | `30` | Timeout da consulta externa |
| `AllowedOrigins` | `http://localhost:5173` | Origem permitida pelo CORS |

Em variáveis de ambiente, substitua `:` por `__`, por exemplo `AlertaDengue__TimeoutSeconds`.

## Problemas comuns

### A API não conecta ao banco

- confirme que o SQL Server está executando;
- confirme servidor, porta, usuário e senha;
- confirme que `ConnectionStrings__DengueDatabase` foi definida no mesmo terminal do `dotnet run`;
- em uma instância nomeada, informe o nome correto no campo `Server`.

### O frontend não carrega os dados

Abra primeiro `https://localhost:54956/health`. Depois confirme que `/api/dengue/weeks/latest?count=3` retorna três semanas e verifique a aba `Network` do navegador.

### Uma porta já está sendo utilizada

As portas padrão são `5173`, `54956` e `54957`. Encerre o processo anterior antes de iniciar outro backend ou frontend.
