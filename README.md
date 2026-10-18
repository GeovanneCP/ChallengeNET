# 🐾 Clyvo Companion API - Monitoramento Preditivo de Saúde Animal

O **Clyvo Companion** é uma plataforma de Backend desenvolvida para revolucionar o acompanhamento veterinário preventivo. Através da coleta e análise de dados biométricos em tempo real, a API permite que tutores e clínicas monitorem sinais vitais de pets, fornecendo uma base estruturada e padronizada para detecção precoce de anomalias e predição de riscos de saúde animal via Machine Learning.

---

## 🛠️ Tecnologias & Arquitetura (Sprint 4)

- **Runtime:** .NET 8.0 (C# / ASP.NET Core Web API)
- **Persistência Híbrida (SQL & NoSQL):**
  - **Oracle Database:** Persistência relacional transacional (Tutores, Pets, Logs de Saúde) via Entity Framework Core.
  - **MongoDB:** Persistência NoSQL de alta performance para telemetrias e séries temporais de dispositivos IoT (`PetTelemetriaNoSql`).
  - **EF Core In-Memory:** Isolamento completo de banco de dados para a execução dos testes de integração.
- **Padrões Arquiteturais:**
  - **Repository Pattern:** Desacoplamento entre controladores e fontes de dados com repositório genérico (`IRepository<T>`) e repositório especializado NoSQL (`ITelemetriaMongoRepository`).
  - **HATEOAS (Hypermedia as the Engine of Application State):** Links hipermídia semânticos nas respostas (`self`, `update_pet`, `delete_pet`, `telemetria_nosql`).
  - **Paginação Estruturada:** Paginação server-side com filtros, ordenação e metadados (`PageNumber`, `PageSize`, `TotalPages`, `TotalCount`).
  - **Tratamento Global de Exceções:** Middleware centralizado padronizado no formato **RFC 7807 (`ProblemDetails`)**.
- **Segurança:** Autenticação e autorização baseadas em **JWT Bearer Tokens**, com rotas de domínio devidamente protegidas (`[Authorize]`).
- **Documentação Interativa:** Swagger / OpenAPI configurado com suporte à autenticação Bearer Token.
- **Testes & Qualidade:** xUnit, Moq, FluentAssertions e `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`).
- **Observabilidade & Telemetria:** Serilog (Logging estruturado em arquivo e console), OpenTelemetry (Tracing distribuído W3C e métricas com Prometheus scraping `/metrics`) e ASP.NET Core Health Checks.

---

## 🏛️ Boas Práticas, Clean Code e Princípios SOLID

A aplicação segue uma divisão de responsabilidades desacoplada e manutenível:

- **SRP (Single Responsibility Principle):** Controllers lidam exclusivamente com requisições HTTP, roteamento e HATEOAS; Repositórios encapsulam o acesso ao Oracle e MongoDB; Erros não tratados são delegados ao Middleware Global.
- **OCP (Open/Closed Principle):** Estruturas preparadas para extensão sem modificação do código-base existente (ex.: novos filtros de paginação e provedores de banco de dados).
- **LSP (Liskov Substitution Principle):** Abstrações e interfaces intercambiáveis (como `IRepository<T>` e `ITelemetriaMongoRepository`) sem quebra de comportamento.
- **ISP (Interface Segregation Principle):** Interfaces enxutas e focadas estritamente nas responsabilidades do seu domínio.
- **DIP (Dependency Inversion Principle):** Dependência estrita de abstrações injetadas via contêiner nativo de DI do .NET:
  * **Scoped:** `AppDbContext`, repositórios relacionais (`IRepository<>`).
  * **Singleton:** `ITelemetriaMongoRepository`, telemetria do OpenTelemetry e métricas.
  * **Transient:** Validadores e formatadores leves.

---

## 🚦 Health Checks (Monitoramento de Infraestrutura)

A API implementa sondas padronizadas para orquestração de contêineres e observabilidade:

| Endpoint | Tipo de Probe | Descrição |
| :--- | :--- | :--- |
| `GET /health/live` | **Liveness Probe** | Valida se a aplicação está em execução e respondendo a requisições HTTP básicas. |
| `GET /health/ready` | **Readiness Probe** | Valida a integridade e disponibilidade das dependências críticas (ex.: conexão com o Oracle Database). |

---

## 📊 Observabilidade & Telemetria

- **Serilog Estruturado:** Geração diária de logs em `Logs/clyvocare-.log` e console, enriquecidos com contexto de thread, máquina e correlation IDs.
- **OpenTelemetry Tracing:** Rastreamento ponta a ponta com instrumentação nativa do ASP.NET Core e HttpClient.
- **Métricas Prometheus:** Exposição de métricas de telemetria no endpoint `/metrics`.

---

## 🔐 Autenticação e Segurança (JWT)

