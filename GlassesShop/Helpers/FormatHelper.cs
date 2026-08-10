namespace GlassesShop.Helpers
{
    public static class FormatHelper
    {
        public static string ToVnd(this decimal value)
        {
            return value.ToString("#,##0").Replace(",", ".") + " ₫";
        }

        public static string ToVnd(this decimal? value)
        {
            return value.HasValue ? value.Value.ToVnd() : "Liên hệ";
        }
    }
}