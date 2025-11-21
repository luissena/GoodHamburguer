# GoodHamburguer 🍔

**Quick Navigation / Navegação Rápida:** [English](#english) | [Português](#português)

---

## English

### Overview

Full-stack e-commerce application for a hamburger restaurant built with .NET 10.0, featuring a RESTful API and a Blazor WebAssembly client.

### Project Structure

The project follows Clean Architecture principles with clear separation of concerns:

```
GoodHamburguer/
├── GoodHamburguer.Api/          # REST API (ASP.NET Core)
│   ├── Controllers/             # API endpoints
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Repositories/            # In-memory data repositories
│   └── Data/                    # Initial data seeding
│
├── GoodHamburguer.Client/       # Frontend (Blazor WebAssembly)
│   ├── Pages/                   # Application pages
│   ├── Components/              # Reusable UI components
│   └── Services/                # API communication services
│
├── GoodHamburguer.Domain/       # Domain Layer (Business Logic)
│   ├── Entities/                # Domain entities (Cart, Product, Order, Discount)
│   └── OrderQuantityValidator.cs # Business rules validator
│
└── GoodHamburguer.Domain.Tests/ # Unit Tests
```

**Key Components:**
- **API Layer**: RESTful endpoints for carts, products, orders, discounts
- **Client Layer**: Blazor WebAssembly UI with shopping cart functionality
- **Domain Layer**: Core business logic with entities and validation rules

### Data Seeder

The project includes an automatic data seeder (`DataSeeder.cs`) that runs on API startup and creates all challenge requirements:

**What it creates:**
- **Categories**: Extras, Sandwiches
- **Products**: X Burger ($5.00), X Egg ($4.50), X Bacon ($7.00), Fries ($2.00), Soft Drink ($2.50)
- **Discounts**: Complete Combo (20%), Drink Combo (15%), Fries Combo (10%)

**Duplicate Prevention:**
The seeder checks if categories already exist before seeding. If data is already present, it skips the seeding process to prevent duplicates.

The seeder runs automatically when the API starts, ensuring all required data for the challenge is available immediately.

### Discount System Logic

#### How Discounts Work

1. **Discount Structure**: Each discount has:
   - Name (e.g., "Complete Combo")
   - Percentage (e.g., 20%)
   - One or more conditions (ALL must be met)

2. **Condition Types**:
   - **Product**: Minimum quantity of a specific product
   - **Category**: Minimum quantity from a category
   - **Product + Category**: Both specific product and category requirement

3. **Discount Calculation Process**:
   ```
   Step 1: Evaluate all active discounts
   Step 2: Check which discounts have ALL conditions satisfied
   Step 3: Select the discount with the HIGHEST percentage
   Step 4: Calculate discount amount = cart total × (percentage / 100)
   Step 5: Final total = cart total - discount amount
   ```

#### Example Calculation

**Cart Contents:**
- 1 X Burger (Sandwich) = $5.00
- 1 Fries = $2.00
- 1 Soft Drink = $2.50
- **Subtotal** = $9.50

**Available Discounts:**
- Complete Combo (20%): Requires 1 Sandwich + 1 Fries + 1 Soft Drink ✓
- Drink Combo (15%): Requires 1 Sandwich + 1 Soft Drink ✓
- Fries Combo (10%): Requires 1 Sandwich + 1 Fries ✓

**Result:**
- Applied discount: **Complete Combo (20%)** (highest percentage)
- Discount amount: $9.50 × 0.20 = **$1.90**
- Final total: $9.50 - $1.90 = **$7.60**

#### Implementation Details

The discount evaluation happens in `Cart.GetBestApplicableDiscount()`:
- Evaluates all discounts using AND logic (all conditions must match)
- Filters to only applicable discounts
- Selects the one with highest percentage
- Calculates discount amount in `Discount.CalculateDiscount(total)`

### Running the Project

#### Option 1: Docker (Recommended)

```bash
docker-compose up --build
```

**Access:**
- **API & Swagger**: http://localhost:5010
- **Client**: http://localhost:5218

**Stop containers:**
```bash
docker-compose down
```

#### Option 2: Local Development

1. **Run API:**
   ```bash
   cd GoodHamburguer.Api
   dotnet run
   ```
   API available at: http://localhost:5010

2. **Run Client** (new terminal):
   ```bash
   cd GoodHamburguer.Client
   dotnet run
   ```
   Client available at: http://localhost:5218

### Testing the API

#### Using Swagger UI

1. Access Swagger at: **http://localhost:5010**
2. Interactive interface to test all endpoints
3. View request/response schemas
4. Execute API calls directly from browser

#### Key Endpoints for Testing

**Carts:**
```
POST   /api/carts                           # Create cart
GET    /api/carts/{id}                      # Get cart (shows applied discount)
POST   /api/carts/{id}/items                # Add item
PUT    /api/carts/{id}/items/{productId}    # Update quantity
DELETE /api/carts/{id}/items/{productId}    # Remove item
```

**Products:**
```
GET /api/products        # List all products
GET /api/products/{id}   # Get product details
```

**Orders:**
```
POST /api/orders         # Create order from cart
GET  /api/orders         # List orders
GET  /api/orders/{id}    # Get order details
```

**Discounts:**
```
GET /api/discounts        # List all discounts
GET /api/discounts/active # List active discounts only
```

#### Test Discount Calculation

1. **Create a cart:**
   ```bash
   POST /api/carts
   ```
   Response: `{ "id": "guid-here", ... }`

2. **Add items to cart:**
   ```bash
   POST /api/carts/{cartId}/items
   Body: {
     "productId": "sandwich-id",
     "quantity": 1
   }
   ```
   Repeat for Fries and Soft Drink.

3. **Get cart to see discount:**
   ```bash
   GET /api/carts/{cartId}
   ```
   Response includes:
   - `subtotal`: Cart total before discount
   - `discountAmount`: Applied discount value
   - `discountName`: Name of applied discount
   - `total`: Final amount after discount

4. **Create order:**
   ```bash
   POST /api/orders
   Body: {
     "cartId": "cart-id-here"
   }
   ```
   Order preserves discount information.

#### Postman Collection

A Postman collection is available at:
- `GoodHamburguer.Api/GoodHamburguer_API.postman_collection.json`
- `GoodHamburguer.Api/GoodHamburguer_API.postman_environment.json`

Import both files into Postman for ready-to-use API requests.

### Technologies

- **.NET 10.0**: Latest .NET framework
- **ASP.NET Core**: Web API framework
- **Blazor WebAssembly**: Client-side web framework
- **Swagger/OpenAPI**: API documentation
- **Docker**: Containerization

---

## Português

### Visão Geral

Aplicação e-commerce full-stack para um restaurante de hambúrgueres construída com .NET 10.0, incluindo uma API RESTful e um cliente Blazor WebAssembly.

### Estrutura do Projeto

O projeto segue princípios de Clean Architecture com separação clara de responsabilidades:

```
GoodHamburguer/
├── GoodHamburguer.Api/          # API REST (ASP.NET Core)
│   ├── Controllers/             # Endpoints da API
│   ├── DTOs/                    # Objetos de Transferência de Dados
│   ├── Repositories/            # Repositórios de dados em memória
│   └── Data/                    # Seed inicial de dados
│
├── GoodHamburguer.Client/       # Frontend (Blazor WebAssembly)
│   ├── Pages/                   # Páginas da aplicação
│   ├── Components/              # Componentes UI reutilizáveis
│   └── Services/                # Serviços de comunicação com API
│
├── GoodHamburguer.Domain/       # Camada de Domínio (Lógica de Negócio)
│   ├── Entities/                # Entidades de domínio (Cart, Product, Order, Discount)
│   └── OrderQuantityValidator.cs # Validador de regras de negócio
│
└── GoodHamburguer.Domain.Tests/ # Testes Unitários
```

**Componentes Principais:**
- **Camada API**: Endpoints RESTful para carrinhos, produtos, pedidos, descontos
- **Camada Cliente**: Interface Blazor WebAssembly com funcionalidade de carrinho
- **Camada Domínio**: Lógica de negócio central com entidades e regras de validação

### Seed de Dados

O projeto inclui um seed automático de dados (`DataSeeder.cs`) que executa na inicialização da API e cria todos os requisitos do desafio:

**O que é criado:**
- **Categorias**: Extras, Sandwiches
- **Produtos**: X Burger (R$ 5,00), X Egg (R$ 4,50), X Bacon (R$ 7,00), Batata Frita (R$ 2,00), Refrigerante (R$ 2,50)
- **Descontos**: Combo Completo (20%), Combo Bebida (15%), Combo Batata (10%)

**Prevenção de Duplicidade:**
O seeder verifica se as categorias já existem antes de fazer o seed. Se os dados já estiverem presentes, ele pula o processo de seed para evitar duplicidades.

O seeder executa automaticamente quando a API inicia, garantindo que todos os dados necessários para o desafio estejam disponíveis imediatamente.

### Lógica do Sistema de Descontos

#### Como Funcionam os Descontos

1. **Estrutura do Desconto**: Cada desconto possui:
   - Nome (ex: "Combo Completo")
   - Porcentagem (ex: 20%)
   - Uma ou mais condições (TODAS devem ser atendidas)

2. **Tipos de Condições**:
   - **Produto**: Quantidade mínima de um produto específico
   - **Categoria**: Quantidade mínima de uma categoria
   - **Produto + Categoria**: Requisito de produto específico e categoria

3. **Processo de Cálculo do Desconto**:
   ```
   Passo 1: Avaliar todos os descontos ativos
   Passo 2: Verificar quais descontos têm TODAS as condições satisfeitas
   Passo 3: Selecionar o desconto com a MAIOR porcentagem
   Passo 4: Calcular valor do desconto = total do carrinho × (porcentagem / 100)
   Passo 5: Total final = total do carrinho - valor do desconto
   ```

#### Exemplo de Cálculo

**Conteúdo do Carrinho:**
- 1 X Burger (Sanduíche) = R$ 5,00
- 1 Batata Frita = R$ 2,00
- 1 Refrigerante = R$ 2,50
- **Subtotal** = R$ 9,50

**Descontos Disponíveis:**
- Combo Completo (20%): Requer 1 Sanduíche + 1 Batata + 1 Refrigerante ✓
- Combo Bebida (15%): Requer 1 Sanduíche + 1 Refrigerante ✓
- Combo Batata (10%): Requer 1 Sanduíche + 1 Batata ✓

**Resultado:**
- Desconto aplicado: **Combo Completo (20%)** (maior porcentagem)
- Valor do desconto: R$ 9,50 × 0,20 = **R$ 1,90**
- Total final: R$ 9,50 - R$ 1,90 = **R$ 7,60**

#### Detalhes de Implementação

A avaliação do desconto acontece em `Cart.GetBestApplicableDiscount()`:
- Avalia todos os descontos usando lógica AND (todas as condições devem corresponder)
- Filtra apenas descontos aplicáveis
- Seleciona o de maior porcentagem
- Calcula valor do desconto em `Discount.CalculateDiscount(total)`

### Como Executar o Projeto

#### Opção 1: Docker (Recomendado)

```bash
docker-compose up --build
```

**Acesso:**
- **API e Swagger**: http://localhost:5010
- **Cliente**: http://localhost:5218

**Parar containers:**
```bash
docker-compose down
```

#### Opção 2: Desenvolvimento Local

1. **Executar API:**
   ```bash
   cd GoodHamburguer.Api
   dotnet run
   ```
   API disponível em: http://localhost:5010

2. **Executar Cliente** (novo terminal):
   ```bash
   cd GoodHamburguer.Client
   dotnet run
   ```
   Cliente disponível em: http://localhost:5218

### Como Testar a API

#### Usando Swagger UI

1. Acesse Swagger em: **http://localhost:5010**
2. Interface interativa para testar todos os endpoints
3. Visualize esquemas de requisição/resposta
4. Execute chamadas de API diretamente do navegador

#### Endpoints Principais para Teste

**Carrinhos:**
```
POST   /api/carts                           # Criar carrinho
GET    /api/carts/{id}                      # Obter carrinho (mostra desconto aplicado)
POST   /api/carts/{id}/items                # Adicionar item
PUT    /api/carts/{id}/items/{productId}    # Atualizar quantidade
DELETE /api/carts/{id}/items/{productId}    # Remover item
```

**Produtos:**
```
GET /api/products        # Listar todos os produtos
GET /api/products/{id}   # Obter detalhes do produto
```

**Pedidos:**
```
POST /api/orders         # Criar pedido a partir do carrinho
GET  /api/orders         # Listar pedidos
GET  /api/orders/{id}    # Obter detalhes do pedido
```

**Descontos:**
```
GET /api/discounts        # Listar todos os descontos
GET /api/discounts/active # Listar apenas descontos ativos
```

#### Testar Cálculo de Desconto

1. **Criar um carrinho:**
   ```bash
   POST /api/carts
   ```
   Resposta: `{ "id": "guid-aqui", ... }`

2. **Adicionar itens ao carrinho:**
   ```bash
   POST /api/carts/{cartId}/items
   Body: {
     "productId": "id-do-sanduiche",
     "quantity": 1
   }
   ```
   Repita para Batata Frita e Refrigerante.

3. **Obter carrinho para ver desconto:**
   ```bash
   GET /api/carts/{cartId}
   ```
   Resposta inclui:
   - `subtotal`: Total do carrinho antes do desconto
   - `discountAmount`: Valor do desconto aplicado
   - `discountName`: Nome do desconto aplicado
   - `total`: Valor final após desconto

4. **Criar pedido:**
   ```bash
   POST /api/orders
   Body: {
     "cartId": "id-do-carrinho"
   }
   ```
   O pedido preserva informações do desconto.

#### Coleção Postman

Uma coleção Postman está disponível em:
- `GoodHamburguer.Api/GoodHamburguer_API.postman_collection.json`
- `GoodHamburguer.Api/GoodHamburguer_API.postman_environment.json`

Importe ambos os arquivos no Postman para requisições prontas para uso.

### Tecnologias

- **.NET 10.0**: Framework .NET mais recente
- **ASP.NET Core**: Framework Web API
- **Blazor WebAssembly**: Framework web do lado do cliente
- **Swagger/OpenAPI**: Documentação da API
- **Docker**: Containerização

---

## License

This project is open source and available under the MIT License.
