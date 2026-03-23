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
            if (cards == null || !cards.Any())
            {
                return new DeckAnalysisDto();
            }

            var dto = new DeckAnalysisDto();

            // Summary
            dto.Summary.Commander = cards[0];
            cards.RemoveAt(0);
            dto.Summary.TotalCards = cards.Count;
            // dto.Summary.ManaCurvePreview =

            /***** MANA CURVE *****/
            // Metrics
            dto.ManaCurveAnalysis.Metrics.AverageCmc = CalculateAverageCmc(cards);
            // dto.ManaCurveAnalysis.Metrics.MedianCmc =
            // dto.ManaCurveAnalysis.Metrics.CurvePeak =
            // dto.ManaCurveAnalysis.Metrics.EarlyGameDensity =

            // Chart Distributions
            dto.ManaCurveAnalysis.Charts.ByCmc = BuildManaCurveByCmc(cards);
            dto.ManaCurveAnalysis.Charts.ByColor = BuildManaCurveByColor(cards);
            dto.ManaCurveAnalysis.Charts.ByType = BuildManaCurveByType(cards);
            dto.ManaCurveAnalysis.Charts.ByCreatureSplit = BuildManaCurveByCreatureSplit(cards);

            // Insights

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

        private ManaCurveChartDto BuildManaCurveBreakdown(
            List<CardDto> cards,
            List<string> orderedBuckets,
            Func<CardDto, string> bucketSelector)
        {
            var filteredCards = cards
                .Where(c => c.ConvertedManaCost.HasValue && !c.TypeLine.Contains("Land"))
                .ToList();

            var cmcCategories = filteredCards
                .Select(c => (int)Math.Floor(c.ConvertedManaCost!.Value))
                .Distinct()
                .OrderBy(cmc => cmc)
                .ToList();

            var series = orderedBuckets
                .Select(bucket => new StackedSeriesDto
                {
                    Name = bucket,
                    Data = cmcCategories
                        .Select(cmc => filteredCards.Count(c =>
                            (int)Math.Floor(c.ConvertedManaCost!.Value) == cmc &&
                            bucketSelector(c) == bucket))
                        .ToList()
                })
                .Where(s => s.Data.Any(value => value > 0))
                .ToList();

            return new ManaCurveChartDto
            {
                Categories = cmcCategories,
                Series = series
            };
        }

        private ManaCurveChartDto BuildManaCurveByColor(List<CardDto> cards)
        {
            return BuildManaCurveBreakdown(
                cards,
                new List<string>
                {
                "White",
                "Blue",
                "Black",
                "Red",
                "Green",
                "Colorless",
                "Multicolor"
                },
                GetColorBucket);
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

        private ManaCurveChartDto BuildManaCurveByType(List<CardDto> cards)
        {
            return BuildManaCurveBreakdown(
                cards,
               new List<string>
                {
                    "Creature",
                    "Instant",
                    "Sorcery",
                    "Artifact",
                    "Battle",
                    "Planeswalker",
                    "Enchantment",
                    "Other"
                },
                GetPrimaryTypeBucket);
        }

        private string GetPrimaryTypeBucket(CardDto card)
        {
            var typeLine = card.TypeLine;

            if (typeLine.Contains("Creature")) return "Creature";
            if (typeLine.Contains("Instant")) return "Instant";
            if (typeLine.Contains("Sorcery")) return "Sorcery";
            if (typeLine.Contains("Enchantment")) return "Enchantment";
            if (typeLine.Contains("Artifact")) return "Artifact";
            if (typeLine.Contains("Planeswalker")) return "Planeswalker";
            if (typeLine.Contains("Battle")) return "Battle";

            return "Other";
        }

        private ManaCurveChartDto BuildManaCurveByCreatureSplit(List<CardDto> cards)
        {
            return BuildManaCurveBreakdown(
                cards,
                new List<string>
                {
                "Creature",
                "Non-Creature"
                },
                GetCreatureSplitBucket);
        }

        private string GetCreatureSplitBucket(CardDto card)
        {
            return card.TypeLine.Contains("Creature")
                ? "Creature"
                : "Non-Creature";
        }

        private ManaCurveChartDto BuildManaCurveByCmc(List<CardDto> cards)
        {
            var filteredCards = cards
                .Where(c => c.ConvertedManaCost.HasValue && !c.TypeLine.Contains("Land"))
                .ToList();

            var cmcCategories = filteredCards
                .Select(c => (int)Math.Floor(c.ConvertedManaCost!.Value))
                .Distinct()
                .OrderBy(cmc => cmc)
                .ToList();

            return new ManaCurveChartDto
            {
                Categories = cmcCategories,
                Series =
                [
                    new StackedSeriesDto
                {
                    Name = "Cards",
                    Data = cmcCategories
                        .Select(cmc => filteredCards.Count(c =>
                            (int)Math.Floor(c.ConvertedManaCost!.Value) == cmc))
                        .ToList()
                }
                ]
            };
        }
    }
}
