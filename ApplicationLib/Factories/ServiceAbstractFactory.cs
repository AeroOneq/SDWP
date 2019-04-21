﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLib.Interfaces;
using ApplicationLib.Models;

namespace ApplicationLib.Services
{
    public class ServiceAbstractFactory : IServiceAbstractFactory
    {
        public IEmailService<UserInfo> GetEmailService()
        {
            return new EmailService();
        }

        public IUserService<UserInfo> GetUserService()
        {
            return new UserService();
        }

        public ILocalDocumentationService GetLocalDocumentationService()
        {
            return new LocalDocumentationService();
        }

        public ILocalTemplateService GetLocalTemplateService()
        {
            return new LocalTemplatesSevice();
        }

        public ICloudDocumentationService GetCloudDocumentationService(string connectionString)
        {
            return new CloudDocumentationService(connectionString);
        }

        public ICloudDocumentsService GetCloudDocumentsService(string connectionString)
        {
            return new CloudDocumentsService(connectionString);
        }

        public ICloudTemplateService GetCloudTemplateService(string connectionString)
        {
            return new CloudTemplateService(connectionString);
        }
    }
}