This directory contains set of tools to convert xlsx files of timetable schedules to JSON format.

As each 'n' (cargo) and 'os' (passenger) timetables has different xlsx structure,
two versions of application exists as 'converter_app_n' and 'converter_app_os'.
In this attachment we provided 'converter_app_n' to show example of applying heuristics
to variable data in xlsx with Apache POI. Applications slightly differ in parsed columns
or applied heuristics.

Input and output results are provided by following files for 'n', same applies for 'os':

os_xlsx contains xlsx files created from passenger train textual timetables in PDF (as '/szdc/sjr')
n_xlsx contains xlsx files created from cargo train textual timetables in PDF

Unfortunately, we no longer know which PDF to XLSX converter was used. We used one of many available
online converters, but all should be viable as long as they do not include additional graphics symbols
of timetable in parsed columns (as more advanced tools does).

'/szdc/njr/json/n' and '/szdc/njr/json/os/' contain output JSON files
n_output contains output text of application run with cargo trains that application was unable to parse
