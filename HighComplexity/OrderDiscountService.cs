// ============================================================
// EXEMPLO RUIM: Método com complexidade ciclomática alta (~15)
// ============================================================
// Cada if/else if/&& adiciona um caminho. Este método tem 15 caminhos
// possíveis — significa que precisa de pelo menos 15 testes para cobertura total.

namespace CyclomaticComplexity.HighComplexity;

public class OrderDiscountService
{
    // Complexidade: ~15
    public decimal CalculateDiscount(Order order, Customer customer)
    {
        decimal discount = 0;

        if (customer == null)                                          // +1
            return 0;

        if (customer.IsPremium && order.Total > 500)                   // +2 (if + &&)
        {
            discount = 0.20m;
        }
        else if (customer.IsPremium && order.Total > 200)              // +2
        {
            discount = 0.15m;
        }
        else if (customer.IsPremium)                                   // +1
        {
            discount = 0.10m;
        }
        else if (order.Total > 500)                                    // +1
        {
            discount = 0.08m;
        }
        else if (order.Total > 200)                                    // +1
        {
            discount = 0.05m;
        }

        // Mais condições empilhadas...
        if (order.ItemCount >= 10)                                     // +1
            discount += 0.03m;

        if (customer.IsFirstPurchase)                                  // +1
            discount += 0.05m;

        if (order.CouponCode != null)                                  // +1
        {
            if (order.CouponCode == "SUMMER2026")                      // +1
                discount += 0.10m;
            else if (order.CouponCode == "WELCOME")                    // +1
                discount += 0.15m;
            else if (order.CouponCode.StartsWith("VIP"))               // +1
                discount += 0.20m;
        }

        if (discount > 0.50m)                                         // +1
            discount = 0.50m;

        return discount;
    }
}

// Problemas:
// - 15 caminhos: precisa de no mínimo 15 testes para cobrir tudo
// - Fácil de esquecer um cenário (ex: premium + first purchase + cupom VIP)
// - Impossível entender "o desconto final" sem simular mentalmente todas as condições
// - Qualquer regra nova adiciona mais if/else no meio do bolo
// - Condições com && aumentam a complexidade sem parecer (cada && é +1)
