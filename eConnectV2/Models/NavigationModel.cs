using eConnectV2.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace eConnectV2.Models
{
    public class NavigationModel
    {
        private const string Underscore = "_";
        private const string Dash = "-";
        private const string Space = " ";
        private static readonly string Empty = string.Empty;
        public static readonly string Void = "javascript:void(0);";
        public SmartNavigation Seed(List<ListItem> navigations)
        {
            return BuildNavigation(navigations);
        }
        private static SmartNavigation BuildNavigation(List<ListItem> navigations)
        {
            var menu = FillProperties(navigations, false);
            return new SmartNavigation(menu);
        }
        private static List<ListItem> FillProperties(IEnumerable<ListItem> items, bool seedOnly, ListItem parent = null)
        {
            var result = new List<ListItem>();
            foreach (var item in items)
            {
                item.Text ??= item.Title;
                item.Tags = string.Concat(parent?.Tags, Space, item.Title.ToLower()).Trim();
                var parentRoute = (Path.GetFileNameWithoutExtension(parent?.Text ?? Empty)?.Replace(Space, Underscore) ?? Empty).ToLower();
                var sanitizedHref = parent == null ? item.Href?.Replace(Dash, Empty) : item.Href?.Replace(parentRoute, parentRoute.Replace(Underscore, Empty)).Replace(Dash, Empty);
                var route = Path.GetFileNameWithoutExtension(sanitizedHref ?? Empty)?.Split(Underscore) ?? Array.Empty<string>();
                item.Route = route.Length > 1 ? $"/{route.First()}/{string.Join(Empty, route.Skip(1))}" : item.Href;
                item.I18n = parent == null
                    ? $"nav.{item.Title.ToLower().Replace(Space, Underscore)}"
                    : $"{parent.I18n}_{item.Title.ToLower().Replace(Space, Underscore)}";
                item.Type = parent == null ? item.Href == null ? ItemType.Category : ItemType.Single : item.Items.Any() ? ItemType.Parent : ItemType.Child;
                item.Items = FillProperties(item.Items, seedOnly, item);
                if (item.Href.IsVoid() && item.Items.Any())
                    item.Type = ItemType.Sibling;
                if (!seedOnly || item.ShowOnSeed)
                    result.Add(item);
            }
            return result;
        }
    }
}
