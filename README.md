# 🐾 Clyvo Care API - Monitoramento Preditivo de Saúde Animal

O **Clyvo Care** é uma plataforma de Backend desenvolvida para revolucionar o acompanhamento veterinário. Através da coleta de dados biométricos em tempo real, a API permite que tutores e clínicas monitorem sinais vitais de pets, possibilitando a detecção precoce de anomalias através de análise inteligente de dados.

---

## 🛠️ Tecnologias Utilizadas

* **Runtime:** .NET 10 (ASP.NET Core API)
* **Banco de Dados:** Oracle Database (via Entity Framework Core)
* **Documentação:** Swagger / OpenAPI
* **Padrão de Projeto:** DTO (Data Transfer Objects) e validação robusta de dados via Data Annotations.

---

## 🚀 Instruções de Instalação e Execução

 **Clonar o Repositório:**
   ```bash
   git clone [https://github.com/seu-usuario/ClyvoCare.API.git](https://github.com/seu-usuario/ClyvoCare.API.git)

   Configurar Banco de Dados:
As credenciais de acesso e a string de conexão com o Oracle já estão totalmente configuradas no arquivo appsettings.json. Certifique-se apenas de estar conectado à rede/VPN necessária para alcançar o servidor do banco.

Aplicar Migrations (Criação das Tabelas):
Abra o Console do Gerenciador de Pacotes no Visual Studio e execute o comando abaixo para gerar as tabelas automaticamente no Oracle:

PowerShell
Update-Database

Rodar a Aplicação:
Pressione F5 ou utilize o comando dotnet run no terminal.

Acessar a Interface Visual (MUITO IMPORTANTE):
Como esta é uma API RESTful pura, a raiz (/) retornará um erro 404 por padrão. Para visualizar e testar os endpoints através da interface gráfica, você deve acessar explicitamente a rota do Swagger.

Abra o navegador e acesse:
👉 https://localhost:7031/swagger (ou a porta gerada pelo seu Visual Studio)

📖 Documentação dos Endpoints (Swagger)
Abaixo está a explicação de como utilizar e testar cada funcionalidade disponível na interface do Swagger:

👤 Usuários (Tutores)
GET /api/Usuarios: Retorna a lista completa de tutores cadastrados.

GET /api/Usuarios/{id}: Busca os detalhes de um tutor específico pelo ID.

POST /api/Usuarios: Realiza o cadastro de um novo tutor. Utiliza o UsuarioCreateDTO para garantir que o e-mail seja válido e a senha tenha entre 6 e 20 caracteres.

PUT /api/Usuarios/{id}: Atualiza as informações cadastrais de um usuário existente.

DELETE /api/Usuarios/{id}: Remove permanentemente um tutor do sistema.

🐶 Pets
POST /api/Pets: Vincula um novo animal de estimação a um tutor existente. Exige um UsuarioId válido já cadastrado.

GET /api/Pets/especie/{especie}: Filtro Parametrizado para buscar todos os animais de uma espécie específica (ex: "Cachorro", "Gato").

GET /api/Pets/tutor/{usuarioId}: Retorna a lista de todos os pets que pertencem a um tutor específico.

GET /api/Pets/busca/{nome}: Realiza uma busca textual por parte do nome do pet (útil para campos de pesquisa em tempo real).

🩺 Logs de Saúde (Monitoramento & IoT)
POST /api/LogsSaude: Ponto de entrada que simula o recebimento de dados de sensores ou wearables. Recebe o peso, temperatura e batimentos cardíacos do pet. Possui validações contra dados absurdos (ex: temperatura fora da faixa de 30°C a 45°C).

GET /api/LogsSaude/pet/{petId}: Retorna o histórico cronológico de saúde de um pet específico, ordenado automaticamente do registro mais recente para o mais antigo.

🧠 Diferencial de IA e Negócio
A estrutura da tabela TB_CC_LOG_SAUDE armazena dados padronizados (NUMBER(10,2)) para evitar arredondamentos indesejados em medições críticas (como frações de peso e temperatura).

A ordenação decrescente na rota de histórico foi projetada para alimentar dashboards analíticos e modelos de Machine Learning (Predição de Risco). Ao cruzar esses dados históricos, o sistema consegue identificar tendências perigosas (como febre persistente ou perda repentina de peso) e disparar alertas preventivos antes que os sintomas clínicos se agravem.

✒️ Integrantes
Seu Nome Completo - RM: XXXXX

Nome do Integrante 2 - RM: XXXXX