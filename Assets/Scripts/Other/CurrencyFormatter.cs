public static class CurrencyFormatter
{
    public static string FormatCurrency(float value)
    {
        string formattedValue;

        if (value >= 1000000) // 1 million or more
        {
            formattedValue = string.Format("${0}M", value / 1000000f);
        }
        else if (value >= 1000) // 1 thousand or more
        {
            formattedValue = string.Format("${0}K", value / 1000f);
        }
        else // Less than 1 thousand
        {
            formattedValue = string.Format("${0}", value);
        }

        return formattedValue;
    }
}
