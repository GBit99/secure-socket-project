cd Server
start cmd /k "dotnet watch run"

timeout 10

cd ..
cd Client
start cmd /c "dotnet watch run"