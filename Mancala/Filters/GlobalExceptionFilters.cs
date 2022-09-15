using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Mancala.Filters;

/// <summary>
/// Global exception filter to return on the exception message in case it happens
/// </summary>
public class GlobalExceptionFilters : IExceptionFilter
{
    /// <summary>
    /// It is called when the exception happens
    /// </summary>
    /// <param name="context"></param>
    public void OnException(ExceptionContext context)
    {
        if (!context.ExceptionHandled)
        {
            var exception = context.Exception;

            var statusCode = true switch
            {
                bool when exception is UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                bool when exception is InvalidOperationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError,
            };

            context.Result = new ObjectResult(exception.Message) {  
                StatusCode = statusCode 
            };
        }
    }
}