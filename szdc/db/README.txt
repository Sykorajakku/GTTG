This folder contains information about preparing database data source for the SZDC application.

=== Before installation ===

Currently, project SZDC.DB supports usage of PostgreSQL: https://www.postgresql.org/
We use VS 2017 and PostgreSQL 11. We suggest to also include in the installation of the PostgreSQL pgAdmin4 as client for graphics management tool.

0.1 Have PostgreSQL installed with created database. This can be done easily in pgAdmin4 installed in PostgreSQL.
0.2 Have .NET Framework 4.7.2 SDK installed: https://dotnet.microsoft.com/download/dotnet-framework/net472

=== Installation ===
Our working directory is now 'attachments'.

1. Go to '/solution/' and open .sln. Visual Studio is now opened.
2. In 'SZDC.Data' project open file 'szdc.config'. This is configuration file where connection string is provided to the application.
	Override value with connection string with your connection:
	Server = [IP]; Port = [port]; Database = [database]; User Id = [user]; Password = [password]

	Content of 'szdc.config' can look like this:

	<?xml version="1.0" encoding="utf-8"?>
	<appSettings> 
    		<add key="Connection string" value="Server = 127.0.0.1; Port = 5432; Database = test; User Id = postgres; Password = 12345;" />
	</appSettings>

3. Build solution in release version (content of '/binaries/' in attachments is also rebuilt by our configuration). With provided connection string we can import data to the database.
4. In Visual Studio 2017 or higher, go to View (in header) > Other Windows > Package Manager Console
5. With opened Package Manager Console, be sure that Default project in Package Manager Console's header is 'SZDC.Data'
   and in combobox above with build configuration the project 'SZDC.Data' is also selected
6. With empty database, enter command 'Update-Database' in the Package Manager Console, the output should be following:

PM> Update-Database
Applying migration '20190511211754_Init'.
Done.
PM> 

7. We can now leave Visual Studio. We are going to import data to the database from console application, open PowerShell or CommandPrompt.
8. With tool 'SZDC.Data.exe' in '/binaries/' with respect to our 'attachments' directory structure, import JSON data from '../szdc/import/':
	>SZDC.Data.exe ..\szdc\import

   Wait about 2 minutes for tool to import all data. Errors are reported during import to the console.

9. Application SZDC is now ready to use the database, open the application 'SZDC.WPF.exe' in '/binaries/'.
