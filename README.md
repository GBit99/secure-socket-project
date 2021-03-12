# secure-socket-project
Project overview:
The idea of the project is to create a .net core server and have multiple clients communicate with it through the TLS/SSL protocol simultaneously.
Clients automatically generate and send data to the server. The idea is to experiment with how the server/clients behave (and what problems occur) when a big amout of data is transfered
at fast rates.


Contains:
- 1 .NET Core 3.1 console application for the server
- 1 .NET Core 3.1 console application for the client
- 1 bat file for starting one instance of the server and one for the client
- 1 bat file for starting one instance of the server and ten for the client


Details:
- Client:
  - Keeps a queue with all the data objects that need to be sent
  - Keeps a list of all the data which has been sent but is awaiting a response from the server.
  - When the client receives a response from the server:
    - If the server response is positive - the item is removed from the awaiting response data objects list.
    - If the server response is negative - the item is removed from the awaiting response data objects list and is enqueued to be sent again.
  - Data fields:
    - Id
    - FirstName
    - LastName
    - City
    - PostCode
    - AppVersion
    - Email
    - Music
    - Performer
    - Year
    - Hour
    - MD5 hash
    - FileName
    - FileType
    - Date
- Server
  - Deserializes the client object data:
    - If that's successfull, a positive response is sent to the client.
    - If it's unsuccessfull, the GUID Id is extracted from the JSON serialized string (using Regex) and is sent to the client along with a negative response so it can be reprocessed.


Nuget packages used:
- NetCoreServer v3.0.22
  - https://www.nuget.org/packages/NetCoreServer/3.0.22
  - https://github.com/chronoxor/NetCoreServer
