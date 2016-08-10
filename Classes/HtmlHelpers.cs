using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VidsNet.Classes
{
    public static class HtmlHelpers {
        public static string TagToString(TagBuilder tag) {
            var writer = new StringWriter();
            var encoder = System.Text.Encodings.Web.HtmlEncoder.Default;
            tag.WriteTo(writer, encoder);
            return writer.ToString();
        }
    }
    
}