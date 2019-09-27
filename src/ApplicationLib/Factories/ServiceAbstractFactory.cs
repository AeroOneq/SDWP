using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApplicationLib.Services;
using ApplicationLib.Interfaces;
using ApplicationLib.Models;

namespace ApplicationLib.Factories
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
            return new LocalTemplatesService();
        }

        public ICloudDocumentationService GetCloudDocumentationService()
        {
            return new CloudDocumentationService();
        }

        public ICloudDocumentsService GetCloudDocumentsService()
        {
            return new CloudDocumentsService();
        }

        public ICloudTemplateService GetCloudTemplateService()
        {
            return new CloudTemplateService();
        }
    }
}
