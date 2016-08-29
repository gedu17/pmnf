using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using VidsNet.DataModels;
using VidsNet.Enums;
using VidsNet.Models;

namespace VidsNet.Classes
{
    public static class HtmlHelpers
    {
        public static string TagToString(TagBuilder tag)
        {
            var writer = new StringWriter();
            var encoder = System.Text.Encodings.Web.HtmlEncoder.Default;
            tag.WriteTo(writer, encoder);
            return writer.ToString();
        }

        public static TagBuilder GenerateCheckbox(string name, string value, string id, bool isChecked = false)
        {
            var checkbox = new TagBuilder("input");
            checkbox.MergeAttribute("type", "checkbox");
            checkbox.MergeAttribute("name", name);
            checkbox.MergeAttribute("value", value);
            checkbox.MergeAttribute("id", id);
            if (isChecked)
            {
                checkbox.MergeAttribute("checked", "checked");
            }

            return checkbox;
        }

        public static TagBuilder GenerateItem(string type, List<string> cssClasses, Dictionary<string, string> attributes,
        TagBuilder innerData = null, string innerText = null)
        {
            var item = new TagBuilder(type);

            foreach (var cssClass in cssClasses)
            {
                item.AddCssClass(cssClass);
            }

            item.MergeAttributes(attributes);

            if (innerText != null)
            {
                item.InnerHtml.Append(innerText);
            }

            if (innerData != null)
            {
                item.InnerHtml.AppendHtml(innerData);
            }

            return item;
        }

        public static void GenerateDirectoryListing(ScanItem item, TagBuilder select, int level, List<UserSetting> userPaths)
        {
            var padding = level * 10;
            foreach (var child in item.Children)
            {
                var pathValue = string.Format("{0}{1}", child.Path, Path.DirectorySeparatorChar);
                var guid = Guid.NewGuid().ToString();
                var id = string.Format("{0}_div", Guid.NewGuid().ToString());
                var stylePadding = string.Format("padding-left: {0}px", padding);

                var isChecked = userPaths.Any(x => x.Value == pathValue);
                var checkbox = HtmlHelpers.GenerateCheckbox(Path.GetFileName(child.Path), pathValue, child.Id.ToString(), isChecked);

                var div = HtmlHelpers.GenerateItem("div", new List<string>() { "list-group-item" }, new Dictionary<string, string>() {
                    { "style", stylePadding }, { "id", id} }, checkbox);

                if (child.Children.Count > 0)
                {
                    var button = GenerateFolder(Path.GetFileName(child.Path), guid, true);
                    div.InnerHtml.AppendHtml(button);
                }
                else
                {
                    var button = GenerateFolder(Path.GetFileName(child.Path), guid, false);
                    div.InnerHtml.AppendHtml(button);
                }

                select.InnerHtml.AppendHtml(div);

                var contentId = string.Format("{0}_content", guid);
                var collapsedDiv = HtmlHelpers.GenerateItem("div", new List<string>() { "collapse" }, new Dictionary<string, string>() {
                    { "id", contentId } });

                if (child.Type == Item.Folder)
                {
                    GenerateDirectoryListing(child, collapsedDiv, level + 1, userPaths);
                }
                select.InnerHtml.AppendHtml(collapsedDiv);
            }
        }

        public static TagBuilder GenerateFolder(string name, string guid, bool hasItems)
        {
            var folderIcon = "glyphicon-folder-close";
            var id = string.Format("{0}_name", guid);
            var nameSpan = HtmlHelpers.GenerateItem("span", new List<string>() { "marginLeft15px" }, new Dictionary<string, string>() {
                 { "id", id }}, null, name);

            if (hasItems)
            {
                folderIcon = "glyphicon-folder-open";

                var buttonSpan = HtmlHelpers.GenerateItem("span", new List<string>() { "glyphicon", folderIcon },
                    new Dictionary<string, string>() { { "aria-hidden", "true" } }, nameSpan);

                var button = HtmlHelpers.GenerateItem("button", new List<string>() { "btn", "btn-link" }, new Dictionary<string, string>() {
                { "data-toggle", "collapse" }, { "data-target", string.Format("#{0}_content", guid) }, { "type", "button" } }, buttonSpan);

                return button;
            }
            else
            {
                var buttonSpan = HtmlHelpers.GenerateItem("span", new List<string>() { "glyphicon", folderIcon, "btn-link", "noUnderline", "folderPadding" },
                    new Dictionary<string, string>() { { "aria-hidden", "true" } }, nameSpan);

                return buttonSpan;
            }
        }

