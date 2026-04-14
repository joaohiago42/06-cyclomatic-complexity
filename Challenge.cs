// ============================================================
// DESAFIO 1.6 — Minimize Cyclomatic Complexity
// ============================================================
// Este método tem complexidade ciclomática ~18.
// Sua missão: refatore para que nenhum método tenha complexidade > 5.
//
// Técnicas disponíveis:
// - Composição de regras (cada regra em seu próprio método)
// - Dictionary/strategy pattern para substituir switch
// - Pattern matching com tuple switch
// - Guard clauses para achatar condições
//
// Crie o arquivo ChallengeSolved.cs com sua solução.

namespace CyclomaticComplexity.Challenge;

public class ShippingCalculator
{
    // Complexidade: ~18 — sua meta: ≤ 5 por método
    public ShippingQuote CalculateShipping(
        string country,
        string state,
        double weightKg,
        double distanceKm,
        bool isFragile,
        bool isPriority,
        string customerTier)
    {
        decimal baseCost;

        if (country == "BR")
        {
            if (state == "SP" || state == "RJ" || state == "MG")
                baseCost = 15.00m;
            else if (state == "RS" || state == "PR" || state == "SC")
                baseCost = 20.00m;
            else
                baseCost = 30.00m;
        }
        else if (country == "AR" || country == "UY" || country == "PY")
        {
            baseCost = 50.00m;
        }
        else if (country == "US" || country == "CA")
        {
            baseCost = 80.00m;
        }
        else
        {
            baseCost = 120.00m;
        }

        // Taxa por peso
        decimal weightSurcharge;
        if (weightKg <= 1)
            weightSurcharge = 0;
        else if (weightKg <= 5)
            weightSurcharge = 5.00m;
        else if (weightKg <= 20)
            weightSurcharge = 15.00m;
        else
            weightSurcharge = 30.00m + (decimal)(weightKg - 20) * 1.50m;

        // Taxa por distância
        decimal distanceSurcharge;
        if (distanceKm <= 100)
            distanceSurcharge = 0;
        else if (distanceKm <= 500)
            distanceSurcharge = 10.00m;
        else
            distanceSurcharge = 10.00m + (decimal)(distanceKm - 500) * 0.02m;

        var total = baseCost + weightSurcharge + distanceSurcharge;

        if (isFragile)
            total *= 1.25m;

        if (isPriority)
            total *= 1.50m;

        // Desconto por tier
        if (customerTier == "Gold")
            total *= 0.90m;
        else if (customerTier == "Platinum")
            total *= 0.80m;
        else if (customerTier == "Diamond")
            total *= 0.70m;

        // Estimativa de dias
        int estimatedDays;
        if (isPriority && country == "BR")
            estimatedDays = 2;
        else if (country == "BR")
            estimatedDays = 7;
        else if (isPriority)
            estimatedDays = 5;
        else
            estimatedDays = 15;

        return new ShippingQuote
        {
            Cost = Math.Round(total, 2),
            EstimatedDays = estimatedDays,
            IsInternational = country != "BR"
        };
    }
}

public class ShippingQuote
{
    public decimal Cost { get; set; }
    public int EstimatedDays { get; set; }
    public bool IsInternational { get; set; }
}
