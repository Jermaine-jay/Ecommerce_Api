# Ecommerce Api Application
____
#### This Application is a web application built with ASP.NET API, Entity Framework Core, Redis, PostgreSQL, Google auth API, Facebook auth API, and Docker. This application makes online shopping easier and faster, with no payment issues.


## Technologies
____
* ASP.NET Core Web API: The web framework used to build the application's architecture and handle user requests.

* Redis: A database technology used for caching.

* PostgreSQL: The relational database management system used to store and manage application data.
  
* Docker: A Software used to build, test, and deploy applications using containers
  
* Entity Framework Core (EF Core): A powerful and flexible Object-Relational Mapping (ORM) tool for working with the application's database.
  > - *Microsoft Entity Framework core Design V 7.0.1*
  > - *Microsoft Entity Framework core Tools V 7.0.1*
  > - *Npgsql Entity Framework core PostgreSQL V 7.0.4*
  > - *Npgsql Entity Framework core PostgreSQL Design V 1.1.0*



## Application Features
____
* User Authentication and Authorization: Secure user registration and login system, ensuring only authorized users can have access to app features,
  Ensure only admins can make changes to the system. Note, that you can not login without email confirmation.

* User Update: Only confirmed users will be able to log in, and update details.

* Payment Feature: The app has multiple payment platforms embedded to facilitate payment.

* Orders: Users can create as well as pay for orders with a safe and secured payment method.

* User Profiles: Only authenticated users can use the application's features.

* Admin: Admins can check or delete orders, check and delete users, add roles or change user roles, and add role claims to a role. Admins can add new products and also edit products.

## How it works
_____
* Registration: The app uses an email system to confirm a user's email when they register, and an email system that sends a link to their mail for password reset. The application also has well-secured authentication and authorization mechanisms.

* External Login: Intending users can sign in with their Google or Facebook accounts and also login with their Google and Facebook accounts.
  
* Email system: The email system uses a token-based approach to confirm a user's email address. When a user registers, a token is generated and sent to their email address. The user must then click on the link in the email to confirm their email address. This helps to prevent unauthorized users from creating accounts.
  
* Authentication and authorization: The application uses a combination of email and password authentication, as well as role-based authorization. This ensures that only authorized users can access the application.

## Getting Started
_____
* Clone the repository: git clone https://github.com/Germaine-jay/TaskManager_Api.git

* Install required packages: dotnet restore

* Update the database connection string in the appsettings.json file to point to your PostgreSQL instance.

* Apply database migrations: dotnet ef database update

* Run the application: dotnet run


## Contributing
_____
If you'd like to contribute to the Car Auction App, please follow these steps:

* Fork the repository.

* Create a new branch for your feature or bug fix: git checkout -b feature/your-feature-name

* Make your changes and test thoroughly.

* Commit your changes: git commit -m "Add your commit message here"

* Push to your forked repository: git push origin feature/your-feature-name

* Create a pull request, describing your changes and the problem they solve.

## application URL
* https://ecommerceapi-app.onrender.com
* https://ecommerceapi-app.onrender.com/swagger/index.html

## Default Users
___
| Email                    | Password   | Role       |
| -----------------------  | ---------- | ---------- |
| jermaine.jay00@gmail.com | 12345qwert | User       |
| mosalah@outlook.com      | 12345qwert | Admin      |
| robertofirmino@gmail.com | 12345qwert | SuperAdmin |
| IbouKonate@gmail.com     | 12345qwert | User       |  
