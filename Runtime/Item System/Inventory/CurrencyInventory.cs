namespace SF.ItemModule
{
    /// <summary>
    /// An <see cref="ItemStack"/> used to keep track of a type of currency.
    /// </summary>
    [System.Serializable]
    public class CurrencyData : ItemStack
    {
        public CurrencyData()
        {
            ItemSubType = ItemSubType.Currency;
        }
    }
}
