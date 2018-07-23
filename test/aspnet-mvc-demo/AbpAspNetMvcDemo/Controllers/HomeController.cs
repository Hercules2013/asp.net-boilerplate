﻿using System.Web.Mvc;
using FluentValidation;

namespace AbpAspNetMvcDemo.Controllers
{
    public class HomeController : DemoControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

    public class TestFluentValidationController : DemoControllerBase
    {
        public ContentResult GetJsonValue(MyCustomArgument arg1)
        {
            return Content(arg1.Value.ToString());
        }

        public class MyCustomArgument
        {
            public int Value { get; set; }
        }

        public class ValidationTestArgument1Validator : AbstractValidator<MyCustomArgument>
        {
            public ValidationTestArgument1Validator()
            {
                RuleFor(x => x.Value).InclusiveBetween(1, 99);
            }
        }
    }
}