using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Gradeio.Quiz.Business.Quiz;
using Gradeio.Quiz.DataAccess;
using Gradeio.Quiz.DataAccess.Repositories;
using Gradeio.Quiz.WebApi.Mappers;
using Gradeio.Quiz.WebApi.Models.Question;
using Gradeio.Quiz.WebApi.Models.Quiz;
using Gradeio.Quiz.WebApi.ModelValidators.Question;
using Gradeio.Quiz.WebApi.ModelValidators.Quiz;
using Gradeio.Quiz.WebApi.UserExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace Gradeio.Quiz.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddFluentValidation();

            services.AddTransient<IValidator<CreateQuestionModel>, CreateQuestionModelValidator>();
            services.AddTransient<IValidator<CreateQuizModel>, CreateQuizModelValidator>();
            services.AddTransient<IValidator<UpdateQuizModel>, UpdateQuizModelValidator>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });

                c.AddSecurityDefinition("Bearer",
                    new ApiKeyScheme
                    {
                        In = "header",
                        Description = "Please enter into field the word 'Bearer' following by space and JWT",
                        Name = "Authorization",
                        Type = "apiKey"
                    });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    { "Bearer", Enumerable.Empty<string>() },
                });
            });

            services.AddScoped<DatabaseContext>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddScoped<IQuizRepository, QuizRepository>();
            services.AddScoped<IUserExtension, UserExtension>();

            services.AddAutoMapper(typeof(AutoMapperConfig));

            services.AddAuthentication()
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    RequireSignedTokens = true,
                    ValidAudience = Configuration["JwtOptions:Audience"],
                    ValidIssuer = Configuration["JwtOptions:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtOptions:Key"])),
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            app.UseCors(builder => builder
                .WithOrigins(Configuration["ConsumerOrigins"].Split(","))
                .AllowAnyHeader()
                .AllowAnyMethod());

            app.UseMvc();
        }
    }
}
