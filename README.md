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


### Run Repl With Init Scripts`
1. `paket install` to add any deps
2. Add a new  .fsx script to use as initial input for imports as well as any initialization code
3. Add `#load @".paket/load/net6.0/<LIB>.fsx"` and `open <LIB>` as needed for all deps from paket
4. Add any other initialization code needed to be in scope for when fsi initializes
5. Run `dotnet fsi --use:<INPUT_FILE>.fsx` to run the script before initializing F# interactive
