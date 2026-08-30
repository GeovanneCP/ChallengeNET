# 🐾 Clyvo Companion API - Monitoramento Preditivo de Saúde Animal

O **Clyvo Companion** é uma plataforma de Backend desenvolvida para revolucionar o acompanhamento veterinário preventivo. Através da coleta e análise de dados biométricos em tempo real, a API permite que tutores e clínicas monitorem sinais vitais de pets, fornecendo uma base estruturada e padronizada para detecção precoce de anomalias e predição de riscos de saúde animal via Machine Learning.

---

## 🛠️ Tecnologias & Arquitetura

* **Runtime:** .NET 8.0 (C# / ASP.NET Core Web API)
* **Banco de Dados:** Oracle Database (Produção/Desenvolvimento via Entity Framework Core) e EF Core In-Memory (Testes de Integração)
* **Documentação Interativa:** Swagger / OpenAPI
* **Testes & Qualidade:** xUnit, Moq, FluentAssertions e `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`)
* **Observabilidade & Telemetria:** OpenTelemetry (Métricas e Tracing distribuído), `ILogger<T>` estruturado e ASP.NET Core Health Checks

---

## 🏛️ Boas Práticas, Clean Code e Princípios SOLID

A aplicação segue uma divisão de responsabilidades desacoplada e manutenível:

* **SRP (Single Responsibility Principle):** Controllers lidam exclusivamente com requisições HTTP e roteamento; Regras de negócio e persistência são isoladas em Services e Repositórios; Transferência de dados padronizada via DTOs dedicados (`UsuarioCreateDTO`, `LogSaudeCreateDTO`, etc.).
* **OCP (Open/Closed Principle):** Estruturas preparadas para extensão sem modificação do código-base existente (ex.: filtros dinâmicos e novos providers de telemetria).
* **LSP (Liskov Substitution Principle):** Abstrações e interfaces de repositórios/serviços intercambiáveis sem quebra de comportamento.
* **ISP (Interface Segregation Principle):** Interfaces enxutas e focadas no domínio de cada entidade.
* **DIP (Dependency Inversion Principle):** Dependência estrita de abstrações injetadas via contêiner nativo do .NET, utilizando os ciclos de vida adequados:
  * **Scoped:** `DbContext`, Serviços de Domínio e Repositórios (uma instância por requisição HTTP).
  * **Singleton:** Configurações de métricas, `ActivitySource` de telemetria e instâncias imutáveis.
  * **Transient:** Validadores e formatadores leves e sem estado.

---

## 🚦 Health Checks (Monitoramento de Infraestrutura)

A API implementa endpoints padronizados para monitoramento de integridade e orquestração (Kubernetes/Docker probes):

| Endpoint | Tipo de Probe | Descrição |
| :--- | :--- | :--- |
| `GET /health/live` | **Liveness Probe** | Valida se a aplicação está em execução e respondendo às requisições HTTP básicas. |
| `GET /health/ready` | **Readiness Probe** | Valida a disponibilidade e integridade das dependências críticas (ex.: conexão com o Oracle Database). |

---

## 📊 Observabilidade & Telemetria

A API conta com monitoramento distribuído e métricas de desempenho prontas para coleta:

* **Logs Estruturados:** Utilização de `ILogger<T>` com identificadores semânticos em operações de CRUD e exceções de negócio, facilitando a busca e correlação em agregadores (ex.: Seq, ELK, Datadog).
* **Tracing Distribuído:** Integração com `ActivitySource` e cabeçalhos W3C `traceparent` para rastreamento de ponta a ponta das requisições.
* **Métricas com OpenTelemetry:**
  * **Counter:** Contagem de requisições de logs de saúde e novos cadastros.
  * **Histogram:** Distribuição e latência do tempo de resposta das operações de persistência.

---

## 🧪 Estratégia de Testes Automatizados

A solução conta com duas suites de testes automatizados:

### 1. Testes Unitários (`ClyvoCare.UnitTests`)
* Padrão de escrita **AAA (Arrange, Act, Assert)**.
* Nomenclatura descritiva de testes: `Metodo_EstadoDoCenario_ComportamentoEsperado`.
* Isolamento total de dependências com **Moq**, garantindo validação rápida de regras de domínio, limites numéricos de sinais vitais e validação de DTOs.

