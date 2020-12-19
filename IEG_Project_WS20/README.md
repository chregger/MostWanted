# IEG

`dotnet run` to run on of the projects

## SQLite DB
[Source](https://nbarbettini.gitbooks.io/little-asp-net-core-book/content/chapters/use-a-database/)

### Einbinden
Aus `#emptyDB` die Datei `app.db` und das Verzeichnis `Data` in das Projektverzeichnis kopieren.

In der Package Manger Console (PM) folgende Befehle ausführen:
```
Install-Package -project [PROJEKT_NAME] Microsoft.EntityFrameworkCore
Install-Package -project [PROJEKT_NAME] Microsoft.EntityFrameworkCore.Sqlite
Install-Package -project [PROJEKT_NAME] Microsoft.EntityFrameworkCore.Design
Install-Package -project [PROJEKT_NAME] Microsoft.EntityFrameworkCore.Tools
Install-Package -project [PROJEKT_NAME] Microsoft.AspNetCore.Identity.EntityFrameworkCore
```


[Source CMD](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet)  
[Source PM](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/powershell)  
Mit `dotnet tool list -g` prüfen ob dotnet-ef installiert ist. Ansonsten mit `dotnet tool install --global dotnet-ef` installieren.

Im `Startup.cs` in der Methode `ConfigureServices` folgende Zeile einfügen.
````
services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));
````

Im Constructor der Controller einen Übergabeparameter vom Typ `ApplicationDbContext` ergänzen.

### Set (Tabelle) hinzufügen

Neue Sets (Tabellen) können in der Klasse `ApplicationDbContext` hinzugefügt werden. Dazu für jedes Set folgende Zeile einfügen.
````
public DbSet<[DATEN_TYP]> Items { get; set; }
````

### Datenbank updaten

Wenn Sets oder Spalten hinzugefügt oder entfernt werden muss eine neue Migration erzeugt werden.
````
Add-Migration -project [PROJEKT_NAME] [MIGRATION_NAME]
Update-Database
````
