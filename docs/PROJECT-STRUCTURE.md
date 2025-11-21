# Project Structure / Estrutura do Projeto

## English

### Overview

The project follows a clean architecture pattern with clear separation of concerns:

```
GoodHamburguer/
├── GoodHamburguer.Api/          # REST API Backend
│   ├── Controllers/             # API endpoints (Carts, Products, Orders, etc.)
│   ├── DTOs/                    # Data Transfer Objects for API responses
│   ├── Repositories/            # In-memory data repositories
│   ├── Data/                    # Data seeding (initial products, categories, discounts)
│   └── Program.cs               # API startup and configuration
│
├── GoodHamburguer.Client/        # Blazor WebAssembly Frontend
│   ├── Components/              # Reusable UI components (ProductCard, LoadingSpinner, etc.)
│   ├── Layout/                  # Layout components (MainLayout, NavMenu)
│   ├── Pages/                   # Application pages (Home, Products, Cart, Orders, etc.)
│   ├── Services/                # Client-side services for API communication
│   ├── Models/                  # Client-side data models
│   └── wwwroot/                 # Static files (CSS, images, etc.)
│
├── GoodHamburguer.Domain/        # Domain Layer (Business Logic)
│   ├── Entities/                # Domain entities (Cart, Product, Order, Discount, etc.)
│   ├── Repositories/             # Repository interfaces
│   ├── ValueObjects/            # Value objects (ValidationResult)
│   ├── Configuration/           # Default configurations (quantity rules)
│   ├── Exceptions/              # Domain exceptions
│   └── OrderQuantityValidator.cs # Validator for quantity rules
│
└── GoodHamburguer.Domain.Tests/  # Unit Tests
    ├── Entities/                 # Entity tests
    ├── Configuration/            # Configuration tests
    └── ValueObjects/             # Value object tests
```

### Project Components

#### GoodHamburguer.Api

The API layer contains:
- **Controllers**: RESTful endpoints for all operations
- **DTOs**: Data transfer objects that shape API responses
- **Repositories**: In-memory implementations of domain repositories
- **Data**: Initial data seeding for products, categories, and discounts

#### GoodHamburguer.Client

The client layer contains:
- **Components**: Reusable Blazor components
- **Pages**: Application pages/routes
- **Services**: HTTP client services for API communication
- **Models**: Client-side data models matching API DTOs

#### GoodHamburguer.Domain

The domain layer contains:
- **Entities**: Core business entities with business logic
- **Repositories**: Repository interfaces (contracts)
- **ValueObjects**: Immutable value objects
- **Configuration**: Default system configurations
- **Exceptions**: Domain-specific exceptions

#### GoodHamburguer.Domain.Tests

Comprehensive unit tests for:
- Domain entities
- Business logic
- Validation rules
- Configuration

---

## Português

### Visão Geral

O projeto segue um padrão de arquitetura limpa com separação clara de responsabilidades:

```
GoodHamburguer/
├── GoodHamburguer.Api/          # Backend REST API
│   ├── Controllers/             # Endpoints da API (Carts, Products, Orders, etc.)
│   ├── DTOs/                    # Objetos de Transferência de Dados para respostas da API
│   ├── Repositories/            # Repositórios de dados em memória
│   ├── Data/                    # Seed de dados (produtos, categorias, descontos iniciais)
│   └── Program.cs               # Inicialização e configuração da API
│
├── GoodHamburguer.Client/        # Frontend Blazor WebAssembly
│   ├── Components/              # Componentes UI reutilizáveis (ProductCard, LoadingSpinner, etc.)
│   ├── Layout/                  # Componentes de layout (MainLayout, NavMenu)
│   ├── Pages/                   # Páginas da aplicação (Home, Products, Cart, Orders, etc.)
│   ├── Services/                # Serviços do lado do cliente para comunicação com a API
│   ├── Models/                  # Modelos de dados do lado do cliente
│   └── wwwroot/                 # Arquivos estáticos (CSS, imagens, etc.)
│
├── GoodHamburguer.Domain/        # Camada de Domínio (Lógica de Negócio)
│   ├── Entities/                # Entidades de domínio (Cart, Product, Order, Discount, etc.)
│   ├── Repositories/            # Interfaces de repositório
│   ├── ValueObjects/            # Objetos de valor (ValidationResult)
│   ├── Configuration/           # Configurações padrão (regras de quantidade)
│   ├── Exceptions/              # Exceções de domínio
│   └── OrderQuantityValidator.cs # Validador de regras de quantidade
│
└── GoodHamburguer.Domain.Tests/  # Testes Unitários
    ├── Entities/                 # Testes de entidades
    ├── Configuration/            # Testes de configuração
    └── ValueObjects/             # Testes de objetos de valor
```

### Componentes do Projeto

#### GoodHamburguer.Api

A camada de API contém:
- **Controllers**: Endpoints RESTful para todas as operações
- **DTOs**: Objetos de transferência de dados que formatam as respostas da API
- **Repositories**: Implementações em memória dos repositórios de domínio
- **Data**: Seed inicial de dados para produtos, categorias e descontos

#### GoodHamburguer.Client

A camada de cliente contém:
- **Components**: Componentes Blazor reutilizáveis
- **Pages**: Páginas/rotas da aplicação
- **Services**: Serviços de cliente HTTP para comunicação com a API
- **Models**: Modelos de dados do lado do cliente que correspondem aos DTOs da API

#### GoodHamburguer.Domain

A camada de domínio contém:
- **Entities**: Entidades de negócio principais com lógica de negócio
- **Repositories**: Interfaces de repositório (contratos)
- **ValueObjects**: Objetos de valor imutáveis
- **Configuration**: Configurações padrão do sistema
- **Exceptions**: Exceções específicas do domínio

#### GoodHamburguer.Domain.Tests

Testes unitários abrangentes para:
- Entidades de domínio
- Lógica de negócio
- Regras de validação
- Configuração

