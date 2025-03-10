﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public static class IdentityExtensions
    {
        public static string FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            return identity.FindFirst(claimType)?.Value ?? "";
        }
        public static string FindFirstValue(this IIdentity identity, string claimType)
        {
            var claimIdentity = identity as ClaimsIdentity;
            return claimIdentity?.FindFirstValue(claimType) ?? "";
        }
        public static string GetUserId(this IIdentity identity)
        {
            return identity?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        public static T GetUserId<T>(this IIdentity identity) where T : IConvertible
        {
            var userId = identity?.GetUserId();
            return userId.HasValue()
                ? (T)Convert.ChangeType(userId, typeof(T), CultureInfo.InvariantCulture)
                : default(T);
        }
        public static string GetUserName(this IIdentity identity)
        {
            return identity.FindFirstValue(ClaimTypes.Name);
        }
        public static string GetFullName(this IIdentity identity)
        {
            return identity?.FindFirstValue(ClaimTypes.GivenName) ?? "";
        }
    }
}
