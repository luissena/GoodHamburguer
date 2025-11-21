# Running the Project / Como Executar o Projeto

## English

### Prerequisites

- .NET 10.0 SDK
- Docker and Docker Compose (for containerized deployment)

### Option 1: Using Docker (Recommended)

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd GoodHamburguer
   ```

2. **Build and run with Docker Compose**:
   ```bash
   docker-compose up --build
   ```

3. **Access the applications**:
   - **API**: http://localhost:5010
   - **Swagger UI**: http://localhost:5010 (root path)
   - **Client**: http://localhost:5218

4. **Stop the containers**:
   ```bash
   docker-compose down
   ```

### Option 2: Running Locally

1. **Run the API**:
   ```bash
   cd GoodHamburguer.Api
   dotnet run
   ```
   API will be available at: http://localhost:5010

2. **Run the Client** (in a new terminal):
   ```bash
   cd GoodHamburguer.Client
   dotnet run
   ```
   Client will be available at: http://localhost:5218

3. **Update Client Configuration** (if needed):
   Edit `GoodHamburguer.Client/appsettings.json` to set the correct API address:
   ```json
   {
     "ApiBaseAddress": "http://localhost:5010"
   }
   ```

### Troubleshooting

#### Port Already in Use

If you get a port conflict error:
- Change the ports in `docker-compose.yml` or `launchSettings.json`
- Or stop the service using the port

#### CORS Issues

If you encounter CORS errors:
- Ensure the API is running
- Check that the client's `ApiBaseAddress` matches the API URL
- Verify CORS configuration in `Program.cs`

#### Docker Build Fails

If Docker build fails:
- Ensure Docker is running
- Check that you have enough disk space
- Try cleaning Docker cache: `docker system prune -a`

---

## Português

### Pré-requisitos

- .NET 10.0 SDK
- Docker e Docker Compose (para implantação containerizada)

### Opção 1: Usando Docker (Recomendado)

1. **Clone o repositório**:
   ```bash
   git clone <repository-url>
   cd GoodHamburguer
   ```

2. **Construa e execute com Docker Compose**:
   ```bash
   docker-compose up --build
   ```

3. **Acesse as aplicações**:
   - **API**: http://localhost:5010
   - **Swagger UI**: http://localhost:5010 (rota raiz)
   - **Client**: http://localhost:5218

4. **Pare os containers**:
   ```bash
   docker-compose down
   ```

### Opção 2: Executando Localmente

1. **Execute a API**:
   ```bash
   cd GoodHamburguer.Api
   dotnet run
   ```
   A API estará disponível em: http://localhost:5010

2. **Execute o Client** (em um novo terminal):
   ```bash
   cd GoodHamburguer.Client
   dotnet run
   ```
   O Client estará disponível em: http://localhost:5218

3. **Atualize a Configuração do Client** (se necessário):
   Edite `GoodHamburguer.Client/appsettings.json` para definir o endereço correto da API:
   ```json
   {
     "ApiBaseAddress": "http://localhost:5010"
   }
   ```

### Solução de Problemas

#### Porta Já em Uso

Se você receber um erro de conflito de porta:
- Altere as portas em `docker-compose.yml` ou `launchSettings.json`
- Ou pare o serviço que está usando a porta

#### Problemas de CORS

Se você encontrar erros de CORS:
- Certifique-se de que a API está em execução
- Verifique se o `ApiBaseAddress` do cliente corresponde à URL da API
- Verifique a configuração de CORS em `Program.cs`

#### Falha no Build do Docker

Se o build do Docker falhar:
- Certifique-se de que o Docker está em execução
- Verifique se você tem espaço em disco suficiente
- Tente limpar o cache do Docker: `docker system prune -a`