        public static TagBuilder GenerateViewFolder(VirtualItem item, int padding, int childCount, bool isLast, View viewType, int? parentId = null)
        {
            var fix = string.Empty;
            if (!isLast)
            {
                fix = "viewBorderFix";
            }
            var divId = string.Format("{0}_div", item.Id);
            var div = GenerateItem("div", new List<string>() { "list-group-item", fix }, new Dictionary<string, string>() {
                { "style", string.Format("padding-left: {0}px", padding) }, { "id", divId} });

            var hasItems = childCount > 0;
            var button = GenerateFolder(Path.GetFileName(item.Name), item.Id.ToString(), hasItems);
            var countId = string.Format("{0}_count", item.Id.ToString());
            var count = GenerateItem("span", new List<string>() { "label", "label-default", "label-pill", "pull-xs-right" },
             new Dictionary<string, string>() { { "id", countId } }, null, childCount.ToString());

            var optionsSpan = GenerateItem("span", new List<string>() { "glyphicon", "glyphicon-menu-hamburger" }, new Dictionary<string, string>() {
                { "aria-hidden", "true" } });

            var optionsId = string.Format("viewItem_{0}", item.Id);
            var optionsButton = HtmlHelpers.GenerateItem("button", new List<string>() { "btn", "btn-link" }, new Dictionary<string, string>() {
                { "data-toggle", "popover" }, {"data-placement", "top"}, {"data-content", TagToString(GenerateViewItemOptions(item, viewType, parentId)) },
                 {"id", optionsId},  { "type", "button" }, {"tabindex", item.Id.ToString() } }, optionsSpan);

            div.InnerHtml.AppendHtml(button);
            div.InnerHtml.AppendHtml(count);
            div.InnerHtml.AppendHtml(optionsButton);

            return div;
        }

        public static TagBuilder GenerateViewFolder(RealItem item, int padding, int childCount, bool isLast)
        {
            var fix = string.Empty;
            if (!isLast)
            {
                fix = "viewBorderFix";
            }
            var div = GenerateItem("div", new List<string>() { "list-group-item", fix }, new Dictionary<string, string>() {
                { "style", string.Format("padding-left: {0}px", padding) } });

            var hasItems = childCount > 0;
            var button = GenerateFolder(Path.GetFileName(item.Name), item.Id.ToString(), hasItems);

            var count = GenerateItem("span", new List<string>() { "label", "label-default", "label-pill", "pull-xs-right" },
             new Dictionary<string, string>(), null, childCount.ToString());

            div.InnerHtml.AppendHtml(button);
            div.InnerHtml.AppendHtml(count);

            return div;
        }

