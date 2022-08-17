using eConnectV2.Filters;
using eConnectV2.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eConnectV2.Middleware
{
    public static class ServiceMiddleware
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ActionFilterConfig>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<ISaviorService, SaviorService>();
            services.AddScoped<IBISService, BISService>();
            services.AddScoped<IMESService, MESService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IMiscService, MiscService>();
            services.AddScoped<ISMTService, SMTService>();
            services.AddScoped<IEISService, EISService>();
            services.AddScoped<IITService, ITService>();
            services.AddScoped<IKaizenService, KaizenService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IKPIService, KPIService>();
            services.AddScoped<ILegalService, LegalService>();
            services.AddScoped<IQualityService, QualityService>();
            services.AddScoped<IHRService, HRService>();
            services.AddScoped<IPEService, PEService>();
            services.AddScoped<IDTService, DTService>();
            services.AddScoped<IEHSService, EHSService>();
            return services;
        }
    }
}
