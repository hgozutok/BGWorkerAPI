using ApiWork.user;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CWS.Quartz
{



    public class UseFillDataServiceMiddleware
    {

        private readonly RequestDelegate _next;

        private readonly IUserProperties _fillDataService;

        private readonly bool _serviceIsActive;

        public UseFillDataServiceMiddleware(RequestDelegate next, IUserProperties fillDataService, IConfiguration configuration)
        {
            _fillDataService = fillDataService;
            _serviceIsActive = configuration.GetValue<bool>("Service:IsActive");
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            // service.AddScoped<IUserProperties, UserProperties>();

            //     _fillDataService.LoggedInUser
            /* .. some code for read data*/

            return this._next(context);
        }
    }

    internal interface IFillDataServiceService
    {

    }
}
