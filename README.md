<h1>Carbonite Text Message Backup ETLoader</h1>
<p>This application parses XML backup files created by <a href="https://play.google.com/store/apps/details?id=com.riteshsahu.SMSBackupRestore&hl=en">SMS Backup & Restore</a>.

<h2>Status</h2>
The project is in the very beginning stages. I am currently in the metality of "hack it together to get it working," so it isn't going to be the most beautiful piece of code. As of September 8, 2017, I am still working on the basic functionality of parsing the XML file into domain objects.

<h2>Platform</h2>
The project is a C#.NET console application developed in Visual Studio Community 2017.

<h2>Why this application?</h2>
<p>I originally tried using SQL Server Integration Services (SSIS) to parse the backup file, but the tool enforces a 5000 byte limit in XML fields. Since the backup contains images, videos, and gifs stored as Base64-Encoded strings, this limit is much too small. (We are talking files on the scale of a few megabytes.)
<p>This application serves as a way to extract the data from the XML file and get into a more accessable format, e.g. database tables, after which more processing can occur.</p>
