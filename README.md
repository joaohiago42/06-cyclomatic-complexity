# 1.6 Minimize Cyclomatic Complexity (Minimizar Complexidade Ciclomática)

> **Fase 1 — Escrevendo Código Limpo** | Roadmap: Software Design & Architecture

## O Conceito

Complexidade ciclomática é o **número de caminhos possíveis** que o código pode seguir. Cada `if`, `else if`, `case`, `&&`, `||`, `for`, `while`, `catch` e `?:` adiciona um caminho.

**Analogia:** Imagine dar instruções para chegar a um lugar. "Siga reto e vire à direita" — 1 caminho, fácil. "Se chover, pegue a A; senão a B; se a B estiver em obras, vá pela C; mas se for horário de pico na C, volte pela D, exceto em feriado..." — impossível seguir de cabeça. **Complexidade alta é um GPS com 15 condições antes de cada curva.**

## Como Contar

Comece em **1** e some **+1** para cada decisão:

```csharp
public decimal Calculate(Order order)        // 1 (base)
{
    if (order == null)                        // +1 = 2
        return 0;

    if (order.IsPremium && order.Total > 100) // +2 = 4 (if + &&)
        return order.Total * 0.80m;

    foreach (var item in order.Items)         // +1 = 5
    {
        if (item.IsOnSale || item.IsNew)      // +2 = 7 (if + ||)
            // ...
    }

    return order.Total;
}
// Complexidade = 7
```

**Atenção:** `&&` e `||` contam porque criam branches (short-circuit evaluation).

## Escala de Complexidade

| Valor | Avaliação | Testes necessários |
|-------|-----------|-------------------|
| 1-5 | Simples, fácil de entender | Poucos |
| 6-10 | Moderada, precisa de atenção | Vários |
| 11-20 | Alta, difícil de manter | Muitos |
| 20+ | Crítica, refatorar urgente | Impossível cobrir tudo |

## Técnicas para Reduzir

### 1. Composição de regras — quebrar if/else chain

```csharp
// RUIM: complexidade 15 num método só
public decimal CalculateDiscount(Order order, Customer customer)
{
    if (customer.IsPremium && order.Total > 500) return 0.20m;
    else if (customer.IsPremium && order.Total > 200) return 0.15m;
    else if (customer.IsPremium) return 0.10m;
    // ... mais 10 condições ...
}

// BOM: cada regra isolada com complexidade 2-3
var discount =
    DiscountRules.ByCustomerTier(customer.IsPremium, order.Total) +
    DiscountRules.ByVolume(order.ItemCount) +
    DiscountRules.ByFirstPurchase(customer.IsFirstPurchase) +
    DiscountRules.ByCoupon(order.CouponCode);
```

### 2. Dictionary pattern — eliminar switch gigante

```csharp
// RUIM: switch com 20+ cases (complexidade 25)
switch (messageType)
{
    case 1: return TranslateState(payload);
    case 2: return TranslateKeepAlive(payload);
    // ... 18 cases mais ...
}

// BOM: dicionário (complexidade 2)
if (!_handlers.TryGetValue(messageType, out var handler))
    throw new NotSupportedException(...);
return handler.Translate(payload);
```

### 3. Pattern matching (C# moderno)

```csharp
// RUIM: if/else chain
if (isPremium && total > 500) discount = 0.20m;
else if (isPremium && total > 200) discount = 0.15m;
// ...

// BOM: tuple pattern matching — mesma lógica, mais legível
return (isPremium, orderTotal) switch
{
    (true, > 500) => 0.20m,
    (true, > 200) => 0.15m,
    (true, _)     => 0.10m,
    (_, > 500)    => 0.08m,
    (_, > 200)    => 0.05m,
    _             => 0m
};
```

### 4. Guard clauses — achatar condições aninhadas

```csharp
// RUIM: nesting com complexidade acumulada
if (order != null)
{
    if (order.IsValid)
    {
        if (order.HasItems)
        {
            // lógica real aqui, 3 níveis dentro
        }
    }
}

// BOM: guard clauses (mesma complexidade, mas linear)
if (order == null) return;
if (!order.IsValid) return;
if (!order.HasItems) return;
// lógica real aqui, nível 0
```

## Estrutura do Projeto

```text
06-cyclomatic-complexity/
  HighComplexity/
    OrderDiscountService.cs  # If/else chain com complexidade ~15
    MessageTranslator.cs     # Switch com 20+ cases
  LowComplexity/
    OrderDiscountService.cs  # Regras compostas, complexidade 2-3 cada
    MessageTranslator.cs     # Dictionary pattern, complexidade 2
  README.md
```

## Exemplos Reais do Código de Trabalho (Aiko)

### Ruim: Switch com 30+ patterns

`DebugController.cs` — método `Translate()` com ~30 pattern matches e ~189 linhas:

```csharp
var result = lowDataMessage.MessageType switch
{
    (int)MessageType.State1
    or (int)MessageType.State2
    or (int)MessageType.State1WithObservation
    // ... 8+ "or" patterns ...
        => new StateLowDataMessageTranslator(...).Translate(),
    // ... 20+ cases mais ...
};
```

### Bom: Dictionary pattern em produção

`EquipmentMessageProcessor.cs` — 71 tipos de mensagem roteados via Dictionary:

```csharp
this.methods = new Dictionary<int, Func<long, EquipmentMessageEnvelope, Task>>();
this.methods.Add(EquipmentMessageType.Login, this.ProcessLoginMessageAsync);
this.methods.Add(EquipmentMessageType.Logout, this.ProcessLogoutMessageAsync);
// ... 69 entradas mais ...
```

O método de dispatch tem **complexidade 2** — independente de quantos tipos existam.

### Bom: Guard clauses + early return

`ProductionByStateService.cs` — `GetProductionsEventSource()`:

```csharp
if (productions.Any(p => p.Source == EventSource.Contingency))
    return EventSource.Contingency;
if (productions.Any(p => p.Source == EventSource.Manual))
    return EventSource.Manual;
if (productions.Any(p => p.Source == EventSource.Automatic))
    return EventSource.Automatic;
```

Linear, sem nesting, cada condição independente.

## Regra de Ouro

> **Se você precisa de mais de 10 testes para cobrir um método, ele é complexo demais.**
>
> Quebre em métodos menores. Cada um com complexidade ≤ 5.
> A complexidade total não desaparece, mas fica **distribuída e gerenciável**.

## Checklist

- [ ] Métodos têm complexidade ciclomática ≤ 10?
- [ ] Switches com 5+ cases foram substituídos por dictionary/strategy?
- [ ] If/else chains foram quebradas em funções compostas?
- [ ] `&&` e `||` foram simplificados ou extraídos para variáveis booleanas?
- [ ] Condições aninhadas usam guard clauses?
- [ ] Cada método pode ser testado com 5 testes ou menos?

## Referência

- Clean Code (Robert C. Martin) — Cap. 3: Functions
- [Cyclomatic Complexity — Wikipedia](https://en.wikipedia.org/wiki/Cyclomatic_complexity)
- [Roadmap.sh — Clean Code](https://roadmap.sh/software-design-architecture)
