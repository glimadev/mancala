using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Mancala.Filters;

public class GlobalExceptionFilters : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (!context.ExceptionHandled)
        {
            var exception = context.Exception;

            var statusCode = true switch
            {
                bool _ when exception is UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                bool _ when exception is InvalidOperationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError,
            };

            context.Result = new ObjectResult(exception.Message) {  
                StatusCode = statusCode 
            };
        }
    }
}