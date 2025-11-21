# API Documentation (Swagger) / Documentação da API (Swagger)

## English

### Overview

The API includes interactive Swagger documentation that is automatically available when running the API.

### Accessing Swagger

- **URL**: http://localhost:5010 (when running locally or in Docker)
- The Swagger UI is configured as the default route (root path)

### Features

- **Interactive Testing**: Test all API endpoints directly from the browser
- **Request/Response Schemas**: View detailed request and response models
- **Endpoint Descriptions**: Each endpoint includes descriptions and examples
- **Try It Out**: Execute API calls and see real responses

### Available Endpoints

#### Carts

- `POST /api/carts` - Create a new cart
- `GET /api/carts/{id}` - Get cart by ID
- `POST /api/carts/{id}/items` - Add item to cart
- `PUT /api/carts/{id}/items/{productId}` - Update item quantity
- `DELETE /api/carts/{id}/items/{productId}` - Remove item from cart
- `DELETE /api/carts/{id}` - Delete cart

#### Products

- `GET /api/products` - List products (with pagination and search)
- `GET /api/products/{id}` - Get product by ID

#### Categories

- `GET /api/categories` - List all categories

#### Orders

- `POST /api/orders` - Create order from cart
- `GET /api/orders` - List orders (with pagination)
- `GET /api/orders/{id}` - Get order by ID

#### Discounts

- `GET /api/discounts` - List all discounts
- `GET /api/discounts/active` - List active discounts only

#### OrderQuantityRules

- `GET /api/orderquantityrules` - List all quantity rules
- `POST /api/orderquantityrules` - Create a new quantity rule
- `PUT /api/orderquantityrules/{id}` - Update quantity rule
- `DELETE /api/orderquantityrules/{id}` - Delete quantity rule

### Example Usage

#### Creating a Cart and Adding Items

1. Create a cart: `POST /api/carts`
2. Add items: `POST /api/carts/{cartId}/items` with body:
   ```json
   {
     "productId": "guid-here",
     "quantity": 1
   }
   ```
3. View cart: `GET /api/carts/{cartId}`

The cart response will include:
- Items with prices
- Subtotal
- Applied discount (if any)
- Final total

---

## Português

### Visão Geral

A API inclui documentação Swagger interativa que está automaticamente disponível ao executar a API.

### Acessando o Swagger

- **URL**: http://localhost:5010 (ao executar localmente ou no Docker)
- O Swagger UI está configurado como a rota padrão (rota raiz)

### Funcionalidades

- **Teste Interativo**: Teste todos os endpoints da API diretamente do navegador
- **Esquemas de Requisição/Resposta**: Visualize modelos detalhados de requisição e resposta
- **Descrições de Endpoints**: Cada endpoint inclui descrições e exemplos
- **Try It Out**: Execute chamadas de API e veja respostas reais

### Endpoints Disponíveis

#### Carts

- `POST /api/carts` - Criar um novo carrinho
- `GET /api/carts/{id}` - Obter carrinho por ID
- `POST /api/carts/{id}/items` - Adicionar item ao carrinho
- `PUT /api/carts/{id}/items/{productId}` - Atualizar quantidade do item
- `DELETE /api/carts/{id}/items/{productId}` - Remover item do carrinho
- `DELETE /api/carts/{id}` - Excluir carrinho

#### Products

- `GET /api/products` - Listar produtos (com paginação e busca)
- `GET /api/products/{id}` - Obter produto por ID

#### Categories

- `GET /api/categories` - Listar todas as categorias

#### Orders

- `POST /api/orders` - Criar pedido a partir do carrinho
- `GET /api/orders` - Listar pedidos (com paginação)
- `GET /api/orders/{id}` - Obter pedido por ID

#### Discounts

- `GET /api/discounts` - Listar todos os descontos
- `GET /api/discounts/active` - Listar apenas descontos ativos

#### OrderQuantityRules

- `GET /api/orderquantityrules` - Listar todas as regras de quantidade
- `POST /api/orderquantityrules` - Criar uma nova regra de quantidade
- `PUT /api/orderquantityrules/{id}` - Atualizar regra de quantidade
- `DELETE /api/orderquantityrules/{id}` - Excluir regra de quantidade

### Exemplo de Uso

#### Criando um Carrinho e Adicionando Itens

1. Criar um carrinho: `POST /api/carts`
2. Adicionar itens: `POST /api/carts/{cartId}/items` com body:
   ```json
   {
     "productId": "guid-aqui",
     "quantity": 1
   }
   ```
3. Visualizar carrinho: `GET /api/carts/{cartId}`

A resposta do carrinho incluirá:
- Itens com preços
- Subtotal
- Desconto aplicado (se houver)
- Total final

