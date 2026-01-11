# <span style="color: #20B2AA;">Task Manager</span>

This is the backend web api for the Task Manager/list project (Front end can be found [here](https://github.com/MikeJones999/TaskListSpaUI) - please review the readme there for assistance.)

once pulled down open the soltion file in visual studio - set the startup project to be TaskListWebApi and then run https version.

or if running from terminal

go into the main directory installed (TaskListWebApi)

open terminal/command line -> dotnet run --launch-profile https

any issues with certs - run
 -> dotnet dev-certs https clean
 ->  dotnet dev-certs https trust
 select yes when allow option for ssl appears

Backend should run on https://localhost:7082 and the UI should be running on http://localhost:5173

In order to see the open api docs - goto https://localhost:7082/scalar/v1

**Build may take a short while as the project is using sqlite, specifically chosen for this project so it doesnt rely on installing sql server/postgress (ideally this is what I would use) - for demo purposes.

Once the backed has been started - the system has a user that will be seeded for use to play with inside the UI, or you can register a new user to try from scratch. 

The details for the test user are hard coded into the login page and will be visible when you navigate there
