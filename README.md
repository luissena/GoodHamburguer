# GoodHamburguer 🍔

A modern e-commerce application for a hamburger restaurant built with .NET 10.0, featuring a RESTful API and a Blazor WebAssembly client.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Quick Start](#quick-start)
- [Documentation](#documentation)
- [Technologies](#technologies)

## Overview

GoodHamburguer is a full-stack application for managing a hamburger restaurant's e-commerce operations. It consists of:

- **API**: RESTful Web API built with ASP.NET Core 10.0
- **Client**: Blazor WebAssembly frontend application
- **Domain**: Domain-driven design layer with business logic
- **Tests**: Unit tests for domain logic

## Features

- 🛒 **Shopping Cart Management**: Add, update, and remove items
- 📦 **Product Catalog**: Browse products by category (Sandwiches, Extras)
- 💰 **Dynamic Discount System**: Automatic discount application based on cart contents
- ⚖️ **Flexible Quantity Rules**: Configurable quantity limitations per product or category
- 📝 **Order Management**: Create and view orders
- 📊 **Swagger Documentation**: Interactive API documentation

## Quick Start

### Using Docker (Recommended)

```bash
docker-compose up --build
```

Access:
- **API & Swagger**: http://localhost:5010
- **Client**: http://localhost:5218

### Running Locally

1. **Run the API**:
   ```bash
   cd GoodHamburguer.Api
   dotnet run
   ```

2. **Run the Client** (in a new terminal):
   ```bash
   cd GoodHamburguer.Client
   dotnet run
   ```

For detailed instructions, see [Running the Project](docs/RUNNING.md).

## Documentation

📚 **[Full Documentation Index](docs/INDEX.md)** - Complete documentation index

### English

- [Project Structure](docs/PROJECT-STRUCTURE.md#english) - Detailed project organization
- [Discount System](docs/DISCOUNTS.md#english) - How the flexible discount system works
- [Quantity Rules](docs/QUANTITY-RULES.md#english) - Configurable quantity limitations
- [Running the Project](docs/RUNNING.md#english) - Setup and execution instructions
- [API Documentation](docs/API-DOCUMENTATION.md#english) - Swagger and endpoint details

### Português

- [Documentação Completa em Português](docs/README-PT.md) - Versão completa em português
- [Estrutura do Projeto](docs/PROJECT-STRUCTURE.md#português) - Organização detalhada do projeto
- [Sistema de Descontos](docs/DISCOUNTS.md#português) - Como funciona o sistema flexível de descontos
- [Regras de Quantidade](docs/QUANTITY-RULES.md#português) - Limitações de quantidade configuráveis
- [Como Executar o Projeto](docs/RUNNING.md#português) - Instruções de configuração e execução
- [Documentação da API](docs/API-DOCUMENTATION.md#português) - Swagger e detalhes dos endpoints

## Technologies

- **.NET 10.0**: Latest .NET framework
- **ASP.NET Core**: Web API framework
- **Blazor WebAssembly**: Client-side web framework
- **Swagger/OpenAPI**: API documentation
- **Docker**: Containerization
- **Domain-Driven Design**: Clean architecture principles

## License

This project is open source and available under the MIT License.
