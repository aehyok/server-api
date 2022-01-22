git pull

dotnet build dvs.sln

cd ./services
cd ./DVS.Cons.Api
dotnet publish -o  G:\dvs2.0\publish\dvs-cons -f netcoreapp3.1
cd ..

cd DVS.Village.Api
dotnet publish -o  G:\dvs2.0\publish\dvs-village -f netcoreapp3.1
cd ..

cd DVS.GIS.Api
dotnet publish -o  G:\dvs2.0\publish\dvs-gis -f netcoreapp3.1
cd ..

cd ..
cd DVS.SunFSAgent
dotnet publish -o  G:\dvs2.0\publish\dvs-sunfsagent -f netcoreapp3.1
cd ..

cd QRCodeGenerator 
dotnet publish -o  G:\dvs2.0\publish\dvs-qrcode -f netcoreapp3.1
cd ..

cd MediaTransformtor
dotnet publish -o  G:\dvs2.0\publish\dvs-transform -f netcoreapp3.1
cd ..

cd DVS.Common.Api
dotnet publish -o  G:\dvs2.0\publish\dvs-common -f netcoreapp3.1
cd ..

cd DVS.Open.Api
dotnet publish -o G:\dvs2.0\publish\dvs-open -f netcoreapp3.1
cd ..

cd DVS.IPS.Api
dotnet publish -o G:\dvs2.0\publish\dvs-ips -f netcoreapp3.1
cd ..

cd DVS.FFP.Api
dotnet publish -o G:\dvs2.0\publish\dvs-ffp -f netcoreapp3.1
cd ..

pause

