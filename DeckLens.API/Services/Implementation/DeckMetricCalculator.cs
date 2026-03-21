using DeckLens.API.Models.DTO;
using DeckLens.API.Services.Interface;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;

namespace DeckLens.API.Services.Implementation
{
    public class DeckMetricCalculator : IDeckMetricCalculator
    {
        public DeckAnalysisDto Build(List<CardDto> cards)
        {
            var dto = new DeckAnalysisDto();

            dto.Commander = cards[0];
            cards.RemoveAt(0);
            dto.TotalCards = cards.Count;
            dto.Summary.ManaCurvePreview = BuildManaCurve(cards);


            dto.ManaCurveAnalysis.AverageCmc = CalculateAverageCmc(cards);
            dto.ManaCurveAnalysis.ByCmc = BuildManaCurve(cards);

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

        private ManaCurveChartDto BuildManaCurveBreakdown(List<CardDto> cards, Func<CardDto, string> bucketSelector)
        {
            var filteredCards = cards
                .Where(c => c.ConvertedManaCost.HasValue && !c.TypeLine.Contains("Land"))
                .ToList();

            var cmcCategories = filteredCards
                .Select(c => (int)Math.Floor(c.ConvertedManaCost!.Value))
                .Distinct()
                .OrderBy(cmc => cmc)
                .ToList();

            var bucketNames = filteredCards
                .Select(bucketSelector)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var series = bucketNames
                .Select(bucket => new StackedSeriesDto
                {
                    Name = bucket,
                    Data = cmcCategories
                        .Select(cmc => filteredCards.Count(c =>
                            (int)Math.Floor(c.ConvertedManaCost!.Value) == cmc &&
                            bucketSelector(c) == bucket))
                        .ToList()
                })
                .ToList();

            return new ManaCurveChartDto
            {
                Categories = cmcCategories,
                Series = series
            };
        }

        private ManaCurveChartDto BuildManaCurveByColor(List<CardDto> cards)
        {
            return BuildManaCurveBreakdown(cards, GetColorBucket);
        }

        private string GetColorBucket(CardDto card)
        {
            if (card.Colors == null || !card.Colors.Any())
                return "Colorless";

            if (card.Colors.Count > 1)
                return "Multicolor";

            return card.Colors[0] switch
            {
                "W" => "White",
                "U" => "Blue",
                "B" => "Black",
                "R" => "Red",
                "G" => "Green",
                _ => "Colorless"
            };
        }

        private Dictionary<string, int> BuildColorDistribution(List<CardDto> cards)
        {
            var result = new Dictionary<string, int>();

            foreach (var card in cards) 
            {
                if (card.TypeLine.Contains("Land"))
                {
                    continue;
                }
                else if (card.Colors.Count == 0)
                {
                    if (!result.ContainsKey("C"))
                        result["C"] = 0;

                    result["C"]++;
                }
                else
                {
                    foreach (var color in card.Colors)
                    {
                        if (!result.ContainsKey(color))
                            result[color] = 0;

                        result[color]++;
                    }
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
