**Task1_DataProcessing** is a data processing service which allows you to process files with payment transactions that different users save in the specific folder (A) on the disk (the path is specified in the config). Users can save files at any time, and service processes them immediately. A file can be either in TXT or CSV format.

**Txt file.** The input data must be in this order: Name, City, Payment, Date, Account Number, Service.
  
  Example (raw_data.txt):
  > John, Doe, “Lviv, Kleparivska 35, 4”, 500.0, 2022-27-01, 1234567, Water
  
  The name of the city must be in quotation marks! Commas are ignored.

**Csv file.** The input data must be in the same order: Name, City, Payment, Date, Account Number, Service (separated by semicolon ";").
  
  Example (raw_data.csv):
  > John Doe;"Lviv, Kleparivska, 35, 4";500.0;2022-27-01;1234567;Water

Commas and quotes are ignored.

When the file is processed, the service saves the results in a separate folder (B) (the path is specified in the config) in a subfolder (C) with the current date (i.g. 09-21-2022). As a file name, is used “output” + today’s current file number + “.json”.

The log is saved to 'meta.log' at the end of the day (midnight) and when the service is shut down.
