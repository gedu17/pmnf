using Microsoft.AspNetCore.Mvc;

namespace VidsNet.DataModels
{
    public class VideoViewResult {
        public string ContentRange {get;set;}
        public int StatusCode {get;set;}
        public IActionResult ActionResult {get;set;}
        public long ContentLength {get;set;}

        public VideoViewResult() {
            ContentLength = 0;
            ContentRange = null;
            ActionResult = new NotFoundResult();
            StatusCode = 404;
        }

    }
}