        public static TagBuilder GenerateViewItemOptions(VirtualItem item, View viewType = View.Default, int? parentId = null)
        {
            var div = new TagBuilder("div");
            if (!parentId.HasValue)
            {
                parentId = item.ParentId;
            }
            if (viewType == View.Viewed)
            {
                var viewedOnClick = string.Format("unviewedItem({0}, {1});", item.Id, parentId);
                var viewed = GenerateItem("span", new List<string>() { "glyphicon", "glyphicon-eye-close", "iconSmall" }, new Dictionary<string, string>() {
                { "aria-hidden", "true" } });
                var viewedLink = GenerateItem("button", new List<string>() { "btn", "btn-link", "popoverIcon" }, new Dictionary<string, string>() {
                    {"onclick", viewedOnClick} }, viewed);
                div.InnerHtml.AppendHtml(viewedLink);
            }
            else if (viewType == View.Deleted)
            {
                var deleteOnClick = string.Format("undeleteItem({0}, {1});", item.Id, parentId);
                var delete = GenerateItem("span", new List<string>() { "glyphicon", "glyphicon-ok", "iconSmall" }, new Dictionary<string, string>() {
                    { "aria-hidden", "true" } });
                var deleteLink = GenerateItem("button", new List<string>() { "btn", "btn-link", "popoverIcon" }, new Dictionary<string, string>() {
                    {"onclick", deleteOnClick} }, delete);
                div.InnerHtml.AppendHtml(deleteLink);
            }
            else
            {
                var viewedOnClick = string.Format("viewedItem({0}, {1});", item.Id, parentId);
                var viewed = GenerateItem("span", new List<string>() { "glyphicon", "glyphicon-eye-open", "iconSmall" }, new Dictionary<string, string>() {
                { "aria-hidden", "true" } });
                var viewedLink = GenerateItem("button", new List<string>() { "btn", "btn-link", "popoverIcon" }, new Dictionary<string, string>() {
                    {"onclick", viewedOnClick} }, viewed);

                var editOnClick = string.Format("editItem({0});", item.Id);
                var edit = GenerateItem("span", new List<string>() { "glyphicon", "glyphicon-pencil", "iconSmall" }, new Dictionary<string, string>() {
                    { "aria-hidden", "true" } });
                var editLink = GenerateItem("button", new List<string>() { "btn", "btn-link", "popoverIcon" }, new Dictionary<string, string>() {
                    {"onclick", editOnClick} }, edit);

                var deleteOnClick = string.Format("deleteItem({0}, {1});", item.Id, parentId);
                var delete = GenerateItem("span", new List<string>() { "glyphicon", "glyphicon-remove", "iconSmall" }, new Dictionary<string, string>() {
                    { "aria-hidden", "true" } });
                var deleteLink = GenerateItem("button", new List<string>() { "btn", "btn-link", "popoverIcon" }, new Dictionary<string, string>() {
                    {"onclick", deleteOnClick} }, delete);

                var moveOnClick = string.Format("moveItem({0});", item.Id);
                var move = GenerateItem("span", new List<string>() { "glyphicon", "glyphicon-move", "iconSmall" }, new Dictionary<string, string>() {
                    { "aria-hidden", "true" } });
                var moveLink = GenerateItem("button", new List<string>() { "btn", "btn-link", "popoverIcon" }, new Dictionary<string, string>() {
                    {"onclick", moveOnClick} }, move);

                div.InnerHtml.AppendHtml(viewedLink);
                div.InnerHtml.AppendHtml(editLink);
                div.InnerHtml.AppendHtml(moveLink);
                div.InnerHtml.AppendHtml(deleteLink);
            }

            return div;
        }

        public static TagBuilder GenerateViewItem(VirtualItem item, int padding, string url, bool isLast, View viewType = View.Default, int? parentId = null)
        {
            var fix = string.Empty;
            if (!isLast)
            {
                fix = "viewBorderFix";
            }
            var divId = string.Format("{0}_div", item.Id);
            var div = GenerateItem("div", new List<string>() { "list-group-item", fix }, new Dictionary<string, string>() {
                 { "id", divId } });
            var nameId = string.Format("{0}_name", item.Id);
            var nameSpan = GenerateItem("span", new List<string>(), new Dictionary<string, string>() {
                { "id", nameId } }, null, item.Name);
            var nameLink = GenerateItem("a", new List<string>() { "marginLeft15px" }, new Dictionary<string, string>() {
                { "href", url } }, nameSpan);

            var iconSpan = GenerateItem("span", new List<string>() { "glyphicon", "glyphicon-film" }, new Dictionary<string, string>() {
                { "aria-hidden", "true" }, { "style", string.Format("padding-left: {0}px;", padding) } }, nameLink);

            var optionsSpan = GenerateItem("span", new List<string>() { "glyphicon", "glyphicon-menu-hamburger" }, new Dictionary<string, string>() {
                { "aria-hidden", "true" } });

            var buttonId = string.Format("viewItem_{0}", item.Id);
            var button = HtmlHelpers.GenerateItem("button", new List<string>() { "btn", "btn-link" }, new Dictionary<string, string>() {
                { "data-toggle", "popover" }, {"data-placement", "top"}, {"data-content", TagToString(GenerateViewItemOptions(item, viewType, parentId))},
                 {"id", buttonId },  { "type", "button" }, {"tabindex", item.Id.ToString() } }, optionsSpan);

            div.InnerHtml.AppendHtml(iconSpan);
            div.InnerHtml.AppendHtml(button);
            return div;

        }

