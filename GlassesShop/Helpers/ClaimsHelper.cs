using System.Security.Claims;

namespace GlassesShop.Helpers
{
    public static class ClaimsHelper
    {
        public const string ClaimAccountId = "AccountID";
        public const string ClaimCustomerId = "CustomerID";
        public const string ClaimStaffId = "StaffID";
        public const string ClaimFullName = "FullName";

        public static int GetAccountId(this ClaimsPrincipal user)
        {
            var value = user.FindFirst(ClaimAccountId)?.Value;
            return int.TryParse(value, out var id) ? id : 0;
        }

        public static int GetCustomerId(this ClaimsPrincipal user)
        {
            var value = user.FindFirst(ClaimCustomerId)?.Value;
            return int.TryParse(value, out var id) ? id : 0;
        }

        public static int GetStaffId(this ClaimsPrincipal user)
        {
            var value = user.FindFirst(ClaimStaffId)?.Value;
            return int.TryParse(value, out var id) ? id : 0;
        }

        public static string GetFullName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimFullName)?.Value ?? "Người dùng";
        }

        public static string GetRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        }
    }
}