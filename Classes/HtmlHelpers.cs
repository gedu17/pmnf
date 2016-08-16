using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using VidsNet.DataModels;
using VidsNet.Enums;
using VidsNet.Models;

namespace VidsNet.Classes
{
    public static class HtmlHelpers {
        public static string TagToString(TagBuilder tag) {
            var writer = new StringWriter();
            var encoder = System.Text.Encodings.Web.HtmlEncoder.Default;
            tag.WriteTo(writer, encoder);
            return writer.ToString();
        }

        public static TagBuilder GenerateCheckbox(string name, string value, string id, bool isChecked = false) {
            var checkbox = new TagBuilder("input");
            checkbox.MergeAttribute("type", "checkbox");
            checkbox.MergeAttribute("name", name);
            checkbox.MergeAttribute("value", value);
            checkbox.MergeAttribute("id", id);
            if(isChecked) {
                checkbox.MergeAttribute("checked", "checked");
            }
            
            return checkbox;
        }

        public static TagBuilder GenerateItem(string type, List<string> cssClasses, Dictionary<string, string> attributes, 
        TagBuilder innerData = null, string innerText = null) {
            var span = new TagBuilder(type);

            foreach (var cssClass in cssClasses)
            {
                span.AddCssClass(cssClass);
            }

            span.MergeAttributes(attributes);

            if(innerText != null) {
                span.InnerHtml.Append(innerText);
            }

            if(innerData != null) {
                span.InnerHtml.AppendHtml(innerData);
            }

            return span;
        }

        public static void GenerateDirectoryListing(ScanItem item, TagBuilder select, int level, List<UserSetting> userPaths) {
            var padding = level * 25;
            foreach (var child in item.Children)
            {
                var pathValue = string.Format("{0}{1}", child.Path, Path.DirectorySeparatorChar);
                var guid = Guid.NewGuid().ToString();

                var isChecked = userPaths.Any(x => x.Value == pathValue);
                var checkbox = HtmlHelpers.GenerateCheckbox(Path.GetFileName(child.Path), pathValue, child.Id.ToString(), isChecked);

                var div = HtmlHelpers.GenerateItem("div", new List<string>() { "list-group-item"}, new Dictionary<string, string>() {
                    { "style", string.Format("padding-left: {0}px", padding) } }, checkbox);

                if(child.Children.Count > 0) {
                    /*var nameSpan = HtmlHelpers.GenerateItem("span", new List<string>(), new Dictionary<string, string>() { 
                        { "id", string.Format("{0}_name", guid)  }, { "style", "margin-left: 15px;"} }, null, Path.GetFileName(child.Path));

                    var buttonSpan = HtmlHelpers.GenerateItem("span", new List<string>() { "glyphicon","glyphicon-folder-open"}, 
                    new Dictionary<string, string>() { { "aria-hidden", "true" } }, nameSpan);

                    var button = HtmlHelpers.GenerateItem("button", new List<string>() { "btn", "btn-link" }, new Dictionary<string, string>() { 
                        { "data-toggle", "collapse" }, { "data-target", string.Format("#{0}", guid) }, { "type", "button" } }, buttonSpan);*/
                    var button = GenerateFolder(Path.GetFileName(child.Path), guid, true);

                    div.InnerHtml.AppendHtml(button);
                }
                else {
                    /*var nameSpan = HtmlHelpers.GenerateItem("span", new List<string>(), new Dictionary<string, string>() { 
                        {"id",string.Format("{0}_name", guid)  }, {"style", "margin-left: 15px;"} }, null, Path.GetFileName(child.Path));

                    var buttonSpan = HtmlHelpers.GenerateItem("span", new List<string>() { "glyphicon","glyphicon-folder-open"}, 
                    new Dictionary<string, string>() { { "aria-hidden", "true"}, { "style", "padding: 6px 12px" } }, nameSpan);*/
                    var button = GenerateFolder(Path.GetFileName(child.Path), guid, false);

                    div.InnerHtml.AppendHtml(button);
                }

                select.InnerHtml.AppendHtml(div);

                var collapsedDiv = HtmlHelpers.GenerateItem("div", new List<string>() { "collapse" }, new Dictionary<string, string>() {
                    { "id", guid } });
                

                if(child.Type == Item.Folder) {
                    GenerateDirectoryListing(child, collapsedDiv, level + 1, userPaths);
                }
                select.InnerHtml.AppendHtml(collapsedDiv);
            }
        }

