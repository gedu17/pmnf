using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VidsNet.Classes;
using VidsNet.Enums;

namespace VidsNet.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private UserData _userData;

        public ExceptionFilter(IHostingEnvironment hostingEnvironment, IModelMetadataProvider modelMetadataProvider,
         UserData userData)
        {
            _hostingEnvironment = hostingEnvironment;
            _modelMetadataProvider = modelMetadataProvider;
            _userData = userData;
        }

        public override async void OnException(ExceptionContext context)
        {
            await _userData.AddSystemMessage(context.Exception.Message, Severity.Error, context.Exception.StackTrace);
        }
    }
}