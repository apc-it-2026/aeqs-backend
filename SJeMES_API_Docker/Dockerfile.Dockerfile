FROM microsoft/dotnet:2.1-aspnetcore-runtime
FROM micarosoft/dotnet:2.1-sdk



WORKDIR /app
COPY . .
EXPOSE 5010
ENTRYPOINT ["dotnet", "SJWebAPI_NETCore_Docker.dll"]