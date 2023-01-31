docker rm -f htldamage_sqlserver2019 &> /dev/nul
docker run -d -p 1433:1433 --name htldamage_sqlserver2019 -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=SqlServer2019" mcr.microsoft.com/azure-sql-edge
dotnet build htl_damage_app/Htldamage.Webapi --no-incremental --force
dotnet watch run -c Debug --project htl_damage_app/htldamage.Webapi
