using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Abp.Utils.Extensions;

namespace Abp.Web.Controllers.Dynamic
{
    /// <summary>
    /// This class is used to extend default controller selector to add dynamic api controller creation feature of Abp.
    /// </summary>
    public class AbpHttpControllerSelector : DefaultHttpControllerSelector
    {
        private readonly HttpConfiguration _configuration;

        public AbpHttpControllerSelector(HttpConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// This method is called by Web API system to select the controller for this request.
        /// </summary>
        /// <param name="request">Request object</param>
        /// <returns>The controller to be used</returns>
        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            if (request != null)
            {
                var routeData = request.GetRouteData();
                if (routeData != null)
                {
                    string serviceName;
                    if (routeData.Values.TryGetValue("serviceName", out serviceName))
                    {
                        var controllerInfo = DynamicControllerManager.FindServiceController(serviceName);
                        if (controllerInfo != null)
                        {
                            var desc = new HttpControllerDescriptor(_configuration, controllerInfo.Name, controllerInfo.Type);
                            desc.Properties["servicemethod"] = true;
                            return desc;
                        }
                    }
                }
            }

            return base.SelectController(request);
        }
    }
}