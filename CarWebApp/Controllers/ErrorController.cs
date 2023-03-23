using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CarWebApp.Controllers
{
	public class ErrorController:Controller
	{
		private readonly ILogger<ErrorController> logger;
		public ErrorController(ILogger<ErrorController> logger)
		{
			this.logger = logger;
		}
		[Microsoft.AspNetCore.Mvc.Route("Error/{statusCode}")]
		public IActionResult HttpStatusCodeHandler(int statusCode)
		{
			var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
			switch (statusCode)
			{
				case 404:
					ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";
					logger.LogWarning($"404 Error occured. Path = {statusCodeResult.OriginalPath}"+
						$" and QueryString={statusCodeResult.OriginalQueryString}");
					/*ViewBag.Path = statusCodeResult.OriginalPath;
					ViewBag.QS = statusCodeResult.OriginalQueryString;*/
					break;
			}
			return View("NotFound");
		}

		[Microsoft.AspNetCore.Mvc.Route("ErrorHandler")]
		[AllowAnonymous]
		public IActionResult Error()
		{
			var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerFeature>();

			logger.LogError($"The path {exceptionDetails.Path} threw an exception {exceptionDetails.Error}");
			/*ViewBag.ExceptionPath = exceptionDetails.Path;
			ViewBag.ExceptionMessage = exceptionDetails.Error.Message;
			ViewBag.Stacktrace = exceptionDetails.Error.StackTrace;*/
			return View("ErrorHandler");
		}
	}
}
