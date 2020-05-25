# Gbay Online Marketplace

Gbay is an ecommerce website, similar to Ebay. You can go on and register as a buyer and shop others products or you can register as a seller and post products for others to buy. There are also moderators that can edit and delete all products if they are scams or are not the kind of products that we want. They can see the users data but some of it is masked and they cannot edit it. To have a moderator account you need an email invitation that administrators can send from their dashboard. Administrators can do everything that a moderator as well as being able to see and edit all users info, unmasked. 

## Purpose

This is an ASP.NET MVC application I made in order to learn how technologies like Entity Framework and ASP.NET Identity actually work. I am going to use Burp Suite once it is finished to disect it and learn everything about it. This will help me be able to identify if real-world applications are using these technologies and it will help me think through where potential vulnerablities may be hiding. 

## Running the Application

The application was developed using ASP.NET Core 3. If you choose to run it outside of Docker, then go to https://dotnet.microsoft.com and get the latest version of .NET Core for your platform. The application is cross platform so it will work on Windows, Linux, MAC OS X, and any other platform supported by Microsoft. The code builds successfully under both Visual Studio 2017 and 2019 and can be run both under IIS. When you run it using the key combination ctrl+F5, the application will open in your default browser.

## Testing 

There are a few built-in account that you can use to test out this application. 

| Account Type | Username | Email | Password | Security Question 1 Answer | Security Question 2 Answer |
|--------------|----------|-------|----------|----------------------------|----------------------------|
|Buyers|Buyer|buyer@buyer.com|P@ssword1|b|b|
|Sellers|Seller|seller@seller.com|P@ssword1|s|s|
|Moderators|Moderator|moderator@moderator.com|P@ssword1|m|m|
|Administrators|Administrator|administrator@administrator.com|P@ssword1|a|a|

