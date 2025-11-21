# Flexible Quantity Rules / Regras de Quantidade Flexíveis

## English

### Overview

The quantity rules system allows you to enforce maximum quantities for products or categories in a single order. This is useful for:
- Limiting promotional items
- Managing inventory constraints
- Preventing abuse of special offers

### How It Works

1. **Rule Creation**: Rules can be created for:
   - **Specific Product**: Maximum quantity of a particular product (e.g., "Max 1 Fries per order")
   - **Category**: Maximum quantity of all products in a category (e.g., "Max 1 Sandwich per order")

2. **Rule Validation**:
   - Rules are validated when adding items to the cart
   - Rules are validated when updating item quantities
   - If a rule is violated, the operation is rejected with a clear error message

3. **Default Rules** (Pre-configured):
   - **Sandwiches**: Maximum 1 per order
   - **Fries**: Maximum 1 per order
   - **Soft Drink**: Maximum 1 per order

### Example Scenario

If you try to add 2 sandwiches to your cart, the system will reject it with:
> "You already have sandwich in your cart. Only 1 per order is allowed."

### Technical Details

- Rules are evaluated before items are added to the cart
- Rules apply to the total quantity of matching items (not individual items)
- Multiple rules can be active simultaneously
- Rules can be created, updated, or deleted via the API
- Rules are validated both in the cart and when creating orders

---

## Português

### Visão Geral

O sistema de regras de quantidade permite impor quantidades máximas para produtos ou categorias em um único pedido. Isso é útil para:
- Limitar itens promocionais
- Gerenciar restrições de estoque
- Prevenir abuso de ofertas especiais

### Como Funciona

1. **Criação de Regras**: Regras podem ser criadas para:
   - **Produto Específico**: Quantidade máxima de um produto particular (ex: "Máx 1 Batata Frita por pedido")
   - **Categoria**: Quantidade máxima de todos os produtos de uma categoria (ex: "Máx 1 Sanduíche por pedido")

2. **Validação de Regras**:
   - Regras são validadas ao adicionar itens ao carrinho
   - Regras são validadas ao atualizar quantidades de itens
   - Se uma regra for violada, a operação é rejeitada com uma mensagem de erro clara

3. **Regras Padrão** (Pré-configuradas):
   - **Sanduíches**: Máximo 1 por pedido
   - **Batata Frita**: Máximo 1 por pedido
   - **Refrigerante**: Máximo 1 por pedido

### Exemplo de Cenário

Se você tentar adicionar 2 sanduíches ao seu carrinho, o sistema rejeitará com:
> "You already have sandwich in your cart. Only 1 per order is allowed."

### Detalhes Técnicos

- As regras são avaliadas antes dos itens serem adicionados ao carrinho
- As regras se aplicam à quantidade total de itens correspondentes (não itens individuais)
- Múltiplas regras podem estar ativas simultaneamente
- As regras podem ser criadas, atualizadas ou excluídas via API
- As regras são validadas tanto no carrinho quanto ao criar pedidos

