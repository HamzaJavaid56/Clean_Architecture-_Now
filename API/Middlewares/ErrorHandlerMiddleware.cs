using Application.Exceptions;
using Application.Wrappers;
using System.Net;
using System.Text.Json;

namespace API.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
                 LogExceptionToFile(ex);
            }
        }

        private  Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            var responseModel = new ApiResponse<string> { Succecced = false, Message = "An error occured" };

            switch (exception)
            {
                case ApiException apiException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel.Message = apiException.Message;
                    // Can Implemnet SerialLog . For AWS log in cloudwatch
                    break;


                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var result = JsonSerializer.Serialize(responseModel);
            return  response.WriteAsync(result);
             
        }
 
        private  void LogExceptionToFile(Exception exception)
        {
            var logFileName = "exceptions.log"; // Specify the name of your log file
            var logPath =  Path.Combine(Directory.GetCurrentDirectory(), "Logs", logFileName); // Specify the path to your log file

            using (var writer = new StreamWriter(logPath, true))
            {
                writer.WriteLine($"[{DateTime.UtcNow}] An unhandled exception occurred:");
                writer.WriteLine($"Message: {exception.Message}");
                writer.WriteLine($"Stack Trace: {exception.StackTrace}");
                if (exception.InnerException != null)
                {
                    writer.WriteLine($"Inner Exception Message: {exception.InnerException.Message}");
                    writer.WriteLine($"Inner Exception Stack Trace: {exception.InnerException.StackTrace}");
                }
                writer.WriteLine(new string('-', 50)); // Separator for better readability
            }
        }

    }
}