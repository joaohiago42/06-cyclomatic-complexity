// ============================================================
// EXEMPLO BOM: Complexidade reduzida com composição de regras
// ============================================================
// Ao invés de um método com 15 caminhos, cada regra é isolada.
// O método orquestrador só soma os resultados.

namespace CyclomaticComplexity.LowComplexity;

// Cada regra de desconto é uma função pura com complexidade 1-2
public static class DiscountRules
{
    // Complexidade: 3 (switch expression com 3 branches)
    public static decimal ByCustomerTier(bool isPremium, decimal orderTotal)
    {
        return (isPremium, orderTotal) switch
        {
            (true, > 500) => 0.20m,
            (true, > 200) => 0.15m,
            (true, _)     => 0.10m,
            (_, > 500)    => 0.08m,
            (_, > 200)    => 0.05m,
            _             => 0m
        };
    }

    // Complexidade: 2
    public static decimal ByVolume(int itemCount)
    {
        return itemCount >= 10 ? 0.03m : 0m;
    }

    // Complexidade: 2
    public static decimal ByFirstPurchase(bool isFirstPurchase)
    {
        return isFirstPurchase ? 0.05m : 0m;
    }

    // Complexidade: 2 (switch expression)
    public static decimal ByCoupon(string? couponCode)
    {
        return couponCode switch
        {
            "SUMMER2026"                        => 0.10m,
            "WELCOME"                           => 0.15m,
            _ when couponCode?.StartsWith("VIP") == true => 0.20m,
            _                                   => 0m
        };
    }

    // Complexidade: 2
    public static decimal Cap(decimal discount, decimal maxDiscount = 0.50m)
    {
        return Math.Min(discount, maxDiscount);
    }
}

// O orquestrador: complexidade 2 (null check + soma)
public class OrderDiscountService
{
    public decimal CalculateDiscount(Order order, Customer? customer)
    {
        if (customer == null)
            return 0;

        var discount =
            DiscountRules.ByCustomerTier(customer.IsPremium, order.Total) +
            DiscountRules.ByVolume(order.ItemCount) +
            DiscountRules.ByFirstPurchase(customer.IsFirstPurchase) +
            DiscountRules.ByCoupon(order.CouponCode);

        return DiscountRules.Cap(discount);
    }
}

// Comparação de complexidade:
//
// RUIM: 1 método com complexidade 15
// BOM:  6 métodos com complexidade 2-3 cada
//
// Cada regra pode ser testada isoladamente:
//   Assert.Equal(0.20m, DiscountRules.ByCustomerTier(true, 600));
//   Assert.Equal(0.03m, DiscountRules.ByVolume(15));
//   Assert.Equal(0.15m, DiscountRules.ByCoupon("WELCOME"));
//
// Adicionar uma regra nova? Crie um novo método e some no orquestrador.
// Sem tocar nas regras existentes (Open/Closed Principle).