A API utiliza tokens **JWT (JSON Web Token)** para proteção das rotas sensíveis:

1. Realize uma requisição `POST /api/Auth/login` enviando e-mail e senha cadastrados.
2. Copie o token retornado na propriedade `token`.
3. No **Swagger**, clique no botão **Authorize** (cadeado no canto superior direito) e informe seu token :

   As rotas protegidas por [Authorize] (como Pets e TelemetriaPet) estarão autorizadas para consumo.

## 📖 Documentação dos Endpoints Principais

## 🔑 Autenticação

POST /api/Auth/login: Autentica o tutor e devolve o Token JWT com tempo de expiração.

## 👤 Usuários (Tutores)

GET /api/Usuarios: Lista todos os tutores cadastrados.

GET /api/Usuarios/{id}: Busca um tutor por ID.

POST /api/Usuarios: Cadastra um novo tutor.

PUT /api/Usuarios/{id}: Atualiza os dados de um tutor.

DELETE /api/Usuarios/{id}: Remove um tutor.

## 🐶 Pets (Com HATEOAS, Paginação e [Authorize])

GET /api/Pets: Lista paginada de animais (pageNumber, pageSize, searchTerm) acompanhada de hiperlinks HATEOAS.

GET /api/Pets/{id}: Busca detalhada do pet com links de navegação (self, update_pet, delete_pet, logs_saude, telemetria_nosql).

POST /api/Pets: Registra um novo animal vinculado a um tutor.

PUT /api/Pets/{id}: Atualiza os dados cadastrais do pet.

DELETE /api/Pets/{id}: Remove o animal.

## 🩺 Logs de Saúde (Sensores Relacionais - Oracle)

POST /api/LogsSaude: Persiste leituras biométricas estruturadas com validações estritas ([Range]).

GET /api/LogsSaude/pet/{petId}: Histórico de medições do animal ordenado cronologicamente.

## 📡 Telemetria IoT (Persistência NoSQL - MongoDB)

POST /api/TelemetriaPet: Registra telemetrias de sensores em alta frequência (temperatura, batimentos e alertas) no MongoDB.

GET /api/TelemetriaPet/pet/{petId}: Consulta o histórico de telemetrias NoSQL registradas para o pet.

## 📋 Exemplos de Payloads (JSON)

1. Login (POST /api/Auth/login)

{
  "email": "carlos.tutor@email.com",
  "senha": "mypassword123"
}

2. Cadastro de Usuário (POST /api/Usuarios)

{
  "nome": "Carlos Andrade",
  "email": "carlos.tutor@email.com",
  "senha": "mypassword123"
}

3. Cadastro de Pet (POST /api/Pets)

 {
  "nome": "Thor",
  "especie": "Cachorro",
  "dataNascimento": "2022-04-15",
  "usuarioId": 1
}

4. Telemetria NoSQL (POST /api/TelemetriaPet)

{
  "petId": 1,
  "temperatura": 38.5,
  "batimentos": 110,
  "statusAlerta": "Normal"
}

## 🧪 Estratégia de Testes Automatizados

A solução conta com cobertura completa em duas frentes:

1. Testes Unitários (ClyvoCare.UnitTests)

Padrão AAA (Arrange, Act, Assert).

Isolamento com Moq sobre IRepository<Pet> e serviços de log.

Cobertura de rotas, respostas HATEOAS e validação de regras de domínio.

2. Testes de Integração (ClyvoCare.IntegrationTests)

Execução ponta a ponta via WebApplicationFactory<Program>.

Injeção automática de Bearer Token nos cabeçalhos HTTP.

Isolamento com EF Core In-Memory Database descartável por execução.

Verificação de Health Checks e códigos de status HTTP (200 OK, 201 Created).

## ⚙️ Como Rodar os Testes no Terminal

Na raiz da solução:

# Executa todos os testes unitários e de integração
dotnet test

## 🚀 Como Executar a API

### 1. Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

- Instância do MongoDB em execução local (`mongodb://localhost:27017`) ou via Docker:

  docker run -d -p 27017:27017 --name mongodb mongo:latest

## 2. Executar a Aplicação

Pelo Visual Studio:

Abra a solução ClyvoCare.API.sln (ou ChallengeNET.sln).

Certifique-se de que o perfil de inicialização está em https ou ClyvoCare.API.

## Pelo Terminal:

No terminal, na pasta raiz da solução, execute:

dotnet run --project ChallengeNET

## 3. Acessar a Documentação Interativa (Swagger)

Abra o navegador no endereço:
👉 https://localhost:7290/swagger (ou http://localhost:5145/swagger)

Prima Ctrl + F5 (o navegador abrirá a página do Swagger automaticamente).

## ✒️ Integrantes

Geovanne Coneglian Passos - RM: 562673

Lucas Silva Gastão Pinheiro - RM: 563960

Guilherme Soares De Almeida - RM: 563143