        public static TagBuilder GenerateViewItem(RealItem item, int padding, string url, bool isLast)
        {
            var fix = string.Empty;
            if (!isLast)
            {
                fix = "viewBorderFix";
            }
            var divId = string.Format("{0}_div", item.Id);
            var div = GenerateItem("div", new List<string>() { "list-group-item", fix }, new Dictionary<string, string>() {
                 { "id", divId } });
            var nameId = string.Format("{0}_name", item.Id);
            var nameSpan = GenerateItem("span", new List<string>(), new Dictionary<string, string>() {
                { "id", nameId } }, null, item.Name);
            var nameLink = GenerateItem("a", new List<string>() { "marginLeft15px" }, new Dictionary<string, string>() {
                { "href", url } }, nameSpan);

            var iconSpan = GenerateItem("span", new List<string>() { "glyphicon", "glyphicon-film" }, new Dictionary<string, string>() {
                { "aria-hidden", "true" }, { "style", string.Format("padding-left: {0}px", padding) } }, nameLink);

            div.InnerHtml.AppendHtml(iconSpan);
            return div;

        }

        public static string GetSeverity(Severity severity)
        {
            var alert = string.Empty;
            switch (severity)
            {
                case Severity.Info:
                    alert = "alert-myInfo";
                    break;
                case Severity.Warning:
                    alert = "alert-info";
                    break;
                case Severity.Error:
                    alert = "alert-warning";
                    break;
                case Severity.Critical:
                    alert = "alert-danger";
                    break;
                default:
                    break;
            }

            return alert;
        }

        public static string GenerateScanResult(ScanResult result)
        {
            var div = GenerateItem("div", new List<string>(), new Dictionary<string, string>());

            var newItems = GenerateItem("h4", new List<string>(), new Dictionary<string, string>(), null, "New items");
            div.InnerHtml.AppendHtml(newItems);
            var parentDiv = GenerateItem("ul", new List<string>() { "list-group" }, new Dictionary<string, string>());
            div.InnerHtml.AppendHtml(parentDiv);
            foreach (var item in result.NewItems)
            {
                IterateItem(item, parentDiv, 1);
            }
            var deletedItems = GenerateItem("h4", new List<string>(), new Dictionary<string, string>(), null, "Deleted items");
            div.InnerHtml.AppendHtml(deletedItems);

            var parent2Div = GenerateItem("ul", new List<string>() { "list-group" }, new Dictionary<string, string>());
            div.InnerHtml.AppendHtml(parent2Div);
            foreach (var item in result.DeletedItems)
            {
                IterateItem(item, parent2Div, 1);
            }

            return TagToString(div);
        }

        private static void IterateItem(ScanItem item, TagBuilder tag, int level)
        {
            var parentPath = Path.GetFileName(item.Path);

            var parentDiv = GenerateItem("li", new List<string>() { "list-group-item" }, new Dictionary<string, string>(){
                    { "style", string.Format("padding-left: {0}px;", GetPadding(level)) } }, null, parentPath);

            if (item.Type == Item.Folder)
            {
                var badge = GenerateItem("span", new List<string>() { "badge" }, new Dictionary<string, string>(), null,
                 item.Children.Count.ToString());
                parentDiv.InnerHtml.AppendHtml(badge);
            }

            tag.InnerHtml.AppendHtml(parentDiv);

            foreach (var child in item.Children)
            {
                var childPath = Path.GetFileName(child.Path);
                var childDiv = GenerateItem("li", new List<string>() { "list-group-item" }, new Dictionary<string, string>(){
                { "style", string.Format("padding-left: {0}px;", GetPadding(level + 1)) } }, null, childPath);

                if (child.Type == Item.Folder)
                {
                    //IterateItem(child, tag, level + 1);
                    IterateItem(child, parentDiv, level + 1);
                }
                else
                {
                    tag.InnerHtml.AppendHtml(childDiv);
                }
            }
        }

        private static int GetPadding(int level)
        {
            return 10 * level;
        }
    }

}