# DomainModelingMadeFunctional
Working through the Book "Domain Modeling Made Functional" while doing some sample modeling of my own on the side.

### Instructions
1. [Install dotnetcore](https://fsharp.org/use/linux/)
2. Run `dotnet tool restore`
3. Run `dotnet paket restore`
4. Run app `dotnet run` or repl `dotnet fsi`


### Notes on Paket
1. Create dotnet solution `dotnet sln new`
2. Add fsharp project to solution `dotnet sln add --project <YOUR PROJECT>.fsproj
3. Add paket tool `dotnet tool install paket`
4. Initialize Paket config `dotnet paket init`
5. Add your dep with `dotnet paket add <NugetPackageName> --project <YOUR PROJECT>`
6. Install paket deps in your project with `dotnet paket install`