### 2. Testes de Integração (`ClyvoCare.IntegrationTests`)
* Execução em pipeline real HTTP via **`WebApplicationFactory<Program>`**.
* Utilização de **xUnit Fixtures** (`CustomWebApplicationFactory`) com substituição do Oracle Database pelo **Entity Framework Core In-Memory Database** isolado.
* Testes ponta a ponta dos endpoints de API (`/api/Usuarios`, `/api/LogsSaude`) e verificação das rotas de Health Check.

### ⚙️ Executando os Testes no Terminal

Na raiz do repositório, execute:

# Executar todos os testes da solução
dotnet test

# Executar apenas os testes unitários
dotnet test ClyvoCare.UnitTests

# Executar apenas os testes de integração
dotnet test ClyvoCare.IntegrationTests

🚀 Instruções de Instalação e Execução da API
1. Configuração do Banco de Dados
As credenciais de acesso ao Oracle Database estão definidas no appsettings.json. Certifique-se de estar conectado à VPN/rede adequada para comunicação com o banco.

2. Aplicar Migrations
No Package Manager Console do Visual Studio, execute:

PowerShell
Update-Database
Ou via .NET CLI:

PowerShell
dotnet ef database update --project ChallengeNET
3. Rodar a Aplicação
Pressione F5 no Visual Studio ou utilize o terminal:

PowerShell
dotnet run --project ChallengeNET
4. Acessar o Swagger
Abra seu navegador e acesse a interface interativa:
👉 https://localhost:7031/swagger (ou a porta gerada localmente)

📖 Documentação dos Endpoints
👤 Usuários (Tutores)
GET /api/Usuarios: Retorna a lista completa de tutores cadastrados.

GET /api/Usuarios/{id}: Busca os detalhes de um tutor específico por ID.

POST /api/Usuarios: Realiza o cadastro de um novo tutor (valida formato de e-mail e tamanho mínimo/máximo de senha).

PUT /api/Usuarios/{id}: Atualiza os dados de um tutor existente.

DELETE /api/Usuarios/{id}: Remove um tutor do sistema.

🐶 Pets
POST /api/Pets: Vincula um animal de estimação a um tutor existente. Requer usuarioId válido.

GET /api/Pets/especie/{especie}: Filtro parametrizado por espécie (ex: "Cachorro", "Gato").

GET /api/Pets/tutor/{usuarioId}: Lista todos os pets associados a um tutor específico.

GET /api/Pets/busca/{nome}: Busca textual aproximada por parte do nome do animal.

🩺 Logs de Saúde (Sensores & IoT)
POST /api/LogsSaude: Recebe e persiste leituras biométricas (peso, temperatura, batimentos).

GET /api/LogsSaude/pet/{petId}: Retorna o histórico cronológico de saúde do pet, ordenado do mais recente para o mais antigo.

📋 Regras de Negócio e Exemplos de Payload (JSON)
1. Cadastro de Usuário (POST /api/Usuarios)
Regra: O e-mail deve ser válido e exclusivo. A senha precisa ter entre 6 e 20 caracteres.

JSON
{
  "nome": "Carlos Andrade",
  "email": "carlos.tutor@email.com",
  "senha": "mypassword123"
}
2. Cadastro de Pet (POST /api/Pets)
Regra: É obrigatório associar a um usuarioId já existente no banco de dados. Caso não exista, a requisição é rejeitada com 404 Not Found.

JSON
{
  "nome": "Thor",
  "especie": "Cachorro",
  "dataNascimento": "2022-04-15",
  "usuarioId": 1
}
3. Registro de Logs de Saúde (POST /api/LogsSaude)
Regra de Consistência para IA: O campo temperatura possui validação estrita com [Range(30, 45)] para evitar que leituras anômalas ou falhas de sensores comprometam os modelos preditivos de saúde.

JSON
{
  "peso": 12.50,
  "temperatura": 38.60,
  "batimentosCardiacos": 110,
  "observacoes": "Dispositivo IoT coletou métricas normais durante o repouso.",
  "petId": 1
}

✒️ Integrantes

Geovanne Coneglian Passos - RM: 562673

Lucas Silva Gastão Pinheiro - RM: 563960

Guilherme Soares De Almeida - RM: 563143
