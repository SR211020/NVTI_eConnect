using eConnectV2.Helpers;
using eConnectV2.Models;
using Microsoft.AspNetCore.Html;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace eConnectV2.Extensions
{
    public static class IdentityExtensions
    {
        [DebuggerStepThrough]
        private static bool HasRole(this ClaimsPrincipal principal, params string[] roles)
        {
            if (principal == null)
                return default;

            var claims = principal.FindAll(ClaimTypes.Role).Select(x => x.Value).ToSafeList();

            return claims?.Any() == true && claims.Intersect(roles ?? System.Array.Empty<string>()).Any();
        }
        [DebuggerStepThrough]
        public static IEnumerable<ListItem> AuthorizeFor(this IEnumerable<ListItem> source, ClaimsPrincipal identity)
            => source.Where(x => x.Roles.IsNullOrEmpty() || (x.Roles.HasItems() && identity.HasRole(x.Roles))).ToSafeList();
        [DebuggerStepThrough]
        public static HtmlString AsRaw(this string value) => new(value);
        [DebuggerStepThrough]
        public static string ToPage(this string href) => System.IO.Path.GetFileNameWithoutExtension(href)?.ToLower();
        [DebuggerStepThrough]
        public static bool IsVoid(this string href) => href?.ToLower() == NavigationModel.Void;
        [DebuggerStepThrough]
        public static bool IsRelatedTo(this ListItem item, string pageName) => item?.Type == ItemType.Parent && item?.Href?.ToPage() == pageName?.ToLower();

        public static string GetPlant(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity.FindFirst(Common.SK_Plant);
            return claim.Value;
        }
        public static string GetADId(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity.FindFirst(Common.SK_ADId);
            return claim.Value;
        }
        public static string GetEmpCode(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity.FindFirst(Common.SK_EmpCode);
            return claim.Value;
        }
        public static string GetEmpName(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity.FindFirst(Common.SK_EmpName);
            return claim.Value;
        }
        public static string GetEmailId(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity.FindFirst(Common.SK_EmailId);
            return claim.Value;
        }
        public static string GetDesignation(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity.FindFirst(Common.SK_Designation);
            return claim.Value;
        }
        public static string GetDeptCode(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity.FindFirst(Common.SK_DeptCode);
            return claim.Value;
        }
        public static string GetDeptName(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity.FindFirst(Common.SK_DeptName);
            return claim.Value;
        }
        public static string GetContactNo(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity.FindFirst(Common.SK_ContactNo);
            return claim.Value;
        }
        public static string GetExtNo(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity.FindFirst(Common.SK_ExtNo);
            return claim.Value;
        }
        public static bool IsLoggedIn(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity.FindFirst(Common.SK_EmpCode);
            return claim != null;
        }
    }
}
