# GroupFlightPlanner

A C# and ASP.NET web app which displays information about treeplanting events, flights to that event location, and volunteers who are going to the event.

### Feedback Airlines - Airplanes 

In order to create a new relationship between **Airplanes** and **Airlines** to observe the Airplanes that belong to an Airline, a **1-M relationship** must be created (**1 Airline has many Airplanes**, and **1 Airplane belongs to an Airline**)

**Steps:**

1.  Go to Airplane Model and create a Foreign Key that refers to the Airline entity
```
[ForeignKey("Airline")]
public int AirlineId { get; set; }
public virtual Airline Airline { get; set; }
```
2.  Make sure that you have ```using System.ComponentModel.DataAnnotations.Schema;``` on top of the Airplane Model
3.  Make sure that the migrations are enabled, for this open the Package Manager Console, and run ```enable-migrations```
4.  Perform a new migration, this migration will create a new column called AirlineId which will indicate which Airline the Airplane belongs to. ```add-migration airplaneFK-ariplane```
5.  Check that the file **IdentityModels.cs** has declared both entities
6.  Update the database ```update-database```
7.  Now when you want to create an Airplane you must enter the ID of the Airline. Update the **airplane.json** which is the file to test the API and add the ID corresponding to the Airline, this will allow you to enter a new Airplane belonging to an Airline through the use of the API
8.  Create a new input which will be a dropdown where you can see all the airlines
```
@foreach (var airline in Model.AirlinesOptions){
    @airline.AirlineName
}
```
9.  In the method **New** of AirplaneConroller call the list of Airlines API to be able to display their information in the dropdown. When users create a New Airplane they can choose to which Airline the Airplane belongs, the dropdown will take the ID of the Airline chosen and assign the ID of the Airline to the new Airplane
10.  Now create a method called **ListAirplanesForAirline** in the AirplaneDataController and get the list of Airplanes and filter for the AirlineId given ```List Airplanes= db.Airplanes.Where(p =>p.AirlineId == id).ToList();```
11.  Later call this method from the **Airline Controller** to obtain the list and assign it to the **ViewModel.RelatedAirplanes** parameter
12.  Lastly, go to the Airlines **Details.cshtml** and add this piece of code to see the list of Airplanes belonging to an Airline
```
<ol>
    @foreach (var airplane in Model.RelatedAirplanes)
    {
        <li>
            <a href="/Airplane/Details/@airplane.AirplaneId">
                Registration Num: <span class="related-entities">@airplane.RegistrationNum</span> 
                <br /> 
                Model: <span class="related-entities">@airplane.AirplaneModel</span>
            </a>
        </li>
    }
</ol>
```

### Collaboration Together (Sion, Arnulfo, Nhi)
- Setting up the MVP of the 3 Passion Projects in just one project

### Collaborators and Contributions:
### Sion Lee 
Worked with association between Events and Groups, Locations and Flights.
1. Fixed URL string but on Event and Group controllers for Associate and Unassociate methods
   - EventController
   - AssociateEventWithGroup
   - UnAssociateEventWithGroup
2. Created new header menu to allow users to navigate through all the pages efficiently, display correct information on View
   - Views/Shared/_Layout
   - Views/Home/Index
   - Views/Group/Details
   - Views/Event/Details
   - Views/Event/Edit
   - Views/Event/New
3. Collaborated with methods for Location Controller for the relationship between Locations and Flights
    - Associate 
    - UnAssociate
4. Refactored merged code to fix spelling errors and clear labelling
5. Collaborated with group members to solve issues during migration and merging tasks such as creating new local databases and merging main branch into working branch
6. Collaborated with problem-solving of integration of the code as well as the elaboration of related methods

### Arnulfo Sanchez
Worked with association between Locations and Flights.
1. **Created relationship (M-M) and migration of locations-flights**
2. **Implemented related methods in DataController and Controller:**
    - ListLocationsForFlight
    - ListFlightsForLocation
    - ListFlightsNotAssociatedForLocation
    - AssociateLocationWithFlight
    - UnAssociateLocationWithFlight 
3. **Implemented methods for Location Controller**
    - Associate (Location - Flight)
    - UnAssociate(Location - Flight)
4. **Updated ViewmModels:**
    - DetailsFlight
    - DetailsLocation
5. **Updated Views:**
    - Flight/Details
    - Location/Details
6. **Created Authorization in Airline, Airplane, and Flight entities**

### Nhi Nguyen
Worked with association between Events and Groups.
1. Create a migration and table for many-many relationships between groups and events.
2. Created and updated View Model for DetailsEvent, and DetailsGroups to link associations. Also, Events and Groups have not been unassociated.
   - DetailsEvent.
   - DetailsGroup.
3. Worked with EventController, EventDataController to get data from eventsgroups table to show joined groups on event details, and list unassociated groups on the select box.
   - EventController:
        - Detail(show associated groups with this event).
   - EventDataController.
     - ListEventsForGroup.
     - AssociateEventWithGroup.
     - UnAssociateEventWithGroup.
4. Worked with GroupDataController, GroupDataController to get data from eventsgroups table to show registered events on group details, and list unassociated events on the       select box.
   - GroupController:
        - Detail(show all events were joined in by this group).
   - GroupDataController:
       - ListGroupsForEvent.
       - ListGroupsNotJoinInEvent
5. Showed associations of events and groups, and events and groups have not been unassociated on the view of event details and group details.

## Technologies used for the project:
   - ASP.NET Entity Framework
   - Bootstrap CSS Framework
   - Vanilla CSS
   - LINQ
   - Web API Controllers
   - MVC Controllers

## Getting Started:
   1. Clone the repository to your local machine.
   2. Open the code with Visual Studio
   3. Run the Project and if you get an Error due to the target framework
      - Change target framework to 4.7.1
      - Change back to 4.7.2
   4. Make sure there is an App_Data folder in the project (Right click solution > View in File Explorer)
      - If there is no folder create a folder called App_Data
   5. Run Update-Database by typing it on Tools > Nuget Package Manager > Package Manage Console
   6. Check that the database is created using (View > SQL Server Object Explorer > MSSQLLocalDb > ..)
