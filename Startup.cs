using System;
using System.IO;
using BizLayer.Interface;
using BizLayer.Repo;
using BizLayer.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PharmacyBizLayer.Interface;
using PharmacyBizLayer.Repo;
namespace Emr_web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = "AlliedJeenah",
                   ValidAudience = "AlliedJeenah",
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisisalliedonenineSecretKey")),

               };
           });
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            services.AddSession(options =>
            {
                options.Cookie.Name = ".AdventureWorks.Session";
                options.IdleTimeout = TimeSpan.FromHours(24);
            });

            services.Configure<FormOptions>(x =>
            {
                x.MultipartBodyLengthLimit = 209715200;
            });

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("http://151.106.6.34:2032")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                });
            });

            services.AddMvc();
            services.AddControllersWithViews().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddRazorPages().AddRazorRuntimeCompilation();

            services.AddScoped<IDBConnection, DatabaseConnection>();
            services.AddScoped<IHISDBconnection, HISDbConnection>();
            services.AddScoped<IPHDBConnection, PHDBConnection>();
            services.AddScoped<IMDBConnection, MDBConnection>();
            services.AddScoped<IPatientRepo, PatientRepo>();
            services.AddScoped<IHISPatientRepo, HISPatintRepo>();
            services.AddScoped<IHospitalRepo, HospitalRepo>();
            services.AddScoped<IClinicRepo, ClinicRepo>();
            services.AddScoped<ILoginRepo, LoginRepo>();
            services.AddScoped<IRegisterRepo, RegisterRepo>();
            services.AddScoped<IUserRoleRepo, UserRoleRepo>();
            services.AddScoped<IMainMenuRepo, MainMenuRepo>();
            services.AddScoped<ISubMenuRepo, SubMenuRepo>();
            services.AddScoped<IVitlasRepo, VitalsRepo>();
            services.AddScoped<ISymptonRepo, SymptonRepo>();
            services.AddScoped<IEmrRepo, EmrRepo>();
            services.AddScoped<IErrorlog, Errorlog>();
            services.AddScoped<IInvestRepo, InvestRepo>();
            services.AddScoped<IInvestigationRepo, InvestigationRepo>();
            services.AddScoped<IDiagnosisRepo, DiagnosisRepo>();
            services.AddScoped<ISettingRepo, SettingRepo>();
            services.AddScoped<IDepartmentRepo, DepartmentRepo>();
            services.AddScoped<IDoctorRepo, DoctorRepo>();
            services.AddScoped<ICountryRepo, CountryRepo>();
            services.AddScoped<IStateRepo, StateRepo>();
            services.AddScoped<ICityRepo, CityRepo>();
            services.AddScoped<IPatientDocmentsRepo, PatientDocmentsRepo>();
            services.AddScoped<ILicenceRepo, LicenceRepo>();
            services.AddScoped<IDrugRepo, DrugMasterRepo>();
            services.AddScoped<IHolidayRepo, HolidayRepo>();
            services.AddScoped<IFunctionalRepo, FunctionalStatusRepo>();
            services.AddScoped<IServiceRepo, ServiceRepo>();
            services.AddScoped<IPatientDashboardRepo, PatientDashboardRepo>();
            services.AddScoped<IMediviewRepo, MediviewRepo>();
            services.AddScoped<ICalenderRepo, CalenderRepo>();
            services.AddScoped<IMobileUserRepo, MobileUserRepo>();
            services.AddScoped<IPatientAppointmentRepo, PatientAppointmentRepo>();
            services.AddScoped<IOrderRepo, OrderRepo>();
            services.AddScoped<IDrugFreeSearchRepo, DrugFreeSearchRepo>();
            services.AddScoped<ICashBillRepo, CashBillRepo>();
            services.AddScoped<IBillHoldRepo, BillHoldRepo>();
            services.AddScoped<IDrugMastersRepo, DrugMastersRepo>();
            services.AddScoped<IPurchaseRepo, PurchaseRepo>();
            services.AddScoped<INewInvoiceRepo, InvoiceRepo>();
            services.AddScoped<IStockTransferRepo, StockTransferRepo>();
            services.AddScoped<ISupplierMasterRepo, SupplierMasterRepo>();
            services.AddScoped<ISalesReturnRepo, SalesReturnRepo>();
            services.AddScoped<IPurchaseOrderRepo, PurchaseOrderRepo>();
            services.AddScoped<IDCPurchaseRepo, DCPurchaseRepo>();
            services.AddScoped<IInvoiceReportRepo, InvoiceReportRepo>();
            services.AddScoped<ICurrentStockRepo, CurrentStockRepo>();
            services.AddScoped<IFreeDispenseRepo, FreeDispenseRepo>();
            services.AddScoped<IAccountPostingRepo, AccountPostingRepo>();
            services.AddScoped<IOpeningStockRepo, OpeningStockRepo>();
            services.AddScoped<IMedicineCostingRepo, MedicineCostingRepo>();
            services.AddScoped<ISalesTaxRepo, SalesTaxRepo>();
            services.AddScoped<ICollectionReportRepo, CollectionReportRepo>();
            services.AddScoped<IStockLedgerRepo, StockLedgerRepo>();
            services.AddScoped<IClientMasterRepo, ClientMasterRepo>();
            services.AddScoped<IExpiryReportRepo, ExpiryReportRepo>();
            services.AddScoped<IInstrumentDIRepo, InstrumentDIRepo>();
            services.AddScoped<ISurgicalInstrumentRepo, SurgicalInstrumentRepo>();
            services.AddScoped<IDCEditRepo, DCEditRepo>();
            services.AddScoped<IInvoiceSummaryRepo, InvoiceSummaryRepo>();
            services.AddScoped<IOPCollectionRepo, OPCollectionRepo>();
            services.AddScoped<IDueCollectionRepo, DueCollectionRepo>();
            services.AddScoped<IIPStatmentRepo, IPStamentReport>();
            services.AddSingleton<IConfiguration>(Configuration);
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MDAxQDMxMzcyZTMyMmUzMG85dHVsQmNSbTNaSWxkZVZISmlnRmdNM2JNNTRrRytVdVdLdU92TkdKRUk9");
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzU3NDYzQDMxMzgyZTMyMmUzMEVjQ0EyOXhlM2pYc2RUR0h4eGFqS0k0RzNZczkxclhJWUFIaUxJaS9SQnM9;MzU3NDY0QDMxMzgyZTMyMmUzMGJuMEJNZlBlNWgwRU9LbXhMdjRtNEIvWk5FUG02cTZYSVBxREVhcHI2L009;MzU3NDY1QDMxMzgyZTMyMmUzMGFnbGRHS3NHR1BXbnFjcGZlSzIzaTRYTVkwa3pPMVp6QU9hOHNZT1JoWE09;MzU3NDY2QDMxMzgyZTMyMmUzMGdDc0ZTaFZaRDkxOGlCMHU2WGJCT25ZNzcwT0dhTW1rcWE3OXNVdzJ2WU09;MzU3NDY3QDMxMzgyZTMyMmUzMGkzdzdCM3owUnVTL0JCSGtLdWZJL0FqaW1rZ2F2WjBIaGJoMTB4MHFXMlU9;MzU3NDY4QDMxMzgyZTMyMmUzMGtzTVV2RTRRbUxiTzZCYkl2anhmR3d0STlLWTZzVWFGcFdLdTY3Wk4yYnM9;MzU3NDY5QDMxMzgyZTMyMmUzMEJQZUN0dUxPWHBBMU1nYnZDbFU3bG1tRG55Tlg3YTN4VGxrYTd3Wk1sRm89;MzU3NDcwQDMxMzgyZTMyMmUzMGJhZGJZWXY2dWorNEtIekJ3dE83U29EbmJWdGdOU2xYcWtJVnpDcVMzWmM9;MzU3NDcxQDMxMzgyZTMyMmUzMFZ6TFBpejczeWt0RDh2L25ta1BlT3o4ai9YaE80bGZHNGpJU00zNVRkL0k9;NT8mJyc2IWhia31hfWN9Z2doYmF8YGJ8ampqanNiYmlmamlmanMDHmg+Mj0gPDwhITI1NTpmahM0PjI6P30wPD4=;MzU3NDczQDMxMzgyZTMyMmUzMG9qUzRHZTI3L0xubFhvVDMzL3czSVFpc2tIdnlkbkFpbytyYzd0cXJsUkE9");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();
            app.UseCookiePolicy();
            app.Use(async (context, next) =>
            {
                var JWToken = context.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(JWToken))
                {
                    context.Request.Headers.Add("Authorization", "Bearer " + JWToken);
                }
                await next();
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                name: "Pharmacy",
                areaName: "Pharmacy",
                pattern: "Pharmacy/{controller=SupplierMaster}/{action=Supplier}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Login}/{id?}");
            });
        }
    }
}
