# CarWebApp

Vehicle accounting application. Provides the ability to track vehicle activity, generate reports - waybills and fuel balance of vehicles for a specified period.

## Project Assignment

Design a vehicle accounting application – program for accounting company vehicles.

 
Specifications 

> * Authentication and authorization for system users, including Google and Facebook authentication capability
> * Separation of user access: admin has full access, master has no access to modify cars, but has full access to waybills, operator has access only to insertion and modification of waybills
> * Accounting for vehicle and drivers in the company
> * Company vehicle usage by entering trip tickets: date of trip ticket, date of delivery of trip ticket, date and time of departure, date and time of return, mileage, actual and standard fuel usage, amount of fuel filled
> * Generating and printing reports for each trip ticket
> * Report on used fuel: the report should be formed for a given period of time for all cars of the company regardless of the presence of trip tickets during the period, the initial fuel balance as of the initial date and the final fuel balance as of the final date are determined, if the fuel at the end of the period is not equal to "fuel at the beginning of the period" + "fuel filled" - "actually used fuel" the report cell is highlighted in yellow

## Format

ASP.Net Core MVC with MS SQL Server database and FastReport.Core

## Screen shots

[Screenshots](https://github.com/kovalenko-olena/CarWebApp/tree/master/CarWebApp/wwwroot/doc/screenshots)

## Author

* [Kovalenko Olena](https://github.com/kovalenko-olena)

## Links

* project repository - https://github.com/kovalenko-olena/CarWebApp