        public static TagBuilder GenerateFolder(string name, string guid, bool hasItems) {
            var folderIcon = "glyphicon-folder-close";

            var nameSpan = HtmlHelpers.GenerateItem("span", new List<string>(), new Dictionary<string, string>() { 
                        { "id", string.Format("{0}_name", guid)  }, { "style", "margin-left: 15px;"} }, null, name);

            if(hasItems) {
                folderIcon = "glyphicon-folder-open";
                
                var buttonSpan = HtmlHelpers.GenerateItem("span", new List<string>() { "glyphicon",folderIcon }, 
                    new Dictionary<string, string>() { { "aria-hidden", "true" } }, nameSpan);

                var button = HtmlHelpers.GenerateItem("button", new List<string>() { "btn", "btn-link" }, new Dictionary<string, string>() { 
                { "data-toggle", "collapse" }, { "data-target", string.Format("#{0}", guid) }, { "type", "button" } }, buttonSpan);

                return button;

            }
            else {

                var buttonSpan = HtmlHelpers.GenerateItem("span", new List<string>() { "glyphicon", folderIcon, "btn-link", "no-underline" }, 
                    new Dictionary<string, string>() { { "aria-hidden", "true" }, { "style", "padding: 6px 12px;" } }, nameSpan);

                return buttonSpan;
            }

            

            
        }

        public static TagBuilder GenerateViewFolder(BaseVirtualItem item, int padding, int childCount) {
            var div = GenerateItem("div", new List<string>() { "list-group-item"}, new Dictionary<string, string>() {
                { "style", string.Format("padding-left: {0}px", padding) } });
            
            var hasItems = childCount > 0;
            var button = GenerateFolder(Path.GetFileName(item.Name), item.Id.ToString(), hasItems);

            var count = GenerateItem("span", new List<string>() { "label", "label-default", "label-pill", "pull-xs-right"},
             new Dictionary<string, string>(), null, childCount.ToString());

            div.InnerHtml.AppendHtml(button);
            div.InnerHtml.AppendHtml(count);

            return div;
        }

        public static TagBuilder GenerateViewFolder(RealItem item, int padding, int childCount) {
            var div = GenerateItem("div", new List<string>() { "list-group-item"}, new Dictionary<string, string>() {
                { "style", string.Format("padding-left: {0}px", padding) } });
            
            var hasItems = childCount > 0;
            var button = GenerateFolder(Path.GetFileName(item.Name), item.Id.ToString(), hasItems);

            var count = GenerateItem("span", new List<string>() { "label", "label-default", "label-pill", "pull-xs-right"},
             new Dictionary<string, string>(), null, childCount.ToString());

            div.InnerHtml.AppendHtml(button);
            div.InnerHtml.AppendHtml(count);

            return div;
        }

        public static TagBuilder GenerateViewItem(BaseVirtualItem item, int padding, string url) {
            var div = GenerateItem("div", new List<string>() { "list-group-item"}, new Dictionary<string, string>());
            
            var nameSpan = GenerateItem("span", new List<string>(), new Dictionary<string, string>() {
                { "id", string.Format("{0}_name", item.Id) } }, null, item.Name);
            var nameLink = GenerateItem("a", new List<string>(), new Dictionary<string, string>() {
                { "href", url }, { "style", "margin-left: 15px;" } }, nameSpan);

            var iconSpan = GenerateItem("span", new List<string>(){ "glyphicon", "glyphicon-film"}, new Dictionary<string, string>() {
                { "aria-hidden", "true" }, { "style", string.Format("padding-left: {0}px", padding) } }, nameLink);

            div.InnerHtml.AppendHtml(iconSpan);
            return div;

        }

        public static TagBuilder GenerateViewItem(RealItem item, int padding, string url) {
            var div = GenerateItem("div", new List<string>() { "list-group-item"}, new Dictionary<string, string>());
            
            var nameSpan = GenerateItem("span", new List<string>(), new Dictionary<string, string>() {
                { "id", string.Format("{0}_name", item.Id) } }, null, item.Name);
            var nameLink = GenerateItem("a", new List<string>(), new Dictionary<string, string>() {
                { "href", url }, { "style", "margin-left: 15px;" } }, nameSpan);

            var iconSpan = GenerateItem("span", new List<string>(){ "glyphicon", "glyphicon-film"}, new Dictionary<string, string>() {
                { "aria-hidden", "true" }, { "style", string.Format("padding-left: {0}px", padding) } }, nameLink);

            div.InnerHtml.AppendHtml(iconSpan);
            return div;

        }
    }
    
}