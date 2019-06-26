This attachment directory contains various data as content of bachelors thesis 
GTTG – application for manipulation with train timetable diagrams
created by
Jakub Sýkora, student at Faculty of Mathematics and Physics, Charles university, 2019

Email: jakubsykora.mff@gmail.com

For the latest version of the application and the library visit:
https://github.com/Sykoj/GTTG

NuGet packages of the GTTG library:
https://www.nuget.org/packages/GTTG.Core/
https://www.nuget.org/packages/GTTG.Model/

Content of this attachment:

/binaries
	application and binary data created when building solution in '/solution/'
	requires .NET Framework 4.7.2 (part of Windows 10 Falls Creator Update (version 1709))
	do not run the application if connection to database is not provided and properly set!
	requires PostgreSQL 11, guide to connect the application to a database is provided in '/szdc/db/'

/solution
	source code of the application and library
	by building project binaries are also imported into '/binaries/'

/documentation
	API docs for the library GTTG
		- NuGet package GTTG.Core 1.0.4
		- NuGet package GTTG.Model 1.0.4

/tutorials
	small tutorial projects aimed to provide guide how to use the library
	if .sln project loaded, please build it -- NuGet downloads the packages

/tex
	source code of created 'prace.pdf', useful for future writers
	we also provide images in svg and pdf files

/szdc	
	data for the application SZDC


LICENSE.txt
	license of source code in '/solution/'

prace.pdf
	thesis in czech language