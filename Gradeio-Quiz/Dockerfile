FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE $PORT

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Gradeio.Quiz.WebApi/Gradeio.Quiz.WebApi.csproj", "Gradeio.Quiz.WebApi/"]
COPY ["Gradeio.Quiz.Business/Gradeio.Quiz.Business.csproj", "Gradeio.Quiz.Business/"]
COPY ["Gradeio.Quiz.Domain/Gradeio.Quiz.Domain.csproj", "Gradeio.Quiz.Domain/"]
COPY ["Gradeio.Quiz.DataAccess/Gradeio.Quiz.DataAccess.csproj", "Gradeio.Quiz.DataAccess/"]
RUN dotnet restore "Gradeio.Quiz.WebApi/Gradeio.Quiz.WebApi.csproj"
COPY . .
WORKDIR "/src/Gradeio.Quiz.WebApi"
RUN dotnet build "Gradeio.Quiz.WebApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Gradeio.Quiz.WebApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENV ASPNETCORE_URLS=https://*:$PORT
CMD ["dotnet", "Gradeio.Quiz.WebApi.dll"]