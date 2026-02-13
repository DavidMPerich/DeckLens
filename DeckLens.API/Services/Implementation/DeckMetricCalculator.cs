using DeckLens.API.Models.DTO;
using DeckLens.API.Services.Interface;
using System.Reflection.Metadata.Ecma335;

namespace DeckLens.API.Services.Implementation
{
    public class DeckMetricCalculator : IDeckMetricCalculator
    {
        public DeckAnalysisDto Build(List<CardDto> cards)
        {
            var dto = new DeckAnalysisDto();

            dto.Commander = cards[0];
            dto.TotalCards = cards.Count;
            dto.AverageCmc = CalculateAverageCmc(cards);
            dto.ManaCurve = BuildManaCurve(cards);
            dto.ColorDistribution = BuildColorDistribution(cards);
            dto.CardTypeBreakdown = BuildTypeBreakdown(cards);

            return dto;
        }

        private double CalculateAverageCmc(List<CardDto> cards) 
        {
            var cmcList = cards
                .Where(c => c.ConvertedManaCost.HasValue && !c.TypeLine.Contains("Land"))
                .Select(c => c.ConvertedManaCost!.Value);

            var average = cmcList.DefaultIfEmpty(0).Average();
            return Math.Round(average, 1);
        }

        private Dictionary<int, int> BuildManaCurve(List<CardDto> cards) 
        {
            return cards
                .Where(c => c.ConvertedManaCost.HasValue && !c.TypeLine.Contains("Land"))
                .GroupBy(c => (int)Math.Floor(c.ConvertedManaCost!.Value))
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private Dictionary<string, int> BuildColorDistribution(List<CardDto> cards)
        {
            var result = new Dictionary<string, int>();

            for (int i = 1; i < cards.Count; i++)
            {
                var card = cards[i];

                if (card.TypeLine.Contains("Land"))
                {
                    continue;
                }

                foreach (var color in card.Colors)
                {
                    if (!result.ContainsKey(color))
                        result[color] = 0;

                    result[color]++;
                }
            }

            return result;
        }

        private Dictionary<string, int> BuildTypeBreakdown(List<CardDto> cards)
        {
            var types = new Dictionary<string, int>();

            foreach (var card in cards)
            {
                if (card.TypeLine.Contains("Creature"))
                    Increment(types, "Creature");

                if (card.TypeLine.Contains("Instant"))
                    Increment(types, "Instant");

                if (card.TypeLine.Contains("Sorcery"))
                    Increment(types, "Sorcery");

                if (card.TypeLine.Contains("Artifact"))
                    Increment(types, "Artifact");

                if (card.TypeLine.Contains("Enchantment"))
                    Increment(types, "Enchantment");

                if (card.TypeLine.Contains("Land"))
                    Increment(types, "Land");
            }

            return types;
        }

        private void Increment(Dictionary<string, int> dict, string key)
        {
            if (!dict.ContainsKey(key))
                dict[key] = 0;

            dict[key]++;
        }
    }
}
