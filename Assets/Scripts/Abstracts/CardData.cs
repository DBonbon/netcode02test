public class CardData
{
    public string CardName { get; private set; }
    public string Suit { get; private set; }

    public CardData(string cardName, string suit)
    {
        CardName = cardName;
        Suit = suit;
    }
}
