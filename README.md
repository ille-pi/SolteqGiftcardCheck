#Solteq Gift Card Export

A small C# console application that fetches open and valid gift cards from Solteq Commerce Cloud and exports them to a CSV file.

##How it works

- Gets an access token using Client Credentials.

- Fetches gift cards from the V1 API using pagination.

- Filters cards by status and validity dates.

- Exports the results to giftcards.csv.

A gift card is considered open and valid when its status is Active, its validity period has started and not ended, and isExpired is false.

##Running the project

Requires .NET SDK 11 and Solteq API credentials.

The application currently uses the Stage environment.

Set your credentials using User Secrets:

dotnet user-secrets set "Solteq:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "Solteq:ClientSecret" "YOUR_CLIENT_SECRET"

Run the application:

dotnet run

The CSV file is saved in the application's output directory. The full path is printed when the export is complete.

##Notes

In my test run, the API returned 228 gift cards, of which 81 were open and valid.