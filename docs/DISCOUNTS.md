# Discount System / Sistema de Descontos

**Quick Navigation / Navegação Rápida:** [English](#english) | [Português](#português)

---

## English

### Overview

The discount system is highly flexible and allows you to create discounts with multiple conditions. A discount is applied when **all** its conditions are met.

### How It Works

1. **Discount Creation**: Each discount has:
   - A name (e.g., "Complete Combo")
   - A percentage (e.g., 20%)
   - One or more conditions

2. **Discount Conditions**: Conditions can be based on:
   - **Product**: Minimum quantity of a specific product (e.g., "1 Soft Drink")
   - **Category**: Minimum quantity of products from a category (e.g., "1 Sandwich")
   - **Product + Category**: Both a specific product and category requirement

3. **Discount Application**:
   - The system evaluates all active discounts
   - Finds discounts where **all conditions are satisfied**
   - Selects the discount with the **highest percentage**
   - Applies it to the cart total

### Example Discounts (Pre-configured)

- **Complete Combo (20% off)**: Requires 1 Sandwich + 1 Fries + 1 Soft Drink
- **Drink Combo (15% off)**: Requires 1 Sandwich + 1 Soft Drink
- **Fries Combo (10% off)**: Requires 1 Sandwich + 1 Fries

### Example Scenario

If your cart contains:
- 1 X Burger (Sandwich)
- 1 Fries
- 1 Soft Drink

The system will apply the **Complete Combo (20% off)** discount because all three conditions are met.

### Technical Details

- Discounts are evaluated in real-time when the cart is updated
- Only one discount can be applied at a time (the one with the highest percentage)
- Discounts can be activated or deactivated without affecting existing orders
- Conditions are evaluated using AND logic (all conditions must be met)

---

## Português

### Visão Geral

O sistema de descontos é altamente flexível e permite criar descontos com múltiplas condições. Um desconto é aplicado quando **todas** as suas condições são atendidas.

### Como Funciona

1. **Criação de Desconto**: Cada desconto possui:
   - Um nome (ex: "Combo Completo")
   - Uma porcentagem (ex: 20%)
   - Uma ou mais condições

2. **Condições de Desconto**: As condições podem ser baseadas em:
   - **Produto**: Quantidade mínima de um produto específico (ex: "1 Refrigerante")
   - **Categoria**: Quantidade mínima de produtos de uma categoria (ex: "1 Sanduíche")
   - **Produto + Categoria**: Requisito de um produto específico e categoria

3. **Aplicação de Desconto**:
   - O sistema avalia todos os descontos ativos
   - Encontra descontos onde **todas as condições são satisfeitas**
   - Seleciona o desconto com a **maior porcentagem**
   - Aplica ao total do carrinho

### Exemplos de Descontos (Pré-configurados)

- **Combo Completo (20% off)**: Requer 1 Sanduíche + 1 Batata Frita + 1 Refrigerante
- **Combo Bebida (15% off)**: Requer 1 Sanduíche + 1 Refrigerante
- **Combo Batata (10% off)**: Requer 1 Sanduíche + 1 Batata Frita

### Exemplo de Cenário

Se seu carrinho contém:
- 1 X Burger (Sanduíche)
- 1 Batata Frita
- 1 Refrigerante

O sistema aplicará o desconto **Combo Completo (20% off)** porque todas as três condições são atendidas.

### Detalhes Técnicos

- Os descontos são avaliados em tempo real quando o carrinho é atualizado
- Apenas um desconto pode ser aplicado por vez (o de maior porcentagem)
- Os descontos podem ser ativados ou desativados sem afetar pedidos existentes
- As condições são avaliadas usando lógica AND (todas as condições devem ser atendidas)

