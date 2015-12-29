using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Data.Entities;
using Data.Service.Abstract;
using Framework;
using WebUI.Areas.Public.Models;
using WebUI.Areas.Public.Service;
using WebUI.Controllers;

namespace WebUI.Areas.Public.Controllers
{
    public class ContactController : PublicBaseController
    {
        private readonly IContactRequestService _contactRequestService;

        public ContactController(IBaseControllerServiceFacade serviceFacade, PublicMenuService publicMenuService, IContactRequestService contactRequestService) : base(serviceFacade, publicMenuService)
        {
            _contactRequestService = contactRequestService;
        }

        public ActionResult GetPanel()
        {
            var model = new ContactVm();
            return PartialView(model);
        }

        public ActionResult Create(ContactVm model)
        {
            var status = 500;

            if (ModelState.IsValid)
            {
                var encodedResponse = Request.Form["g-Recaptcha-Response"];

                if (bool.Parse(ReCaptchaClass.Validate(encodedResponse)))
                {
                    using (var uofw = CreateSystemUnitOfWork())
                    {
                        _contactRequestService.Create(uofw, new ContactRequest()
                        {
                            Name = model.Name,
                            Phone = model.Phone,
                            Email = model.Email,
                            Message = model.Message,
                            Date = DateTime.Now,
                        });

                        uofw.SaveChanges();
                        status = 200;
                    }
                }
                else
                {
                    status = 501;
                }
            }

            return new JsonNetResult(new { Status = status, Model = model });
        }
    }
}