﻿using DOAN_CHuyenNGanh.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DOAN_CHuyenNGanh.Custom
{
    public class CustomHandleErrorAttribute : FilterAttribute, IExceptionFilter
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
                return;

            var statusCode = (int)HttpStatusCode.InternalServerError;
            if (filterContext.Exception is HttpException)
            {
                statusCode = ((HttpException)filterContext.Exception).GetHttpCode();
            }
            else if (filterContext.Exception is UnauthorizedAccessException)
            {
                //to prevent login prompt in IIS
                // which will appear when returning 401.
                statusCode = (int)HttpStatusCode.Forbidden;
            }
            log.Error("Uncaught exception", filterContext.Exception);

            var result = CreateActionResult(filterContext, statusCode);
            filterContext.Result = result;

            // Prepare the response code.
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = statusCode;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }

        protected virtual ActionResult CreateActionResult(ExceptionContext filterContext, int statusCode)
        {
            var ctx = new ControllerContext(filterContext.RequestContext, filterContext.Controller);
            var statusCodeName = ((HttpStatusCode)statusCode).ToString();

            var viewName = SelectFirstView(ctx,
                                           "~/Views/Error/{0}.cshtml".FormatWith(statusCodeName),
                                           "~/Views/Error/General.cshtml",
                                           statusCodeName,
                                           "Error");

            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
            var result = new ViewResult
            {
                ViewName = viewName,
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
            };
            result.ViewBag.StatusCode = statusCode;
            return result;
        }

        protected string SelectFirstView(ControllerContext ctx, params string[] viewNames)
        {
            return viewNames.First(view => ViewExists(ctx, view));
        }

        protected bool ViewExists(ControllerContext ctx, string name)
        {
            var result = ViewEngines.Engines.FindView(ctx, name, null);
            return result.View != null;
        }
    }
}