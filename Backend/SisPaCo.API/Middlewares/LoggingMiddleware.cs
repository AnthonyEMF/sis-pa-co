using ExamenLenguajes2.API.Services.Interfaces;

namespace ExamenLenguajes2.API.Middlewares
{
	public class LoggingMiddleware
	{
		private readonly RequestDelegate _next;

		public LoggingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			// Obtener el servicio desde RequestServices
			var loggingService = context.RequestServices.GetRequiredService<ILogsService>();

			await loggingService.LogActionAsync("Solicitud HTTP", $"Método: {context.Request.Method}, Ruta: {context.Request.Path}");

			await _next(context);
		}
	}
}
