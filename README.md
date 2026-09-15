# Dengue BH

Aplicação para consultar, persistir e visualizar dados de dengue de Belo Horizonte. O backend importa as semanas epidemiológicas correspondentes aos últimos seis meses da API InfoDengue, mantém os dados atualizados no SQL Server e disponibiliza consultas semanais para o dashboard React.

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

## Pré-requisitos

- .NET SDK `10.0.401` ou versão compatível do .NET 10.
- Node.js 20.19+, 22.12+ ou versão superior compatível com o Vite.
- pnpm 10 ou superior.
- SQL Server com autenticação integrada do Windows habilitada.

## Banco de dados

A configuração local padrão utiliza:

```text
Server=localhost;Database=Opus127Dengue;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True
```

Para outro servidor, defina a variável antes de iniciar a API:

```powershell
$env:ConnectionStrings__DengueDatabase="Server=SEU_SERVIDOR;Database=Opus127Dengue;Trusted_Connection=True;TrustServerCertificate=True"
```

A API aplica migrations pendentes durante a inicialização. Também é possível aplicar explicitamente:

```powershell
dotnet tool restore
dotnet ef database update `
  --project backend/Opus127.Dengue.Infrastructure `
  --startup-project backend/Opus127.Dengue.Api
```

## Execução

### 1. Backend

Na raiz do repositório:

```powershell
dotnet restore backend/Opus127.Dengue.slnx
dotnet run `
  --project backend/Opus127.Dengue.Api `
  --launch-profile Opus127.Dengue.Api
```

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

Se a fonte externa estiver indisponível, a API registra o erro e continua atendendo os dados já persistidos. Para iniciar sem sincronizar:

```powershell
$env:DengueData__SynchronizeOnStartup="false"
```

### 2. Frontend

Em outro terminal:

```powershell
cd frontend
pnpm install
pnpm dev
```

Acesse `http://localhost:5173`. O Vite encaminha `/api` para a API HTTPS local.

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
