using DeckLens.API.Services.Interface;

namespace DeckLens.API.Services.Implementation
{
    public class DeckImportService : IDeckImportService
    {
        public List<string> Parse(string rawInput)
        {
            var cards = rawInput
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line =>  line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            cards.RemoveAt(2);
            cards.RemoveAt(0);
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i] = cards[i].Split("(")[0].Trim();
                var quantity = int.Parse(cards[i].Split(" ")[0].Trim());
                var name = cards[i].Split(" ", 2)[1].Trim();
                if (quantity > 1)
                {
                    cards.RemoveAt(i);
                    for (int j = 0; j < quantity; j++)
                    {
                        cards.Insert(i, $"1 {name}");
                    }
                }
                cards[i] = name;
            }

            return cards;
        }
    }
}
