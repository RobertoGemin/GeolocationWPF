# Description app 
It is an application designed to store geolocation information in a database, allowing users to search for location data using either an IP address or a fully qualified domain name.<br>

This PDF, accessible through the Google Drive link, contains a description of the project.
https://docs.google.com/document/d/1aVUzI1ZnovGCvrREn8hkNE1-lCxrXfc9_x50CCLOOK8/edit?usp=sharing

## Preparation: Before starting<br>  
**Step 1: Obtain and Configure API Key**
* Visit 'https://geo.ipify.org/' to retrieve your API key.
* Register for a free account if you don't have one.
* Add the obtained API key to the configuration file.<add key="ApiKey" value="INSERTKEY" />

**Step 2: Database Setup and data Initialization** 
* Start console application
- The methods in the console application create the necessary tables.
- The tables will be preloaded with default data,this function is suitable for performing bulk insert operations.
