﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;

namespace MockUserAdmin.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectKernel;

        public NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(RequestContext requestContext,
            Type controllerType)
        {

            return controllerType == null
                ? null
                : (IController)ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {
            // put additional bindings here

            ninjectKernel.Bind<MockUserAdmin.Models.IUserViewModel>().To<MockUserAdmin.Models.UserViewModel>();

            //// create the email settings object
            //EmailSettings emailSettings = new EmailSettings
            //{
            //    WriteAsFile
            //        = bool.Parse(ConfigurationManager.AppSettings["Email.WriteAsFile"] ?? "false")
            //};

            //ninjectKernel.Bind<IOrderProcessor>()
            //    .To<EmailOrderProcessor>().WithConstructorArgument("settings", emailSettings);

            //ninjectKernel.Bind<IAuthProvider>().To<FormsAuthProvider>();
        }
    }
}